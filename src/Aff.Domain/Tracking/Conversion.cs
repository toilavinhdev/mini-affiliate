using Aff.Domain.Common;

namespace Aff.Domain.Tracking;

public class Conversion : AggregateRoot
{
    public Guid? ClickId { get; private set; }
    public string TrackingCode { get; private set; } = default!;
    public Guid CampaignId { get; private set; }
    public Guid PartnerId { get; private set; }
    public string ServiceType { get; private set; } = default!;
    public string ServiceTransactionId { get; private set; } = default!;
    public string EndUserId { get; private set; } = default!;
    public decimal TransactionAmount { get; private set; }
    public decimal CommissionAmount { get; private set; }
    public ConversionStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public bool IsSuspicious { get; private set; }
    public string? FraudReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public Guid? SettlementId { get; private set; }
    public DateTime? SettledAt { get; private set; }

    private Conversion() { }

    public static Conversion Create(Guid? clickId, string trackingCode, Guid campaignId,
        Guid partnerId, string serviceType, string serviceTransactionId, string endUserId,
        decimal transactionAmount, decimal commissionAmount)
    {
        return new Conversion
        {
            Id = Guid.NewGuid(),
            ClickId = clickId,
            TrackingCode = trackingCode,
            CampaignId = campaignId,
            PartnerId = partnerId,
            ServiceType = serviceType,
            ServiceTransactionId = serviceTransactionId,
            EndUserId = endUserId,
            TransactionAmount = transactionAmount,
            CommissionAmount = commissionAmount,
            Status = ConversionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkSuspicious(string reason)
    {
        IsSuspicious = true;
        FraudReason = reason;
    }

    public void Approve()
    {
        if (Status != ConversionStatus.Pending)
            throw new InvalidOperationException("Only pending conversions can be approved.");
        Status = ConversionStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (Status != ConversionStatus.Pending)
            throw new InvalidOperationException("Only pending conversions can be rejected.");
        Status = ConversionStatus.Rejected;
        RejectionReason = reason;
    }

    public void Settle(Guid settlementId)
    {
        if (Status != ConversionStatus.Approved)
            throw new InvalidOperationException("Only approved conversions can be settled.");
        Status = ConversionStatus.Settled;
        SettlementId = settlementId;
        SettledAt = DateTime.UtcNow;
    }
}
