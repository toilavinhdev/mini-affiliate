using Aff.Domain.Common;

namespace Aff.Domain.Settlements;

public class SettlementItem : Entity
{
    public Guid SettlementId { get; private set; }
    public Guid ConversionId { get; private set; }
    public decimal CommissionAmount { get; private set; }

    private SettlementItem() { }

    public static SettlementItem Create(Guid settlementId, Guid conversionId, decimal commissionAmount)
    {
        return new SettlementItem
        {
            Id = Guid.NewGuid(),
            SettlementId = settlementId,
            ConversionId = conversionId,
            CommissionAmount = commissionAmount
        };
    }
}
