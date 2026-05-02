using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Interfaces.Repositories;

public interface IReservoirRepository
{
    Task<(IReadOnlyList<Reservoir> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reservoir>> GetAllAsync(CancellationToken cancellationToken);

    Task<Reservoir?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Reservoir reservoir, CancellationToken cancellationToken);

    Task DeleteAsync(Reservoir reservoir, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);
}
