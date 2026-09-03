using Azure.Messaging.ServiceBus;
using ConsumerService1.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using Shared.Events;

namespace ConsumerService1.Api;

public sealed class FileEventsWorker(
    ServiceBusClient client,
    IHubContext<FilesHub> hub,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<FileEventsWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        string subscription = configuration["ServiceBus:Subscription"] ?? "consumer-service1";

        await using ServiceBusProcessor processor = client.CreateProcessor(
            EventTopics.FileEvents,
            subscription,
            new ServiceBusProcessorOptions { MaxConcurrentCalls = 4 });

        processor.ProcessMessageAsync += async args =>
        {
            switch (args.Message.Subject)
            {
                case nameof(FileUploadedEvent):
                    await HandleUploadedAsync(args);
                    break;
                case nameof(FileDeletedEvent):
                    await HandleDeletedAsync(args);
                    break;
                default:
                    await args.DeadLetterMessageAsync(
                        args.Message,
                        "UnknownEventType",
                        $"Subject '{args.Message.Subject}' is not a known event type",
                        args.CancellationToken);
                    break;
            }
        };

        processor.ProcessErrorAsync += args =>
        {
            logger.LogError(args.Exception, "Failed to process a file event");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(stopToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stopToken);
        }
        catch (OperationCanceledException)
        {
            // shutdown
        }

        await processor.StopProcessingAsync(CancellationToken.None);
    }

    private async Task HandleUploadedAsync(ProcessMessageEventArgs args)
    {
        var uploadedEvent = args.Message.Body.ToObjectFromJson<FileUploadedEvent>();

        if (uploadedEvent is null)
        {
            await args.DeadLetterMessageAsync(
                args.Message,
                "DeserializationFailed",
                $"Body is not a {nameof(FileUploadedEvent)}",
                args.CancellationToken);

            return;
        }

        await Task.Delay(Random.Shared.Next(1001), args.CancellationToken);

        await using var scope = scopeFactory.CreateAsyncScope();
        var queries = scope.ServiceProvider.GetRequiredService<FileQueries>();

        if (await queries.TryAddAsync(uploadedEvent, args.CancellationToken))
        {
            await hub.Clients.All.SendAsync("fileProcessed", uploadedEvent, args.CancellationToken);

            logger.LogInformation(
                "Indexed {FileName} ({BlobName})",
                uploadedEvent.FileName,
                uploadedEvent.BlobName);
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }

    private async Task HandleDeletedAsync(ProcessMessageEventArgs args)
    {
        var deletedEvent = args.Message.Body.ToObjectFromJson<FileDeletedEvent>();

        if (deletedEvent is null)
        {
            await args.DeadLetterMessageAsync(
                args.Message,
                "DeserializationFailed",
                $"Body is not a {nameof(FileDeletedEvent)}",
                args.CancellationToken);

            return;
        }

        await Task.Delay(Random.Shared.Next(1001), args.CancellationToken);

        await using var scope = scopeFactory.CreateAsyncScope();
        var queries = scope.ServiceProvider.GetRequiredService<FileQueries>();

        if (await queries.TryDeleteAsync(deletedEvent.BlobName, args.CancellationToken))
        {
            await hub.Clients.All.SendAsync("fileDeleted", deletedEvent, args.CancellationToken);

            logger.LogInformation("Removed {BlobName} from the index", deletedEvent.BlobName);
        }

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }
}
