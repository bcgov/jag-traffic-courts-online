# Related Repositories

A database-backed feature usually needs changes in this repo **and** in one or more of the Oracle repos below. They are usually checked out next to this one (for example under `c:\projects\`).

## The Oracle repos: one per schema, plus one per schema's ORDS

The Oracle code lives in **four separate SVN repositories, not git**. Each schema has a main repo for its PL/SQL and a second repo for its ORDS layer:

| Local folder | SVN repo | Contents | Tag subfolder |
|---|---|---|---|
| `TCO-API-OCCAM` | `https://jag.gov.bc.ca/svn/OCCAM` | PL/SQL packages, triggers and scripts for the `OCCAM` schema (`occam_*`) | `OCCAM/` |
| `TCO-API-OCCAMORDS` | `https://jag.gov.bc.ca/svn/OCCAMORDS` | ORDS modules and handlers for OCCAM (`occam_ords*`, `occamords_*`) | `OCCAMORDS/` |
| `TCO-API-TCO` | `https://jag.gov.bc.ca/svn/TCO` | PL/SQL packages, triggers and scripts for the `TCO` schema (`tco_*`) | `TCO/` |
| `TCO-API-TCOORDS` | `https://jag.gov.bc.ca/svn/TCOORDS` | ORDS modules and handlers for TCO (`tco_ords*`, `tcoords_*`) | `TCOORDS/` |

Each repo has the layout `code/`, `dbscripts/` (with `trunk/`, `tags/`, `branches/`) and `docs/`. Use `svn status`, `svn diff` and so on. To make a change, follow [oracle-package-changes.md](../workflows/oracle-package-changes.md).

## Keep each schema's files in its own repo

**A changed file goes in the trunk and release tag of the repo that owns it, and nowhere else.** If a release touches several schemas, it gets a tag folder in each repo involved, and each holds only that repo's files. Each repo numbers its tags independently, so the folders can have different numbers. For example, a release that changes `occam_rest_body.sql`, `tco_dispute_body.sql` and an OCCAM ORDS handler might produce:

- `TCO-API-OCCAM/dbscripts/tags/26.x/26.6/OCCAM/` containing the `occam_*` files
- `TCO-API-TCO/dbscripts/tags/26.x/26.4/TCO/` containing the `tco_*` files
- `TCO-API-OCCAMORDS/dbscripts/tags/26.x/26.0/OCCAMORDS/` containing the ORDS files

Never put an `OCCAM/`, `OCCAMORDS/` or `TCOORDS/` folder inside the TCO repo, or a `TCO/` or `*ORDS/` folder inside the OCCAM repo.

**Why:** each repo is the source of truth for its schema. Copies in another repo get out of date without anyone noticing, and nobody can tell which copy was actually deployed.

### Don't copy the older layout

Many older tags break this rule. Treat them as history, not as a pattern to follow:

- **`TCO-API-TCO` tags 22.x–25.x** contain `OCCAM/`, `OCCAMORDS/`, `TCOORDS/` and `JUSTIN/` folders alongside `TCO/` (for example `tags/25.x/25.5/`). The `occam_*` files in them duplicate files owned by `TCO-API-OCCAM`.
- **`TCO-API-OCCAM` tags 22.0–24.3** contain `OCCAMORDS/` folders.
- **The ORDS repos have no tags of their own yet.** Their release scripts were tagged inside the schema repos, as above. New ORDS changes get their first tags in their own repo, using the same `dbscripts/tags/26.x/26.N/<REPO>/` layout.
- **`TCO-API-OCCAMORDS/TCOORDS/`** is an unversioned local copy of `TCO-API-TCOORDS` that sits inside the OCCAMORDS working copy. Ignore it, and make TCOORDS changes in `TCO-API-TCOORDS`.

- **Even recent tags can break it.** `TCO-API-TCO` tag `26.x/26.5/` (2026-09-10) holds `JUSTIN/justin_occam_interface_body.sql`, but that package's trunk is in `TCO-API-OCCAM`. Before relying on a tag's copy of a file, compare it with the owning repo's trunk.

Tag numbers are per repo, so the same number in two repos usually means two unrelated releases. For example, OCCAM `26.5` and TCO `26.5` contain different changes.

### Cross-schema code

Some packages call into another schema (for example `tco_appearance_data_modify` reads `occam.occam_disputes`, and `occam_rest` joins `tco.tco_disputes`). A file still belongs to the repo of **the schema it is compiled into**, whatever it references.

The exception is the `JUSTIN` schema, which has no repo of its own. Its interface packages live in the repo of the schema they serve:
- `JUSTIN.justin_occam_interface` is in `TCO-API-OCCAM` trunk.
- `JUSTIN.justin_tco_interface` is in `TCO-API-TCO` trunk.

Older TCO tags put them in a `JUSTIN/` subfolder. Before tagging a change to one of these packages, ask the user which tag folder it belongs in.

See [schema-ownership.md](../database/schema-ownership.md) for what these repos own and what another team owns.
