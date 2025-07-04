using Common.DomainEvents;
using Common.Enums;

namespace MediaService.DomainEvents;

public record MediaFileDeletedEvent(
    string MediaFileId,
    string FileName,
    MediaFileType Type,
    string FilePath
) : BaseDomainEvent; 