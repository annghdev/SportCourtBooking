using Common.Base;
using Common.Enums;
using MediaService.DomainEvents;

namespace MediaService.Entities;

public class MediaFile : AggregateRoot
{
    public required string FileName { get; set; }
    public required string OriginalFileName { get; set; }
    public required string FilePath { get; set; }
    public required string ContentType { get; set; }
    public long FileSize { get; set; }
    public MediaFileType Type { get; set; }
    public string? StorageProvider { get; set; }
    public string? CloudUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? UploadedBy { get; set; }
    public string? RelatedEntityId { get; set; }
    public string? RelatedEntityType { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? Alt { get; set; }
    public string? Tags { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? DurationSeconds { get; set; }
    public string? Metadata { get; set; }

    public static MediaFile Create(string fileName, string originalFileName, string filePath, 
        string contentType, long fileSize, MediaFileType type, string? uploadedBy = null)
    {
        var mediaFile = new MediaFile
        {
            FileName = fileName,
            OriginalFileName = originalFileName,
            FilePath = filePath,
            ContentType = contentType,
            FileSize = fileSize,
            Type = type,
            UploadedBy = uploadedBy
        };

        mediaFile.AddDomainEvent(new MediaFileUploadedEvent(mediaFile.Id, fileName, type, fileSize, uploadedBy));
        return mediaFile;
    }

    public void Delete(string deletedBy)
    {
        DeletedBy = deletedBy;
        DeletedDate = DateTimeOffset.UtcNow;
        
        AddDomainEvent(new MediaFileDeletedEvent(Id, FileName, Type, FilePath));
    }
} 