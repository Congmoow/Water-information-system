namespace WaterInfoSystem.Domain.Entities;

public class River : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public decimal Length { get; set; }

    public string Basin { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Description { get; set; } = string.Empty;

    public ICollection<Station> Stations { get; set; } = new List<Station>();
}
