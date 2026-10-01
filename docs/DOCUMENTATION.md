# Documentation map

| File | Purpose | Audience |
| --- | --- | --- |
| [`README.md`](../README.md) | Project overview, status, local run commands | Humans landing on the repo |
| [`CONTRIBUTING.md`](../CONTRIBUTING.md) | Dev workflow: setup, pre-PR checks, branch/commit/PR conventions | Contributors |
| [`CLAUDE.md`](../CLAUDE.md) | AI-agent guidance: architecture, key design decisions, API surface | Claude Code / AI assistants |
| [`SPEC.md`](../SPEC.md) | Original technical spec: architecture rationale, API design, auth schema, data model, animation mapping, non-functional requirements. Historical design intent — `CLAUDE.md` files and code win on divergence | Humans + AI |
| [`backend/CLAUDE.md`](../backend/CLAUDE.md) | Backend solution structure, build/test/run commands, mocked-vs-real matrix, conventions | Claude Code / contributors working in `backend/` |
| [`frontend/CLAUDE.md`](../frontend/CLAUDE.md) | Frontend stack, folder structure, build/lint commands, stubbed-vs-real matrix, conventions | Claude Code / contributors working in `frontend/` |
| [`frontend/README.md`](../frontend/README.md) | Frontend quick-start: stack, status, run commands | Humans working in `frontend/` |

**Not covered here:** `.agents/skills/`, `.claude/agents/`, `.claude/skills/` — vendored
third-party skill/agent definitions for the Claude Code tooling, not WeatherBoard documentation.
