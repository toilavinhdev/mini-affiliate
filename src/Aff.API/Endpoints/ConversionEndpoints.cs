using Aff.Application.Tracking;
using Aff.Application.Tracking.Dtos;

namespace Aff.API.Endpoints;

public static class ConversionEndpoints
{
    public static IEndpointRouteBuilder MapConversionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/conversions").WithTags("Conversions");

        group.MapGet("/", async (TrackingService svc) =>
        {
            var result = await svc.GetAllConversionsAsync();
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, TrackingService svc) =>
        {
            var result = await svc.GetConversionAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        group.MapPost("/{id:guid}/approve", async (Guid id, TrackingService svc) =>
        {
            var result = await svc.ApproveConversionAsync(id);
            return Results.Ok(result);
        });

        group.MapPost("/{id:guid}/reject", async (Guid id, RejectConversionRequest req, TrackingService svc) =>
        {
            var result = await svc.RejectConversionAsync(id, req.Reason);
            return Results.Ok(result);
        });

        app.MapGet("/api/partners/{partnerId:guid}/conversions",
            async (Guid partnerId, TrackingService svc) =>
            {
                var result = await svc.GetPartnerConversionsAsync(partnerId);
                return Results.Ok(result);
            }).WithTags("Conversions");

        return app;
    }
}
