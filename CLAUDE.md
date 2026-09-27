# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project status

- **`backend/`** — scaffolded: a Clean Architecture .NET 8 solution (Domain / Application /
  Infrastructure / Api + a unit test project) implementing the full API surface below. Auth and
  the database are **not** wired up yet — Open-Meteo, favorites persistence, and session
  validation are all mocked/in-memory. See `backend/CLAUDE.md` for the project structure, build/
  test/run commands, and exactly what's mocked vs. real.
- **Frontend and auth microservice** — not started yet. There is no `package.json`, git history,
  or SQL Server instance. Treat `spec.md` as the source of truth for their design until
  implementation begins.

## What WeatherBoard is

A multi-city weather dashboard: users search for cities, save favorites, and see each favorite
as a card with current weather, live local time, and an animated weather visual matched to
conditions.

## Architecture

Four components, each a separate deployable:

- **React SPA** (React 18 + Vite) — frontend. Never calls Open-Meteo directly; always goes
  through the .NET API.
- **.NET API** (ASP.NET Core Web API, .NET 8, EF Core) — the main backend. Proxies/caches
  Open-Meteo (Forecast + Geocoding APIs) and owns the `favorites` table. Validates the caller's
  session on every `/api/favorites*` request before touching data, and never trusts a
  client-supplied user id.
- **Auth microservice** (Node.js/TypeScript, [better-auth](https://www.better-auth.com/)) — a
  separate service because better-auth is JS/TS-only and has no .NET equivalent. Owns
  sign-up/sign-in/session issuance and the `user`/`session`/`account`/`verification` tables.
  Schema for these tables is managed with better-auth's own CLI (`auth generate`/`auth migrate`),
  kept independent from EF Core migrations.
- **SQL Server** — shared by both backends: EF Core owns `favorites` (with a `userId` FK into
  better-auth's `user` table), Kysely (better-auth's adapter) owns the four auth tables.

Cross-service auth flow: the .NET API does **not** read the `session` table directly. It
forwards the incoming cookie/token to the auth microservice's `getSession` endpoint and caches
validated `token → userId` results in memory for a short TTL (~30–60s), so the auth service
stays the single source of truth for session validity.

## Key design decisions (see spec.md for full detail)

- **Local time per card** is never fetched repeatedly — it's computed client-side every second
  from the city's IANA timezone (returned by Open-Meteo, passed through by the .NET API) via
  `Intl.DateTimeFormat`, never the visitor's own device timezone. Day/night for animations is
  derived from this same per-city local time.
- **Weather animations** are CSS/SVG only (no video/GIF/canvas), selected by a
  `getAnimationForCode(code, isDay)` lookup mapping Open-Meteo's WMO weather codes to one
  component per animation type (`<ClearSky />`, `<Rainy />`, etc.).
- **Caching**: the .NET API caches Open-Meteo responses per coordinate for 5–10 minutes to
  respect rate limits; the frontend refetches per-favorite weather on an interval (~10 min) and
  on window focus via React Query.
- **Favorites are per-account, not per-device**: keyed to `user.id`, persisted server-side (not
  localStorage), so they follow the user across devices.
- Preloaded city on first run: Kyiv.
- Out of scope for v1: multi-day/hourly forecast view, social login, push/email alerts.

## API surface (`.NET API`)

| Endpoint | Method | Purpose | Current implementation |
| --- | --- | --- | --- |
| `/api/cities/search?q={text}` | GET | Proxy Open-Meteo Geocoding | Mocked: static in-memory city catalog |
| `/api/weather/current?lat=&lon=` | GET | Proxy Open-Meteo Forecast (current + today's high/low/humidity/wind + timezone) | Mocked: static per-city weather catalog |
| `/api/favorites` | GET/POST | List / add favorites for the signed-in user | In-memory list, seeded with Kyiv; no user scoping yet (no auth) |
| `/api/favorites/{id}` | DELETE | Remove a favorite | In-memory |
| `/api/favorites/reorder` | PUT | Update display order after drag-and-drop | In-memory |

Auth endpoints (sign-up/sign-in/sign-out/session refresh) live on the separate better-auth
microservice, not the .NET API. **Not implemented yet** — no auth microservice exists, so
`/api/favorites*` currently has no session validation and is not scoped per user. See
`backend/CLAUDE.md` for exactly which classes back each endpoint today.
