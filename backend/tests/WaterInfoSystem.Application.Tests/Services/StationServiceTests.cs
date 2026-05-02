using WaterInfoSystem.Application.Contracts.Stations;
using WaterInfoSystem.Application.Tests.Fakes;
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
        var unitOfWork = new FakeUnitOfWork();
        var service = new StationService(stationRepository, reservoirRepository, riverRepository, unitOfWork);

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
        var reservoirRepository = new FakeReservoirRepository(
            new Reservoir { Id = reservoirId, Name = "东湖水库" });
        var riverRepository = new FakeRiverRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new StationService(stationRepository, reservoirRepository, riverRepository, unitOfWork);

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
}
