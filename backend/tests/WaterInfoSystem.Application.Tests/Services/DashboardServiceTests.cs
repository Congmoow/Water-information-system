using WaterInfoSystem.Application.Tests.Fakes;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Tests.Services;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetOverviewAsync_ShouldReturnCorrectStatistics()
    {
        var station1 = new Station { Id = Guid.NewGuid(), Name = "站点1", Status = StationStatus.Online, Type = StationType.WaterLevel };
        var station2 = new Station { Id = Guid.NewGuid(), Name = "站点2", Status = StationStatus.Offline, Type = StationType.Rainfall };
        var station3 = new Station { Id = Guid.NewGuid(), Name = "站点3", Status = StationStatus.Warning, Type = StationType.WaterLevel };

        var reservoirRepository = new FakeReservoirRepository(
            new Reservoir { Id = Guid.NewGuid(), Name = "水库1" },
            new Reservoir { Id = Guid.NewGuid(), Name = "水库2" });

        var riverRepository = new FakeRiverRepository();
        riverRepository.Items.Add(new River { Id = Guid.NewGuid(), Name = "河道1" });

        var stationRepository = new FakeStationRepository(station1, station2, station3);

        var today = DateTime.UtcNow.Date;
        var alarmRepository = new FakeAlarmRepository(
            new AlarmRecord
            {
                Id = Guid.NewGuid(),
                StationId = station1.Id,
                Station = station1,
                Level = AlarmLevel.Warning,
                Status = AlarmStatus.Pending,
                Message = "水位偏高",
                TriggeredAt = today.AddHours(10)
            },
            new AlarmRecord
            {
                Id = Guid.NewGuid(),
                StationId = station2.Id,
                Station = station2,
                Level = AlarmLevel.Critical,
                Status = AlarmStatus.Pending,
                Message = "严重告警",
                TriggeredAt = today.AddHours(12)
            });

        var monitoringRepository = new FakeMonitoringRepository();
        monitoringRepository.Items.Add(new MonitoringData
        {
            Id = Guid.NewGuid(),
            StationId = station1.Id,
            Station = station1,
            DataType = MonitoringDataType.WaterLevel,
            Value = 18.5m,
            CollectedAt = today.AddDays(-1)
        });
        monitoringRepository.Items.Add(new MonitoringData
        {
            Id = Guid.NewGuid(),
            StationId = station2.Id,
            Station = station2,
            DataType = MonitoringDataType.Rainfall,
            Value = 25.0m,
            CollectedAt = today.AddDays(-1)
        });

        var service = new DashboardService(
            reservoirRepository,
            riverRepository,
            stationRepository,
            monitoringRepository,
            alarmRepository);

        var result = await service.GetOverviewAsync(CancellationToken.None);

        Assert.Equal(2, result.ReservoirCount);
        Assert.Equal(1, result.RiverCount);
        Assert.Equal(3, result.StationCount);
        Assert.Equal(1, result.OnlineStationCount);
        Assert.Equal(2, result.TodayAlarmCount);
        Assert.NotEmpty(result.WaterLevelTrend);
        Assert.NotEmpty(result.RainfallTrend);
        Assert.NotEmpty(result.AlarmLevelStats);
        Assert.Equal(3, result.StationStatusStats.Count);
        Assert.NotEmpty(result.RecentAlarms);
    }
}
