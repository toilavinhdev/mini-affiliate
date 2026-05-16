using Aff.Domain.Partners;

namespace Aff.Application.Partners.Dtos;

public record RegisterPartnerRequest(
    string BusinessName,
    string ContactEmail,
    string PhoneNumber,
    string TaxCode,
    string BankName,
    string BankAccountNumber,
    string BankAccountHolder);

public record UpdatePartnerRequest(
    string BusinessName,
    string PhoneNumber,
    string TaxCode,
    string BankName,
    string BankAccountNumber,
    string BankAccountHolder);

public record RejectPartnerRequest(string Reason);

public record PartnerResponse(
    Guid Id,
    string BusinessName,
    string ContactEmail,
    string PhoneNumber,
    string TaxCode,
    string BankName,
    string BankAccountNumber,
    string BankAccountHolder,
    string Status,
    string? RejectionReason,
    DateTime? ApprovedAt,
    DateTime CreatedAt);

public record PartnerDashboardResponse(
    Guid PartnerId,
    string BusinessName,
    int TotalCampaigns,
    int ActiveCampaigns,
    int TotalClicks,
    int TotalConversions,
    int PendingConversions,
    int ApprovedConversions,
    decimal TotalCommissionEarned,
    decimal PendingCommission,
    int TotalSettlements,
    decimal TotalSettled);

public static class PartnerResponseMapper
{
    public static PartnerResponse ToResponse(Partner p) => new(
        p.Id, p.BusinessName, p.ContactEmail, p.PhoneNumber, p.TaxCode,
        p.BankName, p.BankAccountNumber, p.BankAccountHolder,
        p.Status.ToString(), p.RejectionReason, p.ApprovedAt, p.CreatedAt);
}
