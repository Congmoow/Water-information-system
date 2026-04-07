using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Domain.Entities;

public class AlarmRecord : BaseEntity
{
    public Guid StationId { get; set; }

    public Station? Station { get; set; }

    public Guid? MonitoringDataId { get; set; }

    public MonitoringData? MonitoringData { get; set; }

    public AlarmType AlarmType { get; set; } = AlarmType.ThresholdExceeded;

    public AlarmLevel Level { get; set; } = AlarmLevel.Warning;

    public string Message { get; set; } = string.Empty;

    public AlarmStatus Status { get; set; } = AlarmStatus.Pending;

    public DateTime TriggeredAt { get; set; }

    public DateTime? HandledAt { get; set; }

    public Guid? HandledByUserId { get; set; }

    public string? HandleRemark { get; set; }
}
