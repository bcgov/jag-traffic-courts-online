# Serialization Boundaries

These are the rules each hop applies when it parses, formats, or converts a datetime. The per-datetime traces refer back to these sections by name, for example "[B5 Java](#b5-oracle-data-api-java)".

## B1 Angular (citizen-portal and staff-portal)

| Behaviour | Effect | Evidence |
|---|---|---|
| Generated API models declare every date as `string` | A value received from the API stays exactly as sent unless a component converts it. | [staff dispute.model.ts](../../src/frontend/staff-portal/src/app/api/model/dispute.model.ts#L35), [citizen noticeOfDispute.model.ts](../../src/frontend/citizen-portal/src/app/api/model/noticeOfDispute.model.ts#L116) |
| Angular `DatePipe` given a timestamp **without an offset** | The value is read as **browser-local** wall-clock time and the same digits are displayed. A Vancouver value therefore shows the same digits to an Edmonton user, but it is not converted to Edmonton time. | [CustomDatePipe](../../src/frontend/staff-portal/src/app/shared/pipes/custom-date.pipe.ts#L11) delegates to `DatePipe` |
| `DatePipe` given a date-only `yyyy-MM-dd` string | Angular builds a **local midnight** date. No day shift occurs. | Angular behaviour |
| `DatePipe(..., "UTC")` | The input is read as local time, then printed in UTC. For a local-midnight date in western North America, the calendar day stays the same. | e.g. [jj-count.component.html](../../src/frontend/staff-portal/src/app/components/jj-dispute-info/jj-count/jj-count.component.html#L21) |
| Raw `new Date("yyyy-MM-dd")` | JavaScript reads this as **UTC midnight**, unlike `DatePipe`. | citizen [violation-ticket.service.ts](../../src/frontend/citizen-portal/src/app/services/violation-ticket.service.ts#L480) |
| Raw `new Date("yyyy-MM-ddTHH:mm[:ss]")` | JavaScript reads this as **browser-local** time. | — |
| A JS `Date` object in a request body (`JSON.stringify`) | It becomes **UTC with `Z` and milliseconds**, for example `2026-10-28T21:35:00.000Z`. | Angular `HttpClient` default |
| Generated query-parameter helper given a `Date` | It sends `toISOString().substr(0,10)`, which is the **UTC calendar date**. | [staff api dispute.service.ts](../../src/frontend/staff-portal/src/app/api/api/dispute.service.ts#L106) |
| Staff `X-Timezone` header | Taken from `Intl.DateTimeFormat().resolvedOptions().timeZone`, with `America/Vancouver` as the fallback. Sent on every request. | [timezone.interceptor.ts](../../src/frontend/staff-portal/src/app/core/interceptors/timezone.interceptor.ts#L8) |

## B2 Citizen.Service and Staff.Service HTTP (System.Text.Json)

- Neither service registers a custom `DateTime` converter. Citizen adds `DateOnly`/`TimeOnly` string converters ([Startup.cs](../../src/backend/TrafficCourts/Citizen.Service/Startup.cs#L130)). Staff adds only an enum converter ([Startup.cs](../../src/backend/TrafficCourts/Staff.Service/Startup.cs#L95)).
- **Reading:** an offset-less string becomes `Kind=Unspecified`. A `Z` string becomes `Kind=Utc`. An offset string is converted to the server's local time with `Kind=Local`.
- **Writing:** `Unspecified` has no suffix, `Utc` gets `Z`, and `Local` gets an offset such as `-07:00`. `DateTimeOffset` keeps its offset. `DateOnly` is written as `yyyy-MM-dd`.
- Staff controllers check `X-Timezone` with `TimeZoneInfo.TryFindSystemTimeZoneById` and return HTTP 400 if it is missing or invalid ([StaffControllerBase.cs](../../src/backend/TrafficCourts/Staff.Service/Controllers/StaffControllerBase.cs#L15)).

## B3 MassTransit / RabbitMQ (System.Text.Json)

- Only `DateOnly`, `TimeOnly`, and enum converters are added ([BusConfiguratorExtensions.cs](../../src/backend/TrafficCourts/Messaging/BusConfiguratorExtensions.cs#L140)). `DateTime` is written according to its `Kind`, as in B2. `DateTimeOffset` keeps its offset.

## B4 OracleDataApi .NET client (.NET → Java)

- This is an NSwag client using **Newtonsoft with default settings**. `UpdateJsonSerializerSettings` is never implemented ([OracleDataApiClient.g.cs](../../src/backend/TrafficCourts/TrafficCourts.OracleDataApi/Client/V1/OracleDataApiClient.g.cs#L605)).
- **Writing:** `Unspecified` produces `yyyy-MM-ddTHH:mm:ss` plus fractional seconds if they are not zero. `Utc` gets `Z`. `Local` gets an offset.
- **Reading:** the Java service never sends an offset, so every `DateTime` arrives as `Kind=Unspecified` with the digits unchanged.
- Some properties are `DateOnly?` (`DisputantBirthdate`, `FilingDate`, `DecisionMadeDt`), even though the [.nswag](../../src/backend/TrafficCourts/TrafficCourts.OracleDataApi/Client/V1/oracle-data-api.nswag#L67) file says `DateTime`.
- [OracleDomainModelMappingProfile](../../src/backend/TrafficCourts/TrafficCourts.OracleDataApi/Client/V1/OracleDomainModelMappingProfile.cs#L10) maps these to and from domain `DateTime?` at midnight, `Unspecified`. Year `0001` becomes `null`. No other date mapping changes zone or Kind.

## B5 oracle-data-api (Java)

| Item | Behaviour | Evidence |
|---|---|---|
| JVM zone | `TZ=America/Vancouver` | [Dockerfile](../../src/backend/oracle-data-api/Dockerfile#L72) |
| Date-time **deserializer** (from .NET and from ORDS) | The value must match `^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}$`. A `Z`, an offset, or fractional seconds is **rejected** and returned to the caller as an error. The value is parsed as **America/Vancouver** with `setLenient(false)`. As a result, a wall-clock time inside the spring-forward gap (02:00–02:59 on the second Sunday of March) is **rejected**. | [DateTimeDeserializer.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateTimeDeserializer.java#L32), [lenient](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateTimeDeserializer.java#L39) |
| Date-time **serializer** | Writes `yyyy-MM-dd'T'HH:mm:ss` in America/Vancouver with no offset. A parse followed by a format therefore gives back the **same digits**, even when the digits are really UTC. | [DateTimeSerializer.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateTimeSerializer.java#L19) |
| Date-only serializer / deserializer | `yyyy-MM-dd`, read and written at local midnight | [DateOnlySerializer.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateOnlySerializer.java#L18), [DateOnlyDeserializer.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/DateOnlyDeserializer.java#L19) |
| Field annotations | Every API model date field and every generated ORDS model date field has an explicit date-time or date-only (de)serializer. These override `GlobalDateSerializer` and the generated client's UTC `RFC3339DateFormat`. | [JacksonConfig.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/config/JacksonConfig.java#L21) |
| Mappers (MapStruct) | Dates are copied as `java.util.Date` instants without change. Birthdates go through `SimpleDateFormat("yyyy-MM-dd")` in the default zone. | [DisputeMapper.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/mapper/DisputeMapper.java#L37), [JJDisputeMapper.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/mapper/JJDisputeMapper.java#L58) |
| Date-only vs date-time mismatch | These API fields use the **date-time** serializer, but their ORDS field is **date-only**: `DisputeListItem.violationDate`, `JJDispute.violationDate`, `JJDispute.icbcReceivedDate`, `JJDisputedCount.dueDate` / `revisedDueDate` / `violationDate`, `JJDisputeCourtAppearanceAmendments.violationDateDtm`. .NET receives `yyyy-MM-ddT00:00:00`. | See [tco-jj-dispute-datetimes.md](tco-jj-dispute-datetimes.md) |
| `new Date()` | Sets `DisputeUpdateRequest.statusUpdateTs`, `JJDisputedCount.latestPleaUpdateTs` (when the plea changes), and `JJDisputeRemark.remarksMadeTs`. The serializer writes these as **Local** wall-clock time. | [DisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/DisputeService.java#L518), [JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L153), [JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L519) |
| `BeanUtils.copyProperties` on update | Dispute and JJ dispute updates copy **every** incoming date onto the stored object, except the ones listed. The client's value replaces the stored value. | [DisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/DisputeService.java#L156), [JJDisputeService.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/service/JJDisputeService.java#L142) |
| `DateUtil.formatAsDateTime` | Formats in the default zone (Vancouver). Used to build the `assignedBeforeTs` query strings. | [DateUtil.java](../../src/backend/oracle-data-api/src/main/java/ca/bc/gov/open/jag/tco/oracledataapi/util/DateUtil.java#L27) |

**Important consequence:** the Java layer reads every date-time as Vancouver time. A UTC value therefore becomes the wrong instant inside Java. It still reaches Oracle with the same digits, **unless** it falls in the DST gap, in which case the request fails.

## B6 ORDS and PL/SQL (`occam`, `tco`, `occamords`, `tcoords`)

| Item | Behaviour | Evidence |
|---|---|---|
| Column types | Every business datetime is an Oracle `DATE`, which stores no zone. There are no `TIMESTAMP WITH TIME ZONE` columns. | [occam.ddl](../../database/occam/occam.ddl#L230), [tco.ddl](../../database/tco/tco.ddl#L69) |
| v1 output | `TO_CHAR(col,'YYYY-MM-DD"T"HH24:MI:SS')` or `'YYYY-MM-DD'`, with no zone | e.g. [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L168) |
| v1 input | `TO_DATE(str,'YYYY-MM-DD"T"HH24:MI:SS')` or `'YYYY-MM-DD'`, with no zone. A `Z` causes a parse error. | e.g. [occam_rest_body.sql](../../database/occam/definitions/Packages/occam_rest_body.sql#L1178) |
| v2 output (`occamords/v2`, `tcoords/v2`) | `tco_sql_builder.add_select_to_char` with the same masks | [occam_query_body.sql](../../database/occam/definitions/Packages/occam_query_body.sql#L426) |
| v2 filter input `datetime` | `tco_common.parse_datetime` **strips `Z` with no conversion** and casts to `DATE` | [tco_common_body.sql](../../database/tco/definitions/Packages/tco_common_body.sql#L88) |
| v2 filter input `date` | `tco_common.parse_date`, `YYYY-MM-DD` | [tco_common_body.sql](../../database/tco/definitions/Packages/tco_common_body.sql#L63) |
| `SYSDATE` | Database server time, which is **Local** | e.g. [occam_violation_ticket_body.sql](../../database/occam/definitions/Packages/occam_violation_ticket_body.sql#L1064) |
| `SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)` | **UTC** | [tco_dispute_body.sql](../../database/tco/definitions/Packages/tco_dispute_body.sql#L478), [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L1227) |
| `CURRENT_DATE` | Depends on the **session** zone. Used only to select the current JUSTIN address. | [tco_query_body.sql](../../database/tco/definitions/Packages/tco_query_body.sql#L1040) |
| TCO `PUT updateDispute` (JJ save) | The block that writes **top-level dispute dates** is commented out: `SUBMITTED_DT`, `ICBC_RECEIVED_DT`, `VTC_ASSIGNED_DTM`, `VIOLATION_DT`. These are read-only through this endpoint. Count, appearance, and amendment dates **are** written. | [tco_rest_body.sql](../../database/tco/definitions/Packages/tco_rest_body.sql#L929) |

## B7 OrdsDataService (.NET → ORDS v2 directly)

- Handwritten repositories (`Occam/*`, `Tco/*`, `Justin/*`) use System.Text.Json source-generated metadata and no converters ([OrdsDataServiceClient.cs](../../src/backend/TrafficCourts/TrafficCourts.OrdsDataService/OrdsDataServiceClient.cs#L62)). ORDS sends no offset, so values arrive as `Kind=Unspecified` with the digits unchanged.
- The generated NSwag OCCAM/TCO clients use `DateTimeOffset`. Only the hotfix project uses them.

## B8 Conversion helpers (`TrafficCourts.Core`)

| Helper | Behaviour | Evidence |
|---|---|---|
| `UtcToLocalTime(dt, tz)` | Forces `Kind=Utc` **regardless of the input**, converts to `tz`, and returns `Kind=Unspecified`. | [ClockExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/ClockExtensions.cs#L41) |
| `LocalToUtcTime(dt, tz)` | Forces `Kind=Unspecified`, converts `tz` → UTC, and returns `Kind=Unspecified`, so the result has no `Z`. | [ClockExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/ClockExtensions.cs#L60) |
| `GetCurrentPacificTime` | UTC now converted to Vancouver | [ClockExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/ClockExtensions.cs#L21) |
| `Truncate` | Removes sub-second precision and keeps `Kind`. Needed because Java rejects fractional seconds. | [ClockExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/ClockExtensions.cs#L77) |
| `ModelExtensions` | The **only** fields converted between UTC and the X-Timezone zone: `JJDispute.{JjDecisionDate, SubmittedTs, VtcAssignedTs}`, `JJDisputedCount.LatestPleaUpdateTs`, `Dispute.SubmittedTs`, `EmailHistory.EmailSentTs`, `DisputeListItem.{SubmittedTs, JjDecisionDate}`, `DisputeCaseFileSummary.{SubmittedTs, JjDecisionDate, VtcAssignedTs}`, `OccamDisputeListItemModel.{submittedTs, jjDecisionDate}`, `OccamDisputeWithUpdateRequestListItemModel.{submittedTs, jjDecisionDate}` | [ModelExtensions.cs](../../src/backend/TrafficCourts/TrafficCourts.Core/Domain/Models/ModelExtensions.cs#L22) |

Every other datetime passes through Staff.Service **without conversion**.

## B9 Caches

- The Citizen.Service Redis ticket cache and FusionCache use System.Text.Json defaults. The digits and `Kind` are preserved ([RedisCacheService.cs](../../src/backend/TrafficCourts/Citizen.Service/Services/Impl/RedisCacheService.cs#L46)).
- Staff.Service Redis locks store `DateTimeOffset` with the `"o"` format, which includes the offset ([RedisDisputeLockService.cs](../../src/backend/TrafficCourts/Staff.Service/Services/RedisDisputeLockService.cs#L270)).
