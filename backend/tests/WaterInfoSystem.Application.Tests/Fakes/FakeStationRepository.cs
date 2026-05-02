using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal class FakeStationRepository : IStationRepository
{
    public List<Station> Items { get; }

    public FakeStationRepository(params Station[] items)
    {
        Items = items.ToList();
    }

    public Task<(IReadOnlyList<Station> Items, int Total)> SearchAsync(
        string? keyword, StationType? type, StationStatus? status,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        IEnumerable<Station> query = Items;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var items = query.ToList();
        return Task.FromResult(((IReadOnlyList<Station>)items, items.Count));
    }

    public Task<IReadOnlyList<Station>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((IReadOnlyList<Station>)Items);
    }

    public Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
    }

    public Task AddAsync(Station station, CancellationToken cancellationToken)
    {
        Items.Add(station);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Station station, CancellationToken cancellationToken)
    {
        Items.Remove(station);
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.Count);
    }

    public Task<bool> ExistsByRiverIdAsync(Guid riverId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.Any(x => x.RiverId == riverId));
    }

    public Task<bool> ExistsByReservoirIdAsync(Guid reservoirId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.Any(x => x.ReservoirId == reservoirId));
    }
}
