# AGENTS.md

## Project purpose

You are working on **Traffic Courts Online (TCO)**, the BC Government system that lets people dispute violation tickets online and lets court staff and judicial justices (JJs) process those disputes.

The system has Angular portals (`src/frontend/`), .NET services (`src/backend/TrafficCourts/`), and a Java `oracle-data-api` (`src/backend/oracle-data-api/`). These read and write the OCCAM and TCO Oracle schemas through ORDS. Prioritize correctness of dispute data, safe handling of personal information, and consistency with existing service boundaries.

## Context routing

- **Before running, configuring, or debugging services locally:** READ `README.md`.
- **Before opening a pull request or changing workflow:** READ `CONTRIBUTING.md` and `.github/pull_request_template.md`.
- **When working out how a request flows from portal to Oracle:** READ `src/README.md`.
- **When changing a backend service or its configuration:** READ `src/backend/README.md`.
- **When changing a frontend portal:** READ `src/frontend/README.md`.
- **When querying or changing ORDS data access (filters, paging, includes):** READ `docs/ords-data-access.md`.
- **When working with Oracle tables or columns:** CONSULT `docs/oracle-data-model/` (`occam.dbml`, `tco.dbml`).
- **When you meet an unfamiliar domain term or acronym:** READ `docs/terminology.md`.
- **When making a structural, ownership, or schema decision:** CONSULT `docs/decisions.md`.
- **When working on a specific Jira ticket:** CHECK `docs/jira-tickets/README.md`.
- **When changing deployment or OpenShift configuration:** READ `gitops/README.md`.
- **When using developer tooling (API collections, ticket generator, SQL helpers):** READ `tools/README.md`.
- **When reviewing a change or pull request:** ADOPT `.agents/personas/pr-reviewer.md`.

## Build and test

- `develop` is the main branch, and `main` is unused. Branch from `develop`.
- Feature and fix PRs go into the target `release/#.##` branch (MAJOR.MINOR) (squash merge).
- Only hotfixes and non-release changes (docs, dev tooling, DevOps) go directly to `develop`.
- Never create git tags or GitHub releases. A GitHub release generates the version tag and triggers the image build and registration pipeline (GitHub Actions → OpenShift Tekton → Artifactory).
- See `CONTRIBUTING.md` for the full process.
- **Backend (.NET):** `dotnet build` and `dotnet test` on `src/backend/TrafficCourts/TrafficCourts.sln`. **oracle-data-api:** `./mvnw -B clean verify` with JDK 11. Details: `src/backend/README.md`.
- **Frontend:** `yarn lint` and `yarn coverage` in each portal; citizen-portal and staff-portal need different Node versions. Details: `src/frontend/README.md`.
- Run the checks for every component you change before reporting work as done.

## Safety

- Never commit secrets. Local secrets belong in `.env` (git-ignored); copy from `.env.template`.
- Tickets, disputes, and disputant details are personal information. Do not paste real records into commits, docs, or logs.
- Table DDL and seed data are applied by another team. Do not author schema migrations here (see `docs/decisions.md`).

## Validation

- After changing this router or any routed file, use `.agents/skills/validate-context-routes/SKILL.md`.
