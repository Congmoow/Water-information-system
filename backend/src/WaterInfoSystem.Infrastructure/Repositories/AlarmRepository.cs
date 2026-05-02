using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Infrastructure.Persistence;

namespace WaterInfoSystem.Infrastructure.Repositories;

public class AlarmRepository : IAlarmRepository
{
    private readonly WaterInfoDbContext _dbContext;

    public AlarmRepository(WaterInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<AlarmRecord> Items, int Total)> SearchAsync(
        Guid? stationId,
        AlarmLevel? level,
        AlarmStatus? status,
        DateTime? startTime,
        DateTime? endTime,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.AlarmRecords
            .Include(x => x.Station)
            .AsQueryable();

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

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.TriggeredAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<(AlarmLevel Level, int Count)>> GetLevelCountsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.AlarmRecords
            .GroupBy(x => x.Level)
            .Select(g => new ValueTuple<AlarmLevel, int>(g.Key, g.Count()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AlarmRecord>> GetRecentAsync(int take, CancellationToken cancellationToken)
    {
        return await _dbContext.AlarmRecords
            .Include(x => x.Station)
            .OrderByDescending(x => x.TriggeredAt)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AlarmRecord>> GetTriggeredOnDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = date.ToDateTime(TimeOnly.MaxValue);

        return await _dbContext.AlarmRecords
            .Where(x => x.TriggeredAt >= start && x.TriggeredAt <= end)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<AlarmRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.AlarmRecords
            .Include(x => x.Station)
            .Include(x => x.MonitoringData)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(AlarmRecord alarmRecord, CancellationToken cancellationToken)
    {
        await _dbContext.AlarmRecords.AddAsync(alarmRecord, cancellationToken);
    }
}
