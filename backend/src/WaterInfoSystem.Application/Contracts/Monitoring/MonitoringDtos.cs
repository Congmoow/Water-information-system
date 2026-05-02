using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Contracts.Monitoring;

public record MonitoringQueryDto(
    Guid? StationId = null,
    MonitoringDataType? DataType = null,
    DateTime? StartTime = null,
    DateTime? EndTime = null,
    int Page = 1,
    int PageSize = 20);

public record MonitoringCreateDto(
    Guid StationId,
    MonitoringDataType DataType,
    decimal Value,
    DateTime CollectedAt,
    string? Remark);

public record MonitoringListItemDto(
    Guid Id,
    Guid StationId,
    string StationName,
    MonitoringDataType DataType,
    decimal Value,
    DateTime CollectedAt,
    bool TriggeredAlarm,
    string? Remark);

public record MonitoringTrendPointDto(string Label, decimal Value);

public record MonitoringCreateResultDto(
    Guid Id,
    bool TriggeredAlarm,
    Guid? AlarmId,
    string Message);
