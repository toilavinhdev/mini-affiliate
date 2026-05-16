using Aff.Application.Audit;

namespace Aff.API.Endpoints;

public static class AuditEndpoints
{
    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/audit").WithTags("Audit");

        group.MapGet("/", async (string? entityType, Guid? entityId, AuditService svc) =>
        {
            var result = await svc.GetLogsAsync(entityType, entityId);
            return Results.Ok(result);
        });

        return app;
    }
}
