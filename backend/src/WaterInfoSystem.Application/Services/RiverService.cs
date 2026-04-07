using WaterInfoSystem.Application.Contracts.Rivers;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Shared.Exceptions;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Services;

public class RiverService : IRiverService
{
    private readonly IRiverRepository _riverRepository;

    public RiverService(IRiverRepository riverRepository)
    {
        _riverRepository = riverRepository;
    }

    public async Task<PagedResult<RiverListItemDto>> SearchAsync(RiverQueryDto query, CancellationToken cancellationToken)
    {
        var (items, total) = await _riverRepository.SearchAsync(query.Keyword, query.Page, query.PageSize, cancellationToken);
        return new PagedResult<RiverListItemDto>(items.Select(MapListItem).ToList(), total);
    }

    public async Task<RiverDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        return MapDetail(entity);
    }

    public async Task<RiverDetailDto> CreateAsync(RiverUpsertDto request, CancellationToken cancellationToken)
    {
        var entity = new River();
        ApplyChanges(entity, request);
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;

        await _riverRepository.AddAsync(entity, cancellationToken);
        await _riverRepository.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    public async Task<RiverDetailDto> UpdateAsync(Guid id, RiverUpsertDto request, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        ApplyChanges(entity, request);
        entity.UpdatedAt = DateTime.Now;

        await _riverRepository.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredEntityAsync(id, cancellationToken);
        await _riverRepository.DeleteAsync(entity, cancellationToken);
        await _riverRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<River> GetRequiredEntityAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _riverRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException("指定河道不存在");
        }

        return entity;
    }

    private static void ApplyChanges(River entity, RiverUpsertDto request)
    {
        entity.Name = request.Name;
        entity.Length = request.Length;
        entity.Basin = request.Basin;
        entity.Latitude = request.Latitude;
        entity.Longitude = request.Longitude;
        entity.Description = request.Description;
    }

    private static RiverListItemDto MapListItem(River entity)
    {
        return new RiverListItemDto(
            entity.Id,
            entity.Name,
            entity.Length,
            entity.Basin,
            entity.Latitude,
            entity.Longitude,
            entity.Description,
            entity.UpdatedAt);
    }

    private static RiverDetailDto MapDetail(River entity)
    {
        return new RiverDetailDto(
            entity.Id,
            entity.Name,
            entity.Length,
            entity.Basin,
            entity.Latitude,
            entity.Longitude,
            entity.Description,
            entity.CreatedAt,
            entity.UpdatedAt);
    }
}
