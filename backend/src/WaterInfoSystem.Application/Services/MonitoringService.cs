using WaterInfoSystem.Application.Contracts.Monitoring;
using WaterInfoSystem.Application.Interfaces;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Shared.Exceptions;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Services;

public class MonitoringService : IMonitoringService
{
    private readonly IMonitoringRepository _monitoringRepository;
    private readonly IStationRepository _stationRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MonitoringService(
        IMonitoringRepository monitoringRepository,
        IStationRepository stationRepository,
        IAlarmRepository alarmRepository,
        IUnitOfWork unitOfWork)
    {
        _monitoringRepository = monitoringRepository;
        _stationRepository = stationRepository;
        _alarmRepository = alarmRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<MonitoringListItemDto>> SearchAsync(MonitoringQueryDto query, CancellationToken cancellationToken)
    {
        var (items, total) = await _monitoringRepository.SearchAsync(
            query.StationId,
            query.DataType,
            query.StartTime,
            query.EndTime,
            query.Page,
            query.PageSize,
            cancellationToken);

        return new PagedResult<MonitoringListItemDto>(items.Select(MapListItem).ToList(), total);
    }

    public async Task<IReadOnlyList<MonitoringTrendPointDto>> GetHistoryAsync(MonitoringQueryDto query, CancellationToken cancellationToken)
    {
        var (items, _) = await _monitoringRepository.SearchAsync(
            query.StationId,
            query.DataType,
            query.StartTime,
            query.EndTime,
            1,
            200,
            cancellationToken);

        return items
            .OrderBy(x => x.CollectedAt)
            .Select(x => new MonitoringTrendPointDto(x.CollectedAt.ToString("MM-dd HH:mm"), x.Value))
            .ToList();
    }

    public async Task<MonitoringCreateResultDto> CreateAsync(MonitoringCreateDto request, CancellationToken cancellationToken)
    {
        var station = await _stationRepository.GetByIdAsync(request.StationId, cancellationToken);
        if (station is null)
        {
            throw new NotFoundException("指定站点不存在");
        }

        EnsureDataTypeMatches(station, request.DataType);

        var entity = new MonitoringData
        {
            StationId = station.Id,
            Station = station,
            DataType = request.DataType,
            Value = request.Value,
            CollectedAt = request.CollectedAt,
            Remark = request.Remark,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _monitoringRepository.AddAsync(entity, cancellationToken);

        station.LastActiveAt = request.CollectedAt;
        station.Status = StationStatus.Online;
        station.UpdatedAt = DateTime.UtcNow;

        var (triggeredAlarm, alarmId, message) = await TryCreateThresholdAlarmAsync(station, entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MonitoringCreateResultDto(entity.Id, triggeredAlarm, alarmId, message);
    }

    private async Task<(bool TriggeredAlarm, Guid? AlarmId, string Message)> TryCreateThresholdAlarmAsync(Station station, MonitoringData entity, CancellationToken cancellationToken)
    {
        if (entity.Value < station.WarningThreshold)
        {
            return (false, null, "监测数据录入成功");
        }

        var level = entity.Value >= station.CriticalThreshold ? AlarmLevel.Critical : AlarmLevel.Warning;
        var threshold = level == AlarmLevel.Critical ? station.CriticalThreshold : station.WarningThreshold;
        var alarm = new AlarmRecord
        {
            StationId = station.Id,
            Station = station,
            MonitoringDataId = entity.Id,
            MonitoringData = entity,
            AlarmType = AlarmType.ThresholdExceeded,
            Level = level,
            Status = AlarmStatus.Pending,
            Message = $"{station.Name}监测值 {entity.Value} 超过阈值 {threshold}",
            TriggeredAt = entity.CollectedAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        entity.AlarmRecords.Add(alarm);
        station.Status = StationStatus.Warning;
        await _alarmRepository.AddAsync(alarm, cancellationToken);

        return (true, alarm.Id, "监测数据录入成功，并已自动生成告警");
    }

    private static void EnsureDataTypeMatches(Station station, MonitoringDataType dataType)
    {
        var stationType = station.Type switch
        {
            StationType.WaterLevel => MonitoringDataType.WaterLevel,
            StationType.Rainfall => MonitoringDataType.Rainfall,
            StationType.Flow => MonitoringDataType.Flow,
            _ => throw new AppException("站点类型无法匹配监测数据类型")
        };

        if (stationType != dataType)
        {
            throw new AppException("监测数据类型与站点类型不匹配");
        }
    }

    private static MonitoringListItemDto MapListItem(MonitoringData entity)
    {
        return new MonitoringListItemDto(
            entity.Id,
            entity.StationId,
            entity.Station?.Name ?? "--",
            entity.DataType,
            entity.Value,
            entity.CollectedAt,
            entity.AlarmRecords.Any(),
            entity.Remark);
    }
}
