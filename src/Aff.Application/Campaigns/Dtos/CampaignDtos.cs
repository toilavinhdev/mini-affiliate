using Aff.Domain.Campaigns;

namespace Aff.Application.Campaigns.Dtos;

public record CreateCampaignRequest(
    Guid PartnerId,
    string Name,
    string Description,
    string ServiceType,
    CommissionType CommissionType,
    decimal CommissionValue,
    DateTime StartDate,
    DateTime? EndDate = null,
    decimal? MaxBudget = null,
    int? AttributionWindowDays = null);

public record UpdateCampaignRequest(
    string Name,
    string Description,
    CommissionType CommissionType,
    decimal CommissionValue,
    DateTime StartDate,
    DateTime? EndDate = null,
    decimal? MaxBudget = null,
    int? AttributionWindowDays = null);

public record CreateLinkRequest(
    string TargetUrl,
    string Label = "Default");

public record CampaignResponse(
    Guid Id,
    Guid PartnerId,
    string Name,
    string Description,
    string ServiceType,
    string CommissionType,
    decimal CommissionValue,
    decimal? MaxBudget,
    decimal SpentBudget,
    string Status,
    DateTime StartDate,
    DateTime? EndDate,
    int? AttributionWindowDays,
    DateTime CreatedAt);

public record AffiliateLinkResponse(
    Guid Id,
    Guid CampaignId,
    Guid PartnerId,
    string TrackingCode,
    string TrackingUrl,
    string TargetUrl,
    string Label,
    bool IsActive,
    int TotalClicks,
    int TotalConversions,
    DateTime CreatedAt);

public static class CampaignMapper
{
    public static CampaignResponse ToResponse(Campaign c) => new(
        c.Id, c.PartnerId, c.Name, c.Description, c.ServiceType,
        c.CommissionType.ToString(), c.CommissionValue, c.MaxBudget,
        c.SpentBudget, c.Status.ToString(), c.StartDate, c.EndDate,
        c.AttributionWindowDays, c.CreatedAt);

    public static AffiliateLinkResponse ToResponse(AffiliateLink l, string baseUrl) => new(
        l.Id, l.CampaignId, l.PartnerId, l.TrackingCode,
        $"{baseUrl}/r/{l.TrackingCode}",
        l.TargetUrl, l.Label, l.IsActive, l.TotalClicks, l.TotalConversions, l.CreatedAt);
}
