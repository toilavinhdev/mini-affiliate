using Aff.Domain.Common;

namespace Aff.Domain.Tracking;

public class Click : Entity
{
    public string TrackingCode { get; private set; } = default!;
    public Guid CampaignId { get; private set; }
    public Guid PartnerId { get; private set; }
    public string IpAddress { get; private set; } = default!;
    public string UserAgent { get; private set; } = default!;
    public string Referer { get; private set; } = default!;
    public DateTime ClickedAt { get; private set; }
    public DateTime? ConvertedAt { get; private set; }
    public Guid? ConversionId { get; private set; }

    private Click() { }

    public static Click Record(string trackingCode, Guid campaignId, Guid partnerId,
        string ipAddress, string userAgent, string referer)
    {
        return new Click
        {
            Id = Guid.NewGuid(),
            TrackingCode = trackingCode,
            CampaignId = campaignId,
            PartnerId = partnerId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Referer = referer,
            ClickedAt = DateTime.UtcNow
        };
    }

    public void MarkConverted(Guid conversionId)
    {
        ConversionId = conversionId;
        ConvertedAt = DateTime.UtcNow;
    }
}
