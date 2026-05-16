using Aff.Domain.Common;

namespace Aff.Domain.Settlements;

public class Settlement : AggregateRoot
{
    public Guid PartnerId { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public int TotalConversions { get; private set; }
    public decimal GrossCommission { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal NetCommission { get; private set; }
    public SettlementStatus Status { get; private set; }
    public string? PaymentReference { get; private set; }
    public string? Note { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private readonly List<SettlementItem> _items = [];
    public IReadOnlyCollection<SettlementItem> Items => _items.AsReadOnly();

    private Settlement() { }

    public static Settlement Create(Guid partnerId, DateTime periodStart, DateTime periodEnd,
        IEnumerable<(Guid conversionId, decimal commission)> conversions, decimal taxRate = 0.10m)
    {
        var items = conversions.ToList();
        var gross = items.Sum(c => c.commission);
        var tax = Math.Round(gross * taxRate, 2);
        var settlement = new Settlement
        {
            Id = Guid.NewGuid(),
            PartnerId = partnerId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            TotalConversions = items.Count,
            GrossCommission = gross,
            TaxRate = taxRate,
            TaxAmount = tax,
            NetCommission = gross - tax,
            Status = SettlementStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        foreach (var (conversionId, commission) in items)
            settlement._items.Add(SettlementItem.Create(settlement.Id, conversionId, commission));
        return settlement;
    }

    public void Complete(string paymentReference, string? note = null)
    {
        if (Status != SettlementStatus.Pending && Status != SettlementStatus.Processing)
            throw new InvalidOperationException("Settlement cannot be completed in current state.");
        Status = SettlementStatus.Completed;
        PaymentReference = paymentReference;
        Note = note;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string? note = null)
    {
        Status = SettlementStatus.Failed;
        Note = note;
        ProcessedAt = DateTime.UtcNow;
    }
}
