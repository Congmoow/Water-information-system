using WaterInfoSystem.Application.Contracts.Reservoirs;
using WaterInfoSystem.Application.Tests.Fakes;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Shared.Exceptions;

namespace WaterInfoSystem.Application.Tests.Services;

public class ReservoirServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldPersistReservoirAndReturnDetail()
    {
        var repository = new FakeReservoirRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new ReservoirService(repository, unitOfWork);
        var request = new ReservoirUpsertDto("东湖水库", "江州市东湖新区", 1000m, "江州市水务局", 30.1, 114.2, "测试说明");

        var result = await service.CreateAsync(request, CancellationToken.None);

        Assert.Equal("东湖水库", result.Name);
        Assert.Single(repository.Items);
        Assert.Equal("江州市东湖新区", repository.Items[0].Location);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenReservoirDoesNotExist()
    {
        var service = new ReservoirService(new FakeReservoirRepository(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAsync(Guid.NewGuid(), new ReservoirUpsertDto("西山水库", "西山镇", 900m, "西山区水利站", 30.2, 114.3, "说明"), CancellationToken.None));
    }
}
