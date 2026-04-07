using WaterInfoSystem.Application.Contracts.Monitoring;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IMonitoringService
{
    Task<PagedResult<MonitoringListItemDto>> SearchAsync(MonitoringQueryDto query, CancellationToken cancellationToken);

    Task<IReadOnlyList<MonitoringTrendPointDto>> GetHistoryAsync(MonitoringQueryDto query, CancellationToken cancellationToken);

    Task<MonitoringCreateResultDto> CreateAsync(MonitoringCreateDto request, CancellationToken cancellationToken);
}
