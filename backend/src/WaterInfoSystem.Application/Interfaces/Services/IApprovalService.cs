using WaterInfoSystem.Application.Contracts.Approvals;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IApprovalService
{
    Task<PagedResult<ApprovalListItemDto>> SearchAsync(ApprovalQueryDto query, CancellationToken cancellationToken);

    Task<ApprovalDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ApprovalDetailDto> CreateAsync(ApprovalCreateDto request, Guid userId, CancellationToken cancellationToken);

    Task<ApprovalDetailDto> SubmitForReviewAsync(Guid id, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<ApprovalDetailDto> AddAttachmentAsync(Guid applicationId, string fileName, string fileType, string filePath, string attachmentType, CancellationToken cancellationToken);
}
