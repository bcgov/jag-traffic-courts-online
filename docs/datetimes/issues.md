# Issues and Simplification Opportunities

This file lists every incorrect conversion or storage found while tracing the datetimes in this folder. Each issue is linked from the per-datetime traces.

**Confidence levels:**
- **Verified (static):** confirmed by reading every hop of the code. Not run.
- **Needs runtime check:** depends on library or runtime behaviour that should be confirmed with a test.

## Current storage conventions

All columns are Oracle `DATE` and store no zone. The convention below is implied only by the code that writes each column.

| Convention | Columns |
|---|---|
| **UTC** | `OCCAM_DISPUTES.SUBMITTED_DT`, `TCO_DISPUTES.SUBMITTED_DT`, `TCO_DISPUTES.JJ_DECISION_DT`, `TCO_DISPUTES.VTC_ASSIGNED_DTM`, `OCCAM_OUTGOING_EMAILS.EMAIL_SENT_DTM` |
| **Local** (event time) | `OCCAM_DISPUTES.USER_ASSIGNED_DTM`, `OCCAM_DISPUTE_UPDATE_REQUESTS.STATUS_UPDATE_DTM`, `TCO_DISPUTE_REMARKS.REMARKS_MADE_DTM`, `TCO_COURT_APPEARANCES.DISPUTANT_NOT_PRESENT_DTM` (intended), every `ENT_DTM` / `UPD_DTM` |
| **Local** (court/ticket wall-clock time) | `OCCAM_DISPUTES.ISSUED_DT`, `OCCAM_VIOLATION_TICKET_UPLOADS.ISSUED_DT`, `TCO_COURT_APPEARANCES.APPEARANCE_DTM`, JUSTIN `APPR_TM` / `APPR_DT`, `TCO_DISPUTES.ICBC_RECEIVED_DT` |
| **Mixed** | `TCO_DISPUTE_COUNTS.LATEST_PLEA_UPDATE_DTM` (Java writes Local, Staff.Service writes UTC) |
| **Date-only** | birthdates, `FILING_DT`, `VIOLATION_DT`, `FINE_DUE_DT`, `REVISED_DUE_DT`, `VIOLATION_DATE_DTM`, statute dates |

Event timestamps such as assignments, decisions, and status changes are therefore split between UTC and Local, with no rule deciding which one a column uses.

---

## Incorrect conversions or storage

<a id="i-1"></a>
### I-1 Staff dispute update shifts `SUBMITTED_DT` back by twice the UTC offset on every save

