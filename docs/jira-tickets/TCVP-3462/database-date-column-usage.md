# Database Date/Time Column Usage Inventory

Source: `2026-08-21_schema_dump.csv`.

The table and column list below is authoritative for the supplied schema dump. Usage was searched in `dbscripts/trunk` under `TCO-API-OCCAM`, `TCO-API-OCCAMORDS`, `TCO-API-TCO`, and `TCO-API-TCOORDS`. Each usage bullet names the stored procedure/function or package scope and the SQL file. Trigger references are included only where they invoke a stored package routine or assign the field.

## JUSTIN

- `JUSTIN.JUSTIN_AGENCIES`
  - `AGEN_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `AGEN_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_APPEARANCES`
  - `APPR_DT` (DATE, NOT NULL)
    - `appearance_data_insert`, `appearance_result_data_insert` in `justin_tco_interface_body.sql`.
    - `justin_appr_trg.sql` trigger audit reference.
  - `APPR_TM` (DATE, NOT NULL)
    - `get_appearances_cte`, `occam_dispute_update_requests_list_get` in `occam_query_body.sql`.
    - `prTcoDisputeReset`, `appearance_data_insert`, `appearance_result_data_insert` in `justin_tco_interface_body.sql`.
    - `prCreateDispute` in `tco_appearance_data_modify_body.sql`.
    - `get_appearances_cte`, `tco_dispute_get`, `add_where_name_predicates`, `tco_dispute_ticket_counts_get`, `tco_dispute_appearances_get` in `tco_query_body.sql`.
    - `prGetDisputeCountsArray`, `prGetCourtAppearancesArray` in `tco_rest_body.sql`.
    - `TCO_APCC_TRIGGER.sql`, `justin_appc_trg.sql`, and `tcoords_v2_body.sql` package/trigger scope.
  - `APPR_ENT_DT` (DATE, NOT NULL)
    - `justin_appr_trg.sql` trigger audit reference.
  - `APPR_UPD_DT` (DATE, NULLABLE)
    - `justin_appr_trg.sql` trigger audit reference.

- `JUSTIN.JUSTIN_CD_LANGUAGES`
  - `CDLN_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `CDLN_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_CD_PLEAS`
  - `PLEA_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `PLEA_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_CITIES`
  - `CITY_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `CITY_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.
  - `CITY_DEACTIVATED_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_COUNTRIES`
  - `CTRY_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `CTRY_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_PHYSICAL_FILES`
  - `PHYS_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `PHYS_FILE_OPENING_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.
  - `PHYS_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_PROVINCES`
  - `PROV_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `PROV_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_REPORT_OBJECTS`
  - `ENT_DTM` (DATE, NOT NULL)
    - No direct `JUSTIN_REPORT_OBJECTS` stored procedure/function usage found in the available trunk SQL.
  - `UPD_DTM` (DATE, NULLABLE)
    - No direct `JUSTIN_REPORT_OBJECTS` stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_STATUTES`
  - `STAT_EFFECTIVE_DT` (DATE, NOT NULL)
    - `add_columns`, `handle_termination_date_filter` in `tco_justin_query_body.sql`.
    - `declare_justin_statutes` in `tcoords_v2_body.sql`.
  - `STAT_ENT_DT` (DATE, NOT NULL)
    - No stored procedure/function usage found in the available trunk SQL.
  - `STAT_TERMINATION_DT` (DATE, NULLABLE)
    - `add_columns`, `handle_termination_date_filter` in `tco_justin_query_body.sql`.
    - `declare_justin_statutes` in `tcoords_v2_body.sql`.
  - `STAT_UPD_DT` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.JUSTIN_STATUTE_DESIGNATIONS`
  - `ENT_DTM` (DATE, NOT NULL)
    - No direct `JUSTIN_STATUTE_DESIGNATIONS` stored procedure/function usage found in the available trunk SQL.
  - `UPD_DTM` (DATE, NULLABLE)
    - No direct `JUSTIN_STATUTE_DESIGNATIONS` stored procedure/function usage found in the available trunk SQL.

- `JUSTIN.TOAD_PLAN_TABLE`
  - `TIMESTAMP` (DATE, NULLABLE)
    - No stored procedure/function usage found in the available trunk SQL.

## OCCAM

- `OCCAM.OCCAM_AUDIT_LOG_ENTRIES`
  - `ENT_DTM` (DATE, NOT NULL)
    - `pAuditLogEntrySave` in `occam_audit_log.sql` and `occam_audit_log_body.sql`.
    - `occam_audit_log_entries_get`, `occam_audit_log_entries_list_get` in `occam_query_body.sql`.
    - `prAuditLogEntrySearch` in `occam_rest_body.sql`.
    - `OCCAM_ALEN_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pAuditLogEntrySave` in `occam_audit_log.sql` and `occam_audit_log_body.sql`.
    - `occam_audit_log_entries_get`, `occam_audit_log_entries_list_get` in `occam_query_body.sql`.
    - `prAuditLogEntrySearch` in `occam_rest_body.sql`.
    - `OCCAM_ALEN_TRIGGER.sql` invokes `occam_audit.set_fields`.

- `OCCAM.OCCAM_AUDIT_LOG_ENTRY_TYPES`
  - `ENT_DTM` (DATE, NOT NULL)
    - `pAuditLogEntryTypeSave`, `pAuditLogEntryTypeUpdate` in `occam_audit_log.sql` and `occam_audit_log_body.sql`.
    - `OCCAM_ALET_TRIGGER.sql` assigns `:NEW.ENT_DTM := SYSDATE`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pAuditLogEntryTypeSave`, `pAuditLogEntryTypeUpdate` in `occam_audit_log.sql` and `occam_audit_log_body.sql`.
    - `OCCAM_ALET_TRIGGER.sql` assigns `:NEW.UPD_DTM := SYSDATE`.

