using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Stations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.WarningThreshold).HasPrecision(18, 2);
        builder.Property(x => x.CriticalThreshold).HasPrecision(18, 2);
        builder.HasOne(x => x.Reservoir)
            .WithMany(x => x.Stations)
            .HasForeignKey(x => x.ReservoirId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(x => x.River)
            .WithMany(x => x.Stations)
            .HasForeignKey(x => x.RiverId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
