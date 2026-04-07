namespace WaterInfoSystem.Application.Contracts.Maps;

public record MapPointDto(
    Guid Id,
    string Name,
    string Type,
    double Latitude,
    double Longitude,
    string Description,
    string? Status,
    string Source);

public record MapDataDto(IReadOnlyList<MapPointDto> Items);
