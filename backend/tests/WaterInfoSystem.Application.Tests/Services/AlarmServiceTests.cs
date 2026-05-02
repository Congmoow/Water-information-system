using WaterInfoSystem.Application.Contracts.Alarms;
using WaterInfoSystem.Application.Tests.Fakes;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Tests.Services;

public class AlarmServiceTests
{
    [Fact]
    public async Task HandleAsync_ShouldUpdateAlarmStateAndHandledMetadata()
    {
        var station = new Station
        {
            Id = Guid.NewGuid(),
            Name = "Alert Station"
        };
        var alarm = new AlarmRecord
        {
            Id = Guid.NewGuid(),
            StationId = station.Id,
            Station = station,
            AlarmType = AlarmType.ThresholdExceeded,
            Level = AlarmLevel.Warning,
            Status = AlarmStatus.Pending,
            Message = "threshold exceeded",
            TriggeredAt = new DateTime(2026, 4, 7, 8, 0, 0)
        };
        var repository = new FakeAlarmRepository(alarm);
        var unitOfWork = new FakeUnitOfWork();
        var service = new AlarmService(repository, new FakeStationRepository(station), unitOfWork);
        var handledBy = Guid.NewGuid();

        var result = await service.HandleAsync(
            alarm.Id,
            new AlarmHandleDto(AlarmStatus.Resolved, "verified and closed", handledBy),
            CancellationToken.None);

        Assert.Equal(AlarmStatus.Resolved, result.Status);
        Assert.NotNull(result.HandledAt);
        Assert.Equal(handledBy, alarm.HandledByUserId);
        Assert.Equal("verified and closed", alarm.HandleRemark);
    }
}
