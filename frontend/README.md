# WeatherBoard — Frontend

React SPA for WeatherBoard, a multi-city weather dashboard. See the [root README](../README.md)
for the overall project and [`CLAUDE.md`](./CLAUDE.md) for structure/conventions details.

**Status:** scaffolded only — structure exists, no page content yet. See `CLAUDE.md` for the
stubbed-vs-real breakdown.

## Stack

- React 19 + Vite + TypeScript
- Tailwind CSS v4
- TanStack React Query (server cache/polling)
- React Context (favorites/UI state)
- `@dnd-kit` (drag-and-drop reorder)
- `oxlint` (linting)

## Getting started

```bash
npm install
npm run dev        # dev server on http://localhost:5173
npm run build       # tsc -b && vite build
npm run lint          # oxlint
npm run preview        # preview the production build
```

Requires the backend API running (see `../backend/CLAUDE.md`) and `VITE_API_BASE_URL` in
`.env.development` pointed at it.

No test runner is configured yet.
