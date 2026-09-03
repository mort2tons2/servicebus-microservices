namespace Shared.Events;

public sealed record FileUploadedEvent(
    string BlobName,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTime UploadTimestamp);

public sealed record FileDeletedEvent(string BlobName);

public static class EventTopics
{
    public const string FileEvents = "file-events";
}
