using ConsumerService2.Api;
using ConsumerService2.Api.Data;
using ConsumerService2.Api.Hubs;
using EventFanout.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBusClient("service-bus");

var useAzure = string.Equals(builder.Configuration["UseAzure"], "true", StringComparison.OrdinalIgnoreCase)
    || !builder.Environment.IsDevelopment();

if (useAzure)
{
    builder.AddSqlServerDbContext<ConsumerService2DbContext>("consumer-service2-db");
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("consumer-service2-db");

    builder.Services.AddDbContext<ConsumerService2DbContext>(options =>
    {
        options.UseSqlite(connectionString);
    });
}

builder.Services.AddSignalR();

builder.Services.AddScoped<FileQueries>();
builder.Services.AddHostedService<DbInitializer>();
builder.Services.AddHostedService<FileEventsWorker>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/thumbnails", (FileQueries queries, CancellationToken ct) => queries.AllAsync(ct));

app.MapHub<FilesHub>("/hub/files");

await app.RunAsync();
