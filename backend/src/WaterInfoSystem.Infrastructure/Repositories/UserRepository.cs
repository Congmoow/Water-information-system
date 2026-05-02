using Microsoft.EntityFrameworkCore;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Infrastructure.Persistence;

namespace WaterInfoSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly WaterInfoDbContext _dbContext;

    public UserRepository(WaterInfoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == username, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken)
    {
        return _dbContext.Users.AnyAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddRangeAsync(users, cancellationToken);
    }
}
