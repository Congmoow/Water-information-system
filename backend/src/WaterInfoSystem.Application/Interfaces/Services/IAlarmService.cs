using WaterInfoSystem.Application.Contracts.Alarms;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IAlarmService
{
    Task<PagedResult<AlarmListItemDto>> SearchAsync(AlarmQueryDto query, CancellationToken cancellationToken);

    Task<AlarmDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<AlarmDetailDto> CreateAsync(AlarmCreateDto request, CancellationToken cancellationToken);

    Task<AlarmDetailDto> HandleAsync(Guid id, AlarmHandleDto request, CancellationToken cancellationToken);
}
