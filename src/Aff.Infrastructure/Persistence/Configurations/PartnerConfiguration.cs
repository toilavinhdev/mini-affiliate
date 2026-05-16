using Aff.Domain.Partners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aff.Infrastructure.Persistence.Configurations;

public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("Partners");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasConversion<string>();
        builder.Property(p => p.BusinessName).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ContactEmail).IsRequired().HasMaxLength(200);
        builder.HasIndex(p => p.ContactEmail).IsUnique();
        builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(p => p.TaxCode).IsRequired().HasMaxLength(50);
        builder.Property(p => p.BankName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.BankAccountNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.BankAccountHolder).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.RejectionReason).HasMaxLength(500);
        builder.Property(p => p.ApprovedAt).HasConversion<string?>();
        builder.Property(p => p.CreatedAt).HasConversion<string>();
    }
}
