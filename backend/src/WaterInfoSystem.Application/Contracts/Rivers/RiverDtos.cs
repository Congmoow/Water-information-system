namespace WaterInfoSystem.Application.Contracts.Rivers;

public record RiverQueryDto(string? Keyword = null, int Page = 1, int PageSize = 10);

public record RiverUpsertDto(
    string Name,
    decimal Length,
    string Basin,
    double Latitude,
    double Longitude,
    string Description);

public record RiverListItemDto(
    Guid Id,
    string Name,
    decimal Length,
    string Basin,
    double Latitude,
    double Longitude,
    string Description,
    DateTime UpdatedAt);

public record RiverDetailDto(
    Guid Id,
    string Name,
    decimal Length,
    string Basin,
    double Latitude,
    double Longitude,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);
