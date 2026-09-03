using ConsumerService1.Api.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shared.Events;

namespace ConsumerService1.Api;

public sealed class FileQueries(SearchDbContext db)
{
    public async Task<bool> TryAddAsync(FileUploadedEvent uploaded, CancellationToken ct)
    {
        db.Files.Add(new Data.File
        {
            BlobName = uploaded.BlobName,
            FileName = uploaded.FileName,
            ContentType = uploaded.ContentType,
            SizeBytes = uploaded.SizeBytes,
            UploadTimestamp = uploaded.UploadTimestamp,
        });

        try
        {
            await db.SaveChangesAsync(ct);

            return true;
        }
        catch (DbUpdateException e) when (
            e.InnerException is SqlException { Number: 2601 or 2627 }
            or SqliteException { SqliteErrorCode: 19 })
        {
            db.ChangeTracker.Clear();

            return false;
        }
    }

    public async Task<bool> TryDeleteAsync(string blobName, CancellationToken ct) =>
        await db.Files.Where(d => d.BlobName == blobName).ExecuteDeleteAsync(ct) > 0;

    public async Task<IReadOnlyList<FileUploadedEvent>> AllAsync(CancellationToken ct) =>
        await Project(db.Files.OrderByDescending(d => d.UploadTimestamp)).ToListAsync(ct);

    public async Task<IReadOnlyList<FileUploadedEvent>> QueryAsync(string term, CancellationToken ct) =>
        await Project(db.Files
                .Where(d => EF.Functions.Like(d.FileName, $"%{term}%"))
                .OrderByDescending(d => d.UploadTimestamp))
            .ToListAsync(ct);

    private static IQueryable<FileUploadedEvent> Project(IQueryable<Data.File> q) =>
        q.Select(d => new FileUploadedEvent(
            d.BlobName, d.FileName, d.ContentType, d.SizeBytes, d.UploadTimestamp));
}
