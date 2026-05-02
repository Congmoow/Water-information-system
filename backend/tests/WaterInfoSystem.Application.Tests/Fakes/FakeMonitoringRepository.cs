using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal class FakeMonitoringRepository : IMonitoringRepository
{
    public List<MonitoringData> Items { get; } = [];

    public Task<(IReadOnlyList<MonitoringData> Items, int Total)> SearchAsync(
        Guid? stationId, MonitoringDataType? dataType,
        DateTime? startTime, DateTime? endTime,
        int page, int pageSize, CancellationToken cancellationToken)
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
}
