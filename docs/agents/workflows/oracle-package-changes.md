# Changing Oracle Packages (SVN Trunk and Release Tags)

How to change PL/SQL packages and ORDS scripts in the four Oracle SVN repositories (OCCAM, OCCAMORDS, TCO, TCOORDS). Which repo owns which files is described in [related-repositories.md](../architecture/related-repositories.md). They are **SVN working copies, not git**.

## Trunk and release tags

Repeat these steps in **each** repo the release touches:

1. Make the change in that repo's `dbscripts/trunk/...`. Packages are in `dbscripts/trunk/Packages/`.
2. Copy **only that repo's changed files** into its release tag folder, `dbscripts/tags/26.x/26.N/<REPO>/`:

   | Repo | Tag folder |
   |---|---|
   | `TCO-API-OCCAM` | `dbscripts/tags/26.x/26.N/OCCAM/` |
   | `TCO-API-OCCAMORDS` | `dbscripts/tags/26.x/26.N/OCCAMORDS/` |
   | `TCO-API-TCO` | `dbscripts/tags/26.x/26.N/TCO/` |
   | `TCO-API-TCOORDS` | `dbscripts/tags/26.x/26.N/TCOORDS/` |

   **Never put one repo's files in another repo, whether in trunk or in a schema-named tag subfolder.** Older tags did this; [related-repositories.md](../architecture/related-repositories.md#dont-copy-the-older-layout) explains why not to copy them.
3. Add a `driver.sql` and a build file to the tag folder (see below). Each driver lists only the files in its own folder.
4. Run `svn add` on the new tag folder. Don't commit unless the user asks.

When the release is deployed, each repo's driver runs in order:
1. Schema packages: OCCAM, then TCO.
2. ORDS scripts.

TCO packages that read OCCAM objects must compile after the OCCAM changes they depend on.

### Choosing the tag number

- The prefix is the calendar year (`26` for 2026). The suffix is an incrementing integer.
- **Each repo numbers its own tags independently.** Take the highest `26.N` that exists **in that repo** and add 1. The numbers don't need to match across repos, so one change can be OCCAM `26.6` and TCO `26.4`. The ticket notes should record which tag the change got in each repo.
  - **Why:** the user decided this on 2026-10-14. Each repo has its own release history.
- **Check the server, not your working copy.** Run `svn ls <repo-url>/dbscripts/tags/26.x`, or `svn update` first. A stale working copy can hide tags that other people have already committed.
- Some older TCO tags (26.1–26.3) put files directly in the version folder. New tags use the `<REPO>/` subfolder.

### driver.sql

`driver.sql` lists one file name per line, in the order the files must run. Specs come before bodies, and a body must come after any package it depends on. It uses CRLF line endings with no trailing newline. Examples are in `TCO-API-OCCAM/dbscripts/tags/26.x/26.5/OCCAM/driver.sql` and `TCO-API-TCO/dbscripts/tags/26.x/26.3/driver.sql`.

### Build file

- **OCCAM:** `build_occam_26.N.xml`. Copy the previous tag's file, change the version in both places, and list `driver.sql` plus every SQL file in `<include>` elements.
- **TCO:** `build.xml`. Copy the newest TCO tag's `build.xml` (for example `TCO-API-TCO/dbscripts/tags/26.x/26.6/TCO/build.xml`), change the zip file name to the new version, and list `driver.sql` plus every SQL file in `<include>` elements. Some older TCO tags (for example `26.0/TCO/`) zip `*.sql` with no file list instead.
- **Tag copies:** in the TCO repo, create the tag copies with `svn copy trunk/Packages/<file> tags/26.x/26.N/TCO/<file>`. This keeps the file's history and SVN properties, as in the 26.4 tag. The OCCAM tags are plain copies added with `svn add`, which have no properties. Either works.
- **OCCAMORDS and TCOORDS:** there's no example yet, because these repos have never been tagged. Use the TCO-style `build.xml`, with the project name and zip file name changed to the ORDS repo, and confirm the choice with the user.

## Editing the files

- **Read-only files.** Package files have `svn:needs-lock`, so they are read-only in the working copy. You can take a lock (`svn lock`, which other people on the server can see) or clear the read-only flag locally, for example in PowerShell with `Set-ItemProperty <file> -Name IsReadOnly -Value $false`. Ask the user which they want. For TCVP-3480 they chose to clear the flag without locking.
- **Line endings.** Files use CRLF. `sed -i` from Git Bash strips the carriage returns, so SVN sees the whole file as changed. Use an editor that keeps CRLF, or read and write the file in PowerShell with `[IO.File]::ReadAllText` and `WriteAllText`.
- **Check the diff.** After editing, run `svn diff`. It should show only the lines you meant to change. If unrelated trailing whitespace was trimmed, put it back.
- **Change history.** Package bodies that have a history block at the top get a new line in the same format:
  `  --  14-OCT-2026   D Kay           TCVP-3480 pDisputeSave - <what changed>`
- **Adding a procedure parameter.** Callers pass arguments by position, so add new parameters **last**, with `DEFAULT NULL`. Existing callers then keep compiling, and the package can be deployed before the application code.
- **Audit columns.** If `ENT_USER_ID` is NULL on insert, a trigger fills it with the session `USER` (`occam_audit.set_fields`). Logic that checks the inserting user must use `NVL(pEntUserId, USER)`.

## ORDS handlers

POST and PUT handlers pass the request body to PL/SQL as `:body_text`, and the PL/SQL parses it with `json_table`. To accept a new field, add it to the `json_table` columns in the PL/SQL. The ORDS module itself usually needs no change. A field missing from the JSON reads as NULL, so older callers keep working.
