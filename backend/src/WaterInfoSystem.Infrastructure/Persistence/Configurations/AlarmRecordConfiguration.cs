using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class AlarmRecordConfiguration : IEntityTypeConfiguration<AlarmRecord>
{
    public void Configure(EntityTypeBuilder<AlarmRecord> builder)
    {
        builder.ToTable("AlarmRecords");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AlarmType).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Level).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.Message).HasMaxLength(300).IsRequired();
        builder.Property(x => x.HandleRemark).HasMaxLength(300);
        builder.HasOne(x => x.Station)
            .WithMany(x => x.AlarmRecords)
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.MonitoringData)
            .WithMany(x => x.AlarmRecords)
            .HasForeignKey(x => x.MonitoringDataId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