- **Severity:** High. **Confidence:** Verified (static).
- **What happens:**
  1. `GET /api/dispute/{id}` converts `SubmittedTs` from UTC to Local (X-Timezone).
  2. The staff-portal sends the whole dispute back on `PUT /api/dispute/{id}`, for example the ticket-validation **Save** ([ticket-info.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/ticket-info/ticket-info.component.ts#L559)).
  3. `DisputeService.UpdateDisputeAsync` calls `dispute.UtcToLocalTime(timeZone)` on the **incoming** value, so a value that is already local is converted a second time ([DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L175)).
  4. Java copies `submittedTs` onto the stored dispute ([DisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/DisputeService.java#L156)), and `prModifyViolationTicket` writes `SUBMITTED_DT`.
  5. A second `UtcToLocalTime` is applied to the **input** object after the save. The returned `updatedDispute` is **not** converted, so the response contains UTC digits.
- **Example:** submitted 2026-01-15 10:00 PST, stored as `2026-01-15T18:00:00` UTC.
  - A Vancouver user opens it and sees 10:00.
  - On save, 10:00 is converted again to `2026-01-15T02:00:00` and stored.
  - The next GET shows **2026-01-14 18:00**.
  - The shift is 16 h (PST) or 14 h (PDT) for Vancouver users, and 14 h (MST) or 12 h (MDT) for Edmonton users. It repeats on every save.
- **Downstream effects:**
  - The submitted-date filters and inbox sort order are wrong.
  - If the update runs after JUSTIN reconciliation, `TCO_DISPUTES.SUBMITTED_DT` (copied once) and `OCCAM_DISPUTES.SUBMITTED_DT` no longer match.
- **Fix:** convert the input with `dispute.LocalToUtcTime(timeZone)` before saving, and call `updatedDispute.UtcToLocalTime(timeZone)` on the result. This matches `CreateDisputeAsync` and `JJDisputeService`.

<a id="i-2"></a>
### I-2 Staff ticket validation stores a Local `SubmittedTs` in a UTC column

- **Severity:** High. **Confidence:** Verified (static).
- **What happens:**
  1. `PUT /api/dispute/{id}/validate` sends the dispute from GET, with `SubmittedTs` already in Local (X-Timezone) ([dispute.service.ts](../../src/frontend/staff-portal/src/app/services/dispute.service.ts#L338)).
  2. `ValidateDisputeAsync` saves it through OracleDataApi **without** `LocalToUtcTime` ([DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L198)).
  3. The endpoint does not accept `X-Timezone`, so it cannot convert the value.
- **Effect:** `SUBMITTED_DT` moves back by 7–8 h (Vancouver user) or 6–7 h (Edmonton user). It is also wrong in TCO if validation happens before reconciliation copies the value.
- **Fix:** accept `X-Timezone` on validate and call `LocalToUtcTime`. Better still, stop round-tripping `SubmittedTs` from the client at all (see [S-3](#s-3)).

<a id="i-3"></a>
### I-3 Java reads UTC values as Vancouver time and rejects values in the spring-forward gap

- **Severity:** Medium. The window is rare, but the failure blocks the save. **Confidence:** Verified (static), Needs runtime check (JDK non-lenient behaviour).
- **What happens:**
  - `DateTimeDeserializer` parses every date-time as `America/Vancouver` with `setLenient(false)` ([DateTimeDeserializer.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateTimeDeserializer.java#L37)).
  - UTC values arrive as offset-less strings: `SubmittedTs`, `EmailSentTs`, and the JJ-save values `JjDecisionDate`, `VtcAssignedTs`, `LatestPleaUpdateTs`.
  - A non-lenient `GregorianCalendar` throws for a local time that does not exist, such as `02:30` on the spring-forward Sunday. `SimpleDateFormat.parse` reports this as a `ParseException`, so Java returns an error.
- **Example:** a citizen submits on Saturday 2026-03-07 18:30 PST. `SubmittedTs` = `2026-03-08T02:30:00` (UTC). Vancouver moves its clocks forward at 02:00 on 2026-03-08, so Java cannot parse it. `SubmitNoticeOfDisputeConsumer` fails and retries, and the dispute is **not saved**. Any later JJ save carrying that `SubmittedTs` fails in the same way.
- **Also:**
  - Inside Java, these `Date` instants are 7–8 h wrong. No Java logic uses them today, so only parsing is affected.
  - In the fall-back hour the parse picks one of the two possible instants, but formatting gives back the same digits, so nothing is lost.
- **Fix (pick one):**
  - Make the Java date-time (de)serializers zone-neutral: use `TimeZone UTC` for both, or `java.time.LocalDateTime`. This keeps the "digits only" behaviour without DST validation.
  - Send UTC values with `Z` and have Java and ORDS convert explicitly.

<a id="i-4"></a>
### I-4 JJ (TCO) VTC auto-unassign compares a Local threshold with a UTC column

- **Severity:** Medium. **Confidence:** Verified (static).
- **What happens:**
  - `JJDisputeService.unassignJJDisputes` computes `new Date() - 1h` and formats it as Vancouver wall-clock time ([JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L121), [DateUtil.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/util/DateUtil.java#L27)).
  - `pDisputeUnassignVtc` compares it with `VTC_ASSIGNED_DTM`, which is written as `SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)` ([tco_dispute_body.sql](../../database/tco/definitions/Packages/tco_dispute_body.sql#L478), [tco_dispute_body.sql](../../database/tco/definitions/Packages/tco_dispute_body.sql#L499)).
- **Effect:** JJ disputes stay assigned to a VTC staff member for about **9 h (PST) / 8 h (PDT)** instead of 1 h. The OCCAM equivalent is consistent, because both sides there are Local `SYSDATE`.
- **Fix:** use one convention. Either write `VTC_ASSIGNED_DTM` with `SYSDATE` (matching OCCAM `USER_ASSIGNED_DTM`), or have the database compute the threshold, for example `SYS_EXTRACT_UTC(SYSTIMESTAMP) - 1/24`. The Staff.Service `UtcToLocalTime` for `VtcAssignedTs` must change with it.

<a id="i-5"></a>
### I-5 OCCAM update-request list converts a date-only JJ decision date as if it were a UTC timestamp

- **Severity:** Low (latent: the field is returned by the API but not currently displayed). **Confidence:** Verified (static).
- **What happens:**
  - `occam_dispute_update_requests_list` returns `to_char(tco_disp.jj_decision_dt,'YYYY-MM-DD')`, the **UTC calendar date** ([occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L838)).
  - Staff.Service then applies `UtcToLocalTime` to it ([ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L164)). UTC midnight becomes 16:00–18:00 **on the previous day**.
- **Effect:** `OccamDisputeWithUpdateRequestListItemModel.jjDecisionDate` is wrong for decisions made before 16:00 PST / 17:00 PDT. It is also wrong for the other decisions, because the UTC date is not the local date.
- **Fix:** select the DT mask (`'YYYY-MM-DD"T"HH24:MI:SS'`) as the other lists do, or stop converting this field.

<a id="i-6"></a>
### I-6 `LATEST_PLEA_UPDATE_DTM` is written in two conventions and read as UTC

- **Severity:** Low–Medium. **Confidence:** Verified (static).
- **What happens:**
  - Staff.Service writes `LocalToUtcTime(latestPleaUpdateTs)`, which is UTC ([ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L72)).
  - When the plea changes, Java replaces it with `new Date()`, which is **Local** ([JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L153)).
  - Staff.Service reads it back with `UtcToLocalTime` ([ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L44)).
  - Separately, the staff-portal replaces the value with browser-local midnight on **any** count form change ([jj-count.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.ts#L313)).
- **Effect:**
  - Java-written values are shown and printed 7–8 h (Vancouver) or 6–7 h (Edmonton) too early ([PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L428)).
  - Changes made between 00:00 and about 08:00 local show the previous date.
  - The time part is lost as soon as the count form is edited.
- **Fix:** have Java set the timestamp in the same convention as the column (UTC, if UTC is chosen), or have Staff.Service set it. Then decide whether the value is a date or a timestamp, and remove the browser truncation if it is a timestamp.

<a id="i-7"></a>
### I-7 The no-app time is sent as a JS `Date` (`…Z`), which the Java service rejects

- **Severity:** High when used. **Confidence:** Verified (static), Needs runtime check.
- **What happens:**
  1. `courtAppearanceForm.noAppTs` holds a JS `Date` from the picker, "now", or `new Date(existing)`. It is merged into the court-appearance RoP ([jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L600)).
  2. `JSON.stringify` writes `2026-10-28T21:35:00.000Z`.
  3. Staff.Service binds it as `Kind=Utc` and does not convert it.
  4. Newtonsoft writes `…Z`.
  5. Java's regex rejects anything except `yyyy-MM-ddTHH:mm:ss` ([DateTimeDeserializer.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateTimeDeserializer.java#L32)).
- **Effect:** saving a JJ dispute whose current appearance has a no-app time fails. If the value were accepted, it would be a UTC instant stored in a Local column, unlike `APPEARANCE_DTM`.
- **Fix:** format `noAppTs` in the browser as a local offset-less string, as is done for `jjDecisionDate`. Alternatively, normalize it in Staff.Service with `X-Timezone`: convert to Vancouver and set `Kind=Unspecified`.

<a id="i-8"></a>
### I-8 `X-Timezone` is applied to only some fields, so Edmonton users see mixed zones

- **Severity:** Low (design). **Confidence:** Verified (static).
- **Converted to the user's zone:** submitted, JJ decision, VTC assigned (TCO), latest plea update, email sent ([B8](serialization-boundaries.md#b8-conversion-helpers-trafficcourtscore)).
- **Shown as Vancouver digits without a label:** issued date/time, appearance and no-app times, staff assigned (OCCAM), update-request status timestamp, remark and file-history times (`ENT_DTM`), update-request "requested" times.
- **Printing:** DCF and WR documents mix both conventions, and the printed submitted time depends on who printed the document ([external-services-datetimes.md](external-services-datetimes.md#cdogs-staff-print-to-pdf)).
- **Effect:** an Edmonton user looking at one screen sees "submitted 11:00" (Edmonton) next to "status updated 10:00" (Vancouver) for events that happened at the same moment.
- **Fix:** decide which values are *event instants*, and convert all of them to the user's zone. Decide which are *court or ticket wall-clock* values, and either label them with the court's zone or leave them unconverted on purpose. Then apply that rule everywhere, including print.

<a id="i-9"></a>
### I-9 UTC values travel without a UTC marker

- **Severity:** Low (design). **Confidence:** Verified (static).
- **What happens:**
  - `SubmittedTs` and `EmailSentTs` are deliberately set to `Kind=Unspecified` ([Create.cs](../../src/backend/TrafficCourts/Citizen.Service/Features/Disputes/Create.cs#L174), [Mapper.cs](../../src/backend/TrafficCourts/Workflow.Service/Mappings/Mapper.cs#L13)). `LocalToUtcTime` strips `Kind=Utc` ([ClockExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/ClockExtensions.cs#L60)).
  - `UtcToLocalTime` forces `Kind=Utc` whatever the input is ([ClockExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/ClockExtensions.cs#L41)).
  - This is needed because Java rejects `Z` (B5).
- **Effect:** whether a value is UTC is known only by convention for each field. Nothing stops UTC values from being treated as Local, or the reverse; I-1, I-2, I-5, and I-6 are all examples. RabbitMQ messages carry UTC and Local values in the same offset-less format.

<a id="i-10"></a>
### I-10 The JUSTIN dispute-filed date is taken from the UTC `SUBMITTED_DT`

- **Severity:** Medium if the interface is used. It is called by JUSTIN, so whether it is used in production was not determined. **Confidence:** Verified (static).
- **What happens:** `justin_tco_interface.ticket_data_insert` sends `DisputeFiledDate => TO_CHAR(d.SUBMITTED_DT,'YYYY-MM-DD')` ([justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L335), [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L455)).
- **Effect:** disputes submitted after 16:00 PST / 17:00 PDT get the **next day** as their filing date in JUSTIN.
- **Fix:** convert first, for example `TO_CHAR(CAST(FROM_TZ(CAST(d.SUBMITTED_DT AS TIMESTAMP),'UTC') AT TIME ZONE 'America/Vancouver' AS DATE),'YYYY-MM-DD')`.

<a id="i-11"></a>
### I-11 Documentation says UTC for values that are Local

- **Severity:** Low. **Confidence:** Verified (static).
- Java `GET /dispute/status` documents `issuedTime` as UTC, but compares it with Local `TO_CHAR(issued_dt,'HH24:MI')` ([DisputeController.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/controller/v1_0/DisputeController.java#L133)).
- The Java `Dispute.issuedTs` comment also says UTC ([Dispute.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/model/Dispute.java#L69)).
- Anyone who follows the documentation and sends UTC will not find the ticket.

<a id="i-12"></a>
### I-12 Local wall-clock values only work while they stay `Kind=Unspecified` and offset-less

- **Severity:** Low (design). **Confidence:** Verified (static).
- The issued date/time, appearance times, and ARC dates are correct only because no hop adds an offset.
  - If a `DateTime` became `Kind=Utc` or `Local` (for example after `DateTime.SpecifyKind`, or after parsing a `Z` input), ARC would convert it to server-local time before taking `yyyyMMdd` ([TcoDisputeTicket.cs](../../src/backend/TrafficCourts/TrafficCourts.Arc.Dispute.Service/Models/TcoDisputeTicket.cs#L20)), and Java would reject it.
- Browsers read these strings in their own zone. The digits shown stay correct, but calculations do not, for example [I-18](#i-18).

<a id="i-13"></a>
### I-13 Date-only values are typed as date-times in several places

- **Severity:** Low. **Confidence:** Verified (static).
- **Java API:** uses the date-time serializer for ORDS date-only fields. .NET receives `T00:00:00` for violation date, ICBC received, due and revised due dates, and the amended violation date ([serialization-boundaries.md B5](serialization-boundaries.md#b5-oracle-data-api-java)).
- **.NET domain:** uses `DateTime` for most date-only values: birthdate, filing date, due dates, violation date. Only `DecisionMadeDt` is `DateOnly`.
- **Messages:** `SubmitNoticeOfDispute.DisputantBirthdate` is `DateTime`, while the nested `ViolationTicket.DisputantBirthdate` is `DateOnly`.
- **Generated client:** the OracleDataApi client has `DateOnly?` for birthdate, filing date, and decision-made date, even though `.nswag` says `DateTime` and the OpenAPI spec says `date-time`. Regenerating the client may change these types.
- **Effect:** there is no bug today, because every hop stays at local midnight and no zone conversion is applied. A future change that adds a conversion would shift these dates by a day.

<a id="i-14"></a>
### I-14 Conversions on the JJ save path have no effect on what is stored

- **Severity:** Low (simplification). **Confidence:** Verified (static).
- TCO `PUT updateDispute` does not write top-level dates, because that block is commented out ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L929)).
- `JJ_DECISION_DT` is set by the database on CONF/REQH ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1227)).
- As a result, all of the following have no effect on stored data:
  - the browser's `jjDecisionDate` ([jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L385))
  - the `LocalToUtcTime` of `JjDecisionDate`, `SubmittedTs`, and `VtcAssignedTs`
- They only add a way for the save to fail ([I-3](#i-3)).

<a id="i-15"></a>
### I-15 History screens ignore the converted values

- **Severity:** Low. **Confidence:** Verified (static).
- `FileHistoryController` checks `X-Timezone`, but `FileHistoryService` never uses it ([FileHistoryService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/FileHistoryService.cs#L18)).
- `EmailHistoryService` converts `EmailSentTs`, but the UI shows `createdTs` (`ENT_DTM`, Local) ([jj-file-history.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-file-history/jj-file-history.component.ts#L82)).
- Remarks show `createdTs` instead of `remarksMadeTs`.

<a id="i-16"></a>
### I-16 `FINE_DUE_DT` is computed without `TRUNC`

- **Severity:** Low. **Confidence:** Verified (static).
- `MATT_OFFENCE_DT + 30` keeps any time part of the offence date ([tco_appearance_data_modify_body.sql](../../database/tco/definitions/Packages/tco_appearance_data_modify_body.sql#L375)).
- Reads use a date-only mask, so this is only visible in direct SQL comparisons.

<a id="i-17"></a>
### I-17 Manual dispute entry takes the issued date from `toISOString()`

- **Severity:** Low. **Confidence:** Verified (static).
- The date part comes from the **UTC** calendar date of the picker value and is combined with the local time the user typed ([manual-dispute-entry.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/manual-dispute-entry/manual-dispute-entry.component.ts#L1388)).
- This is correct for local-midnight picker values in Vancouver or Edmonton, but breaks for any non-midnight `Date`, or for zones east of UTC.

<a id="i-18"></a>
### I-18 The citizen eligibility check in the browser disagrees with the server rule

- **Severity:** Low. **Confidence:** Verified (static).
- **Browser:** `Math.round((now - new Date(givenDate)) / 1 day)`. A `service_date` (`yyyy-MM-dd`) is read as UTC midnight; an `issued_date` is read as browser-local time ([violation-ticket.service.ts](../../src/frontend/citizen-portal/src/app/services/violation-ticket.service.ts#L480)).
- **Server:** compares Pacific calendar days ([DateOfServiceLT30Rule.cs](../../src/backend/TrafficCourts/Citizen.Service/Validators/Rules/DateOfServiceLT30Rule.cs#L29)).
- The two results can differ near the 30- and 45-day limits.

<a id="i-19"></a>
### I-19 The legacy staff dispute-list filter compares UTC digits as Local

- **Severity:** Low (the staff-portal uses the v2 lists). **Confidence:** Verified (static).
- `GetAllDisputesParameters.From/Thru` (`DateTimeOffset`) are compared with `SubmittedTs` (UTC digits, `Unspecified`) before conversion. .NET reads the `Unspecified` value as server-local time ([DisputeListItemExtension.cs](../../src/backend/TrafficCourts/Staff.Service/Models/Disputes/DisputeListItemExtension.cs#L27)).

<a id="i-20"></a>
### I-20 The current JUSTIN address is chosen using the session's `CURRENT_DATE`

- **Severity:** Low. **Confidence:** Verified (static).
- `ADRS_START_DT <= CURRENT_DATE` depends on the ORDS session's time zone and compares full datetimes ([tco_query_body.sql](../../database/tco/definitions/Packages/tco_query_body.sql#L1040)). `TRUNC(SYSDATE)` would be predictable.

---

## Simplification opportunities

<a id="s-1"></a>
### S-1 Choose one storage convention for event timestamps

- Event timestamps are split between UTC (`SUBMITTED_DT`, `JJ_DECISION_DT`, `VTC_ASSIGNED_DTM`, `EMAIL_SENT_DTM`) and Local (`USER_ASSIGNED_DTM`, `STATUS_UPDATE_DTM`, `REMARKS_MADE_DTM`, `DISPUTANT_NOT_PRESENT_DTM`, all `ENT_DTM`).
- Pick one convention, and record it in DDL column comments.
- Court and ticket wall-clock values (`ISSUED_DT`, `APPEARANCE_DTM` / `APPR_TM`) should stay Local, because JUSTIN owns them.
- If the event columns all became UTC, the I-4 and I-6 type bugs could not happen. If they all became Local, every `UtcToLocalTime` / `LocalToUtcTime` call would go away for Vancouver users, but Edmonton users would still need conversion.

<a id="s-2"></a>
### S-2 Make the Java layer zone-neutral

- Set the Java date-time (de)serializers to `TimeZone.getTimeZone("UTC")`, or move the models to `java.time.LocalDateTime`. Java never uses these instants for business logic, except `isSameDay` and the unassign thresholds, which already run in the database's zone.
- This removes [I-3](#i-3) and makes the "digits pass through unchanged" behaviour explicit.
- Optionally, also accept and drop a trailing `Z` (as `tco_common.parse_datetime` does). This would remove [I-7](#i-7)-style failures, but only if the sender has already converted the value correctly.

<a id="s-3"></a>
### S-3 Stop round-tripping server-owned timestamps through the client

- `SubmittedTs`, `JjDecisionDate`, `VtcAssignedTs`, and `UserAssignedTs` are owned by the server or the database. The client sends them back only because it PUTs whole objects.
- Ignore them on update: Java `BeanUtils.copyProperties` could exclude them, as it already does for `createdTs`. This removes [I-1](#i-1), [I-2](#i-2), and most of [I-14](#i-14).

<a id="s-4"></a>
### S-4 Convert in one place

- Today, conversions happen in `ModelExtensions` (called from about 20 service methods), in two copies of `AddUtcDateRangeFilter`, in a third helper in `JJDisputeService`, and in the browser (`jjDecisionDate`, `revisedDueDate`, `latestPleaUpdateTs`, the amendment date).
- A single response and request mapping layer in Staff.Service, keyed by field category (event instant, court wall-clock, date-only), would make the rules visible and testable.

<a id="s-5"></a>
### S-5 Use `DateOnly` end-to-end for date-only values

- Use `DateOnly` for birthdate, filing date, violation date, due and revised due dates, the amended violation date, and decision made. This includes the Java date-only serializer on the API side.
- This removes the `T00:00:00` and midnight conversions, and the `DatePipe(…,"UTC")` workarounds in the staff-portal.

<a id="s-6"></a>
### S-6 Put UTC markers back on the wire after S-2

- Once Java accepts `Z`, or ignores it consistently, UTC values can travel as `DateTimeOffset` or `Kind=Utc` with `Z`.
- Local wall-clock values would then be the only offset-less values, so the wire format itself would show which convention applies ([I-9](#i-9)).
