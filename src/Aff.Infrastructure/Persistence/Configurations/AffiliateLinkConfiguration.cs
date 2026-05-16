using Aff.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aff.Infrastructure.Persistence.Configurations;

public class AffiliateLinkConfiguration : IEntityTypeConfiguration<AffiliateLink>
{
    public void Configure(EntityTypeBuilder<AffiliateLink> builder)
    {
        builder.ToTable("AffiliateLinks");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasConversion<string>();
        builder.Property(l => l.CampaignId).HasConversion<string>();
        builder.Property(l => l.PartnerId).HasConversion<string>();
        builder.Property(l => l.TrackingCode).IsRequired().HasMaxLength(20);
        builder.HasIndex(l => l.TrackingCode).IsUnique();
        builder.Property(l => l.TargetUrl).IsRequired().HasMaxLength(2000);
        builder.Property(l => l.Label).HasMaxLength(100);
        builder.Property(l => l.IsActive).HasConversion<int>();
        builder.Property(l => l.CreatedAt).HasConversion<string>();
    }
}