- `OCCAM.OCCAM_DISPUTES`
  - `ISSUED_DT` (DATE, NULLABLE)
    - `occam_disputes_get`, `occam_dispute_list_get`, `prViolationTicket`, and related OCCAM REST/query procedures.
  - `SUBMITTED_DT` (DATE, NULLABLE)
    - `occam_disputes_get`, `occam_dispute_list_get`, `prViolationTicket`, and related OCCAM REST/query procedures.
  - `DISPUTANT_BIRTH_DT` (DATE, NULLABLE)
    - `occam_disputes_get` and `prViolationTicket` in the OCCAM query/REST bodies.
  - `FILING_DT` (DATE, NULLABLE)
    - `occam_disputes_get` and `prViolationTicket` in the OCCAM query/REST bodies.
  - `USER_ASSIGNED_DTM` (DATE, NULLABLE)
    - `occam_dispute_update`/assignment logic in `occam_query_body.sql` assigns the value with `SYSDATE`.
    - `pDisputeSave`, `pDisputeUpdate` in `occam_violation_ticket.sql` and `occam_violation_ticket_body.sql`.
  - `ENT_DTM` (DATE, NOT NULL)
    - `pDisputeSave`, `pDisputeUpdate` in `occam_violation_ticket.sql` and `occam_violation_ticket_body.sql`.
    - `occam_disputes_get` in `occam_query_body.sql`; `prViolationTicket` in `occam_rest_body.sql`.
    - OCCAM dispute trigger scope invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pDisputeSave`, `pDisputeUpdate` in `occam_violation_ticket.sql` and `occam_violation_ticket_body.sql`.
    - `occam_disputes_get` in `occam_query_body.sql`; `prViolationTicket` in `occam_rest_body.sql`.
    - OCCAM dispute trigger scope invokes `occam_audit.set_fields`.

- `OCCAM.OCCAM_DISPUTE_COUNTS`
  - `ENT_DTM` (DATE, NOT NULL)
    - `pDisputeCountSave`, `pDisputeCountUpdate` in `occam_violation_ticket.sql` and `occam_violation_ticket_body.sql`.
    - `occam_dispute_counts_get` in `occam_query_body.sql`; `prViolationTicketCounts` in `occam_rest_body.sql`.
    - `OCCAM_DICO_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pDisputeCountSave`, `pDisputeCountUpdate` in `occam_violation_ticket.sql` and `occam_violation_ticket_body.sql`.
    - `occam_dispute_counts_get` in `occam_query_body.sql`; `prViolationTicketCounts` in `occam_rest_body.sql`.
    - `OCCAM_DICO_TRIGGER.sql` invokes `occam_audit.set_fields`.

- `OCCAM.OCCAM_DISPUTE_STATUS_TYPES`
  - `ENT_DTM` (DATE, NOT NULL)
    - OCCAM status-type list procedures and `OCCAM_DIST_TRIGGER.sql` audit scope.
  - `UPD_DTM` (DATE, NULLABLE)
    - OCCAM status-type list procedures and `OCCAM_DIST_TRIGGER.sql` audit scope.

- `OCCAM.OCCAM_DISPUTE_UPDATE_REQUESTS`
  - `ENT_DTM` (DATE, NOT NULL)
    - `pDisputeUpdateRequestSave`, `pDisputeUpdateRequestUpdate` in `occam_dispute_update_request.sql` and its body.
    - `occam_dispute_update_requests_get` in `occam_query_body.sql`; `prDisputeUpdateRequest` in `occam_rest_body.sql`.
    - `OCCAM_DURE_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pDisputeUpdateRequestSave`, `pDisputeUpdateRequestUpdate` in `occam_dispute_update_request.sql` and its body.
    - `occam_dispute_update_requests_get` in `occam_query_body.sql`; `prDisputeUpdateRequest` in `occam_rest_body.sql`.
    - `OCCAM_DURE_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `STATUS_UPDATE_DTM` (DATE, NULLABLE)
    - `occam_dispute_update_requests_get` and `prDisputeUpdateRequest` in the OCCAM query/REST bodies.

