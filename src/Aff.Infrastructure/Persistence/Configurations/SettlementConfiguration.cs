using Aff.Domain.Settlements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aff.Infrastructure.Persistence.Configurations;

public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        builder.ToTable("Settlements");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasConversion<string>();
        builder.Property(s => s.PartnerId).HasConversion<string>();
        builder.Property(s => s.PeriodStart).HasConversion<string>();
        builder.Property(s => s.PeriodEnd).HasConversion<string>();
        builder.Property(s => s.GrossCommission).HasColumnType("REAL");
        builder.Property(s => s.TaxRate).HasColumnType("REAL");
        builder.Property(s => s.TaxAmount).HasColumnType("REAL");
        builder.Property(s => s.NetCommission).HasColumnType("REAL");
        builder.Property(s => s.Status).HasConversion<int>();
        builder.Property(s => s.PaymentReference).HasMaxLength(200);
        builder.Property(s => s.Note).HasMaxLength(500);
        builder.Property(s => s.CreatedAt).HasConversion<string>();
        builder.Property(s => s.ProcessedAt).HasConversion<string?>();

        builder.HasMany(s => s.Items)
               .WithOne()
               .HasForeignKey(i => i.SettlementId);
    }
}

public class SettlementItemConfiguration : IEntityTypeConfiguration<SettlementItem>
{
    public void Configure(EntityTypeBuilder<SettlementItem> builder)
    {
        builder.ToTable("SettlementItems");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasConversion<string>();
        builder.Property(i => i.SettlementId).HasConversion<string>();
        builder.Property(i => i.ConversionId).HasConversion<string>();
        builder.Property(i => i.CommissionAmount).HasColumnType("REAL");
    }
}
