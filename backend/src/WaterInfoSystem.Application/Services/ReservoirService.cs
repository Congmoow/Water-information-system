using WaterInfoSystem.Application.Contracts.Reservoirs;
using WaterInfoSystem.Application.Interfaces;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Shared.Exceptions;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Services;

public class ReservoirService : IReservoirService
{
    private readonly IReservoirRepository _reservoirRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReservoirService(IReservoirRepository reservoirRepository, IUnitOfWork unitOfWork)
    {
        _reservoirRepository = reservoirRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ReservoirListItemDto>> SearchAsync(ReservoirQueryDto query, CancellationToken cancellationToken)
    {
        var (items, total) = await _reservoirRepository.SearchAsync(query.Keyword, query.Page, query.PageSize, cancellationToken);
        return new PagedResult<ReservoirListItemDto>(items.Select(MapListItem).ToList(), total);
    }

    public async Task<ReservoirDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        return MapDetail(entity);
    }

    public async Task<ReservoirDetailDto> CreateAsync(ReservoirUpsertDto request, CancellationToken cancellationToken)
    {
        var entity = new Reservoir();
        ApplyChanges(entity, request);
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _reservoirRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    public async Task<ReservoirDetailDto> UpdateAsync(Guid id, ReservoirUpsertDto request, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        ApplyChanges(entity, request);
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        await _reservoirRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Reservoir> GetRequiredEntityAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _reservoirRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException("指定水库不存在");
        }

        return entity;
    }

    private static void ApplyChanges(Reservoir entity, ReservoirUpsertDto request)
    {
        entity.Name = request.Name;
        entity.Location = request.Location;
        entity.Capacity = request.Capacity;
        entity.ManagementUnit = request.ManagementUnit;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.Description = request.Description;
    }

    private static ReservoirListItemDto MapListItem(Reservoir entity)
    {
        return new ReservoirListItemDto(
            entity.Id,
            entity.Name,
            entity.Location,
            entity.Capacity,
            entity.ManagementUnit,
            entity.Latitude,
            entity.Longitude,
            entity.Description,
            entity.UpdatedAt);
    }

    private static ReservoirDetailDto MapDetail(Reservoir entity)
    {
        return new ReservoirDetailDto(
            entity.Id,
            entity.Name,
            entity.Location,
            entity.Capacity,
            entity.ManagementUnit,
            entity.Latitude,
            entity.Longitude,
            entity.Description,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
