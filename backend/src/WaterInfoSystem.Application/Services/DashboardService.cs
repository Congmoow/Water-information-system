using WaterInfoSystem.Application.Contracts.Dashboard;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IReservoirRepository _reservoirRepository;
    private readonly IRiverRepository _riverRepository;
    private readonly IStationRepository _stationRepository;
    private readonly IMonitoringRepository _monitoringRepository;
    private readonly IAlarmRepository _alarmRepository;

    public DashboardService(
        IReservoirRepository reservoirRepository,
        IRiverRepository riverRepository,
        IStationRepository stationRepository,
        IMonitoringRepository monitoringRepository,
        IAlarmRepository alarmRepository)
    {
        _reservoirRepository = reservoirRepository;
        _riverRepository = riverRepository;
        _stationRepository = stationRepository;
        _monitoringRepository = monitoringRepository;
        _alarmRepository = alarmRepository;
    }

    public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken)
    {
        var reservoirCountTask = _reservoirRepository.CountAsync(cancellationToken);
        var riverCountTask = _riverRepository.CountAsync(cancellationToken);
        var stationCountTask = _stationRepository.CountAsync(cancellationToken);
        var stationItemsTask = _stationRepository.GetAllAsync(cancellationToken);
        var alarmLevelCountsTask = _alarmRepository.GetLevelCountsAsync(cancellationToken);
        var todayAlarmItemsTask = _alarmRepository.GetTriggeredOnDateAsync(DateOnly.FromDateTime(DateTime.UtcNow.Date), cancellationToken);
        var recentAlarmItemsTask = _alarmRepository.GetRecentAsync(6, cancellationToken);
        var waterLevelItemsTask = _monitoringRepository.GetRecentByTypeAsync(MonitoringDataType.WaterLevel, 14, cancellationToken);
        var rainfallItemsTask = _monitoringRepository.GetRecentByTypeAsync(MonitoringDataType.Rainfall, 14, cancellationToken);

        await Task.WhenAll(
            reservoirCountTask,
            riverCountTask,
            stationCountTask,
            stationItemsTask,
            alarmLevelCountsTask,
            todayAlarmItemsTask,
            recentAlarmItemsTask,
            waterLevelItemsTask,
            rainfallItemsTask);

        var reservoirCount = reservoirCountTask.Result;
        var riverCount = riverCountTask.Result;
        var stationCount = stationCountTask.Result;
        var stationItems = stationItemsTask.Result;
        var alarmLevelCounts = alarmLevelCountsTask.Result;
        var todayAlarmItems = todayAlarmItemsTask.Result;
        var recentAlarmItems = recentAlarmItemsTask.Result;
        var waterLevelItems = waterLevelItemsTask.Result;
        var rainfallItems = rainfallItemsTask.Result;

        return new DashboardOverviewDto(
            reservoirCount,
            riverCount,
            stationCount,
            stationItems.Count(x => x.Status == StationStatus.Online),
            todayAlarmItems.Count,
            BuildAverageTrend(waterLevelItems),
            BuildRainfallTrend(rainfallItems),
            alarmLevelCounts.Select(x => new CategoryCountDto(x.Category, x.Count)).ToList(),
            [
                new CategoryCountDto("Online", stationItems.Count(x => x.Status == StationStatus.Online)),
                new CategoryCountDto("Offline", stationItems.Count(x => x.Status == StationStatus.Offline)),
                new CategoryCountDto("Warning", stationItems.Count(x => x.Status == StationStatus.Warning))
            ],
            recentAlarmItems.Select(MapRecentAlarm).ToList());
    }

    private static IReadOnlyList<TrendPointDto> BuildAverageTrend(IReadOnlyList<MonitoringData> items)
    {
        return items
            .GroupBy(x => x.CollectedAt.ToString("MM-dd"))
            .OrderBy(x => x.Key)
            .Select(group => new TrendPointDto(group.Key, Math.Round(group.Average(x => x.Value), 2)))
            .ToList();
    }

    private static IReadOnlyList<TrendPointDto> BuildRainfallTrend(IReadOnlyList<MonitoringData> items)
    {
        return items
            .GroupBy(x => x.CollectedAt.ToString("MM-dd"))
            .OrderBy(x => x.Key)
            .Select(group => new TrendPointDto(group.Key, group.Sum(x => x.Value)))
            .ToList();
    }

    private static RecentAlarmDto MapRecentAlarm(AlarmRecord entity)
    {
        return new RecentAlarmDto(
            entity.Id,
            entity.Station?.Name ?? "--",
            entity.Level,
            entity.Status,
            entity.Message,
            entity.TriggeredAt);
    }
}
