using WaterInfoSystem.Domain.Enums;

namespace WaterInfoSystem.Domain.Entities;

public class Station : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public StationType Type { get; set; } = StationType.WaterLevel;

    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public StationStatus Status { get; set; } = StationStatus.Online;

    public decimal WarningThreshold { get; set; }

    public decimal CriticalThreshold { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime? LastActiveAt { get; set; }

    public Guid? ReservoirId { get; set; }

    public Reservoir? Reservoir { get; set; }

    public Guid? RiverId { get; set; }

    public River? River { get; set; }

    public ICollection<MonitoringData> MonitoringDatas { get; set; } = new List<MonitoringData>();

    public ICollection<AlarmRecord> AlarmRecords { get; set; } = new List<AlarmRecord>();
}
