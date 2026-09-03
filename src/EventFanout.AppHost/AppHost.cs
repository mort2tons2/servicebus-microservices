using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Run local (Service Bus + Storage emulators and SQLite, needs Docker):
//   dotnet run --project src/EventFanout.AppHost
// Run against real Azure (existing resources, connection strings from Key Vault):
//   dotnet run --project src/EventFanout.AppHost --launch-profile azure
//   (or pass -- --UseAzure=true; publishing / non-Development always implies Azure).
var useAzure = string.Equals(builder.Configuration["UseAzure"], "true", StringComparison.OrdinalIgnoreCase)
    || !builder.Environment.IsDevelopment();

var backing = useAzure
    ? AddAzureBacking(builder)
    : AddLocalBacking(builder);

var publisherApi = builder.AddProject<Projects.PublisherService_Api>("publisher-service-api")
    .WithReference(backing.ServiceBus)
    .WithReference(backing.Uploads);

var searchApi = builder.AddProject<Projects.ConsumerService1_Api>("consumer-service1-api")
    .WithReference(backing.ServiceBus)
    .WithReference(backing.SearchDb)
    .WithEnvironment("ServiceBus__Subscription", "consumer-service1")
    .WithEnvironment("UseAzure", useAzure ? "true" : "false");

var thumbnailsApi = builder.AddProject<Projects.ConsumerService2_Api>("consumer-service2-api")
    .WithReference(backing.ServiceBus)
    .WithReference(backing.ThumbnailsDb)
    .WithEnvironment("ServiceBus__Subscription", "consumer-service2")
    .WithEnvironment("UseAzure", useAzure ? "true" : "false");

if (!useAzure)
{
    publisherApi.WaitFor(backing.ServiceBus).WaitFor(backing.Uploads);
    searchApi.WaitFor(backing.ServiceBus);
    thumbnailsApi.WaitFor(backing.ServiceBus);
}

builder.AddViteApp("publisher-service-web", "../../web/publisher-service")
    .WithReference(publisherApi)
    .WaitFor(publisherApi)
    .WithEndpoint("http", e => e.Port = 5171, createIfNotExists: false)
    .WithExternalHttpEndpoints();

builder.AddViteApp("consumer-service-1-web", "../../web/consumer-service-1")
    .WithReference(searchApi)
    .WaitFor(searchApi)
    .WithEndpoint("http", e => e.Port = 5173, createIfNotExists: false)
    .WithExternalHttpEndpoints();

builder.AddViteApp("consumer-service-2-web", "../../web/consumer-service-2")
    .WithReference(thumbnailsApi)
    .WaitFor(thumbnailsApi)
    .WithEndpoint("http", e => e.Port = 5172, createIfNotExists: false)
    .WithExternalHttpEndpoints();

builder.Build().Run();


static Backing AddAzureBacking(IDistributedApplicationBuilder builder)
{
    var vaultUri = builder.Configuration["KeyVault:VaultUri"];

    if (string.IsNullOrWhiteSpace(vaultUri))
    {
        throw new InvalidOperationException(
            "Set KeyVault:VaultUri (user-secrets or KeyVault__VaultUri) to run against Azure.");
    }

    builder.Configuration.AddAzureKeyVault(new Uri(vaultUri), new DefaultAzureCredential());

    return new Backing(
        ServiceBus: builder.AddConnectionString("service-bus"),
        Uploads: builder.AddConnectionString("uploads"),
        SearchDb: builder.AddConnectionString("consumer-service1-db"),
        ThumbnailsDb: builder.AddConnectionString("consumer-service2-db"));
}

static Backing AddLocalBacking(IDistributedApplicationBuilder builder)
{
    var dataDir = Path.Combine(builder.AppHostDirectory, "..", "..", ".data");
    Directory.CreateDirectory(dataDir);

    var serviceBus = builder.AddAzureServiceBus("service-bus")
        .RunAsEmulator();

    var topic = serviceBus.AddServiceBusTopic("file-events");
    topic.AddServiceBusSubscription("consumer-service1");
    topic.AddServiceBusSubscription("consumer-service2");

    var storage = builder.AddAzureStorage("storage")
        .RunAsEmulator();

    return new Backing(
        ServiceBus: serviceBus,
        Uploads: storage.AddBlobContainer("uploads", blobContainerName: "uploads"),
        SearchDb: SqliteFile(builder, "consumer-service1-db", dataDir),
        ThumbnailsDb: SqliteFile(builder, "consumer-service2-db", dataDir));
}

static IResourceBuilder<IResourceWithConnectionString> SqliteFile(
    IDistributedApplicationBuilder builder,
    string name,
    string dataDir)
{
    var path = Path.Combine(dataDir, $"{name}.sqlite");

    return builder.AddConnectionString(name, ReferenceExpression.Create($"Data Source={path}"));
}


sealed record Backing(
    IResourceBuilder<IResourceWithConnectionString> ServiceBus,
    IResourceBuilder<IResourceWithConnectionString> Uploads,
    IResourceBuilder<IResourceWithConnectionString> SearchDb,
    IResourceBuilder<IResourceWithConnectionString> ThumbnailsDb);
