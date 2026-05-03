using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class ApprovalApplicationConfiguration : IEntityTypeConfiguration<ApprovalApplication>
{
    public void Configure(EntityTypeBuilder<ApprovalApplication> builder)
    {
        builder.ToTable("ApprovalApplications");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ApplicantName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.ApplicantIdCard).HasMaxLength(50).IsRequired();
        builder.Property(e => e.EnterpriseName).HasMaxLength(200);
        builder.Property(e => e.EnterpriseLicenseNo).HasMaxLength(100);
        builder.Property(e => e.WaterIntakeLocation).HasMaxLength(200).IsRequired();
        builder.Property(e => e.WaterIntakePurpose).HasMaxLength(200).IsRequired();
        builder.Property(e => e.WaterIntakeAmount).HasPrecision(18, 2);
        builder.Property(e => e.Status).HasMaxLength(30).IsRequired();

        builder.HasOne(e => e.SubmittedByUser)
            .WithMany()
            .HasForeignKey(e => e.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
