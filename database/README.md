# Database reference

This folder holds reference copies of the five Oracle schemas that Traffic Courts Online depends on. It is for reading, not for authoring changes.

- **The `.ddl` files are reference snapshots, not the source of truth.** Table DDL and seed data are applied by another team ([ADR 003](../docs/decisions.md#003-oracle-schema-ddl-is-managed-outside-this-repository)). Do not edit them or write migrations here. If a change needs new tables or columns, request it from the schema owners.
- **The `.dbml` files under [docs/oracle-data-model](../docs/oracle-data-model/) are diagram snapshots of the same tables.** Use them for an overview and the `.ddl` files for exact column definitions. Either may lag the live schema.

## Schemas

| Folder | What it holds | Under this project's control |
| ------ | ------------- | ---------------------------- |
| `occam` | Disputes submitted by citizens. | Packages only |
| `occamords` | The ORDS layer over `occam`. It is the only way the portals' services can reach OCCAM data. | Packages and ORDS modules |
| `tco` | Dispute adjudication. When an appearance is scheduled in JUSTIN, its details are copied into `tco`, using data from both `tco` and `justin`. | Packages only |
| `tcoords` | The ORDS layer over `tco`. It is the only way the portals' services can reach TCO data. | Packages and ORDS modules |
| `justin` | The JUSTIN application's schema, covering court appearances of all types. TCO reads code tables and appearance data from it. | No |

Each folder contains a `<schema>.ddl` file. See [business-process.md](../docs/business-process.md) for where each schema fits in a dispute's lifecycle.

## `definitions` folders

The PL/SQL packages, procedures, triggers, and ORDS modules live in separate SVN repositories, not in this one. To browse them alongside the DDL, run [create-definitions-symlinks.sh](create-definitions-symlinks.sh) from Git Bash. It creates `database/<schema>/definitions` symlinks to the sibling checkouts:

| Link | Target (sibling of this repo) |
| ---- | ----------------------------- |
| `occam/definitions` | `TCO-API-OCCAM/dbscripts/trunk` |
| `occamords/definitions` | `TCO-API-OCCAMORDS/dbscripts/trunk` |
| `tco/definitions` | `TCO-API-TCO/dbscripts/trunk` |
| `tcoords/definitions` | `TCO-API-TCOORDS/dbscripts/trunk` |

- `justin` has no `definitions` folder.
- On Windows, creating real symlinks needs Developer Mode or an elevated shell.
- The script skips any schema whose sibling checkout is missing, so a `definitions` folder may not exist on every machine. Don't assume it does.
- The links are local only and are git-ignored. Don't commit them.
