# TCO / JJ Dispute Datetimes

These datetimes are stored in, or read through, the `tco` schema (`TCO_DISPUTES`, `TCO_DISPUTE_COUNTS`, `TCO_DISPUTE_REMARKS`, `TCO_COURT_APPEARANCES`, `TCO_APPEARANCE_AMENDMENTS`) and the JUSTIN tables it joins. They are mostly used by the JJ Workbench and the Staff decision-validation screens.

Read paths (unless noted):
- **v1:** TCO ORDS `jjDispute` / `jjDisputeList` (`tco_rest`) → Java → OracleDataApi client → Staff.Service `JJDisputeService` → staff-portal.
- **v2:** TCO ORDS v2 `tco_disputes` (`tco_query`) → OrdsDataService → Staff.Service `CourtFiles/Summaries` handler → staff-portal.

Write path: staff-portal → Staff.Service `JJController` / `JJDisputeService.SubmitAdminResolutionAsync` → Java `JJDisputeService.updateJJDispute` → TCO ORDS `PUT updateDispute` (`tco_rest.prModifyDispute`). **That endpoint does not write top-level `TCO_DISPUTES` dates** ([B6](serialization-boundaries.md#b6-ords-and-plsql-occam-tco-occamords-tcoords)).

---

## 1. Submitted timestamp (TCO copy)

`TCO_DISPUTES.SUBMITTED_DT` is a **UTC** copy of `OCCAM_DISPUTES.SUBMITTED_DT`. See [occam-dispute-datetimes.md §2](occam-dispute-datetimes.md#2-dispute-submitted-timestamp).

| Stage | Field | Zone in → out | Notes |
|---|---|---|---|
| v1 read | `JJDispute.SubmittedTs` | UTC → Local (X-Timezone) | `JJDispute.UtcToLocalTime`. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L28) |
| v2 read | `DisputeCaseFileSummary.SubmittedTs` | UTC → Local (X-Timezone) | [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L143) |
| JJ save | `JJDispute.SubmittedTs` | Local (X-Timezone) → UTC | `LocalToUtcTime`. The value is **ignored by ORDS**, but Java must still parse it ([I-3](issues.md#i-3), [I-14](issues.md#i-14)). [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L56) |
| UI | JJ dispute header, DCF and WR inbox tables | Local (X-Timezone) | [jj-dispute.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.html#L170) |

---

## 2. JJ decision date

**Storage:** `TCO_DISPUTES.JJ_DECISION_DT`. **Convention: UTC.** The database sets it to `sys_extract_utc(current_timestamp)` whenever the status changes to `CONF` or `REQH` ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1227)).

### Write

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | staff-portal JJ confirm | `JJDispute.jjDecisionDate` = offset-less string from browser-local `now` getters | — → Local (X-Timezone) | [jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L385) |
| 2 | Staff.Service save | `JJDispute.JjDecisionDate` | Local (X-Timezone) → UTC | `LocalToUtcTime`. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L55) |
| 3 | Java | `JJDispute.jjDecisionDate` → ORDS `jjDecisionDt` | UTC → UTC (digits) | Read as Vancouver time. A value in the DST gap is rejected ([I-3](issues.md#i-3)). |
| 4 | ORDS `PUT updateDispute` | — | — | **Ignored.** The top-level date block is commented out ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L929)). |
| 5 | ORDS `POST disputeStatus` (CONF/REQH) | `JJ_DECISION_DT` | — → UTC | Set by the database. Steps 1–3 have no effect on the stored value ([I-14](issues.md#i-14)). |

### Read

| Consumer | Path and zone | Conversion / notes |
|---|---|---|
| v1 JJ dispute | `to_char(jj_decision_dt, DT-mask)` UTC → `JJDispute.JjDecisionDate` → `UtcToLocalTime` → Local (X-Timezone) | Correct. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L27). Displayed at [jj-dispute.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.html#L765) |
| v2 court-file summaries (decision-validation inbox) | `to_char(jj_decision_dt, DT-mask)` UTC → `DisputeCaseFileSummary.JjDecisionDate` → `UtcToLocalTime` → Local (X-Timezone) | Correct. [tco_query_body.sql](../../database/tco/definitions/Packages/tco_query_body.sql#L798), [dispute-decision-inbox.component.html](../../src/frontend/staff-portal/src/app/components/staff-workbench/dispute-decision-inbox/dispute-decision-inbox.component.html#L34) |
| Legacy `DisputeListItem` | UTC → `UtcToLocalTime` | Correct. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L133) |
| OCCAM v2 update-request list | `to_char(tco_disp.jj_decision_dt,'YYYY-MM-DD')` (**the UTC date only**) → `OccamDisputeWithUpdateRequestListItemModel.jjDecisionDate` → `UtcToLocalTime` → midnight UTC becomes 16:00/17:00 (Vancouver) or 17:00/18:00 (Edmonton) **on the previous day** | **Wrong.** The API returns a date one day early. This is **latent**: the update-request inbox does not currently display the field ([I-5](issues.md#i-5)). [occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L838), [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L164) |
| Decision-date filter | Local (X-Timezone) day → UTC range | Correct. See [filters-and-current-time.md](filters-and-current-time.md#date-range-filters) |

---

## 3. VTC assignment timestamp (TCO)

**Storage:** `TCO_DISPUTES.VTC_ASSIGNED_DTM`. **Convention: UTC** (`SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)`, [tco_dispute_body.sql](../../database/tco/definitions/Packages/tco_dispute_body.sql#L478)).

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | ORDS `POST assignDisputeVtc` | `VTC_ASSIGNED_DTM` | — → UTC | Set by the database |
| 2 | v1 / v2 read | `JJDispute.VtcAssignedTs`, `DisputeCaseFileSummary.VtcAssignedTs` | UTC → Local (X-Timezone) | `UtcToLocalTime`. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L29), [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L145) |
| 3 | JJ save | `JJDispute.VtcAssignedTs` | Local (X-Timezone) → UTC | `LocalToUtcTime`. Ignored by ORDS ([I-14](issues.md#i-14)) |
| 4 | Java scheduled unassign | `hourAgo = new Date() - 1h` → `DateUtil.formatAsDateTime` → `assignedBeforeTs` | — → **Local** | [JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L121), [JJDisputeRepositoryImpl.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/repository/impl/ords/JJDisputeRepositoryImpl.java#L66) |
| 5 | ORDS `tco_dispute.pDisputeUnassignVtc` | `VTC_ASSIGNED_DTM <= pAssignedBeforeTs` | **UTC column vs Local threshold** | **Wrong.** Disputes stay assigned for about 9 h (PST) or 8 h (PDT), not 1 h ([I-4](issues.md#i-4)). [tco_dispute_body.sql](../../database/tco/definitions/Packages/tco_dispute_body.sql#L499) |

---

## 4. Violation (offence) date

**Storage:** `TCO_DISPUTES.VIOLATION_DT` (copy of `MAX(JUSTIN_MATTERS.MATT_OFFENCE_DT)`), **Date-only**. The count-level date comes from TCO. The JJ dispute-level date reads JUSTIN `MATT_OFFENCE_DT` directly ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L171)).

| Stage | Field | Zone in → out | Notes |
|---|---|---|---|
| JUSTIN → TCO reconciliation | `MATT_OFFENCE_DT` → `VIOLATION_DT` | Date-only | Copied as is. [tco_appearance_data_modify_body.sql](../../database/tco/definitions/Packages/tco_appearance_data_modify_body.sql#L115) |
| ORDS | `to_char(...,'YYYY-MM-DD')` | Date-only | |
| Java | ORDS date-only → API `JJDispute.violationDate` / `JJDisputedCount.violationDate` / `DisputeListItem.violationDate` using the **date-time** serializer | Date-only → `yyyy-MM-ddT00:00:00` | [I-13](issues.md#i-13). [JJDispute.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/model/JJDispute.java#L256) |
| Staff.Service | `DateTime?` | Date-only | Not converted |
| staff-portal | `DatePipe "dd-MMM-yyyy"` | Date-only | Correct day. [jj-hearing-table.component.html](../../src/frontend/staff-portal/src/app/components/jj-workbench/jj-hearing-table/jj-hearing-table.component.html#L40). Note: the count panel and the printed `OffenseCount.Offense` show the **issued** date, not this field ([PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L398)). |

---

## 5. ICBC received date

**Storage:** `TCO_DISPUTES.ICBC_RECEIVED_DT`, copied from JUSTIN `JUSTIN_TICKET_INBOX_ENTRIES.TRANSACTION_DTM` (Local datetime) ([tco_appearance_data_modify_body.sql](../../database/tco/definitions/Packages/tco_appearance_data_modify_body.sql#L126)). **Returned as Date-only.**

| Stage | Field | Zone in → out | Notes |
|---|---|---|---|
| ORDS | `to_char(icbc_received_dt,'YYYY-MM-DD')` | Local → Date-only | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L170) |
| Java | `JJDispute.icbcReceivedDate` (date-time serializer) | Date-only → `T00:00:00` | [JJDispute.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/model/JJDispute.java#L265) |
| Staff.Service / UI / print | `DateTime?` → `DatePipe(…,"UTC")` / `dd-MMM-yyyy` | Date-only | Correct. [jj-dispute.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.html#L174) |

---

## 6. Fine due date

**Storage:** `TCO_DISPUTE_COUNTS.FINE_DUE_DT`, **Date-only**. It is calculated as `MATT_OFFENCE_DT + 30` **without `TRUNC`** ([tco_appearance_data_modify_body.sql](../../database/tco/definitions/Packages/tco_appearance_data_modify_body.sql#L375), [I-16](issues.md#i-16)).

Read only: ORDS date-only ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L568)) → Java `JJDisputedCount.dueDate` (date-time serializer) → Staff.Service (not converted) → UI `DatePipe(…,"UTC")` ([jj-count.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.html#L21)) and print `dd-MMM-yyyy`. **Correct.**

---

## 7. Revised due date

**Storage:** `TCO_DISPUTE_COUNTS.REVISED_DUE_DT`, **Date-only.**

| # | Stage | Field | Zone in → out | Notes |
|---|---|---|---|---|
| 1 | staff-portal | `JJDisputedCount.revisedDueDate` = browser-local Y/M/D + `T00:00:00`. "No time to pay" sets **browser-local today** | — → Date-only | [jj-count.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.ts#L326), [jj-count.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.ts#L568) |
| 2 | Staff.Service | `DateTime?` | Date-only | Not converted |
| 3 | Java | API date-time → ORDS date-only (`yyyy-MM-dd`) | Date-only | |
| 4 | ORDS `pDisputeCountUpdateSubset` | `TO_DATE(…,'YYYY-MM-DD')` | Date-only | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1000) |
| Read | as fine due date | Date-only | `DatePipe(…,"UTC")`. [jj-count.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.html#L510) |

**Assessment:** correct. "Today" depends on the browser, so an Edmonton user just after midnight Edmonton time gets a date one day ahead of Vancouver ([filters-and-current-time.md](filters-and-current-time.md#now--today-calculations)).

---

## 8. Latest plea update timestamp

**Storage:** `TCO_DISPUTE_COUNTS.LATEST_PLEA_UPDATE_DTM`. **Convention: Mixed.** Staff.Service treats it as UTC. Java writes Local.

| # | Stage | Field | Zone in → out | Notes |
|---|---|---|---|---|
| 1 | staff-portal | `JJDisputedCount.latestPleaUpdateTs`. On **any** count form change, it is replaced by browser-local midnight of its date (`yyyy-MM-ddT00:00:00`) | Local (X-Timezone) → Local (X-Timezone), time removed | [jj-count.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.ts#L313), [jj-count.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.ts#L576) |
| 2 | Staff.Service save | `JJDisputedCount.LatestPleaUpdateTs` | Local (X-Timezone) → UTC | `LocalToUtcTime`. [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L72) |
| 3 | Java `updateJJDispute` | If the latest plea **changed**, `setLatestPleaUpdateTs(new Date())`, which is **Local**. Otherwise the UTC value from step 2 is kept. | UTC or Local → UTC or Local | [JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L153) |
| 4 | ORDS `pDisputeCountUpdateSubset` | `TO_DATE(DT-mask)` | stored as is | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1004) |
| 5 | Read | `JJDisputedCount.LatestPleaUpdateTs` → `UtcToLocalTime` | assumed UTC → Local (X-Timezone) | **Wrong** for values written by Java: they are shown 7–8 h (Vancouver) or 6–7 h (Edmonton) too early, which can be the previous day ([I-6](issues.md#i-6)). [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L44) |
| 6 | UI / print | `DatePipe "dd-MMM-yyyy"`; print `dd-MMM-yyyy HH:mm` | Local (X-Timezone) | [jj-count.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.html#L183), [PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L428) |

---

## 9. Court appearance date/time

**Storage:** JUSTIN `JUSTIN_APPEARANCES.APPR_TM` (with `APPR_DT` holding the same full datetime) and its copy `TCO_COURT_APPEARANCES.APPEARANCE_DTM`. **Convention: Local** (court wall-clock time).

| # | Stage | Field | Zone in → out | Notes |
|---|---|---|---|---|
| 1 | JUSTIN `justin_tco_interface.appearance_data_insert` | `TO_DATE(date||' '||time,'DD-MON-YYYY HH:MI AM')` → `APPR_DT`, `APPR_TM` | Local → Local | [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L657) |
| 2 | Reconciliation | `APPR_TM` → `TCO_COURT_APPEARANCES.APPEARANCE_DTM` | Local → Local | [tco_appearance_data_modify_body.sql](../../database/tco/definitions/Packages/tco_appearance_data_modify_body.sql#L244) |
| 3 | ORDS v1 / v2 read | `to_char(appr.appr_tm, DT-mask)`, a **live JUSTIN** value | Local → Local | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L713), [tco_query_body.sql](../../database/tco/definitions/Packages/tco_query_body.sql#L857) |
| 4 | Java | `JJDisputeCourtAppearanceRoP.appearanceTs`. `isSameDay(appearanceTs, new Date())` is used for the recall rule | Local → Local | Correct (both Vancouver). [JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L409) |
| 5 | Staff.Service | `JJDisputeCourtAppearanceRoP.AppearanceTs`, `DisputeCaseFileSummary.AppearanceTs` | Local → Local | **Not converted** |
| 6 | staff-portal | `DatePipe` with no zone; the most recent appearance is found by sorting ISO strings | Local → shown as Vancouver digits | Edmonton users see the court's local time with no label ([I-8](issues.md#i-8)). [jj-hearing-table.component.html](../../src/frontend/staff-portal/src/app/components/jj-workbench/jj-hearing-table/jj-hearing-table.component.html#L81) |
| 7 | JJ save | `appearanceTs` sent back unchanged → ORDS `PUT updateDispute` writes `TCO_COURT_APPEARANCES.APPEARANCE_DTM` | Local → Local | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1072) |
| 8 | Print | `Appearance.When` `dd-MMM-yyyy hh:mm tt` | Local | [PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L511) |
| 9 | Hearing-inbox filter | date-only `appr_tm` range | Date-only vs Local | Correct. See [filters-and-current-time.md](filters-and-current-time.md#date-range-filters) |

**Assessment:** correct (Local throughout).

---

## 10. Disputant-not-present (no-app) timestamp

**Storage:** `TCO_COURT_APPEARANCES.DISPUTANT_NOT_PRESENT_DTM`. **Intended convention: Local** (the same table and endpoint as the appearance time).

| # | Stage | Field | Zone in → out | Notes |
|---|---|---|---|---|
| 1 | staff-portal | `courtAppearanceForm.noAppTs` is a **JS `Date`**: `new Date()` ("now"), a picker value, or `new Date(existing)`. It is merged into `JJDisputeCourtAppearanceRoP` | — → **UTC** (`JSON.stringify` → `…Z`, with milliseconds) | [jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L459), [jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L891), [jj-dispute.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-dispute/jj-dispute.component.ts#L600) |
| 2 | Staff.Service | `JJDisputeCourtAppearanceRoP.NoAppTs` (`Kind=Utc`) | UTC → UTC | **Not converted** (it is not in `ModelExtensions`) |
| 3 | OracleDataApi client | Newtonsoft writes `…Z` | UTC → UTC | B4 |
| 4 | Java | `DateTimeDeserializer` regex | **Rejected** | Static analysis shows the JJ save fails whenever `noAppTs` is not null ([I-7](issues.md#i-7)). [DateTimeDeserializer.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateTimeDeserializer.java#L32) |
| 5 | ORDS (if it were reached) | `TO_DATE(DT-mask)` → `DISPUTANT_NOT_PRESENT_DTM` | — | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1076) |
| Read | ORDS DT mask → Java → Staff.Service (not converted) → `new Date(value)` (browser-local) | Local → Local (browser) | Print `Appearance.NoApp` ([PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L515)) |

---

## 11. Decision made date (count)

Derived from JUSTIN `APPR_TM` as `to_char(acc.appr_tm,'YYYY-MM-DD')`, **Date-only (Local)** ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L606)). Java date-only → .NET `DateOnly?` ([OracleDataApiModels.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/OracleDataApiModels.cs#L587)) → Staff JSON `yyyy-MM-dd` → `DatePipe` local midnight ([jj-count.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.html#L923)). **Correct.**

---

## 12. Remark made timestamp

**Storage:** `TCO_DISPUTE_REMARKS.REMARKS_MADE_DTM`. **Convention: Local.**

- Write: staff-portal sends the remark text only → Java `remark.setRemarksMadeTs(new Date())` gives **Local** ([JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L519)) → ORDS `prProcessDisputeRemark` `TO_DATE(DT-mask)` ([tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1490)).
- Read: DT mask → Java → `JJDisputeRemark.RemarksMadeTs`, **not converted**. The UI and print show the **audit `createdTs`** instead ([occam-history-datetimes.md §4](occam-history-datetimes.md#4-file-history-remark-and-update-request-creation-times-audit-derived-displayed)).

**Assessment:** correct (Local). Not visible to the user.

---

## 13. Amended violation date

**Storage:** `TCO_APPEARANCE_AMENDMENTS.VIOLATION_DATE_DTM`, **Date-only** (despite the `_DTM` suffix).

| Stage | Field | Zone in → out | Notes |
|---|---|---|---|
| staff-portal | `JJDisputeCourtAppearanceAmendments.violationDateDtm` is parsed with `new Date`, then rebuilt from **local getters** as `yyyy-MM-ddT00:00:00` | Date-only | [jj-amendments.component.ts](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-amendments/jj-amendments.component.ts#L177) |
| Staff.Service | `DateTime?` | Date-only | Not converted |
| Java | API date-time → ORDS date-only | Date-only | [JJDisputeCourtAppearanceAmendments.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/model/JJDisputeCourtAppearanceAmendments.java#L62) |
| ORDS | `TO_DATE(…,'YYYY-MM-DD')` | Date-only | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1087) |
| UI read | `DatePipe(…,"UTC")` | Date-only | Correct. [staff-amendment-validation.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/staff-amendment-validation/staff-amendment-validation.component.html#L21) |

---

## 14. Hearing date (OCCAM update-request list)

`to_char(appr.appr_tm,'YYYY-MM-DD')`, **Date-only (Local)** ([occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L845)) → `OccamDisputeWithUpdateRequestListItemModel.hearingDate`, not converted ([Handler.cs](../../src/backend/TrafficCourts/Staff.Service/Features/Occam/DisputesWithUpdateRequests/Handler.cs#L360)) → `DatePipe` ([update-request-inbox.component.html](../../src/frontend/staff-portal/src/app/components/staff-workbench/update-request-inbox/update-request-inbox.component.html#L77)). **Correct.** Compare with `jj_decision_dt` in the same query ([I-5](issues.md#i-5)).
