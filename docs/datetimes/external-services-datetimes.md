# External Service Datetimes

These are the datetimes exchanged with services outside the TCO .NET/Java/Oracle stack. The boundary rules (B1–B9) are described in [serialization-boundaries.md](serialization-boundaries.md).

---

## RSI ticket search

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | citizen-portal / staff-portal search form | time input `HH:mm` (`ngx-timepicker-field`, 24 h) | — → Local (the ticket's printed time) | [find-ticket.component.ts](../../src/frontend/citizen-portal/src/app/components/find-ticket/find-ticket.component.ts#L49) |
| 2 | Citizen.Service `Features/Tickets/Search` | `Request.Hour/Minute` → `TimeOnly` | Local → Local | Time of day only. No zone. [Request.cs](../../src/backend/TrafficCourts/Citizen.Service/Features/Tickets/Search/Request.cs#L9) |
| 2′ | Staff.Service `RoadSafetyTicketSearchController` | `HH:mm` → `TimeOnly` | Local → Local | Does not use `X-Timezone`. [RSITicketSearchController.cs](../../src/backend/TrafficCourts/Staff.Service/Controllers/RSITicketSearchController.cs#L49) |
| 3 | RSI request | `GetTicketParams.IssuedTime` formatted `HHmm` | Local → Local | [RoadSafetyTicketSearchService.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Tickets/Search/Rsi/RoadSafetyTicketSearchService.cs#L72), [GetTicketParams.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Tickets/Search/Rsi/GetTicketParams.cs#L15) |
| 4 | RSI response | `Invoice.ViolationDateTime` (`term_due_date`, `yyyy-MM-ddTHH:mm`) | Local → Local | Assumed to be BC wall-clock time. The RSI contract states no zone. [Invoice.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Tickets/Search/Common/Invoice.cs#L49) |
| 5 | Citizen.Service / TicketSearch | `ViolationTicket.IssuedTs` / `Ticket.Issued` (`DateTime`, `Unspecified`) | Local → Local | `ParseExact(..., AssumeLocal)` then `SpecifyKind(Unspecified)`. The digits are unchanged. [TicketSearchService.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Tickets/Search/TicketSearchService.cs#L105), [TicketSearch/TicketSearchService.cs](../../src/backend/TrafficCourts/TrafficCourts.TicketSearch/TicketSearchService.cs#L105) |
| 6 | staff-portal manual entry | `new Date(ticket.issued)` → local getters for time | Local → Local (browser) | An offset-less string is read in the browser's zone and then read back with local getters, so the digits are kept. [manual-dispute-entry.component.ts](../../src/frontend/staff-portal/src/app/components/staff-workbench/manual-dispute-entry/manual-dispute-entry.component.ts#L977) |

Mock search: shifts the mock date relative to `GetCurrentPacificTime()` (Local) ([MockTicketSearchService.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Tickets/Search/Mock/MockTicketSearchService.cs#L60)).

**Assessment:** correct (Local wall-clock time throughout).

---

## Azure Form Recognizer (OCR)

| Field | Zone | Processing |
|---|---|---|
| `violation_date`, `service_date` | Unknown → Date-only | OCR text is cleaned up and formatted `yyyy-MM-dd`. If one of the two dates is missing, the other is copied into it ([FormRecognizerValidator.cs](../../src/backend/TrafficCourts/Citizen.Service/Validators/FormRecognizerValidator.cs#L215)) |
| `violation_time` | Unknown → Local time of day | Parsed as a `TimeSpan` and formatted `hh:mm` ([Field.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/Field.cs#L108)) |
| OCR JSON artifact in object storage | same strings | Written at submission ([Create.cs](../../src/backend/TrafficCourts/Citizen.Service/Features/Disputes/Create.cs#L240)) |

The issued date/time built from these fields is traced in [occam-dispute-datetimes.md §1b](occam-dispute-datetimes.md#1b-write-citizen-submits-a-scanned-ticket-ocr). The 30-day service-date rule is covered in [filters-and-current-time.md](filters-and-current-time.md#now--today-calculations).

---

## ARC

| # | Stage | Field (model) | Zone in → out | Conversion / notes |
|---|---|---|---|---|
| 1 | Staff.Service approval | `Dispute.IssuedTs` → `DisputeApproved.TicketIssuanceDate` (`DateTime?`) | Local → Local | Not converted. [Mapper.cs](../../src/backend/TrafficCourts/Staff.Service/Mappers/Mapper.cs#L49) |
| 2 | RabbitMQ | same | Local → Local | No offset (B3) |
| 3 | Workflow.Service `DisputeApprovedConsumer` | `TcoDisputeTicket.Ticket_issuance_date` | Local → Local | [DisputeApprovedConsumer.cs](../../src/backend/TrafficCourts/Workflow.Service/Consumers/DisputeApprovedConsumer.cs#L87) |
| 4 | ARC client → ARC service | Newtonsoft (no offset for `Unspecified`) → System.Text.Json `TcoDisputeTicket.TicketIssuanceDate` | Local → Local | [ArcDisputeClient.g.cs](../../src/backend/TrafficCourts/TrafficCourts.Arc.Dispute.Client/ArcDisputeClient.g.cs#L92), [TcoDisputeTicket.cs](../../src/backend/TrafficCourts/TrafficCourts.Arc.Dispute.Service/Models/TcoDisputeTicket.cs#L20) |
| 5 | ARC converter | `ArcFileRecord.EffectiveDate`, `AdnotatedTicket.ServiceDate`, `DisputedTicket.ServiceDate` | Local → Date-only (Local) | Copied as is. [DisputeTicketToArcFileRecordListConverter.cs](../../src/backend/TrafficCourts/TrafficCourts.Arc.Dispute.Service/Mappings/DisputeTicketToArcFileRecordListConverter.cs#L50) |
| 6 | ARC converter | `ArcFileRecord.TransactionDateTime` = `TimeProvider.GetLocalNow()` with seconds precision, +1 s per record | — → Local | [DisputeTicketToArcFileRecordListConverter.cs](../../src/backend/TrafficCourts/TrafficCourts.Arc.Dispute.Service/Mappings/DisputeTicketToArcFileRecordListConverter.cs#L163) |
| 7 | ARC file writer → SFTP | `yyyyMMdd` (dates), `HHmmss` (transaction time) | Local → Local | [ArcFileService.cs](../../src/backend/TrafficCourts/TrafficCourts.Arc.Dispute.Service/Services/ArcFileService.cs#L80) |

**Assessment:** correct. This is only true because `TicketIssuanceDate` stays `Kind=Unspecified`. If it ever became `Kind=Utc` or `Local`, the ARC service would read an offset, convert it to server-local time, and the date could change ([I-12](issues.md#i-12)).

---

## Email (SMTP)

- Email templates contain **no** dynamic dates. "7 business days" and similar wording is fixed text ([VerificationEmailTemplate.cs](../../src/backend/TrafficCourts/Workflow.Service/Services/EmailTemplates/VerificationEmailTemplate.cs#L30)).
- The MIME `Date` header is not set explicitly. MailKit's default is used ([EmailSenderService.cs](../../src/backend/TrafficCourts/Workflow.Service/Services/EmailSenderService.cs#L35)).
- The application's sent and filtered timestamps (UTC) are traced in [occam-history-datetimes.md §1](occam-history-datetimes.md#1-email-sent--filtered-timestamp).

---

## Email-verification saga (PostgreSQL)

| Field | Zone | Path |
|---|---|---|
| `CheckEmailVerificationTokenResponse.CheckedAt` (`DateTimeOffset`) | UTC → UTC | `_clock.GetUtcNow()` in the saga → MassTransit response → Citizen.Service. Citizen.Service drops it and only uses the status ([VerifyEmailAddressSagaStateMachine.cs](../../src/backend/TrafficCourts/Workflow.Service/Sagas/VerifyEmailAddressSagaStateMachine.cs#L140), [DisputesController.cs](../../src/backend/TrafficCourts/Citizen.Service/Controllers/DisputesController.cs#L158)) |
| Saga `VerifiedAt` (`DateTimeOffset?`) | UTC → UTC instant | EF Core / Npgsql `timestamp with time zone` ([VerifyEmailAddressSagaState.cs](../../src/backend/TrafficCourts/Workflow.Service/Sagas/VerifyEmailAddressSagaState.cs#L30), [model snapshot](../../src/backend/TrafficCourts/Workflow.Service/Migrations/VerifyEmailAddressStateMigrations/VerifyEmailAddressStateDbContextModelSnapshot.cs#L56)) |
| `EmailVerificationSuccessful.VerifiedAt` | UTC → (not persisted) | The consumer updates Oracle's verified flag only ([EmailVerificationSuccessfulConsumer.cs](../../src/backend/TrafficCourts/Workflow.Service/Consumers/EmailVerificationSuccessfulConsumer.cs#L54)) |

**Assessment:** correct. These are the only business timestamps carried as offset-aware instants.

---

## CDOGS (staff print to PDF)

Staff.Service formats every printed date into a string **before** sending it to CDOGS. CDOGS does no date handling.
- `FormattedDateOnly`: `dd-MMM-yyyy` ([FormattedDateOnly.cs](../../src/backend/TrafficCourts/Staff.Service/Models/DigitalCaseFiles/Print/FormattedDateOnly.cs#L17))
- `FormattedDateTime`: `HH:mm` or `hh:mm tt` ([FormattedDateTime.cs](../../src/backend/TrafficCourts/Staff.Service/Models/DigitalCaseFiles/Print/FormattedDateTime.cs#L17))
- No culture is set, so the process culture is used.

| Printed field | Source | Zone printed |
|---|---|---|
| `TicketSummary.DateOfBirth` | Root `Dispute.DisputantBirthdate` (hard-coded `2001-01-01` by staff) or JJ birthdate | Date-only |
| `TicketSummary.Issued` | Issued date/time | Local |
| `TicketSummary.Submitted`, `WrittenReasons.SubmissionTs` | Submitted timestamp after `UtcToLocalTime` | Local (X-Timezone) |
| `TicketSummary.IcbcReceived` | ICBC received date | Date-only |
| `OffenseCount.Offense` | **Issued** date (not the violation date) | Local date |
| `OffenseCount.Due/RevisedDue/FinalDue` | Count due dates | Date-only |
| `OffenseCount.LatestPleaUpdate` | Latest plea update after `UtcToLocalTime` | Local (X-Timezone), but wrong for Java-written values ([I-6](issues.md#i-6)) |
| `Appearance.When`, `Appearance.NoApp` | Appearance and no-app times | Local |
| `FileRemark.When`, `FileHistoryEvent.When` | Audit `CreatedTs` | Local |

Evidence: [PrintDigitalCaseFileService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/PrintDigitalCaseFileService.cs#L105). A single printout can mix Local and Local (X-Timezone) values ([I-8](issues.md#i-8)).

---

## COMS (object storage)

- The metadata and tags written by TCO contain **no dates** ([DocumentProperties.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/DocumentProperties.cs#L9)).
- The COMS `CreatedAt` value is only used to pick the newest OCR or image object. It is not converted or displayed ([DisputeService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/DisputeService.cs#L675)).

---

## Redis dispute locks (Staff.Service)

`Lock.CreatedAtUtc` and `Lock.ExpiryTimeUtc` are `DateTimeOffset` values from `GetUtcNow()` (UTC). They are stored with the round-trip `"o"` format. `JJDispute.LockExpiresAtUtc` is returned to the portal with an offset ([Lock.cs](../../src/backend/TrafficCourts/Staff.Service/Models/Lock.cs#L14), [RedisDisputeLockService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/RedisDisputeLockService.cs#L162)). **Correct.**

---

## JUSTIN interfaces (database side)

| Interface | Datetime | Zone | Notes |
|---|---|---|---|
| `justin_tco_interface.ticket_data_insert` → `justin_rest.vt_dispute_post` | `DisputeFiledDate = TO_CHAR(OCCAM_DISPUTES.SUBMITTED_DT,'YYYY-MM-DD')` | **The UTC date, sent as a local filing date** | **Wrong** ([I-10](issues.md#i-10)). [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L335) |
| same | `ServiceDate = TO_CHAR(ISSUED_DT,'YYYY-MM-DD')` | Local date | Correct. [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L336) |
| `justin_tco_interface.appearance_data_insert` | `APPR_DT` / `APPR_TM` from `date||' '||time` | Local | `APPR_DT` keeps the time part. [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L657) |
| Appearance validation | `TRUNC(NVL(MDOC_SWORN_DT, SYSDATE)) > TRUNC(appearance)` | Local vs Local | Correct. [justin_tco_interface_body.sql](../../database/tco/definitions/Packages/justin_tco_interface_body.sql#L710) |
| Reconciliation into TCO | `VIOLATION_DT`, `SUBMITTED_DT`, `ICBC_RECEIVED_DT`, `APPEARANCE_DTM`, `FINE_DUE_DT` | Copied as is | See [tco-jj-dispute-datetimes.md](tco-jj-dispute-datetimes.md) |

---

## Other

- **OAuth / BC Services Card `UserInfo.Birthdate`** (`DateTime?`) is parsed from the user-info JWT and cached, but it is **never forwarded or stored** ([UserInfo.cs](../../src/backend/TrafficCourts/Citizen.Service/Models/OAuth/UserInfo.cs#L41)).
- **OIDC, RSI, and CDOGS token expiry**: UTC `DateTimeOffset`, used only for cache expiry ([TokenRefreshService.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Http/TokenRefreshService.cs#L265)).
- **Hotfix data migration**: converts hard-coded Local bounds to UTC and sends them to ORDS v2 as `yyyy-MM-ddTHH:mm:ssZ`. ORDS strips the `Z` (B6). This is correct because `SUBMITTED_DT` is UTC ([Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix.cs](../../src/backend/TrafficCourts/TrafficCourts.Hotfix.DataMigration/Hotfixes/00001_Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix/Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix.cs#L1014)).
