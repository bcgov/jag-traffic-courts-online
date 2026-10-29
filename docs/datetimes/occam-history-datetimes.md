# OCCAM History Datetimes

These datetimes describe events on a dispute: emails, update requests, and file history. The boundary rules (B1–B9) are described in [serialization-boundaries.md](serialization-boundaries.md).

---

## 1. Email sent / filtered timestamp

**Storage:** `OCCAM_OUTGOING_EMAILS.EMAIL_SENT_DTM` (`DATE NOT NULL`). **Convention: UTC.**

### Write

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | Workflow.Service `SendEmailToDisputantConsumer` | local `DateTimeOffset` from `_clock.GetUtcNow()` taken after the SMTP send completes (or after the email is filtered) | — → UTC | [SendEmailToDisputantConsumer.cs](../../src/backend/TrafficCourts/Workflow.Service/Consumers/SendEmailToDisputantConsumer.cs#L74) |
| 2 | RabbitMQ | `DisputantEmailSent.SentAt` / `DisputantEmailFiltered.FilteredAt` (`DateTimeOffset`) | UTC → UTC | The offset `+00:00` is kept (B3). [DisputantEmailSent.cs](../../src/backend/TrafficCourts/Messaging/MessageContracts/DisputantEmailSent.cs#L18) |
| 3 | Workflow.Service mapper | domain `EmailHistory.EmailSentTs` (`DateTime`) | UTC → UTC | `SpecifyKind(src.SentAt.UtcDateTime, Unspecified).Truncate()`. The UTC marker is removed on purpose and the value is truncated to seconds. [Mapper.cs](../../src/backend/TrafficCourts/Workflow.Service/Mappings/Mapper.cs#L13), [Mapper.cs](../../src/backend/TrafficCourts/Workflow.Service/Mappings/Mapper.cs#L31) |
| 4 | OracleDataApi client | `EmailHistory.EmailSentTs` | UTC → UTC | No `Z` (B4) |
| 5 | Java | `EmailHistory.emailSentTs` → ORDS `OutgoingEmail.emailSentDtm` | UTC → UTC (digits) | Java reads it as Vancouver time. The digits are kept, but a value in the DST gap is rejected ([I-3](issues.md#i-3)). [OutgoingEmailMapper.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/mapper/OutgoingEmailMapper.java#L21) |
| 6 | ORDS `occam_rest.prProcessOutgoingEmail` | `EMAIL_SENT_DTM` | UTC → UTC | `TO_DATE(DT-mask)`. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L2351) |

### Read

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | ORDS `outgoingEmailList` | `emailSentDtm` | UTC → UTC | DT mask. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L2043) |
| 2 | Java → .NET | `EmailHistory.EmailSentTs` (`Unspecified`) | UTC → UTC | |
| 3 | Staff.Service `EmailHistoryService` | `EmailHistory.EmailSentTs` | UTC → Local (X-Timezone) | `UtcToLocalTime(tz)`. Correct. [EmailHistoryService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/EmailHistoryService.cs#L24), [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L113) |
| 4 | staff-portal file history | `HistoryRecord.createdTs` ← `EmailHistory.createdTs` | **audit-derived** Local (`ENT_DTM`, `SYSDATE`) | The UI shows **`createdTs`, not the converted `emailSentTs`**. As a result, the conversion in step 3 is not visible to the user ([I-15](issues.md#i-15)). [jj-file-history.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-file-history/jj-file-history.component.ts#L82) |

**Assessment:** storage and the API conversion are correct. The DST-gap risk applies ([I-3](issues.md#i-3)). The UI shows a different column ([I-15](issues.md#i-15)).

---

## 2. Dispute update-request status timestamp

**Storage:** `OCCAM_DISPUTE_UPDATE_REQUESTS.STATUS_UPDATE_DTM`. **Convention: Local.**

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | Workflow.Service / Staff.Service | request to change an update request's status (no timestamp sent) | — | e.g. [DisputeUpdateRequestAcceptedConsumer.cs](../../src/backend/TrafficCourts/Workflow.Service/Consumers/DisputeUpdateRequestAcceptedConsumer.cs#L46) |
| 2 | Java `DisputeService.updateDisputeUpdateRequestStatus` | `DisputeUpdateRequest.statusUpdateTs = new Date()` | — → Local | The serializer writes Vancouver wall-clock time. [DisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/DisputeService.java#L518) |
| 3 | ORDS `prModifyDispUpdateRequest` | `statusUpdateDtm` → `STATUS_UPDATE_DTM` | Local → Local | `TO_DATE(DT-mask)`. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L2635). Not set on create. |
| 4 | Read: ORDS → Java → .NET | `DisputeUpdateRequest.StatusUpdateTs` | Local → Local | **Not converted** by Staff.Service. [DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L515) |
| 5 | staff-portal JJ "updates" panel | `statusTs` | Local → shown as Vancouver digits | [jj-dispute-updates.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute-updates/jj-dispute-updates.component.ts#L157). Edmonton users see Vancouver time ([I-8](issues.md#i-8)). |

**Assessment:** correct. It is Local, which differs from `EMAIL_SENT_DTM` on the same screen (UTC, converted) ([I-8](issues.md#i-8)).

---

## 3. Update-request "submitted" date (oldest pending update request)

**Source:** `MIN(OCCAM_DISPUTE_UPDATE_REQUESTS.ENT_DTM)` — **audit-derived, Local** ([occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L257)).

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | ORDS v2 `occam_dispute_update_requests_list` | `update_request_submitted_dt` | Local → Local | DT mask. [occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L841) |
| 2 | OrdsDataService → Staff.Service | `OccamDisputeWithUpdateRequest.update_request_submitted_dt` → `OccamDisputeWithUpdateRequestListItemModel.updateRequest_OldestDate` | Local → Local | **Not converted.** [Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/Occam/DisputesWithUpdateRequests/Handler.cs#L361) |
| 3 | staff-portal update-request inbox | `DatePipe`, date only | Local → shown as a Vancouver date | [update-request-inbox.component.html](../../src/frontend/staff-portal/src/app/components/staff-workbench/update-request-inbox/update-request-inbox.component.html#L36) |

The same list also returns `jj_decision_dt` and `hearing_dt` as **date-only** values. See [tco-jj-dispute-datetimes.md §2](tco-jj-dispute-datetimes.md#2-jj-decision-date) for the wrong conversion of `jj_decision_dt` ([I-5](issues.md#i-5)).

---

## 4. File history, remark, and update-request creation times (audit-derived, displayed)

These are `ENT_DTM` values, so they are outside the scope of the audit-column exclusion. They are listed here because the UI shows them as business data next to converted fields.

| Displayed value | Source | Zone | Staff.Service | UI |
|---|---|---|---|---|
| File history "when" | OCCAM/TCO `*_AUDIT_LOG_ENTRIES.ENT_DTM` (`SYSDATE`) | Local | `FileHistoryService` checks `X-Timezone` but **does not use it** ([FileHistoryService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/FileHistoryService.cs#L18)) | `dd-MMM-yyyy HH:mm`, Vancouver digits ([jj-file-history.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-file-history/jj-file-history.component.ts#L71)) |
| Email history "when" | `OCCAM_OUTGOING_EMAILS.ENT_DTM` | Local | Not converted | Vancouver digits |
| JJ remark "when" | `TCO_DISPUTE_REMARKS.ENT_DTM` (instead of `REMARKS_MADE_DTM`) | Local | Not converted | Vancouver digits ([jj-dispute-remarks.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute-remarks/jj-dispute-remarks.component.ts#L45)) |
| Update request "requested" | `OCCAM_DISPUTE_UPDATE_REQUESTS.ENT_DTM` | Local | Not converted | Vancouver digits ([jj-dispute-updates.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute-updates/jj-dispute-updates.component.ts#L152)) |
| Printed `FileRemark.When`, `FileHistoryEvent.When` | `CreatedTs` | Local | Not converted | `dd-MMM-yyyy HH:mm` ([PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L477)) |

**Assessment:** these values are consistent with each other (all Local). For an Edmonton user, however, they sit on the same screen as converted Local (X-Timezone) values ([I-8](issues.md#i-8)).
