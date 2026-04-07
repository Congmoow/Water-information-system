namespace WaterInfoSystem.Application.Contracts.Reservoirs;

public record ReservoirQueryDto(string? Keyword = null, int Page = 1, int PageSize = 10);

public record ReservoirUpsertDto(
    string Name,
    string Location,
    decimal Capacity,
    string ManagementUnit,
    double Latitude,
    double Longitude,
    string Description);

public record ReservoirListItemDto(
    Guid Id,
    string Name,
    string Location,
    decimal Capacity,
    string ManagementUnit,
    double Latitude,
    double Longitude,
    string Description,
    DateTime UpdatedAt);

public record ReservoirDetailDto(
    Guid Id,
    string Name,
    string Location,
    decimal Capacity,
    string ManagementUnit,
    double Latitude,
    double Longitude,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);
