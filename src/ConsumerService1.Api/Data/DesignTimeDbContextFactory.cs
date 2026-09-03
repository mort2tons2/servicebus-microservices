using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsumerService1.Api.Data;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SearchDbContext>
{
    public SearchDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SearchDbContext>()
            .UseSqlServer("Server=localhost;Database=consumer-service1-db;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new SearchDbContext(options);
    }
}
