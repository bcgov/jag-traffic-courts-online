# Architecture decision records

Record decisions that change structure, ownership, or conventions. Add new entries at the end, number them sequentially, and mark replaced entries `Superseded by NNN` instead of deleting them.

## 001. Use `AGENTS.md` as the single agent entry point

Status: Accepted

Recorded: 2026-10-20

The root `AGENTS.md` is the one entry point for AI coding agents, whatever the vendor. It routes agents to deeper context only when a task needs it. Do not add vendor-specific files such as `CLAUDE.md` or `.cursorrules` that duplicate it. If a tool requires one, keep it as a one-line pointer to `AGENTS.md`.

Adapted from the [dotagents](https://github.com/bgreenwell/dotagents) convention.

## 002. Keep shared truth visible; reserve `.agents/` for agent-only resources

Status: Accepted

Recorded: 2026-10-20

Documentation that helps both humans and agents stays in its normal location: `README.md`, `CONTRIBUTING.md`, `docs/`, and the component READMEs under `src/`. Only resources written specifically for agents go in `.agents/`, such as review personas and skills. Generated agent state (`.agents/logs/`, `.agents/memory/`) is git-ignored.

## 003. Oracle schema DDL is managed outside this repository

Status: Accepted

Recorded: 2026-10-20

Table DDL and seed scripts for the OCCAM and TCO schemas are applied by another team. Application repositories own only the PL/SQL packages and ORDS modules they call. Changes here must work against the existing schema. Changes that need new tables or columns must be requested from the schema owners rather than scripted in this repo. The `.dbml` files under `docs/oracle-data-model/` are reference snapshots and not the source of truth.

## 004. `develop` is the main branch; releases flow through `release/#.##.##`

Status: Accepted

Recorded: 2026-10-20

`develop` is the main branch, and `main` is not used. Work branches are created from `develop`. Each release gets a `release/#.##.##` branch, and pending work branches are pull-requested into it with squash merges. The release branch is deployed to DEV, then TEST, then PROD. After PROD is validated, it is pull-requested back into `develop` with a merge commit. The full procedure is in `CONTRIBUTING.md`.
