using WaterInfoSystem.Application.Contracts.Stations;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IStationService
{
    Task<PagedResult<StationListItemDto>> SearchAsync(StationQueryDto query, CancellationToken cancellationToken);

    Task<StationDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<StationDetailDto> CreateAsync(StationUpsertDto request, CancellationToken cancellationToken);

    Task<StationDetailDto> UpdateAsync(Guid id, StationUpsertDto request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
