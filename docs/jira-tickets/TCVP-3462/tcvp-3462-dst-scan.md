# TCVP-3462 DST Technical Scan

## Scope

Scanned the active TCO OpenShift application source and deployment configuration in:

- `jag-traffic-courts-online/src`
- `jag-traffic-courts-online/gitops`
- `TCO-API-OCCAM/dbscripts/trunk`
- `TCO-API-OCCAMORDS/dbscripts/trunk`
- `TCO-API-TCO/dbscripts/trunk`
- `TCO-API-TCOORDS/dbscripts/trunk`

The scan covered C# and TypeScript date/time APIs, timezone libraries, Dockerfiles, Helm/OpenShift values, CronJobs, Oracle date functions, and active scheduled-job configuration. Historical tags and generated build/cache directories were excluded from remediation.

## Findings

### Application code

- No active `moment-timezone` dependency or direct usage was found. The citizen portal uses `luxon`; the staff portal has no direct timezone library dependency.
- `ClockExtensions` and staff date-range filtering use `TimeZoneInfo` with IANA zone IDs. UTC instants are converted using the timezone rule data rather than a hardcoded seasonal offset.
- Browser-provided IANA zones are accepted for staff date filtering, with `America/Vancouver` as the fallback. This remains compatible with a future BC rule change when the container tzdata/.NET timezone data is updated.
- No active `DateTime.ToLocalTime()` or `DateTime.Now` dependency was found in the reviewed backend source. UTC-to-local and local-to-UTC conversions are explicit.

### Containers and OpenShift

- OpenShift deployment templates set `TZ=America/Vancouver` for the .NET services.
- The Oracle Data API image also sets `ENV TZ=America/Vancouver` and its production runtime is Debian-based, not Alpine.
- The active environment values set `TZ=America/Vancouver` for the Oracle Data API. The timezone is a named zone, not a fixed `UTC-7`/`UTC-8` offset, so updated timezone data can represent a policy change.

### Scheduled jobs

- Code-table refresh runs at `0 0 * * * *` and dispute unassignment runs every five minutes. These are interval-like schedules and do not target a repeated or skipped local clock hour.
- OpenShift CronJobs use `@daily` and `@hourly`. Their exact execution instant is platform-controlled; no DST-sensitive fixed local hour was found.
- A deployment-time operational check is still required after any BC timezone-data update to confirm the intended CronJob timezone and missed-run policy.

### Integrations and Oracle database processes

- Audit and trigger timestamps use `SYSDATE`; several business queries use `SYSDATE` or `CURRENT_DATE` for rolling windows and address validity. These are session/host-local semantics and require the Oracle database timezone/session configuration to remain aligned with the BC business-time contract.
- Other TCO paths already use `SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)` for UTC persistence.
- No `AT TIME ZONE` conversion or DST-specific fixed offset was found in the active trunks.

## Remediation

- Added transition-boundary regression coverage for the shared UTC-to-Pacific conversion helper, covering the spring skipped hour and fall repeated hour.
- No production conversion was changed to a fixed offset. That would break historical timestamps and would prevent timezone data from representing future BC policy changes.
- No `moment-timezone` replacement was required because the dependency/usage is absent from active source.
- Oracle local-time expressions are documented as residual risks rather than mass-replaced: changing them to UTC would alter BC civil-date behavior for address validity and rolling business windows without a database/application contract decision.

## Validation and residual risks

Run the focused test project:

```powershell
dotnet test src/backend/TrafficCourts/TrafficCourts.Core.Test/TrafficCourts.Core.Test.csproj --filter FullyQualifiedName~ClockExtensionsTests
```

The transition tests verify processing immediately before and after both Pacific transitions. A production validation should also run before, at, and after the first BC policy transition using the deployed image's timezone database and the configured Oracle session timezone.

Residual risk remains in Oracle `SYSDATE`/`CURRENT_DATE` business predicates and in the execution timezone of `@daily`/`@hourly` CronJobs. These require an agreed BC civil-time versus UTC contract and an operations change window; they should not be silently changed as part of a code-only DST scan.

## Database table/date-column inventory

The authoritative schema-dump-based table and stored-routine usage inventory is maintained in [database-date-column-usage.md](database-date-column-usage.md). It supersedes the earlier source-only/inferred table section below and includes all 26 tables and 73 date/time columns from `2026-08-21_schema_dump.csv`.

This inventory is source-derived from the `dbscripts/trunk` packages, triggers, and SQL. The repositories do not contain the base-table DDL and no Oracle connection was configured in the workspace, so the physical data types and any tables never referenced by application code must be confirmed with `ALL_TAB_COLUMNS` before this is treated as a complete database catalog. Package names and stored procedures are excluded from the table list.

