using WaterInfoSystem.Application.Contracts.Alarms;
using WaterInfoSystem.Application.Interfaces.Repositories;
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
        var service = new AlarmService(repository, new FakeStationRepository(station));
        var handledBy = Guid.NewGuid();

        var result = await service.HandleAsync(
            alarm.Id,
            new AlarmHandleDto(AlarmStatus.Resolved, "verified and closed", handledBy),
            CancellationToken.None);

        Assert.Equal("Resolved", result.Status);
        Assert.NotNull(result.HandledAt);
        Assert.Equal(handledBy, alarm.HandledByUserId);
        Assert.Equal("verified and closed", alarm.HandleRemark);
    }

    private sealed class FakeAlarmRepository : IAlarmRepository
    {
        private readonly List<AlarmRecord> _items;

        public FakeAlarmRepository(params AlarmRecord[] items)
        {
            _items = items.ToList();
        }

        public Task<(IReadOnlyList<AlarmRecord> Items, int Total)> SearchAsync(Guid? stationId, AlarmLevel? level, AlarmStatus? status, DateTime? startTime, DateTime? endTime, int page, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<AlarmRecord> query = _items;

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
            IReadOnlyList<AlarmRecord> items = _items.OrderByDescending(x => x.TriggeredAt).Take(take).ToList();
            return Task.FromResult(items);
        }

        public Task<IReadOnlyList<AlarmRecord>> GetTriggeredOnDateAsync(DateOnly date, CancellationToken cancellationToken)
        {
            var items = _items.Where(x => DateOnly.FromDateTime(x.TriggeredAt) == date).ToList();
            return Task.FromResult((IReadOnlyList<AlarmRecord>)items);
        }

        public Task<AlarmRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
        }

        public Task AddAsync(AlarmRecord alarmRecord, CancellationToken cancellationToken)
        {
            _items.Add(alarmRecord);
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
            return Task.FromResult(((IReadOnlyList<Station>)_items, _items.Count));
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
}
