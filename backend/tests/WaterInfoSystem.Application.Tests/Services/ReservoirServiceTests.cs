using WaterInfoSystem.Application.Contracts.Reservoirs;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Shared.Exceptions;

namespace WaterInfoSystem.Application.Tests.Services;

public class ReservoirServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistReservoirAndReturnDetail()
    {
        var repository = new FakeReservoirRepository();
        var service = new ReservoirService(repository);
        var request = new ReservoirUpsertDto("东湖水库", "江州市东湖新区", 1000m, "江州市水务局", 30.1, 114.2, "测试说明");

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("东湖水库", result.Name);
        Assert.Single(repository.Items);
        Assert.Equal("江州市东湖新区", repository.Items[0].Location);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenReservoirDoesNotExist()
    {
        var service = new ReservoirService(new FakeReservoirRepository());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAsync(Guid.NewGuid(), new ReservoirUpsertDto("西山水库", "西山镇", 900m, "西山区水利站", 30.2, 114.3, "说明"), CancellationToken.None));
    }

    private sealed class FakeReservoirRepository : IReservoirRepository
    {
        public List<Reservoir> Items { get; } = [];

        public Task<(IReadOnlyList<Reservoir> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken)
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

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
