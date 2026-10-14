# Schema Ownership

- **Another team owns table DDL and seed data** for the `TCO` and `OCCAM` schemas: new tables, new columns, and code-table rows. They apply those scripts themselves. Those scripts are **not** kept in our repos.
- **We own** the PL/SQL packages (`occam_*`, `tco_*`) and the ORDS modules and handlers, in the [related repositories](../architecture/related-repositories.md).
- Any DDL or seed script you find in our repos (for example `TCO-API-TCO/dbscripts/trunk/Scripts/tco_dispute_source_ins.sql`) is a reference or planning copy only.

**Why:** It's an organizational boundary. Schema changes are made by the team that manages the database.

**How to apply:** Don't add `ALTER TABLE` or seed scripts to release drivers. When a feature needs a schema change, assume it already exists (or ask), and change only the packages, ORDS, and application code.

Data model diagrams and DBML: `docs/oracle-data-model/`.
