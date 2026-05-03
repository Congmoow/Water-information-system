using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IAiServiceClient
{
    Task<AiReviewResult> SubmitReviewAsync(ApprovalApplication application, CancellationToken cancellationToken);
}

public class AiReviewResult
{
    public string ApplicationId { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public string Summary { get; set; } = string.Empty;
    public List<AiReviewFinding> Findings { get; set; } = new();
}

public class AiReviewFinding
{
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Suggestion { get; set; }
}