- `OCCAM.OCCAM_DISPUTE_UPDATE_REQ_TYPES`
  - `ENT_DTM` (DATE, NOT NULL)
    - OCCAM update-request-type save/update package scope.
  - `UPD_DTM` (DATE, NULLABLE)
    - OCCAM update-request-type save/update package scope.

- `OCCAM.OCCAM_DISPUTE_UPDATE_STAT_TYPS`
  - `ENT_DTM` (DATE, NOT NULL)
    - OCCAM update-status-type save/update package scope.
  - `UPD_DTM` (DATE, NULLABLE)
    - OCCAM update-status-type save/update package scope.

- `OCCAM.OCCAM_ERROR_LOGS`
  - `ENT_DTM` (DATE, NOT NULL)
    - `occam_error`/audit error package scope; `OCCAM_ERLO_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `occam_error`/audit error package scope; `OCCAM_ERLO_TRIGGER.sql` invokes `occam_audit.set_fields`.

- `OCCAM.OCCAM_OUTGOING_EMAILS`
  - `EMAIL_SENT_DTM` (DATE, NOT NULL)
    - `occam_outgoing_emails_get` in `occam_query_body.sql`; `prOutgoingEmailSearch` in `occam_rest_body.sql`.
  - `ENT_DTM` (DATE, NOT NULL)
    - `pOutgoingEmailSave` in `occam_audit_log.sql` and `occam_audit_log_body.sql`.
    - `occam_outgoing_emails_get` in `occam_query_body.sql`; `prOutgoingEmailSearch` in `occam_rest_body.sql`.
    - `OCCAM_OUEM_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pOutgoingEmailSave` in `occam_audit_log.sql` and `occam_audit_log_body.sql`.
    - `occam_outgoing_emails_get` in `occam_query_body.sql`; `prOutgoingEmailSearch` in `occam_rest_body.sql`.
    - `OCCAM_OUEM_TRIGGER.sql` invokes `occam_audit.set_fields`.

- `OCCAM.OCCAM_VIOLATION_TICKET_COUNTS`
  - `ENT_DTM` (DATE, NOT NULL)
    - `pViolationTicketCountSave`, `pViolationTicketCountUpdate` in `occam_violation_ticket.sql` and its body.
    - `occam_violation_ticket_counts_get` in `occam_query_body.sql`; `prViolationTicketCounts` in `occam_rest_body.sql`.
    - `OCCAM_VITC_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pViolationTicketCountSave`, `pViolationTicketCountUpdate` in `occam_violation_ticket.sql` and its body.
    - `occam_violation_ticket_counts_get` in `occam_query_body.sql`; `prViolationTicketCounts` in `occam_rest_body.sql`.
    - `OCCAM_VITC_TRIGGER.sql` invokes `occam_audit.set_fields`.

- `OCCAM.OCCAM_VIOLATION_TICKET_UPLOADS`
  - `DISPUTANT_BIRTH_DT` (DATE, NULLABLE)
    - `occam_violation_ticket_uploads_get` in `occam_query_body.sql`; `prViolationTicket` in `occam_rest_body.sql`.
  - `ISSUED_DT` (DATE, NULLABLE)
    - `occam_violation_ticket_uploads_get` in `occam_query_body.sql`; `prViolationTicket` in `occam_rest_body.sql`.
  - `ENT_DTM` (DATE, NOT NULL)
    - `pViolationTicketSave`, `pViolationTicketUpdate` in `occam_violation_ticket.sql` and its body.
    - `occam_violation_ticket_uploads_get` in `occam_query_body.sql`; `prViolationTicket` in `occam_rest_body.sql`.
    - `OCCAM_VITU_TRIGGER.sql` invokes `occam_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pViolationTicketSave`, `pViolationTicketUpdate` in `occam_violation_ticket.sql` and its body.
    - `occam_violation_ticket_uploads_get` in `occam_query_body.sql`; `prViolationTicket` in `occam_rest_body.sql`.
    - `OCCAM_VITU_TRIGGER.sql` invokes `occam_audit.set_fields`.

## TCO

- `TCO.TCO_COURT_APPEARANCES`
  - `APPEARANCE_DTM` (DATE, NOT NULL)
    - `occam_query_body.sql` and `occam_rest_body.sql` select the latest appearance and filter with `SYSDATE - 5`; the 14-day indicator uses `SYSDATE + 14`.
    - `tco_query_body.sql` and `tco_rest_body.sql` return/format appearance dates in `tco_dispute_appearances_get`, `prGetCourtAppearancesArray`, and related routines.
    - `pCourtAppearanceSave`/`pCourtAppearanceUpdate` in `tco_court_appearance.sql` and its body.
    - `TCO_COAP_TRIGGER.sql` invokes `tco_audit.set_fields` for `ENT_DTM`/`UPD_DTM`.
  - `DISPUTANT_NOT_PRESENT_DTM` (DATE, NULLABLE)
    - `tco_query_body.sql` and `tco_rest_body.sql` return the court-appearance field in dispute/appearance routines.
  - `ENT_DTM` (DATE, NOT NULL)
    - `pCourtAppearanceSave` in `tco_court_appearance.sql` and its body.
    - `prGetDisputeCourtApprArray` in `tco_rest_body.sql`.
    - `TCO_COAP_TRIGGER.sql` invokes `tco_audit.set_fields`.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pCourtAppearanceUpdate` in `tco_court_appearance.sql` and its body.
    - `prGetDisputeCourtApprArray` in `tco_rest_body.sql`.
    - `TCO_COAP_TRIGGER.sql` invokes `tco_audit.set_fields`.

