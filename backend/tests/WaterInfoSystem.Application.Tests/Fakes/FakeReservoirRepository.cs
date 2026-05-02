using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal class FakeReservoirRepository : IReservoirRepository
{
    public List<Reservoir> Items { get; }

    public FakeReservoirRepository(params Reservoir[] items)
    {
        Items = items.ToList();
    }

    public Task<(IReadOnlyList<Reservoir> Items, int Total)> SearchAsync(
        string? keyword, int page, int pageSize, CancellationToken cancellationToken)
    {
        return Task.FromResult(((IReadOnlyList<Reservoir>)Items, Items.Count));
    }

    public Task<IReadOnlyList<Reservoir>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((IReadOnlyList<Reservoir>)Items);
    }

    public Task<Reservoir?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
    }

    public Task AddAsync(Reservoir reservoir, CancellationToken cancellationToken)
    {
        Items.Add(reservoir);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Reservoir reservoir, CancellationToken cancellationToken)
    {
        Items.Remove(reservoir);
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.Count);
    }
}
