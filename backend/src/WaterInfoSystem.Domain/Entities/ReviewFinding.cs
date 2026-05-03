namespace WaterInfoSystem.Domain.Entities;

public class ReviewFinding : BaseEntity
{
    public Guid ReviewResultId { get; set; }

    public ReviewResult? ReviewResult { get; set; }

    public string Category { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Suggestion { get; set; }
}
