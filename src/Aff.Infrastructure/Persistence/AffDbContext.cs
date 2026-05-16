using Aff.Domain.Campaigns;
using Aff.Domain.Partners;
using Aff.Domain.Settlements;
using Aff.Domain.Tracking;
using Microsoft.EntityFrameworkCore;

namespace Aff.Infrastructure.Persistence;

public class AffDbContext(DbContextOptions<AffDbContext> options) : DbContext(options)
{
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<AffiliateLink> AffiliateLinks => Set<AffiliateLink>();
    public DbSet<Click> Clicks => Set<Click>();
    public DbSet<Conversion> Conversions => Set<Conversion>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<SettlementItem> SettlementItems => Set<SettlementItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AffDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
