using WaterInfoSystem.Application.Contracts.Alarms;
using WaterInfoSystem.Application.Interfaces;
using WaterInfoSystem.Application.Interfaces.Repositories;
using WaterInfoSystem.Application.Interfaces.Services;
using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;
using WaterInfoSystem.Shared.Exceptions;
using WaterInfoSystem.Shared.Results;

namespace WaterInfoSystem.Application.Services;

public class AlarmService : IAlarmService
{
    private readonly IAlarmRepository _alarmRepository;
    private readonly IStationRepository _stationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AlarmService(IAlarmRepository alarmRepository, IStationRepository stationRepository, IUnitOfWork unitOfWork)
    {
        _alarmRepository = alarmRepository;
        _stationRepository = stationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<AlarmListItemDto>> SearchAsync(AlarmQueryDto query, CancellationToken cancellationToken)
    {
        var (items, total) = await _alarmRepository.SearchAsync(
            query.StationId,
            query.Level,
            query.Status,
            query.StartTime,
            query.EndTime,
            query.Page,
            query.PageSize,
            cancellationToken);

        return new PagedResult<AlarmListItemDto>(items.Select(MapListItem).ToList(), total);
    }

    public async Task<AlarmDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredAsync(id, cancellationToken);
        return MapDetail(entity);
    }

    public async Task<AlarmDetailDto> CreateAsync(AlarmCreateDto request, CancellationToken cancellationToken)
    {
        var station = await _stationRepository.GetByIdAsync(request.StationId, cancellationToken);
        if (station is null)
        {
            throw new NotFoundException("指定站点不存在");
        }

        var entity = new AlarmRecord
        {
            StationId = station.Id,
            Station = station,
            AlarmType = request.AlarmType,
            Level = request.Level,
            Status = AlarmStatus.Pending,
            Message = request.Message,
            TriggeredAt = request.TriggeredAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _alarmRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    public async Task<AlarmDetailDto> HandleAsync(Guid id, AlarmHandleDto request, Guid handledByUserId, CancellationToken cancellationToken)
    {
        var entity = await GetRequiredAsync(id, cancellationToken);
        entity.Status = request.Status;
        entity.HandleRemark = request.HandleRemark;
        entity.HandledByUserId = handledByUserId;
        entity.HandledAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapDetail(entity);
    }

    private async Task<AlarmRecord> GetRequiredAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _alarmRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException("指定告警不存在");
        }

        return entity;
    }

    private static AlarmListItemDto MapListItem(AlarmRecord entity)
    {
        return new AlarmListItemDto(
            entity.Id,
            entity.StationId,
            entity.Station?.Name ?? "--",
            entity.AlarmType,
            entity.Level,
            entity.Status,
            entity.Message,
            entity.TriggeredAt,
            entity.HandledAt);
    }

    private static AlarmDetailDto MapDetail(AlarmRecord entity)
    {
        return new AlarmDetailDto(
            entity.Id,
            entity.StationId,
            entity.Station?.Name ?? "--",
            entity.AlarmType,
            entity.Level,
            entity.Status,
            entity.Message,
            entity.TriggeredAt,
            entity.HandledAt,
            entity.HandleRemark,
            entity.MonitoringDataId,
            entity.HandledByUserId);
    }
}
