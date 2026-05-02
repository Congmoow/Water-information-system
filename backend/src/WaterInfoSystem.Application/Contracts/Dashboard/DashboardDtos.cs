using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Contracts.Dashboard;

public record TrendPointDto(string Label, decimal Value);

public record CategoryCountDto(string Name, int Value);

public record RecentAlarmDto(
    Guid Id,
    string StationName,
    AlarmLevel Level,
    AlarmStatus Status,
    string Message,
    DateTime TriggeredAt);

public record DashboardOverviewDto(
    int ReservoirCount,
    int RiverCount,
    int StationCount,
    int OnlineStationCount,
    int TodayAlarmCount,
    IReadOnlyList<TrendPointDto> WaterLevelTrend,
    IReadOnlyList<TrendPointDto> RainfallTrend,
    IReadOnlyList<CategoryCountDto> AlarmLevelStats,
    IReadOnlyList<CategoryCountDto> StationStatusStats,
    IReadOnlyList<RecentAlarmDto> RecentAlarms);
