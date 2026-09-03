using ConsumerService2.Api.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Shared.Events;

namespace ConsumerService2.Api;

public sealed class FileQueries(ConsumerService2DbContext db)
{
    public async Task<bool> TryAddAsync(FileUploadedEvent uploaded, CancellationToken ct)
    {
        var processedFile = new Data.File
        {
            BlobName = uploaded.BlobName,
            FileName = uploaded.FileName,
            ContentType = uploaded.ContentType,
            SizeBytes = uploaded.SizeBytes,
            UploadTimestamp = uploaded.UploadTimestamp,
        };

        db.Files.Add(processedFile);

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
        await db.Files.Where(f => f.BlobName == blobName).ExecuteDeleteAsync(ct) > 0;

    public async Task<IReadOnlyList<FileUploadedEvent>> AllAsync(CancellationToken ct)
    {
        return await db.Files
            .OrderByDescending(f => f.UploadTimestamp)
            .Select(f => new FileUploadedEvent(
                f.BlobName,
                f.FileName,
                f.ContentType,
                f.SizeBytes,
                f.UploadTimestamp))
            .ToListAsync(ct);
    }
}
