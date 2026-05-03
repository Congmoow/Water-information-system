namespace WaterInfoSystem.Domain.Entities;

public class ReviewResult : BaseEntity
{
    public Guid ApplicationId { get; set; }

    public ApprovalApplication? Application { get; set; }

    public bool IsPassed { get; set; }

    public string Summary { get; set; } = string.Empty;

    public DateTime ReviewedAt { get; set; }

    public string? AgentVersion { get; set; }

    public ICollection<ReviewFinding> Findings { get; set; } = new List<ReviewFinding>();
}
