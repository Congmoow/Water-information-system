namespace WaterInfoSystem.Application.Contracts.Approvals;

public record ApprovalQueryDto(
    string? Keyword = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 10);

public record ApprovalCreateDto(
    string ApplicantName,
    string ApplicantIdCard,
    string? EnterpriseName,
    string? EnterpriseLicenseNo,
    string WaterIntakeLocation,
    string WaterIntakePurpose,
    decimal WaterIntakeAmount);

public record ApprovalAttachmentDto(
    Guid Id,
    string FileName,
    string FileType,
    string AttachmentType,
    DateTime CreatedAt);

public record ReviewFindingDto(
    Guid Id,
    string Category,
    string Severity,
    string Description,
    string? Suggestion);

public record ReviewResultDto(
    Guid Id,
    bool IsPassed,
    string Summary,
    DateTime ReviewedAt,
    string? AgentVersion,
    List<ReviewFindingDto> Findings);

public record ApprovalListItemDto(
    Guid Id,
    string ApplicantName,
    string ApplicantIdCard,
    string? EnterpriseName,
    string WaterIntakeLocation,
    string WaterIntakePurpose,
    decimal WaterIntakeAmount,
    DateTime ApplicationDate,
    string Status,
    DateTime UpdatedAt);

public record ApprovalDetailDto(
    Guid Id,
    string ApplicantName,
    string ApplicantIdCard,
    string? EnterpriseName,
    string? EnterpriseLicenseNo,
    string WaterIntakeLocation,
    string WaterIntakePurpose,
    decimal WaterIntakeAmount,
    DateTime ApplicationDate,
    string Status,
    List<ApprovalAttachmentDto> Attachments,
    List<ReviewResultDto> ReviewResults,
    DateTime CreatedAt,
    DateTime UpdatedAt);
