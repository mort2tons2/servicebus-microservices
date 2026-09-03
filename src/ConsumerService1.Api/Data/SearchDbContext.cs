using Microsoft.EntityFrameworkCore;

namespace ConsumerService1.Api.Data;

public sealed class SearchDbContext(DbContextOptions<SearchDbContext> options) : DbContext(options)
{
    public DbSet<File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var doc = modelBuilder.Entity<File>();

        doc.ToTable("File");

        doc.HasKey(d => d.BlobName);

        doc.Property(d => d.BlobName).HasMaxLength(512);

        doc.Property(d => d.FileName).HasMaxLength(1024);

        doc.Property(d => d.ContentType).HasMaxLength(256);
    }
}
