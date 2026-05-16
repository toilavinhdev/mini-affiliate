using Aff.Domain.Common;

namespace Aff.Domain.Campaigns;

public class Campaign : AggregateRoot
{
    public Guid PartnerId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string ServiceType { get; private set; } = default!;
    public CommissionType CommissionType { get; private set; }
    public decimal CommissionValue { get; private set; }
    public decimal? MaxBudget { get; private set; }
    public decimal SpentBudget { get; private set; }
    public CampaignStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Campaign() { }

    public static Campaign Create(Guid partnerId, string name, string description,
        string serviceType, CommissionType commissionType, decimal commissionValue,
        DateTime startDate, DateTime? endDate = null, decimal? maxBudget = null)
    {
        return new Campaign
        {
            Id = Guid.NewGuid(),
            PartnerId = partnerId,
            Name = name,
            Description = description,
            ServiceType = serviceType,
            CommissionType = commissionType,
            CommissionValue = commissionValue,
            MaxBudget = maxBudget,
            SpentBudget = 0,
            Status = CampaignStatus.Draft,
            StartDate = startDate,
            EndDate = endDate,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Activate()
    {
        if (Status != CampaignStatus.Draft && Status != CampaignStatus.Paused)
            throw new InvalidOperationException("Campaign must be in Draft or Paused state to activate.");
        Status = CampaignStatus.Active;
    }

    public void Pause()
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Only active campaigns can be paused.");
        Status = CampaignStatus.Paused;
    }

    public void End()
    {
        Status = CampaignStatus.Ended;
    }

    public void Update(string name, string description, CommissionType commissionType,
        decimal commissionValue, DateTime startDate, DateTime? endDate, decimal? maxBudget)
    {
        if (Status == CampaignStatus.Ended)
            throw new InvalidOperationException("Cannot update an ended campaign.");
        Name = name;
        Description = description;
        CommissionType = commissionType;
        CommissionValue = commissionValue;
        StartDate = startDate;
        EndDate = endDate;
        MaxBudget = maxBudget;
    }

    public decimal CalculateCommission(decimal transactionAmount)
    {
        return CommissionType == CommissionType.PercentageOfOrder
            ? transactionAmount * CommissionValue / 100m
            : CommissionValue;
    }

    public bool HasBudgetFor(decimal commission)
    {
        return MaxBudget == null || (SpentBudget + commission) <= MaxBudget;
    }

    public void AddSpentBudget(decimal commission) => SpentBudget += commission;

    public void RefundSpentBudget(decimal commission) => SpentBudget = Math.Max(0, SpentBudget - commission);
}
