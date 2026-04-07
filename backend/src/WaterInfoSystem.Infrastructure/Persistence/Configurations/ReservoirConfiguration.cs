using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class ReservoirConfiguration : IEntityTypeConfiguration<Reservoir>
{
    public void Configure(EntityTypeBuilder<Reservoir> builder)
    {
        builder.ToTable("Reservoirs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Location).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ManagementUnit).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Capacity).HasPrecision(18, 2);
    }
}
