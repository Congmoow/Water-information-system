namespace WaterInfoSystem.Domain.Entities;

public class ApprovalApplication : BaseEntity
{
    public string ApplicantName { get; set; } = string.Empty;

    public string ApplicantIdCard { get; set; } = string.Empty;

    public string? EnterpriseName { get; set; }

    public string? EnterpriseLicenseNo { get; set; }

    public string WaterIntakeLocation { get; set; } = string.Empty;

    public string WaterIntakePurpose { get; set; } = string.Empty;

    public decimal WaterIntakeAmount { get; set; }

    public DateTime ApplicationDate { get; set; }

    public string Status { get; set; } = "Pending";

    public Guid SubmittedByUserId { get; set; }

    public User? SubmittedByUser { get; set; }

    public ICollection<ApprovalAttachment> Attachments { get; set; } = new List<ApprovalAttachment>();

    public ICollection<ReviewResult> ReviewResults { get; set; } = new List<ReviewResult>();
}
