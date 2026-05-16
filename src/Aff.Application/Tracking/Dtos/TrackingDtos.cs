using Aff.Domain.Tracking;

namespace Aff.Application.Tracking.Dtos;

public record WebhookConversionRequest(
    string ServiceType,
    string ServiceTransactionId,
    string EndUserId,
    decimal TransactionAmount,
    string? TrackingCode = null,
    Guid? ClickId = null);

public record RejectConversionRequest(string Reason);

public record ConversionResponse(
    Guid Id,
    Guid? ClickId,
    string TrackingCode,
    Guid CampaignId,
    Guid PartnerId,
    string ServiceType,
    string ServiceTransactionId,
    string EndUserId,
    decimal TransactionAmount,
    decimal CommissionAmount,
    string Status,
    string? RejectionReason,
    bool IsSuspicious,
    string? FraudReason,
    DateTime CreatedAt,
    DateTime? ApprovedAt,
    Guid? SettlementId,
    DateTime? SettledAt);

public static class ConversionMapper
{
    public static ConversionResponse ToResponse(Conversion c) => new(
        c.Id, c.ClickId, c.TrackingCode, c.CampaignId, c.PartnerId,
        c.ServiceType, c.ServiceTransactionId, c.EndUserId,
        c.TransactionAmount, c.CommissionAmount, c.Status.ToString(),
        c.RejectionReason, c.IsSuspicious, c.FraudReason,
        c.CreatedAt, c.ApprovedAt, c.SettlementId, c.SettledAt);
}
