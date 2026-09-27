# CLAUDE.md (frontend)

Guidance for Claude Code when working inside `frontend/`. See the repo root `CLAUDE.md` for
overall project context and `../spec.md` for the full target design.

## What this is

The React SPA from the architecture, scaffolded with Vite. **Structure only — no page content
yet.** Every component, hook, and context is an empty placeholder that compiles and renders
`null`; the dashboard UI, search flow, and animations described in `spec.md` still need to be
implemented on top of this shell.

## Stack

- React 19 + Vite 8 + TypeScript
- Tailwind CSS v4 (via `@tailwindcss/vite`, imported in `src/index.css` with `@import "tailwindcss";`)
- TanStack React Query — server cache/polling for weather and favorites
- React Context — favorites/UI state (`FavoritesContext`), not Zustand
- `@dnd-kit/core` + `@dnd-kit/sortable` + `@dnd-kit/utilities` — for the favorites drag-and-drop
  reorder feature (not wired up yet)
- `oxlint` for linting (not ESLint)

## Folder structure

```
frontend/
  src/
    api/
      client.ts            # fetch wrapper: base URL from VITE_API_BASE_URL, credentials: 'include', JSON parsing
      types.ts              # City, Favorite, CurrentWeather — mirror backend/src/WeatherBoard.Domain/Entities
                             #   wire shape is camelCase (latitude/longitude), NOT spec.md's lat/lon example
      cities.ts              # searchCities(query) -> GET /api/cities/search
      weather.ts              # getCurrentWeather(lat, lon) -> GET /api/weather/current
      favorites.ts             # getFavorites/addFavorite/removeFavorite/reorderFavorites -> /api/favorites*
    components/
      Dashboard/Dashboard.tsx        # placeholder, renders null
      FavoriteCard/FavoriteCard.tsx   # placeholder, takes a Favorite prop
      CitySearch/CitySearch.tsx        # placeholder
      animations/                       # one component per WMO weather-code group
        ClearSky.tsx, Cloudy.tsx, Fog.tsx, Rainy.tsx, Snow.tsx, Thunderstorm.tsx
        getAnimationForCode.ts           # (code, isDay) -> component, via a code->component lookup map
    context/
      FavoritesContext.tsx    # Provider + useFavorites() hook; value shape is a stub ({ favorites: [] })
    hooks/
      useLocalTime.ts          # stub signature only — real impl should compute from Intl.DateTimeFormat
                                # per city timezone, ticking every second, never the device's own timezone
    App.tsx                     # wires QueryClientProvider + FavoritesProvider around <Dashboard />
    main.tsx                     # default Vite entry
  .env.development               # VITE_API_BASE_URL=http://localhost:5000 (placeholder — backend's dev
                                  # port is dynamic, see backend/CLAUDE.md; update this if it differs)
```

## Build / run commands

```bash
cd frontend
npm install
npm run dev        # Vite dev server on http://localhost:5173 (matches backend/'s CORS config)
npm run build       # tsc -b && vite build
npm run lint          # oxlint
npm run preview        # preview the production build
```

No test runner is configured yet.

## What's stubbed vs. real

| Concern | Current state | Real implementation (future) |
| --- | --- | --- |
| API client functions (`cities.ts`, `weather.ts`, `favorites.ts`) | Correct method/URL/body shape, but nothing calls them yet | Wire into React Query hooks (`useQuery`/`useMutation`) from real components |
| `FavoritesContext` | Returns a hardcoded empty `favorites: []`, no add/remove/reorder actions | Back with React Query cache + mutations that invalidate on success |
| `useLocalTime` | Returns `''`, no timer | `setInterval` + `Intl.DateTimeFormat('en-GB', { timeZone, hour: '2-digit', minute: '2-digit' })` |
| Animation components | Each renders `null`; `getAnimationForCode` maps only one WMO code per type as a placeholder | Full WMO code ranges per `spec.md`, CSS/SVG-only animations, respect `prefers-reduced-motion` |
| Auth | Not wired — `apiFetch` sends `credentials: 'include'` but no auth microservice exists yet | Once better-auth is running, ensure the session cookie flows through unchanged |

## Conventions

- One folder per component under `src/components/`, named `PascalCase/PascalCase.tsx`.
- API client functions are grouped by backend resource (`cities.ts`/`weather.ts`/`favorites.ts`),
  matching the endpoint groupings in `backend/src/WeatherBoard.Api/Endpoints/`.
- TypeScript types in `src/api/types.ts` must track the backend's actual DTO field names/casing
  (check `backend/src/WeatherBoard.Domain/Entities/*.cs`), not spec.md's example JSON if the two
  ever diverge.
- The frontend never calls Open-Meteo directly — always through the .NET API.
