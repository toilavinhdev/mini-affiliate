using Aff.Domain.Common;

namespace Aff.Domain.Campaigns;

public class AffiliateLink : Entity
{
    public Guid CampaignId { get; private set; }
    public Guid PartnerId { get; private set; }
    public string TrackingCode { get; private set; } = default!;
    public string TargetUrl { get; private set; } = default!;
    public string Label { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public int TotalClicks { get; private set; }
    public int TotalConversions { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AffiliateLink() { }

    public static AffiliateLink Create(Guid campaignId, Guid partnerId,
        string trackingCode, string targetUrl, string label)
    {
        return new AffiliateLink
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            PartnerId = partnerId,
            TrackingCode = trackingCode,
            TargetUrl = targetUrl,
            Label = label,
            IsActive = true,
            TotalClicks = 0,
            TotalConversions = 0,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate() => IsActive = false;

    public void IncrementClicks() => TotalClicks++;

    public void IncrementConversions() => TotalConversions++;
}
