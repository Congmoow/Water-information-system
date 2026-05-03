using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class ReviewResultConfiguration : IEntityTypeConfiguration<ReviewResult>
{
    public void Configure(EntityTypeBuilder<ReviewResult> builder)
    {
        builder.ToTable("ReviewResults");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Summary).HasMaxLength(2000).IsRequired();
        builder.Property(e => e.AgentVersion).HasMaxLength(50);

        builder.HasOne(e => e.Application)
            .WithMany(a => a.ReviewResults)
            .HasForeignKey(e => e.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
