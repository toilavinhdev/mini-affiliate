using Aff.Application.Campaigns;

namespace Aff.API.Endpoints;

public static class LinkEndpoints
{
    public static IEndpointRouteBuilder MapLinkEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/links/{linkId:guid}", async (Guid linkId, CampaignService svc) =>
        {
            await svc.DeactivateLinkAsync(linkId);
            return Results.NoContent();
        }).WithTags("Links");

        app.MapGet("/api/partners/{partnerId:guid}/campaigns",
            async (Guid partnerId, CampaignService svc) =>
            {
                var result = await svc.GetByPartnerAsync(partnerId);
                return Results.Ok(result);
            }).WithTags("Campaigns");

        return app;
    }
}
