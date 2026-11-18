# Filters, Searches, and "Now" / "Today" Calculations

These datetimes are not stored. They are computed values or query parameters that are compared with stored datetimes. The boundary rules (B1–B9) are described in [serialization-boundaries.md](serialization-boundaries.md).

---

## Date range filters

All active staff-portal list screens filter on the server using ORDS v2. Calendar dates selected in the portal arrive at Staff.Service as `yyyy-MM-dd`.

| Filter (screen) | Browser → query | Staff.Service conversion | Compared with (zone) | Assessment |
|---|---|---|---|---|
| Submitted date range (staff ticket inbox, OCCAM v2 `occam_dispute_list`) | `bsDatepicker` `Date` → generated helper `toISOString().substr(0,10)` → `from`/`thru` ([table-filters.component.ts](../../src/frontend/staff-portal/src/app/components/table-filters/table-filters.component.ts#L58), [dispute.service.ts](../../src/frontend/staff-portal/src/app/api/api/dispute.service.ts#L106)) | `from` → midnight in the X-Timezone zone → UTC `submitted_dt_ge`. `thru + 1 day` → midnight in that zone → UTC `submitted_dt_lt`. Formatted with a trailing **`Z`** ([Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/Occam/Disputes/Handler.cs#L160)) | `OCCAM_DISPUTES.SUBMITTED_DT` (UTC). ORDS strips the `Z` ([tco_common_body.sql](../../database/tco/definitions/Packages/tco_common_body.sql#L88)) | Correct |
| Submitted date range (update-request inbox, OCCAM v2 `occam_dispute_update_requests_list`) | same | same. A null zone defaults to Vancouver ([Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/Occam/DisputesWithUpdateRequests/Handler.cs#L177)) | `OCCAM_DISPUTES.SUBMITTED_DT` (UTC). It does **not** filter on the update request's own date | Correct |
| Submitted date range (JJ / DCF, TCO v2 `tco_disputes`) | `submitted_from` / `submitted_thru` | Same half-open UTC range, **without** `Z` ([Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/CourtFiles/Summaries/Handler.cs#L163)) | `TCO_DISPUTES.SUBMITTED_DT` (UTC) | Correct |
| JJ decision date range (decision-validation inbox) | `jj_decision_dt_from` / `_thru` ([dispute-decision-inbox.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/dispute-decision-inbox/dispute-decision-inbox.component.ts#L94)) | Same half-open UTC range ([Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/CourtFiles/Summaries/Handler.cs#L164)) | `TCO_DISPUTES.JJ_DECISION_DT` (UTC) | Correct |
| Appearance date (JJ hearing inbox) | `Date.toLocaleDateString("en-CA")` → `appearance_dt_from` = `_thru` ([jj-dispute-hearing-inbox.component.ts](../../src/frontend/staff-portal/src/app/components/jj-workbench/jj-dispute-hearing-inbox/jj-dispute-hearing-inbox.component.ts#L127)) | **No zone conversion.** Becomes `appr_tm_ge` = date and `appr_tm_lt` = date + 1 ([Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/CourtFiles/Summaries/Handler.cs#L166)) | JUSTIN `APPR_TM` (Local), filter type `date` ([tco_query_body.sql](../../database/tco/definitions/Packages/tco_query_body.sql#L770)) | Correct. A calendar day at the court is the right meaning here. |
| Legacy dispute list `From` / `Thru` (`GetAllDisputesParameters`) | `DateTimeOffset?` | Compared directly with `SubmittedTs` (UTC digits, `Unspecified`) **before** `UtcToLocalTime`. .NET reads the `Unspecified` value as server-local time when comparing ([GetAllDisputesParameters.cs](../../src/backend/TrafficCourts/Staff.Service/Models/Disputes/GetAllDisputesParameters.cs#L38), [DisputeListItemExtension.cs](../../src/backend/TrafficCourts/Staff.Service/Models/Disputes/DisputeListItemExtension.cs#L27)) | `SubmittedTs` (UTC) read as Local | Off by the UTC offset. Low impact: the staff-portal uses v2 instead ([I-19](issues.md#i-19)) |
| `JJDisputeService` summary-parameter helper (query `timeZone`, not the header) | `decisionDateFrom/Thru`, `dateSubmittedFrom/Thru` | Midnight in the given zone → UTC. Unknown zones fall back to Vancouver ([JJDisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/JJDisputeService.cs#L189)) | UTC columns | Correct. This duplicates the handler logic. |
| Statute effective-on (citizen and staff lookups) | optional | Defaults to server `DateTime.Today` (Local, **not** X-Timezone), then formatted `yyyy-MM-dd` ([StatuteLookupService.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Lookups/StatuteLookupService.cs#L46), [Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/Lookups/Statutes/Handler.cs#L19), [StatuteRepository.cs](../../src/backend/TrafficCourts/TrafficCourts.OrdsDataService/Justin/StatuteRepository.cs#L46)) | JUSTIN `STAT_EFFECTIVE_DT` / `STAT_TERMINATION_DT` (Date-only) | Correct |

Note on the picker's `toISOString()`: a picker `Date` at local midnight in Vancouver or Edmonton has the same UTC calendar date, so the selected day is kept. This would not hold for users east of UTC.

---

## Ticket and dispute search by issued time

| # | Stage | Field | Zone in → out | Notes |
|---|---|---|---|---|
| 1 | citizen-portal | `form.time` (user input) or `DatePipe(issued_date,'HH:mm')` (offset-less, so the digits are kept) | — → Local | [violation-ticket.service.ts](../../src/frontend/citizen-portal/src/app/services/violation-ticket.service.ts#L330) |
| 2 | Citizen.Service `GET /api/disputes/search` | `time` → `SearchDisputeRequest.IssuedTime` (string) | Local → Local | Forwarded as is. Unlike ticket search, it is not normalized through `TimeOnly`. [DisputesController.cs](../../src/backend/TrafficCourts/Citizen.Service/Controllers/DisputesController.cs#L219) |
| 3 | Workflow.Service `SearchDisputeConsumer` → OracleDataApi `FindDisputeStatusesAsync(issuedTime)` | string | Local → Local | [SearchDisputeConsumer.cs](../../src/backend/TrafficCourts/Workflow.Service/Consumers/SearchDisputeConsumer.cs#L32) |
| 4 | Java `GET /dispute/status` | `@DateTimeFormat("HH:mm") Date issuedTime`, **documented as UTC** → `SimpleDateFormat("HH:mm")` in the default zone → ORDS `issueTime` | Local → Local | Parsed and formatted in the same zone, so the digits are kept. The documentation is wrong ([I-11](issues.md#i-11)). [DisputeController.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/controller/v1_0/DisputeController.java#L133), [DisputeRepositoryImpl.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/repository/impl/ords/DisputeRepositoryImpl.java#L96) |
| 5 | ORDS | `TO_CHAR(d.issued_dt,'HH24:MI') = issueTime` | Local vs Local | Correct. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L491) |

RSI ticket search by time is traced in [external-services-datetimes.md](external-services-datetimes.md#rsi-ticket-search).

---

## Scheduled unassignment (Java)

The schedulers set no zone, so they use the JVM default (Vancouver) ([DisputeTasks.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/scheduled/DisputeTasks.java#L24), [application.yml](../../src/backend/oracle-data-api/src/main/resources/application.yml#L57)).

| Job | Threshold | Column compared | Assessment |
|---|---|---|---|
| OCCAM dispute unassign | `new Date() - 30 min` → `DateUtil.formatAsDateTime`, **Local** ([DisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/DisputeService.java#L369)) | `OCCAM_DISPUTES.USER_ASSIGNED_DTM` (Local, `SYSDATE`) ([occam_violation_ticket_body.sql](../../database/occam/definitions/Packages/occam_violation_ticket_body.sql#L1083)) | Correct |
| TCO VTC unassign | `new Date() - 1 h` → **Local** ([JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L121)) | `TCO_DISPUTES.VTC_ASSIGNED_DTM` (**UTC**) ([tco_dispute_body.sql](../../database/tco/definitions/Packages/tco_dispute_body.sql#L499)) | **Wrong** ([I-4](issues.md#i-4)) |

---

## "Now" / "today" calculations

| Where | Expression | Zone | Used for |
|---|---|---|---|
| Citizen.Service | `_clock.GetUtcNow().DateTime.Truncate()` | UTC (`Unspecified`) | `SubmittedTs` ([Create.cs](../../src/backend/TrafficCourts/Citizen.Service/Features/Disputes/Create.cs#L174)) |
| Citizen.Service | `TimeProvider.GetLocalNow().DateTime` → today at midnight | Local | OCR "date of service within 30 days" rule ([DateOfServiceLT30Rule.cs](../../src/backend/TrafficCourts/Citizen.Service/Validators/Rules/DateOfServiceLT30Rule.cs#L29)) |
| citizen-portal | `(Date.now() - new Date(givenDate)) / 86400000`, rounded. `service_date` (`yyyy-MM-dd`) is read as **UTC midnight**; `issued_date` (offset-less) is read as **browser-local** | Unknown | Eligibility limits of 30 or 45 days. Can disagree with the server rule near the boundary ([I-18](issues.md#i-18)). [violation-ticket.service.ts](../../src/frontend/citizen-portal/src/app/services/violation-ticket.service.ts#L480) |
| Citizen.Service / Staff.Service | `DateTime.Today` | Local | Statute effective-on default |
| Citizen.Service mock search | `GetCurrentPacificTime()` | Local | Mock ticket dates |
| Staff.Service | `DateTimeOffset.Now` | Local | Legacy "next hearing" selection ([DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L459)) |
| Staff.Service | `GetUtcNow()` / `DateTime.UtcNow` | UTC | Dispute locks |
| staff-portal | browser `new Date()` | Local (X-Timezone) | Header date ([header.component.ts](../../src/frontend/staff-portal/src/app/shared/components/header/header.component.ts#L19)); hearing-inbox default day ([hearing-inbox-filter.service.ts](../../src/frontend/staff-portal/src/app/services/hearing-inbox-filter.service.ts#L8)); manual-entry "today" and default submitted date ([manual-dispute-entry.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/manual-dispute-entry/manual-dispute-entry.component.ts#L54)); revised-due "today" ([jj-count.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.ts#L86)); JJ decision "now" ([jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L381)); no-app "now" ([jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L464)); 2027-07-01 surcharge cut-off ([jj-count.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.ts#L105)) |
| Workflow.Service | `_clock.GetUtcNow()` | UTC | Email sent / filtered; saga checked / verified |
| ARC service | `TimeProvider.GetLocalNow()` | Local | ARC transaction date/time |
| Java | `new Date()`, formatted Local | Local | `statusUpdateTs`, `latestPleaUpdateTs`, `remarksMadeTs`; unassign thresholds; `isSameDay` recall rule |
| Oracle | `SYSDATE` | Local | `USER_ASSIGNED_DTM`; `appear_less_than_14`; appearances `>= SYSDATE-5` ([occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L671)); audit columns |
| Oracle | `SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)` | UTC | `VTC_ASSIGNED_DTM`, `JJ_DECISION_DT` |
| Oracle | `CURRENT_DATE` | Session zone | Picks the current JUSTIN address ([tco_query_body.sql](../../database/tco/definitions/Packages/tco_query_body.sql#L1040), [I-20](issues.md#i-20)) |

"Today" is decided in several different places: the browser (user zone), the .NET and Java servers (Vancouver), and Oracle (Vancouver, UTC, or the session zone). For an Edmonton user between 00:00 and 01:00 Edmonton time, the browser's "today" is one day ahead of the server's.