### OCCAM tables

- `OCCAM_AUDIT_LOG`
	- Date/time columns: `ENT_DTM`, `UPD_DTM` (inferred from the audit package contract; confirm in `ALL_TAB_COLUMNS`).
	- Procedure usage:
		- `OCCAM_AUDIT` package body: assigns `ENT_DTM := SYSDATE` and `UPD_DTM := SYSDATE`.

- `OCCAM_AUDIT_LOG_ENTRIES`
	- Date/time columns: audit entry timestamp column not named in the available trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by `OCCAM_QUERY`/`OCCAM_REST` list procedures; no direct date expression found in trunk source.

- `OCCAM_AUDIT_LOG_ENTRY_TYPES`
	- Date/time columns: none identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by OCCAM list procedures and unit-test setup scripts.

- `OCCAM_DISPUTE_COUNTS`
	- Date/time columns: no named date column identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by `OCCAM_QUERY` and `OCCAM_REST` procedures.

- `OCCAM_DISPUTE_STATUS_TYPES`
	- Date/time columns: none identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by OCCAM dispute-list procedures.

- `OCCAM_DISPUTE_UPDATE_REQ_TYPES`
	- Date/time columns: none identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by OCCAM update-request procedures.

- `OCCAM_DISPUTE_UPDATE_REQUEST`
	- Date/time columns: `ENT_DTM`, `UPD_DTM`, and request/status timestamps are inferred from the update-request package and trigger conventions; confirm names and types in `ALL_TAB_COLUMNS`.
	- Procedure usage: processed by `OCCAM_DISPUTE_UPDATE_REQUEST` package procedures and `OCCAM_QUERY`/`OCCAM_REST` list procedures.

- `OCCAM_DISPUTE_UPDATE_REQUESTS`
	- Date/time columns: request/status timestamps not explicitly named in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by OCCAM update-request list procedures.

- `OCCAM_DISPUTE_UPDATE_STAT_TYPS`
	- Date/time columns: none identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by OCCAM update-request procedures.

- `OCCAM_DISPUTES`
	- Date/time columns: `ENT_DTM`, `UPD_DTM`, `USER_ASSIGNED_DTM`; `USER_ASSIGNED_DTM` is directly assigned with `SYSDATE` in `OCCAM_QUERY` and `OCCAM_VIOLATION_TICKET`.
	- Procedure usage:
		- `OCCAM_QUERY` package body: assigns `USER_ASSIGNED_DTM = SYSDATE` and uses dispute date fields in list queries.
		- `OCCAM_REST` package body: returns dispute date fields and applies date-related list predicates.
		- OCCAM table triggers: assign `ENT_DTM`/`UPD_DTM` with `SYSDATE` where the trigger targets the table.

- `OCCAM_ERROR_LOGS`
	- Date/time columns: error timestamp not explicitly named in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: OCCAM error/audit package procedures.

- `OCCAM_OUTGOING_EMAILS`
	- Date/time columns: `EMAIL_SENT_DTM` is referenced by the OCCAM email/history contract; confirm type in `ALL_TAB_COLUMNS`.
	- Procedure usage: OCCAM REST/query procedures return email history; no direct DST calculation found.

- `OCCAM_VIOLATION_TICKET`
	- Date/time columns: `ENT_DTM`, `UPD_DTM`, `USER_ASSIGNED_DTM`; assignment uses `SYSDATE` in `OCCAM_VIOLATION_TICKET`.
	- Procedure usage:
		- `OCCAM_VIOLATION_TICKET` package body: assigns `USER_ASSIGNED_DTM = SYSDATE`.
		- OCCAM ticket triggers: assign `ENT_DTM`/`UPD_DTM` with `SYSDATE`.

- `OCCAM_VIOLATION_TICKET_COUNTS`
	- Date/time columns: no named date column identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by OCCAM violation-ticket list procedures.

- `OCCAM_VIOLATION_TICKET_UPLOADS`
	- Date/time columns: upload timestamp not explicitly named in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by OCCAM violation-ticket procedures.

### TCO tables

- `TCO_APPEARANCE_AMENDMENTS`
	- Date/time columns: amendment timestamp not explicitly named in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: `TCO_APPEARANCE_AMENDMENT` package procedures.

- `TCO_APPEARANCE_CHARGE_COUNTS`
	- Date/time columns: no named date column identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: `TCO_APPEARANCE_DATA_MODIFY` and appearance-charge procedures.

