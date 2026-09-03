using Microsoft.EntityFrameworkCore;

namespace ConsumerService2.Api.Data;

public sealed class ConsumerService2DbContext(DbContextOptions<ConsumerService2DbContext> options) : DbContext(options)
{
    public DbSet<File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var file = modelBuilder.Entity<File>();

        file.ToTable("File");

        file.HasKey(f => f.BlobName);

        file.Property(f => f.BlobName).HasMaxLength(512);

        file.Property(f => f.FileName).HasMaxLength(1024);

        file.Property(f => f.ContentType).HasMaxLength(256);
    }
}
