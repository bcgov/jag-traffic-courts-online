# Related Repositories

A database-backed feature usually needs changes in this repo **and** in one or more of the repos below. They are usually checked out next to this one (for example under `c:\projects\`).

| Repo | Contents |
|---|---|
| `TCO-API-OCCAM` | PL/SQL packages for the `OCCAM` schema (`occam_*`) |
| `TCO-API-OCCAMORDS` | ORDS modules and handlers that expose OCCAM data |
| `TCO-API-TCO` | PL/SQL packages for the `TCO` schema (`tco_*`) |
| `TCO-API-TCOORDS` | ORDS modules and handlers that expose TCO data |

Each repo uses the same layout: `code/`, `dbscripts/`, `docs/`.

See [schema-ownership.md](../database/schema-ownership.md) for what these repos own and what they don't.
