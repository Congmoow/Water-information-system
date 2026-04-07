using WaterInfoSystem.Application.Contracts.Monitoring;
using WaterInfoSystem.Application.Interfaces.Repositories;
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
        var service = new MonitoringService(monitoringRepository, stationRepository, alarmRepository);
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
        var service = new MonitoringService(monitoringRepository, stationRepository, alarmRepository);

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

    private sealed class FakeMonitoringRepository : IMonitoringRepository
    {
        public List<MonitoringData> Items { get; } = [];

        public Task<(IReadOnlyList<MonitoringData> Items, int Total)> SearchAsync(Guid? stationId, MonitoringDataType? dataType, DateTime? startTime, DateTime? endTime, int page, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<MonitoringData> query = Items;

            if (stationId.HasValue)
            {
                query = query.Where(x => x.StationId == stationId.Value);
            }

            if (dataType.HasValue)
            {
                query = query.Where(x => x.DataType == dataType.Value);
            }

            if (startTime.HasValue)
            {
                query = query.Where(x => x.CollectedAt >= startTime.Value);
            }

            if (endTime.HasValue)
            {
                query = query.Where(x => x.CollectedAt <= endTime.Value);
            }

            var items = query.ToList();
            return Task.FromResult(((IReadOnlyList<MonitoringData>)items, items.Count));
        }

        public Task<IReadOnlyList<MonitoringData>> GetRecentByTypeAsync(MonitoringDataType dataType, int take, CancellationToken cancellationToken)
        {
            IReadOnlyList<MonitoringData> items = Items
                .Where(x => x.DataType == dataType)
                .OrderByDescending(x => x.CollectedAt)
                .Take(take)
                .ToList();
            return Task.FromResult(items);
        }

        public Task AddAsync(MonitoringData monitoringData, CancellationToken cancellationToken)
        {
            Items.Add(monitoringData);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeStationRepository : IStationRepository
    {
        private readonly List<Station> _items;

        public FakeStationRepository(params Station[] items)
        {
            _items = items.ToList();
        }

        public Task<(IReadOnlyList<Station> Items, int Total)> SearchAsync(string? keyword, StationType? type, StationStatus? status, int page, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<Station> query = _items;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            if (type.HasValue)
            {
                query = query.Where(x => x.Type == type.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var items = query.ToList();
            return Task.FromResult(((IReadOnlyList<Station>)items, items.Count));
        }

        public Task<IReadOnlyList<Station>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((IReadOnlyList<Station>)_items);
        }

        public Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(Station station, CancellationToken cancellationToken)
        {
            _items.Add(station);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Station station, CancellationToken cancellationToken)
        {
            _items.Remove(station);
            return Task.CompletedTask;
        }

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_items.Count);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    private sealed class FakeAlarmRepository : IAlarmRepository
    {
        public List<AlarmRecord> Items { get; } = [];

        public Task<(IReadOnlyList<AlarmRecord> Items, int Total)> SearchAsync(Guid? stationId, AlarmLevel? level, AlarmStatus? status, DateTime? startTime, DateTime? endTime, int page, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<AlarmRecord> query = Items;

            if (stationId.HasValue)
            {
                query = query.Where(x => x.StationId == stationId.Value);
            }

            if (level.HasValue)
            {
                query = query.Where(x => x.Level == level.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (startTime.HasValue)
            {
                query = query.Where(x => x.TriggeredAt >= startTime.Value);
            }

            if (endTime.HasValue)
            {
                query = query.Where(x => x.TriggeredAt <= endTime.Value);
            }

            var items = query.ToList();
            return Task.FromResult(((IReadOnlyList<AlarmRecord>)items, items.Count));
        }

        public Task<IReadOnlyList<AlarmRecord>> GetRecentAsync(int take, CancellationToken cancellationToken)
        {
            IReadOnlyList<AlarmRecord> items = Items.OrderByDescending(x => x.TriggeredAt).Take(take).ToList();
            return Task.FromResult(items);
        }

        public Task<IReadOnlyList<AlarmRecord>> GetTriggeredOnDateAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var items = Items.Where(x => DateOnly.FromDateTime(x.TriggeredAt) == date).ToList();
            return Task.FromResult((IReadOnlyList<AlarmRecord>)items);
        }

        public Task<AlarmRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(AlarmRecord alarmRecord, CancellationToken cancellationToken)
        {
            Items.Add(alarmRecord);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
