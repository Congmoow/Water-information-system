namespace WaterInfoSystem.Application.Contracts.Auth;

public record LoginRequestDto(string Username, string Password);

public record UserProfileDto(
    Guid Id,
    string Username,
    string FullName,
    string Role,
    DateTime CreatedAt);

public record LoginResponseDto(
    string Token,
    DateTime ExpiresAt,
    UserProfileDto User);
