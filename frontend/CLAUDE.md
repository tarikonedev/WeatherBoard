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

| Concern                                                          | Current state                                                                               | Real implementation (future)                                                                     |
| ---------------------------------------------------------------- | ------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------ |
| API client functions (`cities.ts`, `weather.ts`, `favorites.ts`) | Correct method/URL/body shape, but nothing calls them yet                                   | Wire into React Query hooks (`useQuery`/`useMutation`) from real components                      |
| `FavoritesContext`                                               | Returns a hardcoded empty `favorites: []`, no add/remove/reorder actions                    | Back with React Query cache + mutations that invalidate on success                               |
| `useLocalTime`                                                   | Returns `''`, no timer                                                                      | `setInterval` + `Intl.DateTimeFormat('en-GB', { timeZone, hour: '2-digit', minute: '2-digit' })` |
| Animation components                                             | Each renders `null`; `getAnimationForCode` maps only one WMO code per type as a placeholder | Full WMO code ranges per `spec.md`, CSS/SVG-only animations, respect `prefers-reduced-motion`    |
| Auth                                                             | Not wired — `apiFetch` sends `credentials: 'include'` but no auth microservice exists yet   | Once better-auth is running, ensure the session cookie flows through unchanged                   |

## Design tokens

Tailwind v4 is config-free — theme customization lives in the `@theme` block in `src/index.css`,
ported from the WeatherBoard Figma file
(https://www.figma.com/design/swNIFORChyAYgKiRC7YVlL/WeatherBoard). Token names mirror the Figma
variable/style names (`/` → `-`) so they stay traceable back to source.

**Spacing and `rounded-full` are NOT overridden** — Tailwind's default numeric spacing scale
(`p-0.5`=2px, `p-1`=4px, `p-2`=8px, `p-3`=12px, `p-4`=16px, `p-5`=20px, `p-6`=24px, `p-8`=32px,
`p-10`=40px, `p-12`=48px) already matches the Figma spacing scale exactly — use those utilities
directly instead of inventing new spacing tokens.

| Figma token | Tailwind utility | Value |
| --- | --- | --- |
| `background/page` | `bg-bg-page` | `#F3F6FB` |
| `background/surface` | `bg-bg-surface` | `#FFFFFF` |
| `background/surface-muted` | `bg-bg-surface-muted` | `#EEF2F8` |
| `text/primary` | `text-text-primary` | `#11182B` |
| `text/secondary` | `text-text-secondary` | `#5B6479` |
| `text/inverse` | `text-text-inverse` | `#FFFFFF` |
| `border/default` | `border-border` | `#DDE3EF` |
| `accent/primary` | `bg-accent-primary` / `text-accent-primary` | `#2F6FED` |
| `accent/primary-hover` | `bg-accent-primary-hover` | `#1E58D1` |
| `accent/danger` | `text-danger` | `#E5484D` |
| `weather/clear`, `-deep` | `bg-weather-clear[-deep]` | `#4FA6F7` / `#1E63C9` |
| `weather/cloud`, `-deep` | `bg-weather-cloud[-deep]` | `#8C9BB5` / `#5A6A85` |
| `weather/rain`, `-deep` | `bg-weather-rain[-deep]` | `#4A6A8A` / `#2E4A68` |
| `weather/snow`, `-deep` | `bg-weather-snow[-deep]` | `#AFC4DE` / `#7792B8` |
| `weather/night`, `-deep` | `bg-weather-night[-deep]` | `#1B2748` / `#0D1430` |
| `radius/sm` | `rounded-sm` | `8px` (overrides Tailwind default) |
| `radius/md` | `rounded-md` | `16px` (overrides Tailwind default) |
| `radius/lg` | `rounded-lg` | `24px` (overrides Tailwind default) |
| `radius/full` | `rounded-full` | `9999px` (Tailwind default, unchanged) |

Type ramp — each Figma text style is one Tailwind utility bundling font-size, line-height, letter-
spacing, and weight:

| Figma text style | Tailwind utility |
| --- | --- |
| `Display/Temperature` | `text-display-temp` |
| `Heading/City` | `text-heading-city` |
| `Heading/Section` | `text-heading-section` |
| `Body/Large` | `text-body-lg` |
| `Body/Default` | `text-body` |
| `Body/Strong` | `text-body-strong` |
| `Caption/Default` | `text-caption` |
| `Caption/Strong` | `text-caption-strong` |
| `Label/Default` | `text-label` |

Font: Inter, loaded via Google Fonts `<link>` tags in `index.html` (weights 400/500/600/700),
applied globally through `--font-sans` and `body { font-family: var(--font-sans) }`.

## Conventions

- One folder per component under `src/components/`, named `PascalCase/PascalCase.tsx`.
- API client functions are grouped by backend resource (`cities.ts`/`weather.ts`/`favorites.ts`),
  matching the endpoint groupings in `backend/src/WeatherBoard.Api/Endpoints/`.
- TypeScript types in `src/api/types.ts` must track the backend's actual DTO field names/casing
  (check `backend/src/WeatherBoard.Domain/Entities/*.cs`), not spec.md's example JSON if the two
  ever diverge.
- The frontend never calls Open-Meteo directly — always through the .NET API.
