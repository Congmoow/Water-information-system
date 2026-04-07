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
        var reservoirCount = await _reservoirRepository.CountAsync(cancellationToken);
        var riverCount = await _riverRepository.CountAsync(cancellationToken);
        var stationCount = await _stationRepository.CountAsync(cancellationToken);
        var stationItems = await _stationRepository.GetAllAsync(cancellationToken);
        var todayAlarmItems = await _alarmRepository.GetTriggeredOnDateAsync(DateOnly.FromDateTime(DateTime.Today), cancellationToken);
        var recentAlarmItems = await _alarmRepository.GetRecentAsync(6, cancellationToken);
        var waterLevelItems = await _monitoringRepository.GetRecentByTypeAsync(MonitoringDataType.WaterLevel, 14, cancellationToken);
        var rainfallItems = await _monitoringRepository.GetRecentByTypeAsync(MonitoringDataType.Rainfall, 14, cancellationToken);
        var infoAlarm = await _alarmRepository.SearchAsync(null, AlarmLevel.Info, null, null, null, 1, 1, cancellationToken);
        var warningAlarm = await _alarmRepository.SearchAsync(null, AlarmLevel.Warning, null, null, null, 1, 1, cancellationToken);
        var criticalAlarm = await _alarmRepository.SearchAsync(null, AlarmLevel.Critical, null, null, null, 1, 1, cancellationToken);

        return new DashboardOverviewDto(
            reservoirCount,
            riverCount,
            stationCount,
            stationItems.Count(x => x.Status == StationStatus.Online),
            todayAlarmItems.Count,
            BuildAverageTrend(waterLevelItems),
            BuildRainfallTrend(rainfallItems),
            [
                new CategoryCountDto("Info", infoAlarm.Total),
                new CategoryCountDto("Warning", warningAlarm.Total),
                new CategoryCountDto("Critical", criticalAlarm.Total)
            ],
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
            entity.Level.ToString(),
            entity.Status.ToString(),
            entity.Message,
            entity.TriggeredAt);
    }
}
