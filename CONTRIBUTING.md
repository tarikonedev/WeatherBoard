# Contributing to WeatherBoard

This is an early-stage scaffold (see [`README.md`](./README.md) for current status), but these
conventions are meant to hold as the project grows into multiple services.

## Project layout

- `frontend/` — React SPA. See `frontend/CLAUDE.md` for structure/conventions.
- `backend/` — .NET API. See `backend/CLAUDE.md` for structure/conventions.
- `SPEC.md` — the original design doc. Treat it as historical intent, not a live source of
  truth — the `CLAUDE.md` files and the code win on divergence.

## Local setup

```bash
# Backend
cd backend
dotnet build WeatherBoard.slnx
dotnet run --project src/WeatherBoard.Api   # Swagger UI at /swagger

# Frontend (separate terminal)
cd frontend
npm install
npm run dev
```

The auth microservice and SQL Server aren't part of local setup yet — this section will be
updated once they exist (likely `docker-compose` for SQL Server, and a third terminal/process
for the auth service).

## Before opening a PR

Run the checks for whichever side you touched:

```bash
# backend/
dotnet build WeatherBoard.slnx
dotnet test WeatherBoard.slnx
dotnet format WeatherBoard.slnx --verify-no-changes

# frontend/
npm run lint
npm run build
```

There's no CI configured yet, so these are on you to run locally before pushing.

## Commits & branches

- Branch off `master`, name branches descriptively (`feature/favorites-reorder`,
  `fix/weather-cache-ttl`).
- Commit messages: short imperative summary line (`Add favorites drag-and-drop`), body only if
  the "why" isn't obvious from the diff.
- Keep commits scoped to one logical change — easier to review and to `git bisect` later.

## Pull requests

- Keep PRs focused; a bug fix doesn't need to carry refactors.
- Describe *why*, not just *what* — the diff already shows what changed.
- Update the relevant `CLAUDE.md` (and `README.md` status section, if applicable) in the same PR
  if your change moves a "mocked"/"stubbed" item closer to real, or changes build/run commands.
- Reference `SPEC.md` sections you're implementing against, and flag in the PR description if
  your implementation deviates from the spec.

## Code conventions

See `backend/CLAUDE.md` and `frontend/CLAUDE.md` for the authoritative, current conventions
(dependency direction, folder naming, state management choices, etc.) — they're kept close to
the code and are more reliable than duplicating them here.

## Adding dependencies

- Backend: justify new NuGet packages in the PR description — Clean Architecture layering means
  most packages belong in `Infrastructure`/`Api`, rarely `Domain`/`Application`.
- Frontend: check `frontend/CLAUDE.md`'s stack list first; prefer what's already installed
  (React Query, Context, dnd-kit) over introducing a parallel tool for the same job (e.g. don't
  add Zustand or Redux alongside Context without discussion).

## Questions / design decisions

Open questions and confirmed decisions are tracked in `SPEC.md`'s "Open questions" section.
Flag significant new design decisions there (or in a PR description) rather than only in commit
history, so future contributors can find the rationale.
