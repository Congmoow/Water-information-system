using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Interfaces.Repositories;

public interface IRiverRepository
{
    Task<(IReadOnlyList<River> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken);

    Task<IReadOnlyList<River>> GetAllAsync(CancellationToken cancellationToken);

    Task<River?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(River river, CancellationToken cancellationToken);

    Task DeleteAsync(River river, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
}
