# Affiliate Management System

An affiliate marketing management system consisting of a backend API, a web-based CMS, and a mobile app simulator.

---

## Overview

The system allows businesses (Partners) to register and run affiliate campaigns. Each campaign generates unique tracking links. When an end user clicks a link and makes a purchase, the system automatically records the conversion and calculates the commission.

Key feature: **the mobile app does not use cookies** — tracking works by passing a `click_id` through the URL and storing it in `localStorage`, with support for **per-product Last-Click Attribution**.

---

## Architecture

```
Aff/
├── src/
│   ├── Aff.Domain          # Entities, enums, domain logic
│   ├── Aff.Application     # Services, DTOs
│   ├── Aff.Infrastructure  # EF Core, SQLite, DbContext
│   └── Aff.API             # Minimal API endpoints, middleware
└── frontend/
    ├── cms.html            # Affiliate management CMS
    └── shop.html           # Mobile App simulator (no-cookie)
```

4-layer Clean Architecture. The API uses **ASP.NET Core Minimal API** (no controllers). The database is **SQLite** and migrations run automatically on startup.

---

## How It Works

```
Partner registers → Admin approves → Create Campaign → Create Affiliate Link
                                                               │
                                                 User clicks tracking URL
                                                               │
                                         API records Click, redirects to Shop
                                         with ?aff_click={clickId} in the URL
                                                               │
                                         Shop (mobile mock) saves clickId
                                         to localStorage (NO cookie used)
                                                               │
                                                      User makes a purchase
                                                               │
                                         Shop sends POST /api/webhooks/conversion
                                         with clickId from localStorage
                                                               │
                                         API calculates commission → Conversion (Pending)
                                                               │
                                         Admin approves → Approved → Settlement
```

### Per-Product Tracking & Last-Click Attribution

Each affiliate link can target a **specific product** (via `?product=P001` in the target URL). `localStorage` stores tracking data per product:

```js
aff_products = {
  "P001": { clickId: "...", ts: 1234567890 },
  "ALL":  { clickId: "...", ts: 1234567891 }  // shop-wide link
}
```

If the same product is clicked from two different partners, **the later click overwrites the earlier one** (last-click wins). At checkout, each tracked product sends its own webhook, resulting in multiple independent conversions.

---

## Getting Started

### Requirements

- .NET 8 SDK
- `dotnet-serve` global tool (to serve the frontend)

```bash
dotnet tool install -g dotnet-serve
```

### Running

```bash
# Terminal 1 — API (http://localhost:5223)
dotnet run --project src/Aff.API/Aff.API.csproj

# Terminal 2 — Frontend (http://localhost:8081)
dotnet serve -d frontend -p 8081
```

| URL | Description |
|-----|-------------|
| `http://localhost:8081/cms.html` | Affiliate management CMS |
| `http://localhost:8081/shop.html` | Mobile App simulator |
| `http://localhost:5223/swagger` | Swagger UI (Development only) |

---

## API Reference

### Partners

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/partners` | List all partners |
| `POST` | `/api/partners/register` | Register a new partner |
| `GET` | `/api/partners/{id}` | Get partner details |
| `PUT` | `/api/partners/{id}` | Update partner info |
| `POST` | `/api/partners/{id}/approve` | Approve a pending partner |
| `POST` | `/api/partners/{id}/reject` | Reject a partner (requires reason) |
| `POST` | `/api/partners/{id}/suspend` | Suspend an active partner |
| `GET` | `/api/partners/{id}/dashboard` | Aggregated partner stats |
| `GET` | `/api/partners/{id}/campaigns` | Partner's campaigns |
| `GET` | `/api/partners/{id}/conversions` | Partner's conversions |
| `GET` | `/api/partners/{id}/settlements` | Partner's settlements |

### Campaigns & Links

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/campaigns` | List all campaigns |
| `POST` | `/api/campaigns/` | Create a campaign |
| `POST` | `/api/campaigns/{id}/activate` | Activate a campaign |
| `POST` | `/api/campaigns/{id}/pause` | Pause a campaign |
| `POST` | `/api/campaigns/{id}/links` | Create an affiliate link |
| `GET` | `/api/campaigns/{id}/links` | List campaign links |
| `DELETE` | `/api/links/{id}` | Deactivate a link |

### Tracking

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/r/{trackingCode}` | Click redirect — entry point for all affiliate traffic |
| `POST` | `/api/webhooks/conversion` | Receive a conversion event from an integrated service |

### Conversions

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/conversions` | List all conversions |
| `GET` | `/api/conversions/{id}` | Get conversion details |
| `POST` | `/api/conversions/{id}/approve` | Approve a conversion |
| `POST` | `/api/conversions/{id}/reject` | Reject a conversion (requires reason) |

### Settlements

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/settlements` | List all settlements |
| `POST` | `/api/settlements/generate` | Generate settlements from selected conversions |
| `GET` | `/api/settlements/{id}` | Get settlement details |
| `POST` | `/api/settlements/{id}/process` | Confirm payment (with payment reference) |

#### Webhook Conversion Payload

```json
{
  "serviceType": "ecommerce",
  "serviceTransactionId": "ORD-1234-P001",
  "endUserId": "user@example.com",
  "transactionAmount": 890000,
  "clickId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

Attribution prioritizes `clickId` (exact match). Falls back to `trackingCode` (most recent unconverted click) if `clickId` is not provided.

---

## Domain Model

### Partner Status

```
Pending → Active    (approve)
Pending → Rejected  (reject)
Active  → Suspended (suspend)
```

### Campaign Status

```
Draft          → Active (activate)
Active         → Paused (pause)
Active/Paused  → Ended
```

### Conversion Status

```
Pending  → Approved (admin approves)
Pending  → Rejected (admin rejects)
Approved → Settled  (included in a settlement)
```

### Commission Types

| CommissionType | Description |
|----------------|-------------|
| `PercentageOfOrder` (0) | A percentage of the order value |
| `FixedPerConversion` (1) | A fixed amount per conversion |

---

## CMS Features

- **Dashboard** — System-wide stats and recent conversions
- **Partners** — Register, approve/reject/suspend, view per-partner dashboard
- **Campaigns** — Create, activate/pause, view performance
- **Affiliate Links** — Create links targeting a specific product or the entire shop, copy tracking URL, simulate a click
- **Conversions** — List all, approve/reject individually
- **Settlements** — Manually pick which approved conversions to settle, preview gross/tax/net in real time before confirming

---

## Mobile App Simulator — No-Cookie Mechanism

`shop.html` simulates the behavior of a mobile app that cannot use cookies:

1. Receives `?aff_click={clickId}` from the API redirect URL
2. Does **not** write to `document.cookie`
3. Saves to `localStorage.aff_products`, keyed per product
4. At checkout, sends one webhook per tracked product
5. The **Debug** tab shows real-time cookie state, localStorage state, and a preview of the webhook payloads that will be sent

---

## Database

SQLite, stored at `src/Aff.API/aff.db`. Migrations run automatically on API startup.

To add a migration after changing the domain model:

```bash
dotnet ef migrations add <MigrationName> --project src/Aff.API --context AffDbContext
```
