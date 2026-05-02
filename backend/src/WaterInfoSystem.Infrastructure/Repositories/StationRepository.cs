using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Infrastructure.Persistence;

namespace WaterInfoSystem.Infrastructure.Repositories;

public class StationRepository : IStationRepository
{
    private readonly WaterInfoDbContext _dbContext;

    public StationRepository(WaterInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<Station> Items, int Total)> SearchAsync(
        string? keyword,
        StationType? type,
        StationStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Stations
            .Include(x => x.Reservoir)
            .Include(x => x.River)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword));
        }

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Station>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Stations
            .Include(x => x.Reservoir)
            .Include(x => x.River)
            .OrderBy(x => x.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task<Station?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Stations
            .Include(x => x.Reservoir)
            .Include(x => x.River)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Station station, CancellationToken cancellationToken)
    {
        await _dbContext.Stations.AddAsync(station, cancellationToken);
    }

    public Task DeleteAsync(Station station, CancellationToken cancellationToken)
    {
        _dbContext.Stations.Remove(station);
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Stations.CountAsync(cancellationToken);
    }

    public Task<bool> ExistsByRiverIdAsync(Guid riverId, CancellationToken cancellationToken)
    {
        return _dbContext.Stations.AnyAsync(x => x.RiverId == riverId, cancellationToken);
    }

    public Task<bool> ExistsByReservoirIdAsync(Guid reservoirId, CancellationToken cancellationToken)
    {
        return _dbContext.Stations.AnyAsync(x => x.ReservoirId == reservoirId, cancellationToken);
    }
}
