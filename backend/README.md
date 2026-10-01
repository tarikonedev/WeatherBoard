# WeatherBoard — Backend

.NET API for WeatherBoard, a multi-city weather dashboard. See the [root README](../README.md)
for the overall project and [`CLAUDE.md`](./CLAUDE.md) for structure/conventions details.

**Status:** scaffolded only — Clean Architecture solution implementing the full API surface, but
auth and the database are not wired up yet. See `CLAUDE.md` for the mocked-vs-real breakdown.

## Stack

- ASP.NET Core Web API (.NET 8)
- Clean Architecture (Domain / Application / Infrastructure / Api)
- Minimal APIs
- EF Core (not wired to a real database yet)
- xUnit (unit tests)

## Getting started

```bash
cd backend
dotnet build WeatherBoard.slnx                 # build everything
dotnet test WeatherBoard.slnx                  # run the xUnit suite
dotnet format WeatherBoard.slnx                # format/lint
dotnet run --project src/WeatherBoard.Api      # start the API (Swagger UI at /swagger in Development)
```

`launchSettings.json` assigns the dev port dynamically — check the console output on
`dotnet run`, or override with `ASPNETCORE_URLS`. CORS is pre-configured for a Vite dev server at
`http://localhost:5173` (see `../frontend`).

No database or auth microservice is required to run this today — Open-Meteo, favorites
persistence, and session validation are all mocked/in-memory.
