# OCCAM Dispute Datetimes

These datetimes are stored in the `occam` schema (`OCCAM_DISPUTES`, `OCCAM_VIOLATION_TICKET_UPLOADS`), plus the dispute-level dates the OCCAM endpoints read from `tco`. The boundary rules (B1–B9) are described in [serialization-boundaries.md](serialization-boundaries.md).

---

## 1. Ticket issued date/time

**Storage:** `OCCAM_DISPUTES.ISSUED_DT` and `OCCAM_VIOLATION_TICKET_UPLOADS.ISSUED_DT`, as Oracle `DATE`. **Convention: Local** (the wall-clock time printed on the ticket).

### 1a. Write: citizen submits a dispute for a ticket found through RSI lookup

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | RSI ticket search API | `Invoice.ViolationDateTime` (JSON `term_due_date`, `yyyy-MM-ddTHH:mm`) | Local → Local | The external value has no offset. It is assumed to be the BC ticket's printed time. [Invoice.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Tickets/Search/Common/Invoice.cs#L49) |
| 2 | Citizen.Service ticket assembly | `Models.Tickets.ViolationTicket.IssuedTs` (`DateTime`) | Local → Local | `TryParseExact(..., AssumeLocal)` then `SpecifyKind(Unspecified)`. The digits are unchanged. [TicketSearchService.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Tickets/Search/TicketSearchService.cs#L105) |
| 3 | Redis ticket cache | same | Local → Local | STJ, offset-less (B9) |
| 4 | HTTP response to citizen-portal | JSON `issued_date` | Local → Unknown (to the browser) | Sent as e.g. `2026-10-28T14:35:00` with no offset (B2). [TicketsController.cs](../../src/backend/TrafficCourts/Citizen.Service/Controllers/TicketsController.cs#L96) |
| 5 | citizen-portal | `ViolationTicket.issued_date` → `NoticeOfDispute.issued_date` (string) | Unknown → Unknown | Kept as a string. Displayed with `DatePipe` and no zone, so the digits are unchanged (B1). [resolution-header.component.ts](../../src/frontend/citizen-portal/src/app/shared/components/resolution-header/resolution-header.component.ts#L27). The string is spread into the notice and POSTed unchanged. [notice-of-dispute.service.ts](../../src/frontend/citizen-portal/src/app/services/notice-of-dispute.service.ts#L340) |
| 6 | Citizen.Service create | `NoticeOfDispute.IssuedTs` → `SubmitNoticeOfDispute.IssuedTs`; nested `SubmitNoticeOfDispute.ViolationTicket.IssuedTs` is rebuilt from the **cached** ticket | Unknown → Local (`Unspecified`) | AutoMapper copy, no conversion. The top-level and nested values come from different sources. [MappingProfile.cs](../../src/backend/TrafficCourts/Citizen.Service/Mappings/MappingProfile.cs#L11), [Create.cs](../../src/backend/TrafficCourts/Citizen.Service/Features/Disputes/Create.cs#L184) |
| 7 | RabbitMQ | `SubmitNoticeOfDispute.IssuedTs` | Local → Local | No offset (B3) |
| 8 | Workflow.Service | domain `Dispute.IssuedTs`, `ViolationTicket.IssuedTs` | Local → Local | AutoMapper copy. [MappingProfile.cs](../../src/backend/TrafficCourts/Workflow.Service/Mappings/MappingProfile.cs#L10), [SubmitNoticeOfDisputeConsumer.cs](../../src/backend/TrafficCourts/Workflow.Service/Consumers/SubmitNoticeOfDisputeConsumer.cs#L32) |
| 9 | OracleDataApi client | `Dispute.IssuedTs`, `ViolationTicket.IssuedTs` | Local → Local | Newtonsoft, no offset (B4) |
| 10 | Java | `Dispute.issuedTs`, `ViolationTicket.issuedTs` → ORDS `issuedDt` | Local → Local | Parsed as Vancouver, so this is the **correct instant**. Formatted back with the same digits (B5). [Dispute.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/model/Dispute.java#L69). The Java docs call this field UTC, which is wrong (see [I-11](issues.md#i-11)). |
| 11 | ORDS `occam_rest.prProcessViolationTicket` | JSON `issuedDt` → `ISSUED_DT` | Local → Local | `TO_DATE(DT-mask)`. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L1178) |

### 1b. Write: citizen submits a scanned ticket (OCR)

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | Azure Form Recognizer | `OcrViolationTicket.Fields["violation_date"/"violation_time"]` (strings) | Unknown → Unknown | Text is read from the image. [FormRecognizerService_2022_08_31.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Impl/FormRecognizerService_2022_08_31.cs#L163) |
| 2 | Citizen.Service validator | same | Unknown → Date-only + time-of-day | Cleaned up to `yyyy-MM-dd` and `HH:mm`. No zone. [FormRecognizerValidator.cs](../../src/backend/TrafficCourts/Citizen.Service/Validators/FormRecognizerValidator.cs#L204) |
| 3 | citizen-portal | `issued_date` = `` `${violation_date}T${violation_time}` `` | Date-only + time → Local (intended) | String concatenation. [violation-ticket.service.ts](../../src/frontend/citizen-portal/src/app/services/violation-ticket.service.ts#L261) |
| 4–11 | as 1a steps 6–11 | | Local → Local | For OCR disputes, the Oracle wrapper **replaces** the nested ticket with a stub, so `OCCAM_VIOLATION_TICKET_UPLOADS.ISSUED_DT` is not populated from the message. [OracleDataApiService.cs](../../src/backend/TrafficCourts/TrafficCourts.OracleDataApi/OracleDataApiService.cs#L610) |

### 1c. Write: staff manual dispute entry or ticket edit

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | staff-portal manual entry | `Dispute.issuedTs` = `date.toISOString().split('T')[0]` + entered `HH:mm` + `:00` | Local → Local | The ticket's printed time is entered as is. The date comes from the **UTC** calendar date of the picker's local-midnight `Date`. This works for Vancouver and Edmonton, but it is fragile ([I-17](issues.md#i-17)). [manual-dispute-entry.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/manual-dispute-entry/manual-dispute-entry.component.ts#L1388) |
| 1′ | staff-portal ticket edit | `Dispute.issuedTs` and `Dispute.violationTicket.issuedTs` rebuilt as `yyyy-MM-ddTHH:mm` | Local → Local | Built by splitting the string. No `Date` parsing. [ticket-info.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/ticket-info/ticket-info.component.ts#L584) |
| 2 | Staff.Service create / update / validate | domain `Dispute.IssuedTs`, `ViolationTicket.IssuedTs` | Local → Local | **Not** converted. `X-Timezone` only affects `SubmittedTs` (B8). |
| 3–5 | OracleDataApi client → Java → ORDS `prModifyViolationTicket` | | Local → Local | B4, B5, B6. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L1569) |

### 1d. Reads

| Consumer | Path and zone | Conversion / notes |
|---|---|---|
| Staff workbench dispute GET | DB Local → Java Local → .NET `Unspecified` → Staff.Service (not converted) → staff-portal string → `DatePipe` | The value is shown as the **ticket's Vancouver wall-clock time**, including to Edmonton users. Reasonable for a printed ticket time. See [I-8](issues.md#i-8). |
| JJ dispute | TCO `tco_rest` `issuedTs` = `to_char(od.issued_dt, DT-mask)` → Java `JJDispute.issuedTs` → Staff.Service (not converted) | Local → Local. [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L168) |
| Citizen GET dispute (Workflow `GetDisputeConsumer`) | DB → Java → Workflow → `SubmitNoticeOfDispute` response → Citizen.Service → portal | Local → Local, not converted. [GetDisputeConsumer.cs](../../src/backend/TrafficCourts/Workflow.Service/Consumers/GetDisputeConsumer.cs#L33) |
| Dispute search by ticket number and time | See [filters-and-current-time.md](filters-and-current-time.md#ticket-and-dispute-search-by-issued-time) | Local `HH:mm` compared with `TO_CHAR(ISSUED_DT,'HH24:MI')` |
| `DisputeApproved` → ARC | `Dispute.IssuedTs` → `DisputeApproved.TicketIssuanceDate` → ARC `EffectiveDate` / `ServiceDate` (`yyyyMMdd`) | Local → Local date. See [external-services-datetimes.md](external-services-datetimes.md#arc) |
| Staff print (CDOGS) | `TicketSummary.Issued` formatted `dd-MMM-yyyy` + `HH:mm` | Local. [PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L110) |
| Staff ICBC/RSI lookup | `IssuedTs.TimeOfDay` → ticket search `HH:mm` | Local. [DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L601) |
| JUSTIN (`ticket_data_insert`) | `TO_CHAR(ISSUED_DT,'YYYY-MM-DD')` → `ServiceDate` | Local date. [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L336) |

**Assessment:** correct throughout. The value is never converted, and every stage treats the digits as Vancouver wall-clock time. Its zone is never stated on the wire, so callers have to know the convention ([I-12](issues.md#i-12)).

---

## 2. Dispute submitted timestamp

**Storage:** `OCCAM_DISPUTES.SUBMITTED_DT` as Oracle `DATE`. It is copied as is to `TCO_DISPUTES.SUBMITTED_DT` ([tco_appearance_data_modify_body.sql](../../database/tco/definitions/Packages/tco_appearance_data_modify_body.sql#L121)). **Convention: UTC**. Nothing in the column or on the wire says so.

### 2a. Write: citizen submission

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | citizen-portal | (none) | — | Generated on the server |
| 2 | Citizen.Service create | `SubmitNoticeOfDispute.SubmittedTs` (`DateTime?`) | — → UTC | `_clock.GetUtcNow().DateTime.Truncate()`. This gives UTC digits with `Kind=Unspecified`, by design. [Create.cs](../../src/backend/TrafficCourts/Citizen.Service/Features/Disputes/Create.cs#L174) |
| 3 | RabbitMQ | same | UTC → UTC | Written **without `Z`** (B3) |
| 4 | Workflow.Service | domain `Dispute.SubmittedTs` | UTC → UTC | AutoMapper copy |
| 5 | OracleDataApi client | `Dispute.SubmittedTs` | UTC → UTC | No `Z` (B4) |
| 6 | Java | `Dispute.submittedTs` → ORDS `submittedDt` | UTC → UTC (digits) | Java **misreads the value as Vancouver time**. The digits survive the round trip, but a value in the DST gap is **rejected**, so the dispute is never saved ([I-3](issues.md#i-3)). |
| 7 | ORDS `prProcessViolationTicket` | `SUBMITTED_DT` | UTC → UTC | `TO_DATE(DT-mask)`. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L1179) |
| 8 | TCO reconciliation (JUSTIN trigger) | `TCO_DISPUTES.SUBMITTED_DT` | UTC → UTC | Copied as is |

### 2b. Write: staff portal

| Path | Stages and zones | Notes |
|---|---|---|
| **Manual entry (create)** | staff-portal builds an offset-less string from browser-local getters, Local (X-Timezone) → Staff.Service `dispute.LocalToUtcTime(tz)` → UTC → Java → DB | Correct. [manual-dispute-entry.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/manual-dispute-entry/manual-dispute-entry.component.ts#L206), [DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L146) |
| **Ticket validation "save" (update)** | staff-portal sends back the `submittedTs` it got from GET, Local (X-Timezone) → Staff.Service `dispute.UtcToLocalTime(tz)`, which converts in the **wrong direction**, Local - offset → Java `BeanUtils.copyProperties` → ORDS `prModifyViolationTicket` writes `SUBMITTED_DT` | **Wrong.** Every save moves `SUBMITTED_DT` 12–16 hours earlier ([I-1](issues.md#i-1)). [DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L175), [DisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/DisputeService.java#L156) |
| **Ticket validation "validate"** | staff-portal sends Local (X-Timezone) → Staff.Service saves **without** `LocalToUtcTime` → DB | **Wrong.** A local time is stored in a UTC column ([I-2](issues.md#i-2)). [DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L198) |
| JJ dispute save | `JJDispute.SubmittedTs`: Local (X-Timezone) → `LocalToUtcTime` → UTC → Java → TCO `PUT updateDispute` | Not written. Top-level TCO dates are read-only (B6). The value must still get past Java's parser ([I-3](issues.md#i-3), [I-14](issues.md#i-14)). |

### 2c. Reads

| Consumer | Path and zone | Conversion / notes |
|---|---|---|
| Staff dispute GET / create result | DB UTC → Java → .NET `Unspecified` → `Dispute.UtcToLocalTime(tz)` → Local (X-Timezone) → staff-portal `DatePipe` | Correct. [DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L123), [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L87) |
| Staff ticket inbox (OCCAM v2 `occam_dispute_list`) | `to_char(submitted_dt, DT-mask)` UTC → `OccamDispute.submitted_dt` (`Unspecified`) → `OccamDisputeListItemModel.submittedTs` → `UtcToLocalTime(tz)` → Local (X-Timezone) | Correct. [occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L426), [Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/Occam/Disputes/Handler.cs#L338) |
| Staff update-request inbox (OCCAM v2) | same, `OccamDisputeWithUpdateRequestListItemModel.submittedTs` | Correct. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L163) |
| Legacy `DisputeListItem` / `DisputeWithUpdates` | UTC → `UtcToLocalTime(tz)` | Correct. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L132), [DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L405) |
| JJ dispute and court-file summaries (TCO) | TCO UTC → `JJDispute.UtcToLocalTime` / `DisputeCaseFileSummary.UtcToLocalTime` → Local (X-Timezone) | Correct. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L28), [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L143) |
| Submitted-date filters | Local (X-Timezone) day boundaries → UTC | Correct. See [filters-and-current-time.md](filters-and-current-time.md#date-range-filters) |
| Staff print | `TicketSummary.Submitted`, `WrittenReasons.SubmissionTs`, already converted to Local (X-Timezone) | Correct for the user. A printed document's zone depends on who printed it ([I-8](issues.md#i-8)). [PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L114) |
| JUSTIN `ticket_data_insert` | `TO_CHAR(SUBMITTED_DT,'YYYY-MM-DD')` → `DisputeFiledDate` | **Wrong.** The **UTC** date is sent, so disputes filed after 16:00 PST / 17:00 PDT get the next day ([I-10](issues.md#i-10)). [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L335) |
| Citizen GET dispute | Not exposed. The reverse mapping has no destination. | — |

---

## 3. Disputant birthdate

**Storage:** `OCCAM_DISPUTES.DISPUTANT_BIRTH_DT` and `OCCAM_VIOLATION_TICKET_UPLOADS.DISPUTANT_BIRTH_DT` (`DATE`), **Date-only**. JJ disputes read JUSTIN `PROF_BIRTH_DT` instead ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L173)).

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | citizen-portal | `NoticeOfDispute.disputant_birthdate` (string) | — | **No form control or OCR source.** It is never set. [noticeOfDispute.model.ts](../../src/frontend/citizen-portal/src/app/api/model/noticeOfDispute.model.ts#L120) |
| 1′ | staff-portal | `Dispute.disputantBirthdate` | Date-only | **Hard-coded to `"2001-01-01"`** on create, update, and validate. [dispute.service.ts](../../src/frontend/staff-portal/src/app/services/dispute.service.ts#L255) |
| 2 | Citizen.Service | `Dispute.DisputantBirthdate` (`DateTime`) → `SubmitNoticeOfDispute.DisputantBirthdate` (`DateTime`); nested `ViolationTicket.DisputantBirthdate` (`DateOnly`) has no source | Date-only → Date-only | `default(DateTime)` is passed through. [Dispute.cs](../../src/backend/TrafficCourts/Citizen.Service/Models/Disputes/Dispute.cs#L27) |
| 3 | Workflow / Staff → OracleDataApi client | domain `DateTime?` → client `DateOnly?` | Date-only → Date-only | Year, month, and day are extracted. Year `0001` becomes `null`. (B4) |
| 4 | Java | `Dispute.disputantBirthdate` (date-only serializer) → ORDS `disputantBirthDt` **String** via `SimpleDateFormat("yyyy-MM-dd")` | Date-only → Date-only | Formatted at local midnight in the default zone, so no shift. [DisputeMapperImpl](../../src/backend/oracle-data-api/target/generated-sources/annotations/ca/bc/gov/open/jag/tco/oracledataapi/mapper/DisputeMapperImpl.java#L69) |
| 5 | ORDS | `TO_DATE(…,'YYYY-MM-DD')` | Date-only | [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L1184) |
| Read | DB → Java (date-only) → client `DateOnly` → domain `DateTime` midnight → Staff JSON `yyyy-MM-ddT00:00:00` → staff-portal `DatePipe(…,"UTC")` | Date-only | Shows the correct day for Vancouver and Edmonton users. [jj-dispute.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.html#L154). Printed as `dd-MMM-yyyy` ([PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L105)). |

**Assessment:** correct, since no zone conversion is ever applied. The value is effectively unused: `2001-01-01` or null. Its type changes between `DateOnly` and `DateTime` along the way ([I-13](issues.md#i-13)).

---

## 4. Filing date

**Storage:** `OCCAM_DISPUTES.FILING_DT`, **Date-only**.

- No submission path sets it. `SubmitNoticeOfDisputeConsumer` leaves it null.
- It is only passed through unchanged when a full dispute body is updated: domain `DateTime?` ↔ client `DateOnly?` (B4) → Java date-only → `TO_DATE('YYYY-MM-DD')` ([occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L1213)).
- Read in OCCAM v2 lists as `to_char(filing_dt,'YYYY-MM-DD')` ([occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L439)). Not converted.

**Assessment:** correct (date-only throughout).

---

## 5. Staff (VTC) assignment timestamp, OCCAM

**Storage:** `OCCAM_DISPUTES.USER_ASSIGNED_DTM`. **Convention: Local** (`SYSDATE`).

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | DB assign | `USER_ASSIGNED_DTM = SYSDATE` | — → Local | [occam_violation_ticket_body.sql](../../database/occam/definitions/Packages/occam_violation_ticket_body.sql#L1064), [occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L1377) |
| 2 | ORDS → Java → .NET | `Dispute.userAssignedTs` / `DisputeListItem.userAssignedTs` | Local → Local | B5, B4 |
| 3 | Staff.Service | domain `Dispute.UserAssignedTs` | Local → Local | **Not converted**, unlike TCO `VtcAssignedTs` |
| 4 | staff-portal | ticket-inbox tooltip `DatePipe` | Local → shown as Vancouver digits | [ticket-inbox.component.html](../../src/frontend/staff-portal/src/app/components/staff-workbench/ticket-inbox/ticket-inbox.component.html#L152). Edmonton users see Vancouver time ([I-8](issues.md#i-8)). |
| 5 | Dispute update round trip | staff-portal sends it back unchanged → Java copies it → `prModifyViolationTicket` writes it | Local → Local | Stays the same |
| 6 | Auto-unassign (Java scheduled) | `new Date() - 30 min` → `DateUtil.formatAsDateTime` → `assignedBeforeTs` → `USER_ASSIGNED_DTM <= threshold` | Local vs Local | Correct. [DisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/DisputeService.java#L369), [occam_violation_ticket_body.sql](../../database/occam/definitions/Packages/occam_violation_ticket_body.sql#L1083) |

**Assessment:** correct. Note that it uses a different convention from the TCO VTC assignment column, which is UTC ([I-4](issues.md#i-4)).

---

## 6. Dispute-level appearance date/time and "appearance < 14 days"

**Source:** `TCO_COURT_APPEARANCES.APPEARANCE_DTM`, **Local** (court wall-clock time). See [tco-jj-dispute-datetimes.md](tco-jj-dispute-datetimes.md#9-court-appearance-datetime).

- OCCAM v1/v2 ticket lists return `appearance_dtm` with the DT mask and compute `appear_less_than_14` using `SYSDATE`. Both sides are Local, so the result is correct. [occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L670)
- Java `Dispute.appearanceDtm` → .NET `Dispute.AppearanceDtm`: not converted. The citizen portal only gets the boolean `appearance_less_than_14_days`.
- Staff legacy `DisputeWithUpdates.HearingDate` picks the first `AppearanceTs > DateTimeOffset.Now`. The `Unspecified` value is read in the server's zone (Vancouver), so the comparison is correct ([DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L459)).

---

## 7. Violation date on dispute lists

- OCCAM v2 `occam_dispute_list` returns `to_char(tco_disp.violation_dt,'YYYY-MM-DD')`, which is **Date-only** ([occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L449)). It is mapped to `OccamDisputeListItemModel` without conversion.
- Java `DisputeListItem.violationDate` uses the date-time serializer for an ORDS date-only field, so it reaches .NET as `yyyy-MM-ddT00:00:00` ([I-13](issues.md#i-13)).

**Assessment:** correct (date-only, never converted).
