using WaterInfoSystem.Application.Interfaces.Security;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Tests.Fakes;

internal sealed class FakeTokenService : ITokenService
{
    public (string Token, DateTime ExpiresAt) CreateToken(User user)
    {
        return ($"token-{user.Username}", new DateTime(2026, 4, 7, 9, 0, 0, DateTimeKind.Utc));
    }
}
