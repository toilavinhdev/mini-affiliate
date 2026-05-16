using Aff.Application.Tracking.Dtos;
using Aff.Domain.Campaigns;
using Aff.Domain.Tracking;
using Aff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aff.Application.Tracking;

public class TrackingService(AffDbContext db)
{
    /// <summary>
    /// Returns (targetUrl, clickId) for redirect. Throws if link not found or inactive.
    /// </summary>
    public async Task<(string TargetUrl, Guid ClickId)> HandleClickAsync(
        string trackingCode, string ipAddress, string userAgent, string referer)
    {
        var link = await db.AffiliateLinks
            .FirstOrDefaultAsync(l => l.TrackingCode == trackingCode)
            ?? throw new KeyNotFoundException("Tracking link not found.");

        if (!link.IsActive)
            throw new InvalidOperationException("This tracking link is no longer active.");

        var campaign = await db.Campaigns.FindAsync(link.CampaignId)
            ?? throw new InvalidOperationException("Associated campaign not found.");

        if (campaign.Status != CampaignStatus.Active)
            throw new InvalidOperationException("Campaign is not active.");

        var click = Click.Record(trackingCode, link.CampaignId, link.PartnerId,
            ipAddress, userAgent, referer);

        db.Clicks.Add(click);
        link.IncrementClicks();
        await db.SaveChangesAsync();

        return (link.TargetUrl, click.Id);
    }

    /// <summary>
    /// Processes a conversion webhook from an integrated service.
    /// </summary>
    public async Task<ConversionResponse> HandleWebhookAsync(WebhookConversionRequest req)
    {
        var alreadyExists = await db.Conversions
            .AnyAsync(c => c.ServiceTransactionId == req.ServiceTransactionId);
        if (alreadyExists)
            throw new InvalidOperationException($"Conversion for transaction '{req.ServiceTransactionId}' already recorded.");

        Click? click = null;
        string trackingCode;

        if (req.ClickId.HasValue)
        {
            click = await db.Clicks.FindAsync(req.ClickId.Value);
            trackingCode = click?.TrackingCode ?? req.TrackingCode
                ?? throw new InvalidOperationException("TrackingCode or ClickId is required.");
        }
        else if (!string.IsNullOrWhiteSpace(req.TrackingCode))
        {
            trackingCode = req.TrackingCode;
            // Find the most recent unconverted click for this tracking code
            click = await db.Clicks
                .Where(c => c.TrackingCode == trackingCode && c.ConversionId == null)
                .OrderByDescending(c => c.ClickedAt)
                .FirstOrDefaultAsync();
        }
        else
        {
            throw new InvalidOperationException("Either TrackingCode or ClickId must be provided.");
        }

        var link = await db.AffiliateLinks
            .FirstOrDefaultAsync(l => l.TrackingCode == trackingCode)
            ?? throw new KeyNotFoundException("Tracking link not found.");

        var campaign = await db.Campaigns.FindAsync(link.CampaignId)
            ?? throw new InvalidOperationException("Campaign not found.");

        var commission = campaign.CalculateCommission(req.TransactionAmount);

        if (!campaign.HasBudgetFor(commission))
            throw new InvalidOperationException("Campaign budget exceeded. Conversion not recorded.");

        var conversion = Conversion.Create(
            click?.Id, trackingCode, campaign.Id, campaign.PartnerId,
            req.ServiceType, req.ServiceTransactionId, req.EndUserId,
            req.TransactionAmount, commission);

        db.Conversions.Add(conversion);
        campaign.AddSpentBudget(commission);
        link.IncrementConversions();

        if (click != null)
            click.MarkConverted(conversion.Id);

        await db.SaveChangesAsync();
        return ConversionMapper.ToResponse(conversion);
    }

    public async Task<List<ConversionResponse>> GetAllConversionsAsync()
    {
        var conversions = await db.Conversions
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return conversions.Select(ConversionMapper.ToResponse).ToList();
    }

    public async Task<ConversionResponse?> GetConversionAsync(Guid id)
    {
        var conversion = await db.Conversions.FindAsync(id);
        return conversion == null ? null : ConversionMapper.ToResponse(conversion);
    }

    public async Task<List<ConversionResponse>> GetPartnerConversionsAsync(Guid partnerId)
    {
        var conversions = await db.Conversions
            .Where(c => c.PartnerId == partnerId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return conversions.Select(ConversionMapper.ToResponse).ToList();
    }

    public async Task<ConversionResponse> ApproveConversionAsync(Guid id)
    {
        var conversion = await db.Conversions.FindAsync(id)
            ?? throw new KeyNotFoundException("Conversion not found.");
        conversion.Approve();
        await db.SaveChangesAsync();
        return ConversionMapper.ToResponse(conversion);
    }

    public async Task<ConversionResponse> RejectConversionAsync(Guid id, string reason)
    {
        var conversion = await db.Conversions.FindAsync(id)
            ?? throw new KeyNotFoundException("Conversion not found.");

        // Refund the campaign budget
        var campaign = await db.Campaigns.FindAsync(conversion.CampaignId);
        campaign?.RefundSpentBudget(conversion.CommissionAmount);

        conversion.Reject(reason);
        await db.SaveChangesAsync();
        return ConversionMapper.ToResponse(conversion);
    }
}
