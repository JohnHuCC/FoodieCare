# FoodieCare Modern API (Migration Track)

This is the cross-platform replacement track for the legacy `WebSite2` (ASP.NET Web Forms / .NET Framework).

## Why this exists

- Runs on macOS/Linux/Windows with .NET 8
- Keeps core recommendation flow while removing Web Forms coupling
- Moves SQL to parameterized queries
- Starts replacing Session-based auth with token-based auth

## Prerequisites

1. Install .NET 8 SDK
2. Run MySQL and import `foodiecare.sql`
3. Update `ModernApi/appsettings.json` connection string and `AuthSecret`

## Run

```bash
cd ModernApi
dotnet restore
dotnet run
```

Default URL (dev): `http://localhost:5000` or `https://localhost:5001`.

Frontend pages:

- `/index.html` login/register
- `/main.html` location + entry
- `/recommender.html` recommendation flow
- `/browse.html` nearby food browser flow

`recommender.html` now includes:
- card-based candidate selection
- paginated store list (10 per page)
- embedded map preview panel

Global fallback mode:
- If DB query fails or returns empty, the API auto-falls back to `Data/global_stores.json`.
- Fallback is global (not city-limited), and always returns nearest seed stores to avoid empty UI.
- You can use `Use Global Demo (London)` in `/main.html` for a quick test.

## Endpoints

### Public

- `GET /api/health`
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/recommendation/options`
- `POST /api/recommendation/stores`
- `POST /api/stores/browse`

### Auth required (Bearer token)

- `POST /api/interactions/click`
- `POST /api/interactions/recommendation-feedback`

## Example flow

### 1) Register

```json
POST /api/auth/register
{
  "username": "demo_user",
  "password": "demo_pass",
  "gender": 1,
  "age": 22
}
```

### 2) Login

```json
POST /api/auth/login
{
  "username": "demo_user",
  "password": "demo_pass"
}
```

Response includes `token`.

### 3) Get recommendation options

```json
POST /api/recommendation/options
{
  "userId": 1,
  "price": "100_199",
  "eatMode": "friend",
  "hunger": "eat_less",
  "distance": "1to5km",
  "hotCold": "hot",
  "taste": "normal",
  "latitude": 22.6282,
  "longitude": 120.2620
}
```

### 4) Search stores

```json
POST /api/recommendation/stores
{
  "type": "日式料理",
  "price": "100_199",
  "latitude": 22.6282,
  "longitude": 120.2620,
  "distanceKm": 5,
  "limit": 20
}
```

### 5) Record click (Bearer required)

```json
POST /api/interactions/click
Authorization: Bearer <token>
{
  "storeName": "某某店"
}
```

### 6) Record questionnaire feedback (Bearer required)

```json
POST /api/interactions/recommendation-feedback
Authorization: Bearer <token>
{
  "tasteId": 1,
  "hungerId": 2,
  "hotColdId": 1,
  "eatModeId": 2,
  "distanceId": 2,
  "priceId": 1,
  "type": "小吃",
  "agree": true,
  "storeName": "某某店"
}
```

## Current scope

This migration pass covers recommendation engine, store query, auth, and user interactions.
Legacy Web Forms pages are still online and should be replaced page-by-page.
