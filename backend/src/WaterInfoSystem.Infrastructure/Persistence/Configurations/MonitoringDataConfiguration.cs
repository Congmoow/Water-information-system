using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class MonitoringDataConfiguration : IEntityTypeConfiguration<MonitoringData>
{
    public void Configure(EntityTypeBuilder<MonitoringData> builder)
    {
        builder.ToTable("MonitoringDatas");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.DataType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.Remark).HasMaxLength(300);
        builder.HasOne(x => x.Station)
            .WithMany(x => x.MonitoringDatas)
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.StationId, x.CollectedAt });
    }
}
