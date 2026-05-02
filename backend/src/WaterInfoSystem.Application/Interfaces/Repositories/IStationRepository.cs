using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Interfaces.Repositories;

public interface IStationRepository
{
    Task<(IReadOnlyList<Station> Items, int Total)> SearchAsync(
        string? keyword,
        StationType? type,
        StationStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Station>> GetAllAsync(CancellationToken cancellationToken);

    Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Station station, CancellationToken cancellationToken);

    Task DeleteAsync(Station station, CancellationToken cancellationToken);

    Task<int> CountAsync(CancellationToken cancellationToken);

    Task<bool> ExistsByRiverIdAsync(Guid riverId, CancellationToken cancellationToken);

    Task<bool> ExistsByReservoirIdAsync(Guid reservoirId, CancellationToken cancellationToken);
}
