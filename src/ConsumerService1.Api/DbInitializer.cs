using ConsumerService1.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ConsumerService1.Api;

public sealed class DbInitializer(
    IServiceScopeFactory scopeFactory,
    ILogger<DbInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SearchDbContext>();

        if (db.Database.IsSqlite())
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }
        else
        {
            await db.Database.MigrateAsync(cancellationToken);
        }

        logger.LogInformation("consumer-service1-db schema is up to date");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
