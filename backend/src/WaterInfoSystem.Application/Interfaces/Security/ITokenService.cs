using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Application.Interfaces.Security;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(User user);
}
