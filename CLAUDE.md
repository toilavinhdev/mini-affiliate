# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build src/Aff.API/Aff.API.csproj

# Run API (http://localhost:5223, swagger at /swagger)
dotnet run --project src/Aff.API/Aff.API.csproj

# Serve frontend (separate terminal) — requires dotnet-serve global tool
# Install once: dotnet tool install -g dotnet-serve
dotnet serve -d frontend -p 8081
# CMS:  http://localhost:8081/cms.html
# Shop: http://localhost:8081/shop.html

# Add EF Core migration
dotnet ef migrations add <MigrationName> --project src/Aff.API --context AffDbContext

# Apply migrations manually (also runs automatically on startup)
dotnet ef database update --project src/Aff.API
```

The SQLite database file (`aff.db`) is created at `src/Aff.API/aff.db` on first run. Migrations apply automatically via `db.Database.Migrate()` in `Program.cs`.

## Architecture

Clean Architecture in 4 projects under `src/`:

```
Aff.Domain        → Entities, enums, no dependencies
Aff.Application   → Services + DTOs, depends on Domain + Infrastructure
Aff.Infrastructure → EF Core DbContext + config, depends on Domain
Aff.API           → Minimal API endpoints + middleware, depends on Application
```

**No controllers** — routes are defined in `Aff.API/Endpoints/` as static extension classes (`MapXxxEndpoints()`), each registered in `Program.cs`.

**Domain entities** use private setters and static factory methods (e.g., `Partner.Register(...)`, `Conversion.Create(...)`). State transitions are enforced inside the entity (e.g., `partner.Approve()` throws if not `Pending`). Never set entity properties directly from services.

**Application services** receive a scoped `AffDbContext` via primary constructor injection. They return DTOs (records defined alongside the service), never domain entities. Mappers are static classes (`PartnerResponseMapper`, `CampaignMapper`, `ConversionMapper`, `SettlementMapper`).

**Error handling** is centralized in `Aff.API/Middleware/ExceptionMiddleware.cs`:
- `KeyNotFoundException` → 404
- `InvalidOperationException` → 400

## Key Domain Flows

### Affiliate Click Tracking
`GET /r/{trackingCode}` → `TrackingService.HandleClickAsync()` → records a `Click` entity, increments `AffiliateLink.TotalClicks`, sets cookie `aff_click_id` (7-day HttpOnly), redirects to `{targetUrl}?aff_click={clickId}`.

The `aff_click` query param (the Click GUID) is how the mobile app receives attribution without relying on cookies.

### Conversion Attribution
`POST /api/webhooks/conversion` accepts either `clickId` (preferred, exact match) or `trackingCode` (finds latest unconverted click). Commission is calculated by `Campaign.CalculateCommission()` based on `CommissionType` enum (0 = `PercentageOfOrder`, 1 = `FixedPerConversion`). Budget guard runs before saving.

### Settlement Generation
Aggregates `Approved` conversions within a date range, groups by partner, applies tax rate, creates one `Settlement` + `SettlementItem`s per partner. Marks conversions as `Settled`. Idempotency: conversions with `SettlementId != null` are excluded.

### Partner Lifecycle
`Pending` → `Active` (approve) or `Rejected` (reject with reason). `Active` → `Suspended`. A partner must be `Active` to create campaigns; campaigns must be `Active` to accept clicks.

## API Endpoints Summary

```
GET    /api/partners                           List all partners
POST   /api/partners/register                  Register partner
GET    /api/partners/{id}                      Get partner
PUT    /api/partners/{id}                      Update partner
POST   /api/partners/{id}/approve|reject|suspend
GET    /api/partners/{id}/dashboard            Aggregated stats
GET    /api/partners/{id}/campaigns            Partner's campaigns
GET    /api/partners/{id}/conversions          Partner's conversions
GET    /api/partners/{id}/settlements          Partner's settlements

GET    /api/campaigns                          List all campaigns
POST   /api/campaigns/                         Create campaign
GET    /api/campaigns/{id}                     Get campaign
PUT    /api/campaigns/{id}                     Update campaign
POST   /api/campaigns/{id}/activate|pause
POST   /api/campaigns/{id}/links               Create affiliate link
GET    /api/campaigns/{id}/links               List campaign links

DELETE /api/links/{id}                         Deactivate link

GET    /r/{trackingCode}                        Click redirect (entry point)
POST   /api/webhooks/conversion                 Record conversion

GET    /api/conversions                        List all conversions
GET    /api/conversions/{id}
POST   /api/conversions/{id}/approve|reject

GET    /api/settlements                        List all settlements
POST   /api/settlements/generate               Generate for period
GET    /api/settlements/{id}
POST   /api/settlements/{id}/process           Mark paid
```

JSON uses `camelCase` (configured globally). Enum values are serialized as strings (e.g., `"Active"`, `"Pending"`). CommissionType is sent as integer (0/1) on create/update, returned as string.

## Frontend (No-Cookie Mobile Mock)

`frontend/cms.html` — Admin panel (no framework, vanilla JS + Tailwind CDN). All API calls go to `http://localhost:5223`. Tabs: Dashboard, Partners, Campaigns, Links, Conversions, Settlements.

`frontend/shop.html` — Simulates a mobile app that receives affiliate links but **cannot use cookies**:
1. Reads `?aff_click={clickId}` from redirect URL
2. Saves to `localStorage` only (never `document.cookie`)
3. On checkout, sends `POST /api/webhooks/conversion` with `clickId` from localStorage
4. "Debug" panel shows cookies vs localStorage state in real time

The Target URL when creating affiliate links should point to `http://localhost:8081/shop.html` so the API redirect lands on the shop.
