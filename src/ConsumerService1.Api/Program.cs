using ConsumerService1.Api;
using ConsumerService1.Api.Data;
using ConsumerService1.Api.Hubs;
using EventFanout.ServiceDefaults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBusClient("service-bus");

var useAzure = string.Equals(builder.Configuration["UseAzure"], "true", StringComparison.OrdinalIgnoreCase)
    || !builder.Environment.IsDevelopment();

if (useAzure)
{
    builder.AddSqlServerDbContext<SearchDbContext>("consumer-service1-db");
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("consumer-service1-db");

    builder.Services.AddDbContext<SearchDbContext>(options =>
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

app.MapGet("/documents", (FileQueries queries, CancellationToken cancellationToken) => queries.AllAsync(cancellationToken));
app.MapGet("/search", (string q, FileQueries queries, CancellationToken cancellationToken) => queries.QueryAsync(q, cancellationToken));

app.MapHub<FilesHub>("/hub/files");

await app.RunAsync();