- `TCO.TCO_DISPUTES`
  - `VIOLATION_DT` (DATE, NOT NULL)
    - `tco_dispute_get`, dispute list, and REST dispute procedures in `tco_query_body.sql` and `tco_rest_body.sql`.
  - `SUBMITTED_DT` (DATE, NOT NULL)
    - `tco_dispute_get`, dispute list, and REST dispute procedures in `tco_query_body.sql` and `tco_rest_body.sql`.
  - `ICBC_RECEIVED_DT` (DATE, NULLABLE)
    - TCO dispute list and REST procedures return the field; no DST arithmetic found.
  - `VTC_ASSIGNED_DTM` (DATE, NULLABLE)
    - `pDisputeSave`, `pDisputeUpdate`, `pDisputeUpdateSubset` in `tco_dispute.sql` and its body.
    - `TCO_DISPUTE` package body assigns it with `SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)`.
    - TCO dispute query/REST procedures return and format the field.
  - `JJ_DECISION_DT` (DATE, NULLABLE)
    - `tco_dispute_get`, dispute list, and REST procedures return and format the field.
    - `tco_rest_body.sql` assigns it with `SYS_EXTRACT_UTC(CURRENT_TIMESTAMP)`.
  - `ENT_DTM` (DATE, NOT NULL)
    - `pDisputeSave`, `pDisputeUpdate`, `pDisputeUpdateSubset` in `tco_dispute.sql` and its body.
    - `tco_dispute_ticket_get`, `tco_dispute_ticket_counts_get`, `tco_dispute_remarks_get` in `tco_query_body.sql`.
    - `prDisputeGet`, `prGetDisputeValuesArray` in `tco_rest_body.sql`.
    - `TCO_DISP_TRIGGER.sql` invokes `tco_audit.set_fields`; TCO audit `set_fields` assigns `SYSDATE` on insert.
  - `UPD_DTM` (DATE, NULLABLE)
    - `pDisputeSave`, `pDisputeUpdate`, `pDisputeUpdateSubset` in `tco_dispute.sql` and its body.
    - `pDisputeUnassignJj` compares it with `pAssignedBeforeTs` in `tco_dispute_body.sql`.
    - `tco_dispute_ticket_get`, `tco_dispute_ticket_counts_get`, `tco_dispute_remarks_get` in `tco_query_body.sql`.
    - `prDisputeGet`, `prGetDisputeValuesArray` in `tco_rest_body.sql`.
    - `TCO_DISP_TRIGGER.sql` invokes `tco_audit.set_fields`; TCO audit `set_fields` assigns `SYSDATE` on update.

## Usage notes

- `DATE` is Oracle's date-plus-time type; the dump contains no `TIMESTAMP`, `TIMESTAMP WITH TIME ZONE`, or `TIMESTAMP WITH LOCAL TIME ZONE` rows despite `TOAD_PLAN_TABLE.TIMESTAMP` being named as a column.
- `ENT_DTM` and `UPD_DTM` are commonly populated through audit packages/triggers. Their use of `SYSDATE` is local/session-time dependent and is a DST review item.
- `APPR_DT`, `APPR_TM`, `APPEARANCE_DTM`, `STAT_EFFECTIVE_DT`, and `STAT_TERMINATION_DT` participate in date filtering or formatting and require special attention to BC civil-time semantics.
- A name-only search can match an identically named field from another table alias. The usage bullets above are narrowed to the table/package relationships visible in the SQL; review the cited source when making remediation decisions.
