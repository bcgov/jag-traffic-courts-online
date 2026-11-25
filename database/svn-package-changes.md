# Changing Oracle packages and ORDS (SVN)

How to change PL/SQL packages and ORDS scripts in the four Oracle SVN repositories, and how to tag them for a release. Table DDL and seed data are not ours to change ([ADR 003](../docs/decisions.md#003-oracle-schema-ddl-is-managed-outside-this-repository)).

A database-backed feature usually needs changes in this repo **and** in one or more of the SVN repos below. They are usually checked out next to this one (for example under `c:\projects\`), and [create-definitions-symlinks.sh](create-definitions-symlinks.sh) can link their trunks into `database/<schema>/definitions`.

## The repositories: one per schema, plus one per schema's ORDS

The Oracle code lives in **four separate SVN repositories, not git**. Each schema has a main repo for its PL/SQL and a second repo for its ORDS layer:

| Local folder | SVN repo | Contents | Tag subfolder |
|---|---|---|---|
| `TCO-API-OCCAM` | `https://jag.gov.bc.ca/svn/OCCAM` | PL/SQL packages, triggers and scripts for the `OCCAM` schema (`occam_*`) | `OCCAM/` |
| `TCO-API-OCCAMORDS` | `https://jag.gov.bc.ca/svn/OCCAMORDS` | ORDS modules and handlers for OCCAM (`occam_ords*`, `occamords_*`) | `OCCAMORDS/` |
| `TCO-API-TCO` | `https://jag.gov.bc.ca/svn/TCO` | PL/SQL packages, triggers and scripts for the `TCO` schema (`tco_*`) | `TCO/` |
| `TCO-API-TCOORDS` | `https://jag.gov.bc.ca/svn/TCOORDS` | ORDS modules and handlers for TCO (`tco_ords*`, `tcoords_*`) | `TCOORDS/` |

Each repo has the layout `code/`, `dbscripts/` (with `trunk/`, `tags/`, `branches/`) and `docs/`. Use `svn status`, `svn diff` and so on.

### Keep each schema's files in its own repo

**A changed file goes in the trunk and release tag of the repo that owns it, and nowhere else.** If a release touches several schemas, it gets a tag folder in each repo involved, and each holds only that repo's files. For example, a release that changes `occam_rest_body.sql`, `tco_dispute_body.sql` and an OCCAM ORDS handler might produce:

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

### Cross-schema code

Some packages call into another schema (for example `tco_appearance_data_modify` reads `occam.occam_disputes`, and `occam_rest` joins `tco.tco_disputes`). A file still belongs to the repo of **the schema it is compiled into**, whatever it references.

The exception is the `JUSTIN` schema, which has no repo of its own. Its interface packages live in the repo of the schema they serve:

- `JUSTIN.justin_occam_interface` is in `TCO-API-OCCAM` trunk.
- `JUSTIN.justin_tco_interface` is in `TCO-API-TCO` trunk.

Older TCO tags put them in a `JUSTIN/` subfolder. Before tagging a change to one of these packages, confirm which tag folder it belongs in.

## Trunk and release tags

Repeat these steps in **each** repo the release touches:

1. Make the change in that repo's `dbscripts/trunk/...`. Packages are in `dbscripts/trunk/Packages/`.
2. Copy **only that repo's changed files** into its release tag folder, `dbscripts/tags/26.x/26.N/<REPO>/` (see the table above).
3. Add a `driver.sql` and a build file to the tag folder (see below). Each driver lists only the files in its own folder.
4. Run `svn add` on the new tag folder, then commit when the change is ready.

When the release is deployed, each repo's driver runs in order:

1. Schema packages: OCCAM, then TCO.
2. ORDS scripts.

TCO packages that read OCCAM objects must compile after the OCCAM changes they depend on.

Record each tag and its SVN revision on the Jira ticket it belongs to.

### Choosing the tag number

- The prefix is the calendar year (`26` for 2026). The suffix is an incrementing integer.
- **Each repo numbers its own tags independently.** Take the highest `26.N` that exists **in that repo** and add 1. The numbers don't need to match across repos, so one change can be OCCAM `26.6` and TCO `26.4`. Record which tag the change got in each repo.
  - **Why:** decided on 2026-10-14. Each repo has its own release history.
  - The same number in two repos usually means two unrelated releases. For example, OCCAM `26.5` and TCO `26.5` contain different changes.
- **Check the server, not your working copy.** Run `svn ls <repo-url>/dbscripts/tags/26.x`, or `svn update` first. A stale working copy can hide tags that other people have already committed.
- Some older TCO tags (26.1–26.3) put files directly in the version folder. New tags use the `<REPO>/` subfolder.

### driver.sql

`driver.sql` lists one file name per line, in the order the files must run. Specs come before bodies, and a body must come after any package it depends on. It uses CRLF line endings with no trailing newline. Examples are in `TCO-API-OCCAM/dbscripts/tags/26.x/26.5/OCCAM/driver.sql` and `TCO-API-TCO/dbscripts/tags/26.x/26.3/driver.sql`.

### Build file

- **OCCAM:** `build_occam_26.N.xml`. Copy the previous tag's file, change the version in both places, and list `driver.sql` plus every SQL file in `<include>` elements.
- **TCO:** `build.xml`. Copy the newest TCO tag's `build.xml` (for example `TCO-API-TCO/dbscripts/tags/26.x/26.6/TCO/build.xml`), change the zip file name to the new version, and list `driver.sql` plus every SQL file in `<include>` elements. Some older TCO tags (for example `26.0/TCO/`) zip `*.sql` with no file list instead.
- **Tag copies:** in the TCO repo, create the tag copies with `svn copy trunk/Packages/<file> tags/26.x/26.N/TCO/<file>`. This keeps the file's history and SVN properties, as in the 26.4 tag. The OCCAM tags are plain copies added with `svn add`, which have no properties. Either works.
- **OCCAMORDS and TCOORDS:** there's no example yet, because these repos have never been tagged. Use the TCO-style `build.xml`, with the project name and zip file name changed to the ORDS repo.

## Editing the files

- **Read-only files.** Package files have `svn:needs-lock`, so they are read-only in the working copy. Either take a lock (`svn lock`, which other people on the server can see) or clear the read-only flag locally, for example in PowerShell with `Set-ItemProperty <file> -Name IsReadOnly -Value $false`.
- **Line endings.** Files use CRLF. `sed -i` from Git Bash strips the carriage returns, so SVN sees the whole file as changed. Use an editor that keeps CRLF, or read and write the file in PowerShell with `[IO.File]::ReadAllText` and `WriteAllText`.
- **Check the diff.** After editing, run `svn diff`. It should show only the lines you meant to change. If unrelated trailing whitespace was trimmed, put it back.
- **Change history.** Package bodies that have a history block at the top get a new line in the same format:
  `  --  14-OCT-2026   D Kay           TCVP-3480 pDisputeSave - <what changed>`
- **Adding a procedure parameter.** Callers pass arguments by position, so add new parameters **last**, with `DEFAULT NULL`. Existing callers then keep compiling, and the package can be deployed before the application code.
- **Audit columns.** If `ENT_USER_ID` is NULL on insert, a trigger fills it with the session `USER` (`occam_audit.set_fields`). Logic that checks the inserting user must use `NVL(pEntUserId, USER)`.

## ORDS handlers

POST and PUT handlers pass the request body to PL/SQL as `:body_text`, and the PL/SQL parses it with `json_table`. To accept a new field, add it to the `json_table` columns in the PL/SQL. The ORDS module itself usually needs no change. A field missing from the JSON reads as NULL, so older callers keep working.
