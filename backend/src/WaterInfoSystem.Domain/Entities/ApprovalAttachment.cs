namespace WaterInfoSystem.Domain.Entities;

public class ApprovalAttachment : BaseEntity
{
    public Guid ApplicationId { get; set; }

    public ApprovalApplication? Application { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string AttachmentType { get; set; } = string.Empty;
}
