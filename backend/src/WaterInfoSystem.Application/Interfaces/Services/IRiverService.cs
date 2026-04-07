using WaterInfoSystem.Application.Contracts.Rivers;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IRiverService
{
    Task<PagedResult<RiverListItemDto>> SearchAsync(RiverQueryDto query, CancellationToken cancellationToken);

    Task<RiverDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<RiverDetailDto> CreateAsync(RiverUpsertDto request, CancellationToken cancellationToken);

    Task<RiverDetailDto> UpdateAsync(Guid id, RiverUpsertDto request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
