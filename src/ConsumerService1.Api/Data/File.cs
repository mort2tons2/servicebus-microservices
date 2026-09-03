namespace ConsumerService1.Api.Data;

public sealed class File
{
    public required string BlobName { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public long SizeBytes { get; init; }
    public DateTime UploadTimestamp { get; init; }
}
