using WaterInfoSystem.Application.Contracts.Stations;
using WaterInfoSystem.Application.Interfaces;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Shared.Exceptions;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Services;

public class StationService : IStationService
{
    private readonly IStationRepository _stationRepository;
    private readonly IReservoirRepository _reservoirRepository;
    private readonly IRiverRepository _riverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StationService(
        IStationRepository stationRepository,
        IReservoirRepository reservoirRepository,
        IRiverRepository riverRepository,
        IUnitOfWork unitOfWork)
    {
        _stationRepository = stationRepository;
        _reservoirRepository = reservoirRepository;
        _riverRepository = riverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<StationListItemDto>> SearchAsync(StationQueryDto query, CancellationToken cancellationToken)
    {
        var (items, total) = await _stationRepository.SearchAsync(
            query.Keyword,
            query.Type,
            query.Status,
            query.Page,
            query.PageSize,
            cancellationToken);

        return new PagedResult<StationListItemDto>(items.Select(MapListItem).ToList(), total);
    }

    public async Task<StationDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        return MapDetail(entity);
    }

    public async Task<StationDetailDto> CreateAsync(StationUpsertDto request, CancellationToken cancellationToken)
    {
        var entity = new Station
        {
            CreatedAt = DateTime.UtcNow
        };

        var parents = await ResolveParentsAsync(request, cancellationToken);
        ApplyChanges(entity, request);
        entity.Reservoir = parents.Reservoir;
        entity.River = parents.River;
        entity.UpdatedAt = DateTime.UtcNow;

        await _stationRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await GetRequiredEntityAsync(entity.Id, cancellationToken);
        return MapDetail(created);
    }

    public async Task<StationDetailDto> UpdateAsync(Guid id, StationUpsertDto request, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        var parents = await ResolveParentsAsync(request, cancellationToken);
        ApplyChanges(entity, request);
        entity.Reservoir = parents.Reservoir;
        entity.River = parents.River;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        await _stationRepository.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<(Reservoir? Reservoir, River? River)> ResolveParentsAsync(StationUpsertDto request, CancellationToken cancellationToken)
    {
        Reservoir? reservoir = null;
        River? river = null;

        if (request.ReservoirId.HasValue)
        {
            reservoir = await _reservoirRepository.GetByIdAsync(request.ReservoirId.Value, cancellationToken);
            if (reservoir is null)
            {
                throw new NotFoundException("关联水库不存在");
            }
        }

        if (request.RiverId.HasValue)
        {
            river = await _riverRepository.GetByIdAsync(request.RiverId.Value, cancellationToken);
            if (river is null)
            {
                throw new NotFoundException("关联河道不存在");
            }
        }

        return (reservoir, river);
    }

    private async Task<Station> GetRequiredEntityAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _stationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException("指定站点不存在");
        }

        return entity;
    }

    private static void ApplyChanges(Station entity, StationUpsertDto request)
    {
        entity.Name = request.Name;
        entity.Type = request.Type;
        entity.Longitude = request.Longitude;
        entity.Latitude = request.Latitude;
        entity.Status = request.Status;
        entity.WarningThreshold = request.WarningThreshold;
        entity.CriticalThreshold = request.CriticalThreshold;
        entity.Description = request.Description;
        entity.LastActiveAt = request.LastActiveAt;
        entity.ReservoirId = request.ReservoirId;
        entity.RiverId = request.RiverId;
    }

    private static StationListItemDto MapListItem(Station entity)
    {
        return new StationListItemDto(
            entity.Id,
            entity.Name,
            entity.Type,
            entity.Longitude,
            entity.Latitude,
            entity.Status,
            entity.WarningThreshold,
            entity.CriticalThreshold,
            entity.Description,
            entity.Reservoir?.Name ?? entity.River?.Name,
            entity.LastActiveAt,
            entity.UpdatedAt);
    }

    private static StationDetailDto MapDetail(Station entity)
    {
        return new StationDetailDto(
            entity.Id,
            entity.Name,
            entity.Type,
            entity.Longitude,
            entity.Latitude,
            entity.Status,
            entity.WarningThreshold,
            entity.CriticalThreshold,
            entity.Description,
            entity.ReservoirId,
            entity.Reservoir?.Name,
            entity.RiverId,
            entity.River?.Name,
            entity.LastActiveAt,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