- `TCO_AUDIT_LOG`
	- Date/time columns: `ENT_DTM`, `UPD_DTM` (inferred from the audit package contract; confirm in `ALL_TAB_COLUMNS`).
	- Procedure usage:
		- `TCO_AUDIT` package body: assigns `ENT_DTM := SYSDATE` and `UPD_DTM := SYSDATE`.

- `TCO_AUDIT_LOG_ENTRIES`
	- Date/time columns: audit entry timestamp column not named in the available trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by TCO audit/query/REST list procedures.

- `TCO_AUDIT_LOG_ENTRY_TYPES`
	- Date/time columns: none identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: queried by TCO audit/query procedures.

- `TCO_COURT_APPEARANCES`
	- Date/time columns: `APPEARANCE_DTM`.
	- Procedure usage:
		- `OCCAM_QUERY` package body: selects the latest `APPEARANCE_DTM` and filters `APPEARANCE_DTM >= SYSDATE - 5`; calculates the 14-day indicator with `SYSDATE + 14`.
		- `OCCAM_REST` package body: filters `APPEARANCE_DTM >= SYSDATE - 5` and calculates the 14-day indicator.
		- TCO appearance query/data-modification packages: read and write court appearance dates.

- `TCO_DISPUTE_COUNTS`
	- Date/time columns: no named date column identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: TCO dispute/query/REST procedures.

- `TCO_DISPUTE_REMARKS`
	- Date/time columns: `REMARKS_MADE_DTM` is referenced by the dispute/remarks contract; confirm type in `ALL_TAB_COLUMNS`.
	- Procedure usage: TCO dispute and REST procedures read/write remarks.

- `TCO_DISPUTE_STATUS_TYPES`
	- Date/time columns: none identified in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: TCO dispute-list procedures.

- `TCO_DISPUTES`
	- Date/time columns: `SUBMITTED_DT`, `VIOLATION_DT`, `JJ_DECISION_DT`, `VTC_ASSIGNED_DTM`, `ENT_DTM`, and `UPD_DTM` are referenced by TCO/ORDS mappings and packages; confirm physical types in `ALL_TAB_COLUMNS`.
	- Procedure usage:
		- `TCO_DISPUTE` package body: assigns `VTC_ASSIGNED_DTM = SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)`.
		- `TCO_QUERY` package body: filters dispute/address records using `CURRENT_DATE`.
		- `TCO_REST` package body: filters using `CURRENT_DATE`, assigns `JJ_DECISION_DT = SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)`, and emits `SYSDATE` in the JSON `CREATE_DATE` field.
		- TCO audit/trigger procedures: assign audit timestamps with `SYSDATE`.

- `TCO_ERROR_LOGS`
	- Date/time columns: error timestamp not explicitly named in trunk SQL; confirm in `ALL_TAB_COLUMNS`.
	- Procedure usage: TCO error/debug/audit package procedures; `TCO_DEBUG` formats `SYSDATE` for diagnostic text.

### Additional unqualified application tables

The trunk packages also reference unqualified `JUSTIN_*` tables and views, including `JUSTIN_ADDRESSES`, `JUSTIN_APPEARANCES`, `JUSTIN_MATTERS`, `JUSTIN_DOCUMENTS`, `JUSTIN_PARTICIPANTS`, `JUSTIN_WORK_ASSIGNMENTS`, `JUSTIN_POLICE_COMMITMENTS`, and related lookup/count objects. Their date columns and procedure usages cannot be safely enumerated from schema-qualified references alone; run the catalog query below and join the results to `ALL_DEPENDENCIES`/`ALL_SOURCE`.

## Catalog query required for completion

Run this query as the owning Oracle user, or with catalog privileges, to replace inferred columns with authoritative data types and include tables not referenced in application SQL:

```sql
SELECT owner,
			 table_name,
			 column_name,
			 data_type,
			 data_length,
			 data_precision,
			 data_scale,
			 nullable
FROM   all_tab_columns
WHERE  owner IN ('TCO', 'OCCAM', 'JUSTIN')
AND    data_type IN ('DATE', 'TIMESTAMP', 'TIMESTAMP WITH TIME ZONE', 'TIMESTAMP WITH LOCAL TIME ZONE')
ORDER BY owner, table_name, column_id;
```

For each returned column, search `ALL_SOURCE` for the column name and package/procedure source, and include each owning routine in the corresponding field sublist. The current source scan confirms the explicit `SYSDATE`, `CURRENT_DATE`, `CURRENT_TIMESTAMP`, and `SYS_EXTRACT_UTC` usages listed above; it does not claim that an unqualified or dynamically generated SQL reference is absent.