using Aff.Application.Tracking;
using Aff.Application.Tracking.Dtos;

namespace Aff.API.Endpoints;

public static class TrackingEndpoints
{
    public static IEndpointRouteBuilder MapTrackingEndpoints(this IEndpointRouteBuilder app)
    {
        // Click redirect — the entry point for all affiliate traffic
        app.MapGet("/r/{trackingCode}", async (string trackingCode, TrackingService svc,
            HttpContext ctx) =>
        {
            var ipAddress = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = ctx.Request.Headers.UserAgent.ToString();
            var referer = ctx.Request.Headers.Referer.ToString();

            var (targetUrl, clickId) = await svc.HandleClickAsync(trackingCode, ipAddress, userAgent, referer);

            // Set cookie for 7-day attribution window
            ctx.Response.Cookies.Append("aff_click_id", clickId.ToString(), new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });

            // Append click_id to target URL for server-side attribution
            var separator = targetUrl.Contains('?') ? "&" : "?";
            return Results.Redirect($"{targetUrl}{separator}aff_click={clickId}", permanent: false);
        }).WithTags("Tracking");

        // Webhook from integrated services (e-commerce, food delivery, etc.)
        app.MapPost("/api/webhooks/conversion", async (WebhookConversionRequest req, TrackingService svc) =>
        {
            var result = await svc.HandleWebhookAsync(req);
            return Results.Ok(result);
        }).WithTags("Tracking");

        return app;
    }
}
