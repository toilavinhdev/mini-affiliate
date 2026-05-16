using Aff.Domain.Settlements;

namespace Aff.Application.Settlements.Dtos;

public record GenerateSettlementsRequest(
    List<Guid> ConversionIds,
    decimal TaxRate = 0.10m);

public record ProcessSettlementRequest(
    string PaymentReference,
    string? Note = null);

public record SettlementItemResponse(
    Guid Id,
    Guid ConversionId,
    decimal CommissionAmount);

public record SettlementResponse(
    Guid Id,
    Guid PartnerId,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    int TotalConversions,
    decimal GrossCommission,
    decimal TaxRate,
    decimal TaxAmount,
    decimal NetCommission,
    string Status,
    string? PaymentReference,
    string? Note,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    IReadOnlyCollection<SettlementItemResponse> Items);

public static class SettlementMapper
{
    public static SettlementResponse ToResponse(Settlement s) => new(
        s.Id, s.PartnerId, s.PeriodStart, s.PeriodEnd,
        s.TotalConversions, s.GrossCommission, s.TaxRate, s.TaxAmount, s.NetCommission,
        s.Status.ToString(), s.PaymentReference, s.Note, s.CreatedAt, s.ProcessedAt,
        s.Items.Select(i => new SettlementItemResponse(i.Id, i.ConversionId, i.CommissionAmount))
               .ToList().AsReadOnly());
}
