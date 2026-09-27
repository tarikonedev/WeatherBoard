# WeatherBoard – Technical Specification

*Sep 27, 2026 · @Taras Pokushevskyi*

WeatherBoard is a multi-city weather dashboard: a React (Vite) frontend backed by a .NET API, using Open-Meteo as the weather/geocoding data source.

## Overview

**Goal:** a dashboard where a user searches for cities, adds them to a favorites list, and sees each favorite as a card with current weather, local time, and an animated weather visual.

**In scope for v1:**

- City search (geocoding) and add/remove favorites
- Current weather + today's high/low, humidity, wind
- Live local time per city, computed from that city's own timezone
- Animated weather visual per card, matched to current conditions
- User accounts (sign-up/sign-in) with favorites persisted per user via the .NET backend (not just localStorage)

**Out of scope for v1:**

- Multi-day/hourly forecast detail view
- Social login providers (email/password only for v1; see [Authentication](#authentication))
- Push/email weather alerts

## Architecture

WeatherBoard has four components:

- **React SPA** — the frontend. Never calls Open-Meteo directly.
- **.NET API** (ASP.NET Core) — the main backend. Proxies/caches Open-Meteo, and owns the favorites data. Validates the session on each request before touching favorites.
- **Auth microservice** (Node.js/TypeScript, running [better-auth](https://www.better-auth.com/)) — owns sign-up/sign-in/session issuance and the `user`/`session`/`account`/`verification` tables. Runs as its own service because better-auth is a JS/TS library and cannot run inside the .NET process. See [Authentication](#authentication).
- **SQL Server database** — shared by the .NET API (favorites) and the auth microservice (user/session/account/verification), so a favorite row can have a simple foreign key to a real user id.

Plus one external dependency: **Open-Meteo** (Forecast + Geocoding APIs), called only from the .NET API.

Routing the .NET API can cache weather responses, keep an API key server-side if OpenWeatherMap is ever added later, and own the favorites data — while auth concerns live entirely in the dedicated auth service.

## Tech stack

| Layer | Choice | Notes |
| --- | --- | --- |
| Frontend framework | React 18 + Vite | Fast dev server, small prod bundle |
| Frontend state | React Query (server cache) + Zustand or Context (favorites/UI state) | Query handles polling/refetch of weather |
| Styling | CSS Modules or Tailwind | Card-based, dark-mode friendly |
| Backend framework | ASP.NET Core Web API (.NET 8) | Minimal APIs or controllers |
| Backend data access | Entity Framework Core | Maps favorites to the database |
| Authentication | better-auth (Node.js/TypeScript microservice) | Runs separately from the .NET API since better-auth is JS/TS-only; owns the `user`, `session`, `account`, and `verification` tables (see [Authentication](#authentication)) |
| Auth database access | better-auth's built-in Kysely adapter | Kysely supports SQL Server (MSSQL) directly, so the auth service uses the same SQL Server instance without adding a second database |
| Database | SQL Server | Confirmed choice; shared by EF Core (favorites) and better-auth's Kysely adapter (auth tables) |
| External weather/geocoding | Open-Meteo (Forecast API + Geocoding API) | Free, no API key required |
| HTTP caching | In-memory cache or Redis in the .NET API | Avoids re-hitting Open-Meteo every dashboard refresh |
| Hosting | Azure App Service (.NET backend and Node auth microservice, as separate App Services); Azure Static Web Apps (frontend build) | Confirmed platform: Azure |

## Backend API design

The .NET API sits between the React app and Open-Meteo, and owns the favorites list. Authentication itself (sign-up, sign-in, sign-out, session refresh) is handled by the separate better-auth microservice, not the .NET API — see [Authentication](#authentication).

| Endpoint | Method | Purpose |
| --- | --- | --- |
| `/api/cities/search?q={text}` | GET | Proxies Open-Meteo Geocoding; returns matching cities (name, country, lat/lon, timezone) |
| `/api/weather/current?lat={lat}&lon={lon}` | GET | Proxies Open-Meteo Forecast; returns current conditions, today's high/low, humidity, wind, and the city's timezone/UTC offset |
| `/api/favorites` | GET | Returns the signed-in user's saved favorite cities, in display order |
| `/api/favorites` | POST | Adds a city (lat, lon, name, country, timezone) to favorites |
| `/api/favorites/{id}` | DELETE | Removes a favorite |
| `/api/favorites/reorder` | PUT | Updates display order after drag-and-drop |

The weather endpoint is a thin proxy so the API can add short-lived caching (e.g. 5–10 minutes) without the frontend needing to know Open-Meteo's shape.

Every `/api/favorites*` request must carry a valid better-auth session (cookie or bearer token from the auth microservice). The .NET API validates it by calling the auth microservice's session-check endpoint (better-auth's `getSession`), forwarding the incoming cookie/token, rather than reading the `session` table directly — this keeps the auth service as the single source of truth and avoids coupling the .NET API to better-auth's internal schema (which could change between versions, or move sessions to secondary storage like Redis instead of SQL Server). To avoid a network round-trip on every request, the .NET API caches validated `token → userId` results in memory for a short TTL (e.g. 30–60s). The API never trusts a client-supplied user id.

## Authentication

Real user accounts (email/password for v1) are handled by a dedicated Node.js/TypeScript microservice running [better-auth](https://www.better-auth.com/), rather than inside the ASP.NET Core API. better-auth is a JS/TS library with no .NET equivalent, so isolating it to its own small service keeps the rest of the backend on .NET while still getting battle-tested auth logic (password hashing, session rotation, email verification tokens, and a path to add social providers later without a rewrite).

The auth microservice owns four tables in the shared SQL Server database, using better-auth's core schema:

**`user`**

| Field | Type | Notes |
| --- | --- | --- |
| `id` | string | Primary key |
| `name` | string | Display name |
| `email` | string | Unique |
| `emailVerified` | boolean | |
| `image` | string \| null | Optional avatar URL |
| `createdAt` | datetime | |
| `updatedAt` | datetime | |

**`session`**

| Field | Type | Notes |
| --- | --- | --- |
| `id` | string | Primary key |
| `userId` | string | FK → `user.id` |
| `token` | string | Unique session token (the value stored in the session cookie) |
| `expiresAt` | datetime | |
| `ipAddress` | string \| null | |
| `userAgent` | string \| null | |
| `createdAt` | datetime | |
| `updatedAt` | datetime | |

**`account`**

| Field | Type | Notes |
| --- | --- | --- |
| `id` | string | Primary key (local account row id) |
| `userId` | string | FK → `user.id` |
| `accountId` | string | Provider-side identity; for the `credential` provider this is the user's `id` |
| `providerId` | string | `"credential"` for email/password in v1; a social provider name if added later |
| `password` | string \| null | Hashed password, credential accounts only |
| `accessToken`, `refreshToken`, `idToken` | string \| null | Unused in v1 (no OAuth providers yet) |
| `accessTokenExpiresAt`, `refreshTokenExpiresAt` | datetime \| null | |
| `scope` | string \| null | |
| `createdAt` | datetime | |
| `updatedAt` | datetime | |

**`verification`**

| Field | Type | Notes |
| --- | --- | --- |
| `id` | string | Primary key |
| `identifier` | string | e.g. the email being verified |
| `value` | string | The verification token |
| `expiresAt` | datetime | |
| `createdAt` | datetime | |
| `updatedAt` | datetime | |

The `favorites` table (owned by the .NET API/EF Core) gets a `userId` column that is a foreign key to `user.id` — replacing the earlier "anonymous device id" design. All favorites endpoints are scoped to the authenticated user's id, resolved from the validated session.

Schema management for these four tables uses better-auth's own CLI (`npx auth generate` / `auth migrate`) against the shared SQL Server database, kept separate from the EF Core migrations that manage the `favorites` table.

## Frontend data model & state

**FavoriteCity shape** (returned by `/api/favorites`, cached client-side):

```json
{
  "id": "guid",
  "name": "Kyiv",
  "country": "Ukraine",
  "lat": 50.45,
  "lon": 30.52,
  "timezone": "Europe/Kyiv",
  "order": 0
}
```

**State management:**

- Favorites list: fetched once from `/api/favorites`, cached in React Query; mutations (add/remove/reorder) call the API then invalidate the cache.
- Per-city weather: one React Query call per favorite (`/api/weather/current`), refetched on an interval (e.g. every 10 minutes) and on window focus.
- Local time per card: NOT fetched repeatedly — computed client-side every second from the city's `timezone`/UTC offset using `Intl.DateTimeFormat` with `timeZone`, so the clock is smooth without hammering the API.
- Search results: local component state, cleared once a city is added.

## Favorites persistence & local time

**Persistence:** favorites live in the database behind the .NET API, keyed to the signed-in user's `id` (see [Authentication](#authentication)). This survives across devices, since it's tied to the user's account rather than a browser or device.

**Local time per card:**

1. Open-Meteo's Forecast response includes `timezone` (IANA name, e.g. `Europe/Kyiv`) and `utc_offset_seconds` for the queried coordinates.
2. The backend passes the IANA timezone name through to the frontend as part of the favorite/weather payload.
3. The frontend computes and displays local time with `Intl.DateTimeFormat('en-GB', { timeZone: city.timezone, hour: '2-digit', minute: '2-digit' })`, updated via a `setInterval` every second (or every 30s if a coarser clock is acceptable) — never using the visitor's own device timezone.

## Weather animation approach

Open-Meteo returns a numeric WMO weather code. Map code ranges to an animation type, then render with CSS keyframes/SVG (no video/GIFs, for a light bundle):

| WMO code range | Condition | Animation |
| --- | --- | --- |
| 0 | Clear | Rotating/pulsing sun rays (day) or twinkling stars (night, based on card's local time) |
| 1–3 | Partly/mostly cloudy | Drifting cloud shapes, layered parallax |
| 45, 48 | Fog | Slow horizontal haze bands, low opacity |
| 51–67, 80–82 | Rain/drizzle | Falling animated line/drop particles |
| 71–77, 85–86 | Snow | Falling dot particles with slight horizontal drift |
| 95–99 | Thunderstorm | Cloud layer + periodic lightning flash (opacity keyframe) |

Implementation: one React component per animation type (`<ClearSky />`, `<Rainy />`, etc.), selected by a lookup function `getAnimationForCode(code, isDay)`. Day/night is derived from the same local-time calculation used for the clock, not the visitor's own clock.

## Non-functional requirements

- **Responsiveness:** grid of cards reflows from multi-column (desktop) to single column (mobile); animations stay lightweight (CSS/SVG, no canvas-heavy effects) to keep mobile performance smooth.
- **Caching:** backend caches Open-Meteo responses per coordinate for a short TTL (5–10 min) to stay within Open-Meteo's rate limits and reduce latency.
- **Error handling:** city not found in search → inline "no results" state; weather fetch failure → card shows a retry affordance instead of blocking the whole dashboard.
- **Loading states:** skeleton/placeholder card while a newly added city's first weather fetch resolves.
- **Accessibility:** animations respect `prefers-reduced-motion`; text/temperature values remain readable without relying on the animation alone.
- **Session security:** better-auth issues httpOnly, secure session cookies; the .NET API only trusts a validated session and never accepts a client-supplied user id.

## Open questions

- [x] Auth: resolved — real user accounts (email/password for v1) via better-auth, run as a separate Node.js/TypeScript microservice against the shared SQL Server database. Favorites are keyed to `user.id`, not a device id.
- [x] Database: SQL Server (confirmed)
- [x] Hosting: Azure (confirmed)
- [ ] Units: stored as a switchable user preference (°C/°F), defaulting to the user's locale
- [x] Preloaded city on first run: Kyiv (confirmed)
