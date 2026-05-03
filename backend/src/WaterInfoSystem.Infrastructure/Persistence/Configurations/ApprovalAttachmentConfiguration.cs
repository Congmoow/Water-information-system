using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WaterInfoSystem.Domain.Entities;

namespace WaterInfoSystem.Infrastructure.Persistence.Configurations;

public class ApprovalAttachmentConfiguration : IEntityTypeConfiguration<ApprovalAttachment>
{
    public void Configure(EntityTypeBuilder<ApprovalAttachment> builder)
    {
        builder.ToTable("ApprovalAttachments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.FileName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.FileType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(e => e.AttachmentType).HasMaxLength(50).IsRequired();

        builder.HasOne(e => e.Application)
            .WithMany(a => a.Attachments)
            .HasForeignKey(e => e.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
