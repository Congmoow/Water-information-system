using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class RiverConfiguration : IEntityTypeConfiguration<River>
{
    public void Configure(EntityTypeBuilder<River> builder)
    {
        builder.ToTable("Rivers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Basin).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Length).HasPrecision(18, 2);
    }
}
