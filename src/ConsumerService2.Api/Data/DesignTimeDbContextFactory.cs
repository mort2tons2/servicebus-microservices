using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsumerService2.Api.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ConsumerService2DbContext>
{
    public ConsumerService2DbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ConsumerService2DbContext>()
            .UseSqlServer("Server=localhost;Database=consumer-service2-db;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new ConsumerService2DbContext(options);
    }
}
