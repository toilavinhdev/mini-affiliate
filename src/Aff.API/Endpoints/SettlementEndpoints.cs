using Aff.Application.Settlements;
using Aff.Application.Settlements.Dtos;

namespace Aff.API.Endpoints;

public static class SettlementEndpoints
{
    public static IEndpointRouteBuilder MapSettlementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/settlements").WithTags("Settlements");

        group.MapGet("/", async (SettlementService svc) =>
        {
            var result = await svc.GetAllAsync();
            return Results.Ok(result);
        });

        group.MapPost("/generate", async (GenerateSettlementsRequest req, SettlementService svc) =>
        {
            var results = await svc.GenerateAsync(req);
            return Results.Ok(results);
        });

        group.MapGet("/{id:guid}", async (Guid id, SettlementService svc) =>
        {
            var result = await svc.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/{id:guid}/process", async (Guid id, ProcessSettlementRequest req, SettlementService svc) =>
        {
            var result = await svc.ProcessAsync(id, req);
            return Results.Ok(result);
        });

        app.MapGet("/api/partners/{partnerId:guid}/settlements",
            async (Guid partnerId, SettlementService svc) =>
            {
                var result = await svc.GetByPartnerAsync(partnerId);
                return Results.Ok(result);
            }).WithTags("Settlements");

        return app;
    }
}
