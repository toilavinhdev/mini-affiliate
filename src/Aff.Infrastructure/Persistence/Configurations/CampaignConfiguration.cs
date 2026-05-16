using Aff.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aff.Infrastructure.Persistence.Configurations;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("Campaigns");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion<string>();
        builder.Property(c => c.PartnerId).HasConversion<string>();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.ServiceType).IsRequired().HasMaxLength(50);
        builder.Property(c => c.CommissionType).HasConversion<int>();
        builder.Property(c => c.CommissionValue).HasColumnType("REAL");
        builder.Property(c => c.MaxBudget).HasColumnType("REAL");
        builder.Property(c => c.SpentBudget).HasColumnType("REAL");
        builder.Property(c => c.Status).HasConversion<int>();
        builder.Property(c => c.StartDate).HasConversion<string>();
        builder.Property(c => c.EndDate).HasConversion<string?>();
        builder.Property(c => c.AttributionWindowDays);
        builder.Property(c => c.MinOrderAmount).HasColumnType("REAL");
        builder.Property(c => c.MaxCommissionPerConversion).HasColumnType("REAL");
        builder.Property(c => c.CreatedAt).HasConversion<string>();
    }
}
