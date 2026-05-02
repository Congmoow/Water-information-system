using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal class FakeAlarmRepository : IAlarmRepository
{
    public List<AlarmRecord> Items { get; }

    public FakeAlarmRepository(params AlarmRecord[] items)
    {
        Items = items.ToList();
    }

    public Task<(IReadOnlyList<AlarmRecord> Items, int Total)> SearchAsync(
        Guid? stationId, AlarmLevel? level, AlarmStatus? status,
        DateTime? startTime, DateTime? endTime,
        int page, int pageSize, CancellationToken cancellationToken)
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

    public Task<IReadOnlyList<(AlarmLevel Level, int Count)>> GetLevelCountsAsync(CancellationToken cancellationToken)
    {
        var counts = Items
            .GroupBy(x => x.Level)
            .Select(g => (g.Key, g.Count()))
            .ToList();
        return Task.FromResult((IReadOnlyList<(AlarmLevel Level, int Count)>)counts);
    }
}
