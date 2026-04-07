using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Infrastructure.Persistence;

namespace WaterInfoSystem.Infrastructure.Repositories;

public class RiverRepository : IRiverRepository
{
    private readonly WaterInfoDbContext _dbContext;

    public RiverRepository(WaterInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(IReadOnlyList<River> Items, int Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _dbContext.Rivers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword) || x.Basin.Contains(keyword));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<River>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Rivers.OrderBy(x => x.Name).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<River?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Rivers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddAsync(River river, CancellationToken cancellationToken)
    {
        await _dbContext.Rivers.AddAsync(river, cancellationToken);
    }

    public Task DeleteAsync(River river, CancellationToken cancellationToken)
    {
        _dbContext.Rivers.Remove(river);
        return Task.CompletedTask;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Rivers.CountAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
