using Aff.Application.Audit;
using Aff.Application.Settlements.Dtos;
using Aff.Domain.Settlements;
using Aff.Domain.Tracking;
using Aff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aff.Application.Settlements;

public class SettlementService(AffDbContext db, AuditService audit)
{
    /// <summary>
    /// Generates settlement(s) for explicitly selected conversion IDs.
    /// Groups by partner; period is derived from min/max ApprovedAt of selected conversions.
    /// </summary>
    public async Task<List<SettlementResponse>> GenerateAsync(GenerateSettlementsRequest req)
    {
        if (req.ConversionIds.Count == 0)
            throw new InvalidOperationException("Vui lòng chọn ít nhất một conversion để settle.");

        var conversions = await db.Conversions
            .Where(c => req.ConversionIds.Contains(c.Id)
                     && c.Status == ConversionStatus.Approved
                     && c.SettlementId == null)
            .ToListAsync();

        if (conversions.Count == 0)
            throw new InvalidOperationException("Không có conversion nào hợp lệ (cần trạng thái Approved và chưa được settle).");

        var grouped = conversions.GroupBy(c => c.PartnerId);
        var results = new List<Settlement>();

        foreach (var group in grouped)
        {
            var periodStart = group.Min(c => c.ApprovedAt!.Value);
            var periodEnd   = group.Max(c => c.ApprovedAt!.Value);
            var items = group.Select(c => (c.Id, c.CommissionAmount)).ToList();
            var settlement = Settlement.Create(group.Key, periodStart, periodEnd, items, req.TaxRate);
            db.Settlements.Add(settlement);
            results.Add(settlement);

            foreach (var conversion in group)
                conversion.Settle(settlement.Id);

            audit.Log("Settlement", settlement.Id, "Generated", newStatus: "Pending",
                metadata: $"conversions:{group.Count()}");
        }

        await db.SaveChangesAsync();
        return results.Select(SettlementMapper.ToResponse).ToList();
    }

    public async Task<List<SettlementResponse>> GetAllAsync()
    {
        var settlements = await db.Settlements
            .Include(s => s.Items)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
        return settlements.Select(SettlementMapper.ToResponse).ToList();
    }

    public async Task<SettlementResponse?> GetByIdAsync(Guid id)
    {
        var settlement = await db.Settlements
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);
        return settlement == null ? null : SettlementMapper.ToResponse(settlement);
    }

    public async Task<List<SettlementResponse>> GetByPartnerAsync(Guid partnerId)
    {
        var settlements = await db.Settlements
            .Include(s => s.Items)
            .Where(s => s.PartnerId == partnerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
        return settlements.Select(SettlementMapper.ToResponse).ToList();
    }

    public async Task<SettlementResponse> ProcessAsync(Guid id, ProcessSettlementRequest req)
    {
        var settlement = await db.Settlements
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException("Settlement not found.");

        settlement.Complete(req.PaymentReference, req.Note);
        audit.Log("Settlement", id, "Processed", "Pending", "Completed", metadata: req.PaymentReference);
        await db.SaveChangesAsync();
        return SettlementMapper.ToResponse(settlement);
    }
}
