# CLAUDE.md (backend)

Guidance for Claude Code when working inside `backend/`. See the repo root `CLAUDE.md` for
overall project context and `../spec.md` for the full target design.

## What this is

The `.NET API` component from the architecture, scaffolded as Clean Architecture. **No auth and
no real database yet** — Open-Meteo, `favorites` persistence, and session validation are all
mocked/in-memory so the API surface is runnable today. This is a deliberate stepping stone: swap
the mocked pieces for real ones (Open-Meteo HTTP calls, EF Core + SQL Server, better-auth session
validation) without reshaping Domain/Application.

## Solution structure

```
backend/
  WeatherBoard.slnx
  src/
    WeatherBoard.Domain/          # Entities: City, Favorite, CurrentWeather. No dependencies.
    WeatherBoard.Application/     # Interfaces (IGeocodingService, IWeatherService,
                                  # IFavoritesRepository) + FavoritesService (the only use-case
                                  # class — assigns id/order on add, validates reorder id sets).
                                  # Depends on Domain only.
    WeatherBoard.Infrastructure/  # Mock implementations of the Application interfaces:
                                  #   Geocoding/MockGeocodingService + MockCityCatalog (10 cities)
                                  #   Weather/MockWeatherService + MockWeatherCatalog (static,
                                  #     deterministic per-city weather incl. varied WMO codes)
                                  #   Favorites/InMemoryFavoritesRepository (List<Favorite>
                                  #     behind a lock, seeded with Kyiv as favorite #0)
                                  # Depends on Application + Domain.
    WeatherBoard.Api/             # Composition root: Program.cs wires DI, CORS (localhost:5173),
                                  # Swagger UI. Minimal API endpoints in Endpoints/
                                  # (CitiesEndpoints, WeatherEndpoints, FavoritesEndpoints).
  tests/
    WeatherBoard.UnitTests/       # xUnit. Covers FavoritesService, MockGeocodingService,
                                  # MockWeatherService.
```

Dependency direction: `Domain <- Application <- Infrastructure`, with `Api` referencing
`Application` (endpoints call interfaces/`FavoritesService` directly) and `Infrastructure` (only
in `Program.cs`, for DI registration — endpoints never reference Infrastructure types).

No separate DTO layer: `City`, `Favorite`, `CurrentWeather` are plain records serialized directly
as JSON responses. Add real DTOs only if a response needs to diverge from its entity shape.

## Build / test / run

```bash
cd backend
dotnet build WeatherBoard.slnx      # build everything
dotnet test WeatherBoard.slnx       # run the xUnit suite (WeatherBoard.UnitTests)
dotnet format WeatherBoard.slnx     # format/lint (no custom .editorconfig yet — uses .NET defaults)
dotnet run --project src/WeatherBoard.Api   # start the API (Swagger UI at /swagger in Development)
```

The Api project's `launchSettings.json` controls the dev port (currently assigns one dynamically
under `http://localhost:5xxx` — check the console output on `dotnet run`, or override with
`ASPNETCORE_URLS`). CORS is pre-configured for a Vite dev server at `http://localhost:5173`.

## What's mocked vs. real

| Concern | Current state | Real implementation (future) |
| --- | --- | --- |
| City search | `MockGeocodingService` — substring match over `MockCityCatalog` (10 hardcoded cities) | Proxy Open-Meteo Geocoding API |
| Current weather | `MockWeatherService` — matches lat/lon to nearest catalog city (±0.5°), else a default preset | Proxy Open-Meteo Forecast API, cache 5–10 min per coordinate |
| Favorites storage | `InMemoryFavoritesRepository` — single shared in-process `List<Favorite>`, lost on restart, seeded with Kyiv | EF Core + SQL Server, `favorites` table with `userId` FK |
| Auth / session validation | None — `/api/favorites*` has no session check and isn't scoped per user | Forward cookie/token to better-auth's `getSession`, cache `token → userId` for ~30–60s |

When wiring up the real integrations, replace the `AddInfrastructure()` registrations in
`WeatherBoard.Infrastructure/DependencyInjection.cs` — the `Application` interfaces and
`FavoritesService` should not need to change.

## Conventions

- Minimal APIs (not controllers) — one static `Map*Endpoints` extension per resource in
  `WeatherBoard.Api/Endpoints/`.
- Endpoints depend on `Application` interfaces/services only, never on `Infrastructure` types.
- Tests instantiate real mock/in-memory implementations directly (e.g.
  `new FavoritesService(new InMemoryFavoritesRepository())`) rather than mocking frameworks —
  there's nothing worth mocking yet since the "real" dependencies are already fakes.
