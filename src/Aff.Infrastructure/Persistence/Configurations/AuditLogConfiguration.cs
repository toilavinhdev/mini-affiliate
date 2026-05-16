using Aff.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aff.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasConversion<string>();
        builder.Property(l => l.EntityType).IsRequired().HasMaxLength(100);
        builder.Property(l => l.EntityId).IsRequired().HasConversion<string>();
        builder.Property(l => l.Action).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Actor).IsRequired().HasMaxLength(200);
        builder.Property(l => l.OldStatus).HasMaxLength(50);
        builder.Property(l => l.NewStatus).HasMaxLength(50);
        builder.Property(l => l.Metadata).HasColumnType("TEXT");
        builder.Property(l => l.Timestamp).HasConversion<string>();
        builder.HasIndex(l => new { l.EntityType, l.EntityId });
        builder.HasIndex(l => l.Timestamp);
    }
}
