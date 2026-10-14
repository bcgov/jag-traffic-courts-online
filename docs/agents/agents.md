# Agent Documentation Index

This folder is for AI coding agents working on Traffic Courts Online (TCO). Human-facing docs live elsewhere in `docs/` and in each project's `README.md`.

**Start here:** read this index, then open only the files that relate to your task.

## Index

| File | What it covers |
|---|---|
| [conventions.md](conventions.md) | How to write and maintain the files in this folder |
| [architecture/overview.md](architecture/overview.md) | What TCO is, the main components, and how they connect |
| [architecture/repository-map.md](architecture/repository-map.md) | Where things live in this repo |
| [architecture/related-repositories.md](architecture/related-repositories.md) | The four Oracle SVN repos (one per schema, plus one per schema's ORDS), and which files belong in each |
| [database/schema-ownership.md](database/schema-ownership.md) | What we own in the TCO/OCCAM schemas, and what another team owns |
| [workflows/](workflows/README.md) | Step-by-step procedures: building, testing, common change types |
| [tickets/](tickets/README.md) | Notes for Jira tickets that are in progress |

## Ground rules

- Treat these docs as a starting point. Check them against the code before acting on them. If a doc is wrong, fix it in the same change.
- Never commit secrets, credentials, connection strings, or real citizen data to this folder.
- When you learn something that a future agent would need and couldn't easily get from the code, add it here and link it from this index.
