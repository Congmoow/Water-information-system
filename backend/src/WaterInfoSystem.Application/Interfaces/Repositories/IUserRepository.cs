using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> AnyAsync(CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken);
}
