using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventFanout.ServiceDefaults;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureServiceBusClient("service-bus");

builder.AddAzureBlobContainerClient("uploads");

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/files", async (IFormFile file, BlobContainerClient container, ServiceBusClient bus, CancellationToken ct) =>
{
    // using - instead of / to render blobs as files instead of folders in Azure Storage Explorer
    var blobName = $"{Guid.NewGuid():N}-{file.FileName}";

    var blob = container.GetBlobClient(blobName);
    await using (var stream = file.OpenReadStream())
    {
        await blob.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType },
        }, ct);
    }

    var uploaded = new FileUploadedEvent(
        blobName,
        file.FileName,
        file.ContentType,
        file.Length,
        DateTime.Now);

    await using var sender = bus.CreateSender(EventTopics.FileEvents);
    await sender.SendMessageAsync(
        new ServiceBusMessage(BinaryData.FromObjectAsJson(uploaded))
        {
            ContentType = "application/json",
            Subject = nameof(FileUploadedEvent),
            MessageId = blobName,
        },
        ct);

    return Results.Accepted($"/files/{blobName}", uploaded);
})
.DisableAntiforgery();

app.MapDelete("/files/{blobName}", async (string blobName, BlobContainerClient container, ServiceBusClient bus, CancellationToken ct) =>
{
    var blob = container.GetBlobClient(blobName);
    var deleted = (await blob.DeleteIfExistsAsync(cancellationToken: ct)).Value;

    if (!deleted)
    {
        return Results.NotFound();
    }

    var deletedEvent = new FileDeletedEvent(blobName);

    await using var sender = bus.CreateSender(EventTopics.FileEvents);
    await sender.SendMessageAsync(
        new ServiceBusMessage(BinaryData.FromObjectAsJson(deletedEvent))
        {
            ContentType = "application/json",
            Subject = nameof(FileDeletedEvent),
            MessageId = $"{blobName}:deleted",
        },
        ct);

    return Results.Ok();
});

await app.RunAsync();
