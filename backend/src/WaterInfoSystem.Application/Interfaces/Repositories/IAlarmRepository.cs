using WaterInfoSystem.Domain.Entities;
using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Application.Interfaces.Repositories;

public interface IAlarmRepository
{
    Task<(IReadOnlyList<AlarmRecord> Items, int Total)> SearchAsync(
        Guid? stationId,
        AlarmLevel? level,
        AlarmStatus? status,
        DateTime? startTime,
        DateTime? endTime,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<(string Category, int Count)>> GetLevelCountsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<AlarmRecord>> GetRecentAsync(int take, CancellationToken cancellationToken);

    Task<IReadOnlyList<AlarmRecord>> GetTriggeredOnDateAsync(DateOnly date, CancellationToken cancellationToken);

    Task<AlarmRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(AlarmRecord alarmRecord, CancellationToken cancellationToken);
}
