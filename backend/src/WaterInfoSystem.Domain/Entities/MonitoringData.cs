using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Domain.Entities;

public class MonitoringData : BaseEntity
{
    public Guid StationId { get; set; }

    public Station? Station { get; set; }

    public MonitoringDataType DataType { get; set; } = MonitoringDataType.WaterLevel;

    public decimal Value { get; set; }

    public DateTime CollectedAt { get; set; }

    public string? Remark { get; set; }

    public ICollection<AlarmRecord> AlarmRecords { get; set; } = new List<AlarmRecord>();
}
