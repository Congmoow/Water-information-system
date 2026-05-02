using WaterInfoSystem.Application.Contracts.Monitoring;
using WaterInfoSystem.Application.Tests.Fakes;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Tests.Services;

public class MonitoringServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateCriticalAlarm_WhenValueExceedsCriticalThreshold()
    {
        var station = new Station
        {
            Id = Guid.NewGuid(),
            Name = "Test Water Level Station",
            Type = StationType.WaterLevel,
            Status = StationStatus.Online,
            WarningThreshold = 18.5m,
            CriticalThreshold = 20m
        };
        var monitoringRepository = new FakeMonitoringRepository();
        var stationRepository = new FakeStationRepository(station);
        var alarmRepository = new FakeAlarmRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new MonitoringService(monitoringRepository, stationRepository, alarmRepository, unitOfWork);
        var collectedAt = new DateTime(2026, 4, 7, 10, 30, 0);

        var result = await service.CreateAsync(
            new MonitoringCreateDto(
                station.Id,
                MonitoringDataType.WaterLevel,
                20.6m,
                collectedAt,
                "critical"),
            CancellationToken.None);

        Assert.True(result.TriggeredAlarm);
        Assert.NotNull(result.AlarmId);
        Assert.Single(monitoringRepository.Items);
        Assert.Single(alarmRepository.Items);
        Assert.Equal(StationStatus.Warning, station.Status);
        Assert.Equal(collectedAt, station.LastActiveAt);
        Assert.Equal(AlarmLevel.Critical, alarmRepository.Items[0].Level);
        Assert.Equal(AlarmType.ThresholdExceeded, alarmRepository.Items[0].AlarmType);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotCreateAlarm_WhenValueIsBelowWarningThreshold()
    {
        var station = new Station
        {
            Id = Guid.NewGuid(),
            Name = "Test Rainfall Station",
            Type = StationType.Rainfall,
            Status = StationStatus.Online,
            WarningThreshold = 30m,
            CriticalThreshold = 50m
        };
        var monitoringRepository = new FakeMonitoringRepository();
        var stationRepository = new FakeStationRepository(station);
        var alarmRepository = new FakeAlarmRepository();
        var unitOfWork = new FakeUnitOfWork();
        var service = new MonitoringService(monitoringRepository, stationRepository, alarmRepository, unitOfWork);

        var result = await service.CreateAsync(
            new MonitoringCreateDto(
                station.Id,
                MonitoringDataType.Rainfall,
                18m,
                new DateTime(2026, 4, 7, 9, 0, 0),
                "normal"),
            CancellationToken.None);

        Assert.False(result.TriggeredAlarm);
        Assert.Null(result.AlarmId);
        Assert.Single(monitoringRepository.Items);
        Assert.Empty(alarmRepository.Items);
        Assert.Equal(StationStatus.Online, station.Status);
    }
}
