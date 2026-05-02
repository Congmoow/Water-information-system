using WaterInfoSystem.Application.Contracts.Auth;
using WaterInfoSystem.Application.Tests.Fakes;
using WaterInfoSystem.Application.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Shared.Exceptions;

namespace WaterInfoSystem.Application.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldReturnTokenAndProfile_WhenCredentialsAreValid()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            PasswordHash = "hashed-password",
            FullName = "张伟",
            Role = UserRole.Administrator,
            CreatedAt = new DateTime(2026, 4, 6, 9, 0, 0, DateTimeKind.Utc)
        };

        var repository = new FakeUserRepository(user);
        var passwordHasher = new FakePasswordHasher(true);
        var tokenService = new FakeTokenService();
        var service = new AuthService(repository, passwordHasher, tokenService);

        var result = await service.LoginAsync(new LoginRequestDto("admin", "Admin@123"), CancellationToken.None);

        Assert.Equal("token-admin", result.Token);
        Assert.Equal("admin", result.User.Username);
        Assert.Equal("张伟", result.User.FullName);
        Assert.Equal(UserRole.Administrator, result.User.Role);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "viewer",
            PasswordHash = "hashed-password",
            FullName = "李敏",
            Role = UserRole.User
        };

        var service = new AuthService(new FakeUserRepository(user), new FakePasswordHasher(false), new FakeTokenService());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(new LoginRequestDto("viewer", "wrong"), CancellationToken.None));
    }

    [Fact]
    public async Task GetProfileAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var service = new AuthService(new FakeUserRepository(null), new FakePasswordHasher(true), new FakeTokenService());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetProfileAsync(Guid.NewGuid(), CancellationToken.None));
    }
}
