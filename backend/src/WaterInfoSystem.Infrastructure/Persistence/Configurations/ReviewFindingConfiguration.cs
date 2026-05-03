using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class ReviewFindingConfiguration : IEntityTypeConfiguration<ReviewFinding>
{
    public void Configure(EntityTypeBuilder<ReviewFinding> builder)
    {
        builder.ToTable("ReviewFindings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Category).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Severity).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Suggestion).HasMaxLength(1000);

        builder.HasOne(e => e.ReviewResult)
            .WithMany(r => r.Findings)
            .HasForeignKey(e => e.ReviewResultId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
