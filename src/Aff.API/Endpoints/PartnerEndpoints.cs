using Aff.Application.Partners;
using Aff.Application.Partners.Dtos;

namespace Aff.API.Endpoints;

public static class PartnerEndpoints
{
    public static IEndpointRouteBuilder MapPartnerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/partners").WithTags("Partners");

        group.MapGet("/", async (PartnerService svc) =>
        {
            var result = await svc.GetAllAsync();
            return Results.Ok(result);
        });

        group.MapPost("/register", async (RegisterPartnerRequest req, PartnerService svc) =>
        {
            var result = await svc.RegisterAsync(req);
            return Results.Created($"/api/partners/{result.Id}", result);
        });

        group.MapGet("/{id:guid}", async (Guid id, PartnerService svc) =>
        {
            var result = await svc.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdatePartnerRequest req, PartnerService svc) =>
        {
            var result = await svc.UpdateAsync(id, req);
            return Results.Ok(result);
        });

        group.MapPost("/{id:guid}/approve", async (Guid id, PartnerService svc) =>
        {
            var result = await svc.ApproveAsync(id);
            return Results.Ok(result);
        });

        group.MapPost("/{id:guid}/reject", async (Guid id, RejectPartnerRequest req, PartnerService svc) =>
        {
            var result = await svc.RejectAsync(id, req.Reason);
            return Results.Ok(result);
        });

        group.MapPost("/{id:guid}/suspend", async (Guid id, PartnerService svc) =>
        {
            var result = await svc.SuspendAsync(id);
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}/dashboard", async (Guid id, PartnerService svc) =>
        {
            var result = await svc.GetDashboardAsync(id);
            return Results.Ok(result);
        });

        return app;
    }
}
