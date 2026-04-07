using WaterInfoSystem.Application.Contracts.Stations;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Shared.Exceptions;

namespace WaterInfoSystem.Application.Tests.Services;

public class StationServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldThrowNotFoundException_WhenParentReservoirDoesNotExist()
    {
        var stationRepository = new FakeStationRepository();
        var reservoirRepository = new FakeReservoirRepository();
        var riverRepository = new FakeRiverRepository();
        var service = new StationService(stationRepository, reservoirRepository, riverRepository);

        var request = new StationUpsertDto(
            "东湖1号水位站",
            StationType.WaterLevel,
            114.3,
            30.6,
            StationStatus.Online,
            18.5m,
            20m,
            "测试站点",
            DateTime.Now,
            Guid.NewGuid(),
            null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistStation_WhenParentExists()
    {
        var reservoirId = Guid.NewGuid();
        var stationRepository = new FakeStationRepository();
        var reservoirRepository = new FakeReservoirRepository
        {
            Reservoir = new Reservoir { Id = reservoirId, Name = "东湖水库" }
        };
        var riverRepository = new FakeRiverRepository();
        var service = new StationService(stationRepository, reservoirRepository, riverRepository);

        var request = new StationUpsertDto(
            "东湖1号水位站",
            StationType.WaterLevel,
            114.3,
            30.6,
            StationStatus.Online,
            18.5m,
            20m,
            "测试站点",
            DateTime.Now,
            reservoirId,
            null);

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("东湖1号水位站", result.Name);
        Assert.Single(stationRepository.Items);
        Assert.Equal("东湖水库", result.ReservoirName);
    }

    private sealed class FakeStationRepository : IStationRepository
    {
        public List<Station> Items { get; } = [];

        public Task<(IReadOnlyList<Station> Items, int Total)> SearchAsync(string? keyword, StationType? type, StationStatus? status, int page, int pageSize, CancellationToken cancellationToken)
        {
            return Task.FromResult(((IReadOnlyList<Station>)Items, Items.Count));
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

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeReservoirRepository : IReservoirRepository
    {
        public Reservoir? Reservoir { get; init; }

        public Task<(IReadOnlyList<Reservoir> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken)
        {
            var items = Reservoir is null ? Array.Empty<Reservoir>() : [Reservoir];
            return Task.FromResult(((IReadOnlyList<Reservoir>)items, items.Length));
        }

        public Task<IReadOnlyList<Reservoir>> GetAllAsync(CancellationToken cancellationToken)
        {
            IReadOnlyList<Reservoir> items = Reservoir is null ? [] : [Reservoir];
            return Task.FromResult(items);
        }

        public Task<Reservoir?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Reservoir?.Id == id ? Reservoir : null);
        }

        public Task AddAsync(Reservoir reservoir, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task DeleteAsync(Reservoir reservoir, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<int> CountAsync(CancellationToken cancellationToken) => Task.FromResult(Reservoir is null ? 0 : 1);
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    private sealed class FakeRiverRepository : IRiverRepository
    {
        public Task<(IReadOnlyList<River> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken)
        {
            return Task.FromResult(((IReadOnlyList<River>)Array.Empty<River>(), 0));
        }

        public Task<IReadOnlyList<River>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IReadOnlyList<River>)Array.Empty<River>());
        }

        public Task<River?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult<River?>(null);
        }

        public Task AddAsync(River river, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task DeleteAsync(River river, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task<int> CountAsync(CancellationToken cancellationToken) => Task.FromResult(0);
        public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
