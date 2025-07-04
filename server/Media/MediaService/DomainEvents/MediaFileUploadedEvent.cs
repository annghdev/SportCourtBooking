using Common.DomainEvents;
using Common.Enums;

namespace MediaService.DomainEvents;

public record MediaFileUploadedEvent(
    string MediaFileId,
    string FileName,
    MediaFileType Type,
    long FileSize,
    string? UploadedBy
) : BaseDomainEvent; 