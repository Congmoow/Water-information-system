using WaterInfoSystem.Application.Contracts.Rivers;
using WaterInfoSystem.Application.Tests.Fakes;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Shared.Exceptions;

namespace WaterInfoSystem.Application.Tests.Services;

public class RiverServiceTests
{
    [Fact]
    public async Task SearchAsync_ShouldReturnAllItems()
    {
        var repository = new FakeRiverRepository();
        repository.Items.Add(new River { Id = Guid.NewGuid(), Name = "长江", Length = 6300m, Basin = "长江流域" });
        repository.Items.Add(new River { Id = Guid.NewGuid(), Name = "黄河", Length = 5464m, Basin = "黄河流域" });
        var service = new RiverService(repository, new FakeStationRepository(), new FakeUnitOfWork());

        var result = await service.SearchAsync(new RiverQueryDto(), CancellationToken.None);

        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDetail_WhenExists()
    {
        var riverId = Guid.NewGuid();
        var repository = new FakeRiverRepository();
        repository.Items.Add(new River
        {
            Id = riverId,
            Name = "长江",
            Length = 6300m,
            Basin = "长江流域",
            Latitude = 30.0,
            Longitude = 114.0,
            Description = "中国最长河流"
        });
        var service = new RiverService(repository, new FakeStationRepository(), new FakeUnitOfWork());

        var result = await service.GetByIdAsync(riverId, CancellationToken.None);

        Assert.Equal(riverId, result.Id);
        Assert.Equal("长江", result.Name);
        Assert.Equal(6300m, result.Length);
        Assert.Equal("长江流域", result.Basin);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowNotFoundException_WhenNotExists()
    {
        var service = new RiverService(new FakeRiverRepository(), new FakeStationRepository(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistRiverAndReturnDetail()
    {
        var repository = new FakeRiverRepository();
        var service = new RiverService(repository, new FakeStationRepository(), new FakeUnitOfWork());
        var request = new RiverUpsertDto("长江", 6300m, "长江流域", 30.0, 114.0, "中国最长河流");

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("长江", result.Name);
        Assert.Single(repository.Items);
        Assert.Equal("长江流域", repository.Items[0].Basin);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRiver()
    {
        var riverId = Guid.NewGuid();
        var repository = new FakeRiverRepository();
        repository.Items.Add(new River { Id = riverId, Name = "长江" });
        var service = new RiverService(repository, new FakeStationRepository(), new FakeUnitOfWork());

        await service.DeleteAsync(riverId, CancellationToken.None);

        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenHasAssociatedStations()
    {
        var riverId = Guid.NewGuid();
        var repository = new FakeRiverRepository();
        repository.Items.Add(new River { Id = riverId, Name = "长江" });

        var stationRepository = new FakeStationRepository(
            new Station { Id = Guid.NewGuid(), Name = "长江1号站", RiverId = riverId });
        var service = new RiverService(repository, stationRepository, new FakeUnitOfWork());

        await Assert.ThrowsAsync<AppException>(
            () => service.DeleteAsync(riverId, CancellationToken.None));
    }
}
