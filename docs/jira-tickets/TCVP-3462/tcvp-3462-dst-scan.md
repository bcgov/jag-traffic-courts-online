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