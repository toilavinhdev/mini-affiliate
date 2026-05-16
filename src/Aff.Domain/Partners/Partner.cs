using Aff.Domain.Common;

namespace Aff.Domain.Partners;

public class Partner : AggregateRoot
{
    public string BusinessName { get; private set; } = default!;
    public string ContactEmail { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public string TaxCode { get; private set; } = default!;
    public string BankName { get; private set; } = default!;
    public string BankAccountNumber { get; private set; } = default!;
    public string BankAccountHolder { get; private set; } = default!;
    public PartnerStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Partner() { }

    public static Partner Register(
        string businessName,
        string contactEmail,
        string phoneNumber,
        string taxCode,
        string bankName,
        string bankAccountNumber,
        string bankAccountHolder)
    {
        return new Partner
        {
            Id = Guid.NewGuid(),
            BusinessName = businessName,
            ContactEmail = contactEmail,
            PhoneNumber = phoneNumber,
            TaxCode = taxCode,
            BankName = bankName,
            BankAccountNumber = bankAccountNumber,
            BankAccountHolder = bankAccountHolder,
            Status = PartnerStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve()
    {
        if (Status != PartnerStatus.Pending)
            throw new InvalidOperationException("Only pending partners can be approved.");
        Status = PartnerStatus.Active;
        ApprovedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (Status != PartnerStatus.Pending)
            throw new InvalidOperationException("Only pending partners can be rejected.");
        Status = PartnerStatus.Rejected;
        RejectionReason = reason;
    }

    public void Suspend()
    {
        if (Status != PartnerStatus.Active)
            throw new InvalidOperationException("Only active partners can be suspended.");
        Status = PartnerStatus.Suspended;
    }

    public void Update(string businessName, string phoneNumber, string taxCode,
        string bankName, string bankAccountNumber, string bankAccountHolder)
    {
        BusinessName = businessName;
        PhoneNumber = phoneNumber;
        TaxCode = taxCode;
        BankName = bankName;
        BankAccountNumber = bankAccountNumber;
        BankAccountHolder = bankAccountHolder;
    }
}
