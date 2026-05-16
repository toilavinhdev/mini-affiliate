using Aff.Application.Audit;
using Aff.Application.Campaigns.Dtos;
using Aff.Domain.Campaigns;
using Aff.Domain.Partners;
using Aff.Infrastructure.Persistence;
using Aff.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Aff.Application.Campaigns;

public class CampaignService(AffDbContext db, AuditService audit)
{
    public async Task<CampaignResponse> CreateAsync(CreateCampaignRequest req)
    {
        var partner = await db.Partners.FindAsync(req.PartnerId)
            ?? throw new KeyNotFoundException("Partner not found.");
        if (partner.Status != PartnerStatus.Active)
            throw new InvalidOperationException("Partner must be active to create campaigns.");

        var campaign = Campaign.Create(
            req.PartnerId, req.Name, req.Description, req.ServiceType,
            req.CommissionType, req.CommissionValue, req.StartDate, req.EndDate, req.MaxBudget);

        db.Campaigns.Add(campaign);
        audit.Log("Campaign", campaign.Id, "Created", newStatus: "Draft");
        await db.SaveChangesAsync();
        return CampaignMapper.ToResponse(campaign);
    }

    public async Task<CampaignResponse?> GetByIdAsync(Guid id)
    {
        var campaign = await db.Campaigns.FindAsync(id);
        return campaign == null ? null : CampaignMapper.ToResponse(campaign);
    }

    public async Task<CampaignResponse> UpdateAsync(Guid id, UpdateCampaignRequest req)
    {
        var campaign = await db.Campaigns.FindAsync(id)
            ?? throw new KeyNotFoundException("Campaign not found.");
        campaign.Update(req.Name, req.Description, req.CommissionType,
            req.CommissionValue, req.StartDate, req.EndDate, req.MaxBudget);
        await db.SaveChangesAsync();
        return CampaignMapper.ToResponse(campaign);
    }

    public async Task<CampaignResponse> ActivateAsync(Guid id)
    {
        var campaign = await db.Campaigns.FindAsync(id)
            ?? throw new KeyNotFoundException("Campaign not found.");
        var oldStatus = campaign.Status.ToString();
        campaign.Activate();
        audit.Log("Campaign", id, "Activated", oldStatus, "Active");
        await db.SaveChangesAsync();
        return CampaignMapper.ToResponse(campaign);
    }

    public async Task<CampaignResponse> PauseAsync(Guid id)
    {
        var campaign = await db.Campaigns.FindAsync(id)
            ?? throw new KeyNotFoundException("Campaign not found.");
        campaign.Pause();
        audit.Log("Campaign", id, "Paused", "Active", "Paused");
        await db.SaveChangesAsync();
        return CampaignMapper.ToResponse(campaign);
    }

    public async Task<List<CampaignResponse>> GetAllAsync()
    {
        var campaigns = await db.Campaigns
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return campaigns.Select(CampaignMapper.ToResponse).ToList();
    }

    public async Task<List<CampaignResponse>> GetByPartnerAsync(Guid partnerId)
    {
        var campaigns = await db.Campaigns
            .Where(c => c.PartnerId == partnerId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return campaigns.Select(CampaignMapper.ToResponse).ToList();
    }

    public async Task<AffiliateLinkResponse> CreateLinkAsync(Guid campaignId, CreateLinkRequest req, string baseUrl)
    {
        var campaign = await db.Campaigns.FindAsync(campaignId)
            ?? throw new KeyNotFoundException("Campaign not found.");
        if (campaign.Status == CampaignStatus.Ended)
            throw new InvalidOperationException("Cannot create links for ended campaigns.");

        string trackingCode;
        do { trackingCode = TrackingCodeGenerator.Generate(); }
        while (await db.AffiliateLinks.AnyAsync(l => l.TrackingCode == trackingCode));

        var link = AffiliateLink.Create(campaignId, campaign.PartnerId, trackingCode, req.TargetUrl, req.Label);
        db.AffiliateLinks.Add(link);
        audit.Log("Campaign", campaignId, "LinkCreated", metadata: trackingCode);
        await db.SaveChangesAsync();
        return CampaignMapper.ToResponse(link, baseUrl);
    }

    public async Task<List<AffiliateLinkResponse>> GetLinksAsync(Guid campaignId, string baseUrl)
    {
        var links = await db.AffiliateLinks
            .Where(l => l.CampaignId == campaignId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
        return links.Select(l => CampaignMapper.ToResponse(l, baseUrl)).ToList();
    }

    public async Task DeactivateLinkAsync(Guid linkId)
    {
        var link = await db.AffiliateLinks.FindAsync(linkId)
            ?? throw new KeyNotFoundException("Link not found.");
        link.Deactivate();
        await db.SaveChangesAsync();
    }
}
