using Aff.API.Endpoints;
using Aff.API.Middleware;
using Aff.Application.Audit;
using Aff.Application.Campaigns;
using Aff.Application.Partners;
using Aff.Application.Settlements;
using Aff.Application.Tracking;
using Aff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AffDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
                      ?? "Data Source=aff.db", b => b.MigrationsAssembly("Aff.API")));

builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<PartnerService>();
builder.Services.AddScoped<CampaignService>();
builder.Services.AddScoped<TrackingService>();
builder.Services.AddScoped<SettlementService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new() { Title = "Affiliate System API", Version = "v1" }); });

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AffDbContext>();
    db.Database.Migrate();
}

app.UseCors();
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPartnerEndpoints();
app.MapCampaignEndpoints();
app.MapLinkEndpoints();
app.MapTrackingEndpoints();
app.MapConversionEndpoints();
app.MapSettlementEndpoints();
app.MapAuditEndpoints();

app.Run();