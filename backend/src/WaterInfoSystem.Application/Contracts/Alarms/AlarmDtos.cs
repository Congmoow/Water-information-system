using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Contracts.Alarms;

public record AlarmQueryDto(
    Guid? StationId = null,
    AlarmLevel? Level = null,
    AlarmStatus? Status = null,
    DateTime? StartTime = null,
    DateTime? EndTime = null,
    int Page = 1,
    int PageSize = 20);

public record AlarmCreateDto(
    Guid StationId,
    AlarmType AlarmType,
    AlarmLevel Level,
    string Message,
    DateTime TriggeredAt);

public record AlarmHandleDto(
    AlarmStatus Status,
    string? HandleRemark);

public record AlarmListItemDto(
    Guid Id,
    Guid StationId,
    string StationName,
    AlarmType AlarmType,
    AlarmLevel Level,
    AlarmStatus Status,
    string Message,
    DateTime TriggeredAt,
    DateTime? HandledAt);

public record AlarmDetailDto(
    Guid Id,
    Guid StationId,
    string StationName,
    AlarmType AlarmType,
    AlarmLevel Level,
    AlarmStatus Status,
    string Message,
    DateTime TriggeredAt,
    DateTime? HandledAt,
    string? HandleRemark,
    Guid? MonitoringDataId,
    Guid? HandledByUserId);
