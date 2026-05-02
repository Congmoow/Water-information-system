using WaterInfoSystem.Application.Contracts.Reservoirs;
using WaterInfoSystem.Application.Tests.Fakes;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Shared.Exceptions;

namespace WaterInfoSystem.Application.Tests.Services;

public class ReservoirServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistReservoirAndReturnDetail()
    {
        var repository = new FakeReservoirRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new ReservoirService(repository, new FakeStationRepository(), unitOfWork);
        var request = new ReservoirUpsertDto("东湖水库", "江州市东湖新区", 1000m, "江州市水务局", 30.1, 114.2, "测试说明");

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("东湖水库", result.Name);
        Assert.Single(repository.Items);
        Assert.Equal("江州市东湖新区", repository.Items[0].Location);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenReservoirDoesNotExist()
    {
        var service = new ReservoirService(new FakeReservoirRepository(), new FakeStationRepository(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAsync(Guid.NewGuid(), new ReservoirUpsertDto("西山水库", "西山镇", 900m, "西山区水利站", 30.2, 114.3, "说明"), CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnEmpty_WhenNoMatch()
    {
        var repository = new FakeReservoirRepository();
        var service = new ReservoirService(repository, new FakeStationRepository(), new FakeUnitOfWork());

        var result = await service.SearchAsync(new ReservoirQueryDto("不存在的关键词"), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowAppException_WhenStationsExist()
    {
        var reservoir = new Reservoir
        {
            Id = Guid.NewGuid(),
            Name = "东湖水库",
            Location = "江州市东湖新区",
            Capacity = 1000m,
            ManagementUnit = "江州市水务局",
            Latitude = 30.1,
            Longitude = 114.2
        };
        var station = new Station
        {
            Id = Guid.NewGuid(),
            Name = "东湖1号站",
            ReservoirId = reservoir.Id
        };
        var repository = new FakeReservoirRepository(reservoir);
        var stationRepository = new FakeStationRepository(station);
        var service = new ReservoirService(repository, stationRepository, new FakeUnitOfWork());

        await Assert.ThrowsAsync<AppException>(() =>
            service.DeleteAsync(reservoir.Id, CancellationToken.None));
    }
}
