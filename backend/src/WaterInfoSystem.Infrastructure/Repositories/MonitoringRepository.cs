using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Infrastructure.Persistence;

namespace WaterInfoSystem.Infrastructure.Repositories;

public class MonitoringRepository : IMonitoringRepository
{
    private readonly WaterInfoDbContext _dbContext;

    public MonitoringRepository(WaterInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<MonitoringData> Items, int Total)> SearchAsync(
        Guid? stationId,
        MonitoringDataType? dataType,
        DateTime? startTime,
        DateTime? endTime,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.MonitoringDatas
            .Include(x => x.Station)
            .Include(x => x.AlarmRecords)
            .AsQueryable();

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

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.CollectedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<MonitoringData>> GetRecentByTypeAsync(MonitoringDataType dataType, int take, CancellationToken cancellationToken)
    {
        return await _dbContext.MonitoringDatas
            .Where(x => x.DataType == dataType)
            .OrderByDescending(x => x.CollectedAt)
            .Take(take)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MonitoringData monitoringData, CancellationToken cancellationToken)
    {
        await _dbContext.MonitoringDatas.AddAsync(monitoringData, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
