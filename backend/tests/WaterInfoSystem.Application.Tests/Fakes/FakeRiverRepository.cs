using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal class FakeRiverRepository : IRiverRepository
{
    public List<River> Items { get; } = [];

    public Task<(IReadOnlyList<River> Items, int Total)> SearchAsync(
        string? keyword, int page, int pageSize, CancellationToken cancellationToken)
    {
        return Task.FromResult(((IReadOnlyList<River>)Items, Items.Count));
    }

    public Task<IReadOnlyList<River>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult((IReadOnlyList<River>)Items);
    }

    public Task<River?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
    }

    public Task AddAsync(River river, CancellationToken cancellationToken)
    {
        Items.Add(river);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(River river, CancellationToken cancellationToken)
    {
        Items.Remove(river);
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Items.Count);
    }
}
