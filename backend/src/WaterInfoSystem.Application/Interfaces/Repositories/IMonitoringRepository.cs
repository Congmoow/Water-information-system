using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Interfaces.Repositories;

public interface IMonitoringRepository
{
    Task<(IReadOnlyList<MonitoringData> Items, int Total)> SearchAsync(
        Guid? stationId,
        MonitoringDataType? dataType,
        DateTime? startTime,
        DateTime? endTime,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MonitoringData>> GetRecentByTypeAsync(MonitoringDataType dataType, int take, CancellationToken cancellationToken);

    Task AddAsync(MonitoringData monitoringData, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
