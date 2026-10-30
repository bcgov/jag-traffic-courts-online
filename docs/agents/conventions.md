# Conventions for Agent Docs

## Structure

- `agents.md` is the index. Keep it short: one row per file or folder, each with a one-line description.
- Group related files into folders (`architecture/`, `database/`, `workflows/`, `tickets/`). Each folder gets a `README.md` that indexes its contents.
- Use one topic per file. Split a file once it grows past a few screens.
- Name files in lowercase kebab-case, for example `ords-handler-changes.md`.

## Content

- Write down what isn't obvious from the code: decisions, constraints, ownership, gotchas, and procedures that span repos.
- Don't copy what the code, `git log`, or an existing README already tells you. Link to it instead.
- Use relative links to repo files, for example `[Staff.Service](../../../src/backend/TrafficCourts/Staff.Service)`.
- Write dates as absolute dates (`2026-10-14`), never "last week".
- When a doc records a decision, include **why** it was made so a later agent can judge edge cases.

## Maintenance

- If you find a doc that contradicts the code, fix or delete it in the same change.
- Delete ticket notes once the ticket is merged. Before you do, move any lasting knowledge into the relevant topic file.
