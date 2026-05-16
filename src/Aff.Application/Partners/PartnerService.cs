using Aff.Application.Audit;
using Aff.Application.Partners.Dtos;
using Aff.Domain.Partners;
using Aff.Domain.Tracking;
using Aff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aff.Application.Partners;

public class PartnerService(AffDbContext db, AuditService audit)
{
    public async Task<PartnerResponse> RegisterAsync(RegisterPartnerRequest req)
    {
        var exists = await db.Partners.AnyAsync(p => p.ContactEmail == req.ContactEmail);
        if (exists)
            throw new InvalidOperationException($"Email '{req.ContactEmail}' already registered.");

        var partner = Partner.Register(
            req.BusinessName, req.ContactEmail, req.PhoneNumber,
            req.TaxCode, req.BankName, req.BankAccountNumber, req.BankAccountHolder);

        db.Partners.Add(partner);
        audit.Log("Partner", partner.Id, "Registered", newStatus: "Pending");
        await db.SaveChangesAsync();
        return PartnerResponseMapper.ToResponse(partner);
    }

    public async Task<PartnerResponse?> GetByIdAsync(Guid id)
    {
        var partner = await db.Partners.FindAsync(id);
        return partner == null ? null : PartnerResponseMapper.ToResponse(partner);
    }

    public async Task<PartnerResponse> UpdateAsync(Guid id, UpdatePartnerRequest req)
    {
        var partner = await db.Partners.FindAsync(id)
            ?? throw new KeyNotFoundException("Partner not found.");
        partner.Update(req.BusinessName, req.PhoneNumber, req.TaxCode,
            req.BankName, req.BankAccountNumber, req.BankAccountHolder);
        audit.Log("Partner", id, "Updated");
        await db.SaveChangesAsync();
        return PartnerResponseMapper.ToResponse(partner);
    }

    public async Task<PartnerResponse> ApproveAsync(Guid id)
    {
        var partner = await db.Partners.FindAsync(id)
            ?? throw new KeyNotFoundException("Partner not found.");
        partner.Approve();
        audit.Log("Partner", id, "Approved", "Pending", "Active");
        await db.SaveChangesAsync();
        return PartnerResponseMapper.ToResponse(partner);
    }

    public async Task<PartnerResponse> RejectAsync(Guid id, string reason)
    {
        var partner = await db.Partners.FindAsync(id)
            ?? throw new KeyNotFoundException("Partner not found.");
        partner.Reject(reason);
        audit.Log("Partner", id, "Rejected", "Pending", "Rejected", metadata: reason);
        await db.SaveChangesAsync();
        return PartnerResponseMapper.ToResponse(partner);
    }

    public async Task<PartnerResponse> SuspendAsync(Guid id)
    {
        var partner = await db.Partners.FindAsync(id)
            ?? throw new KeyNotFoundException("Partner not found.");
        partner.Suspend();
        audit.Log("Partner", id, "Suspended", "Active", "Suspended");
        await db.SaveChangesAsync();
        return PartnerResponseMapper.ToResponse(partner);
    }

    public async Task<List<PartnerResponse>> GetAllAsync()
    {
        var partners = await db.Partners
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
        return partners.Select(PartnerResponseMapper.ToResponse).ToList();
    }

    public async Task<PartnerDashboardResponse> GetDashboardAsync(Guid id)
    {
        var partner = await db.Partners.FindAsync(id)
            ?? throw new KeyNotFoundException("Partner not found.");

        var campaigns = await db.Campaigns
            .Where(c => c.PartnerId == id)
            .ToListAsync();

        var clicks = await db.Clicks
            .CountAsync(c => c.PartnerId == id);

        var conversions = await db.Conversions
            .Where(c => c.PartnerId == id)
            .ToListAsync();

        var settlements = await db.Settlements
            .Where(s => s.PartnerId == id)
            .ToListAsync();

        return new PartnerDashboardResponse(
            PartnerId: id,
            BusinessName: partner.BusinessName,
            TotalCampaigns: campaigns.Count,
            ActiveCampaigns: campaigns.Count(c => c.Status == Aff.Domain.Campaigns.CampaignStatus.Active),
            TotalClicks: clicks,
            TotalConversions: conversions.Count,
            PendingConversions: conversions.Count(c => c.Status == ConversionStatus.Pending),
            ApprovedConversions: conversions.Count(c => c.Status == ConversionStatus.Approved),
            TotalCommissionEarned: conversions
                .Where(c => c.Status is ConversionStatus.Approved or ConversionStatus.Settled)
                .Sum(c => c.CommissionAmount),
            PendingCommission: conversions
                .Where(c => c.Status == ConversionStatus.Pending)
                .Sum(c => c.CommissionAmount),
            TotalSettlements: settlements.Count,
            TotalSettled: settlements
                .Where(s => s.Status == Aff.Domain.Settlements.SettlementStatus.Completed)
                .Sum(s => s.NetCommission)
        );
    }
}
