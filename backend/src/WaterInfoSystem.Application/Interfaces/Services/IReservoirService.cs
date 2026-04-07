using WaterInfoSystem.Application.Contracts.Reservoirs;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IReservoirService
{
    Task<PagedResult<ReservoirListItemDto>> SearchAsync(ReservoirQueryDto query, CancellationToken cancellationToken);

    Task<ReservoirDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ReservoirDetailDto> CreateAsync(ReservoirUpsertDto request, CancellationToken cancellationToken);

    Task<ReservoirDetailDto> UpdateAsync(Guid id, ReservoirUpsertDto request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
