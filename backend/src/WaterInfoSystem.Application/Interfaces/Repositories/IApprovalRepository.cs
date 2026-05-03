using WaterInfoSystem.Application.Contracts.Approvals;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Interfaces.Repositories;

public interface IApprovalRepository
{
    Task<(List<ApprovalListItemDto> Items, int Total)> SearchAsync(string? keyword, string? status, int page, int pageSize, CancellationToken cancellationToken);

    Task<ApprovalApplication?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(ApprovalApplication entity, CancellationToken cancellationToken);

    Task DeleteAsync(ApprovalApplication entity, CancellationToken cancellationToken);
}
