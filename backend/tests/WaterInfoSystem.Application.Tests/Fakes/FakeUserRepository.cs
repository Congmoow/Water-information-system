using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly User? _user;

    public FakeUserRepository(User? user)
    {
        _user = user;
    }

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return Task.FromResult(_user?.Username == username ? _user : null);
    }

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_user?.Id == userId ? _user : null);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_user is not null);
    }

    public Task AddRangeAsync(IEnumerable<User> users, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }
}
