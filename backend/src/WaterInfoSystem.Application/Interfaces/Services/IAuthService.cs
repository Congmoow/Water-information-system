using WaterInfoSystem.Application.Contracts.Auth;

namespace WaterInfoSystem.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

    Task<UserProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken);
}
