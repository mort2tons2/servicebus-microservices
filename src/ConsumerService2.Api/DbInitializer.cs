using ConsumerService2.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsumerService2.Api;

public sealed class DbInitializer(
    IServiceScopeFactory scopeFactory,
    ILogger<DbInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ConsumerService2DbContext>();

        if (db.Database.IsSqlite())
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }
        else
        {
            await db.Database.MigrateAsync(cancellationToken);
        }

        logger.LogInformation("consumer-service2-db schema is up to date");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
