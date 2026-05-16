using Aff.Application.Campaigns;
using Aff.Application.Campaigns.Dtos;

namespace Aff.API.Endpoints;

public static class CampaignEndpoints
{
    public static IEndpointRouteBuilder MapCampaignEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/campaigns").WithTags("Campaigns");

        group.MapGet("/", async (CampaignService svc) =>
        {
            var result = await svc.GetAllAsync();
            return Results.Ok(result);
        });

        group.MapPost("/", async (CreateCampaignRequest req, CampaignService svc) =>
        {
            var result = await svc.CreateAsync(req);
            return Results.Created($"/api/campaigns/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, CampaignService svc) =>
        {
            var result = await svc.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateCampaignRequest req, CampaignService svc) =>
        {
            var result = await svc.UpdateAsync(id, req);
            return Results.Ok(result);
        });

        group.MapPost("/{id:guid}/activate", async (Guid id, CampaignService svc) =>
        {
            var result = await svc.ActivateAsync(id);
            return Results.Ok(result);
        });

        group.MapPost("/{id:guid}/pause", async (Guid id, CampaignService svc) =>
        {
            var result = await svc.PauseAsync(id);
            return Results.Ok(result);
        });

        group.MapPost("/{id:guid}/links", async (Guid id, CreateLinkRequest req,
            CampaignService svc, HttpRequest httpReq) =>
        {
            var baseUrl = $"{httpReq.Scheme}://{httpReq.Host}";
            var result = await svc.CreateLinkAsync(id, req, baseUrl);
            return Results.Created($"/api/campaigns/{id}/links/{result.Id}", result);
        });

        group.MapGet("/{id:guid}/links", async (Guid id, CampaignService svc, HttpRequest httpReq) =>
        {
            var baseUrl = $"{httpReq.Scheme}://{httpReq.Host}";
            var result = await svc.GetLinksAsync(id, baseUrl);
            return Results.Ok(result);
        });

        return app;
    }
}
