namespace WaterInfoSystem.Domain.Entities;

public class Reservoir : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public decimal Capacity { get; set; }

    public string ManagementUnit { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Description { get; set; } = string.Empty;

    public ICollection<Station> Stations { get; set; } = new List<Station>();
}
