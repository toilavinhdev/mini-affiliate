using Aff.Domain.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aff.Infrastructure.Persistence.Configurations;

public class ConversionConfiguration : IEntityTypeConfiguration<Conversion>
{
    public void Configure(EntityTypeBuilder<Conversion> builder)
    {
        builder.ToTable("Conversions");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasConversion<string>();
        builder.Property(c => c.ClickId).HasConversion<string?>();
        builder.Property(c => c.TrackingCode).IsRequired().HasMaxLength(20);
        builder.Property(c => c.CampaignId).HasConversion<string>();
        builder.Property(c => c.PartnerId).HasConversion<string>();
        builder.Property(c => c.ServiceType).IsRequired().HasMaxLength(50);
        builder.Property(c => c.ServiceTransactionId).IsRequired().HasMaxLength(200);
        builder.HasIndex(c => c.ServiceTransactionId).IsUnique();
        builder.Property(c => c.EndUserId).IsRequired().HasMaxLength(200);
        builder.Property(c => c.TransactionAmount).HasColumnType("REAL");
        builder.Property(c => c.CommissionAmount).HasColumnType("REAL");
        builder.Property(c => c.Status).HasConversion<int>();
        builder.Property(c => c.RejectionReason).HasMaxLength(500);
        builder.Property(c => c.IsSuspicious);
        builder.Property(c => c.FraudReason).HasMaxLength(500);
        builder.Property(c => c.CreatedAt).HasConversion<string>();
        builder.Property(c => c.ApprovedAt).HasConversion<string?>();
        builder.Property(c => c.SettlementId).HasConversion<string?>();
        builder.Property(c => c.SettledAt).HasConversion<string?>();
    }
}
