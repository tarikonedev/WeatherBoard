# WeatherBoard

A multi-city weather dashboard: search for cities, save favorites, and see each favorite as a
card with current weather, live local time, and an animated weather visual matched to conditions.

Full design detail lives in [`SPEC.md`](./SPEC.md). AI-agent-specific guidance (build commands,
conventions, what's mocked vs. real) lives in [`CLAUDE.md`](./CLAUDE.md) and the per-component
`CLAUDE.md` files linked below.

## Status

Early scaffold — not yet functional end-to-end.

- **`frontend/`** — Vite + React 19 + TypeScript SPA, structure only, no page content yet.
  See [`frontend/README.md`](./frontend/README.md) / [`frontend/CLAUDE.md`](./frontend/CLAUDE.md).
- **`backend/`** — .NET 8 Clean Architecture API, full API surface implemented but Open-Meteo,
  favorites persistence, and auth are mocked/in-memory. See
  [`backend/CLAUDE.md`](./backend/CLAUDE.md).
- **Auth microservice** (Node.js/TypeScript, better-auth) — not started.
- **SQL Server** — not provisioned.

## Architecture

Four deployables: a React SPA, a .NET API (proxies/caches Open-Meteo, owns favorites), a Node.js
auth microservice (better-auth), and a shared SQL Server database. See `SPEC.md` for the full
breakdown and rationale.

## Running locally

```bash
# Backend
cd backend
dotnet run --project src/WeatherBoard.Api   # Swagger UI at /swagger

# Frontend (separate terminal)
cd frontend
npm install
npm run dev                                  # http://localhost:5173
```

The auth microservice and SQL Server are not yet part of local setup — see `SPEC.md` for the
target design once they're added.
