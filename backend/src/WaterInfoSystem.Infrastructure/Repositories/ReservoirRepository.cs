using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Infrastructure.Persistence;

namespace WaterInfoSystem.Infrastructure.Repositories;

public class ReservoirRepository : IReservoirRepository
{
    private readonly WaterInfoDbContext _dbContext;

    public ReservoirRepository(WaterInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<Reservoir> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Reservoirs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) || x.Location.Contains(keyword) || x.ManagementUnit.Contains(keyword));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<Reservoir>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Reservoirs.OrderBy(x => x.Name).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<Reservoir?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Reservoirs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(Reservoir reservoir, CancellationToken cancellationToken)
    {
        await _dbContext.Reservoirs.AddAsync(reservoir, cancellationToken);
    }

    public Task DeleteAsync(Reservoir reservoir, CancellationToken cancellationToken)
    {
        _dbContext.Reservoirs.Remove(reservoir);
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Reservoirs.CountAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
