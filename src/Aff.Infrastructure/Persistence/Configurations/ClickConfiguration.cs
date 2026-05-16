using Aff.Domain.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aff.Infrastructure.Persistence.Configurations;

public class ClickConfiguration : IEntityTypeConfiguration<Click>
{
    public void Configure(EntityTypeBuilder<Click> builder)
    {
        builder.ToTable("Clicks");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion<string>();
        builder.Property(c => c.TrackingCode).IsRequired().HasMaxLength(20);
        builder.HasIndex(c => c.TrackingCode);
        builder.Property(c => c.CampaignId).HasConversion<string>();
        builder.Property(c => c.PartnerId).HasConversion<string>();
        builder.Property(c => c.IpAddress).HasMaxLength(50);
        builder.Property(c => c.UserAgent).HasMaxLength(500);
        builder.Property(c => c.Referer).HasMaxLength(2000);
        builder.Property(c => c.ClickedAt).HasConversion<string>();
        builder.Property(c => c.ConvertedAt).HasConversion<string?>();
        builder.Property(c => c.ConversionId).HasConversion<string?>();
    }
}
