using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Contracts.Stations;

public record StationQueryDto(
    string? Keyword = null,
    StationType? Type = null,
    StationStatus? Status = null,
    int Page = 1,
    int PageSize = 10);

public record StationUpsertDto(
    string Name,
    StationType Type,
    double Longitude,
    double Latitude,
    StationStatus Status,
    decimal WarningThreshold,
    decimal CriticalThreshold,
    string Description,
    DateTime? LastActiveAt,
    Guid? ReservoirId,
    Guid? RiverId);

public record StationListItemDto(
    Guid Id,
    string Name,
    StationType Type,
    double Longitude,
    double Latitude,
    StationStatus Status,
    decimal WarningThreshold,
    decimal CriticalThreshold,
    string Description,
    string? ParentName,
    DateTime? LastActiveAt,
    DateTime UpdatedAt);

public record StationDetailDto(
    Guid Id,
    string Name,
    StationType Type,
    double Longitude,
    double Latitude,
    StationStatus Status,
    decimal WarningThreshold,
    decimal CriticalThreshold,
    string Description,
    Guid? ReservoirId,
    string? ReservoirName,
    Guid? RiverId,
    string? RiverName,
    DateTime? LastActiveAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);
