create sequence TCO_DISP_SEQ
/

create sequence TCO_DICO_SEQ
/

create sequence TCO_DIRE_SEQ
/

create sequence TCO_ERLO_SEQ
/

create sequence TCO_COAP_SEQ
/

create sequence TCO_APCC_SEQ
/

create sequence TCO_ALE_SEQ
/

create sequence TCO_JJAT_SEQ
/

create sequence TCO_APAM_SEQ
/

create table TCO_DISPUTE_STATUS_TYPES
(
    DISPUTE_STATUS_TYPE_CD  VARCHAR2(10) not null
        constraint DSTT_PK
            primary key,
    DISPUTE_STATUS_TYPE_DSC VARCHAR2(50) not null
        constraint DSTT_UK
            unique,
    ACTIVE_YN               VARCHAR2(1)  not null
        constraint AVCON_1670886040_ACTIV_000
            check (ACTIVE_YN IN ('Y', 'N')),
    ENT_DTM                 DATE         not null,
    ENT_USER_ID             VARCHAR2(30) not null,
    UPD_DTM                 DATE,
    UPD_USER_ID             VARCHAR2(30)
)
/

create table TCO_DISPUTES
(
    DISPUTE_ID                     NUMBER(15)    not null
        constraint DISP_PK
            primary key,
    DISPUTE_STATUS_TYPE_CD         VARCHAR2(10)  not null
        constraint DISP_DSTT_FK
            references TCO_DISPUTE_STATUS_TYPES,
    TICKET_NUMBER_TXT              VARCHAR2(12)  not null,
    OFFICER_FROM_AGEN_ID           NUMBER(15, 4) not null
        constraint DISP_JUST_AGEN_FK1
            references JUSTIN.JUSTIN_AGENCIES (),
    TO_BE_HEARD_AT_AGEN_ID         NUMBER(15, 4) not null
        constraint DISP_JUST_AGEN_FK2
            references JUSTIN.JUSTIN_AGENCIES (),
    JUSTIN_RCC_ID                  NUMBER(15, 4) not null
        constraint DISP_JUST_RCC_FK
            references JUSTIN.JUSTIN_RCCS (),
    TICKET_INBOX_ENTRY_ID          NUMBER(15)    not null
        constraint DISP_JUST_TINE_FK
            references JUSTIN.JUSTIN_TICKET_INBOX_ENTRIES (),
    OCCAM_VIOLATION_TICKET_UPLD_ID NUMBER(15)    not null,
    OCCAM_DISPUTE_ID               NUMBER(15, 4) not null,
    VIOLATION_DT                   DATE          not null,
    SUBMITTED_DT                   DATE          not null,
    ICBC_RECEIVED_DT               DATE,
    VTC_ASSIGNED_TO                VARCHAR2(30),
    VTC_ASSIGNED_DTM               DATE,
    JJ_ASSIGNED_TO                 VARCHAR2(30),
    JJ_DECISION_DT                 DATE,
    REMARKS_TXT                    VARCHAR2(500),
    ENT_DTM                        DATE          not null,
    ENT_USER_ID                    VARCHAR2(30)  not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30),
    HEARING_TYPE_CD                VARCHAR2(1)   not null
        constraint AVCON_1671748550_HEARI_000
            check (HEARING_TYPE_CD IN ('W', 'C', 'U')),
    MOST_RECENT_DECISION_MADE_BY   VARCHAR2(30)
)
/

create index DISP_JUST_AGEN_FK2_I
    on TCO_DISPUTES (TO_BE_HEARD_AT_AGEN_ID)
/

create index DISP_JUST_TINE_FK_I
    on TCO_DISPUTES (TICKET_INBOX_ENTRY_ID)
/

create index DISP_DSTT_FK_I
    on TCO_DISPUTES (DISPUTE_STATUS_TYPE_CD)
/

create index DISP_JUST_AGEN_FK1_I
    on TCO_DISPUTES (OFFICER_FROM_AGEN_ID)
/

create index DISP_JUST_RCC_FK_I
    on TCO_DISPUTES (JUSTIN_RCC_ID)
/

create table TCO_DISPUTE_COUNTS
(
    DISPUTE_COUNT_ID          NUMBER(15)   not null
        constraint DICO_PK
            primary key,
    DISPUTE_ID                NUMBER(15)   not null
        constraint DICO_DISP_FK
            references TCO_DISPUTES,
    COUNT_NO                  NUMBER(1),
    VIOLATION_TICKET_COUNT_ID NUMBER(15)   not null
        constraint DICO_OCCM_VITC_FK
            references OCCAM.OCCAM_VIOLATION_TICKET_COUNTS (),
    TICKET_INBOX_COUNT_SEQ_NO NUMBER(4)    not null,
    TICKET_INBOX_ENTRY_ID     NUMBER(15)   not null,
    JUSTIN_STAT_ID            NUMBER(8)    not null
        constraint DICO_JUST_STAT_FK
            references JUSTIN.JUSTIN_STATUTES (),
    INITIAL_PLEA_CD           VARCHAR2(3)  not null
        constraint DICO_PLEA_FK1
            references JUSTIN.JUSTIN_CD_PLEAS (),
    FINE_DUE_DT               DATE,
    ADJUSTED_AMT              NUMBER(8, 2),
    INCLUDES_SURCHARGE_YN     VARCHAR2(1)
        constraint AVCON_1670886040_INCLU_000
            check (INCLUDES_SURCHARGE_YN IN ('Y', 'N')),
    REVISED_DUE_DT            DATE,
    TOTAL_FINE_AMT            NUMBER(8, 2),
    COMMENTS_TXT              VARCHAR2(4000),
    SYSTEM_MESSAGE_TXT        VARCHAR2(4000),
    ENT_DTM                   DATE         not null,
    ENT_USER_ID               VARCHAR2(30) not null,
    UPD_DTM                   DATE,
    UPD_USER_ID               VARCHAR2(30),
    LATEST_PLEA_CD            VARCHAR2(3)
        constraint DICO_PLEA_FK2
            references JUSTIN.JUSTIN_CD_PLEAS (),
    LATEST_PLEA_UPDATE_DTM    DATE,
    GRANT_TIME_TO_PAY_YN      VARCHAR2(1)
        constraint AVCON_1777310370_GRANT_000
            check (GRANT_TIME_TO_PAY_YN IN ('Y', 'N')),
    constraint DICO_UK1
        unique (DISPUTE_ID, COUNT_NO),
    constraint DICO_JUST_TINC_FK
        foreign key (TICKET_INBOX_ENTRY_ID, TICKET_INBOX_COUNT_SEQ_NO) references JUSTIN.JUSTIN_TICKET_INBOX_COUNTS ()
)
/

create index DICO_JUST_TINC_FK_I
    on TCO_DISPUTE_COUNTS (TICKET_INBOX_ENTRY_ID, TICKET_INBOX_COUNT_SEQ_NO)
/

create index DICO_DISP_FK_I
    on TCO_DISPUTE_COUNTS (DISPUTE_ID)
/

create index DICO_JUST_STAT_FK_I
    on TCO_DISPUTE_COUNTS (JUSTIN_STAT_ID)
/

create index DICO_OCCM_VITC_FK_I
    on TCO_DISPUTE_COUNTS (VIOLATION_TICKET_COUNT_ID)
/

create index DICO_PLEA_FK1_I
    on TCO_DISPUTE_COUNTS (INITIAL_PLEA_CD)
/

create index DICO_PLEA_FK2_I
    on TCO_DISPUTE_COUNTS (LATEST_PLEA_CD)
/

create table TCO_DISPUTE_REMARKS
(
    DISPUTE_REMARK_ID  NUMBER(15)     not null
        constraint DIRE_PK
            primary key,
    DISPUTE_ID         NUMBER(15)     not null
        constraint DIRE_DISP_FK
            references TCO_DISPUTES,
    DISPUTE_REMARK_TXT VARCHAR2(4000) not null,
    FULL_USER_NAME_TXT VARCHAR2(200)  not null,
    REMARKS_MADE_DTM   DATE,
    ENT_DTM            DATE           not null,
    ENT_USER_ID        VARCHAR2(30)   not null,
    UPD_DTM            DATE,
    UPD_USER_ID        VARCHAR2(30)
)
/

create index DIRE_DISP_FK_I
    on TCO_DISPUTE_REMARKS (DISPUTE_ID)
/

create table TCO_ERROR_LOGS
(
    ERROR_LOG_ID        NUMBER(15)     not null
        constraint ERLO_PK
            primary key,
    ERROR_MESSAGE_NO    NUMBER(10)     not null,
    ERROR_MESSAGE_TXT   VARCHAR2(2000) not null,
    PROGRAM_NM          VARCHAR2(240),
    PROGRAM_VERSION_TXT VARCHAR2(5),
    ERROR_DETAIL_TXT    VARCHAR2(4000),
    ENT_DTM             DATE           not null,
    ENT_USER_ID         VARCHAR2(30)   not null,
    UPD_DTM             DATE,
    UPD_USER_ID         VARCHAR2(30)
)
/

create table TCO_COURT_APPEARANCES
(
    COURT_APPEARANCE_ID         NUMBER(15)    not null
        constraint COAP_PK
            primary key,
    DISPUTE_ID                  NUMBER(15)    not null
        constraint COAP_DISP_FK
            references TCO_DISPUTES,
    APPEARANCE_DTM              DATE          not null,
    COURTROOM_NUMBER_TXT        VARCHAR2(50)  not null,
    APPEARANCE_REASON_TXT       VARCHAR2(100) not null,
    DISPUTANT_PRESENCE_CD       VARCHAR2(3)
        constraint AVCON_1670354677_DISPU_000
            check (DISPUTANT_PRESENCE_CD IN ('P', 'N', 'A')),
    DISPUTANT_NOT_PRESENT_DTM   DATE,
    RECORDING_CLERK_NAME_TXT    VARCHAR2(100),
    DEFENCE_COUNSEL_NAME_TXT    VARCHAR2(100),
    CROWN_PRESENCE_CD           VARCHAR2(1)
        constraint AVCON_1670354677_CROWN_000
            check (CROWN_PRESENCE_CD IN ('P', 'N')),
    SEIZED_YN                   VARCHAR2(1)   not null
        constraint AVCON_1671131254_SEIZE_000
            check (SEIZED_YN IN ('Y', 'N')),
    JUDGE_OR_JJ_NAME_TXT        VARCHAR2(240),
    COMMENTS_TXT                VARCHAR2(4000),
    ENT_DTM                     DATE          not null,
    ENT_USER_ID                 VARCHAR2(30)  not null,
    UPD_DTM                     DATE,
    UPD_USER_ID                 VARCHAR2(30),
    JUSTIN_APPR_ID              NUMBER(15, 4) not null
        constraint COAP_APPR_FK
            references JUSTIN.JUSTIN_APPEARANCES (),
    DEFENCE_COUNSEL_PRESENCE_CD VARCHAR2(1)
        constraint AVCON_1671131254_DEFEN_000
            check (DEFENCE_COUNSEL_PRESENCE_CD IN ('C', 'A', 'N'))
)
/

create index COAP_DISP_FK_I
    on TCO_COURT_APPEARANCES (DISPUTE_ID)
/

create index COAP_APPR_FK_I
    on TCO_COURT_APPEARANCES (JUSTIN_APPR_ID)
/

create table TCO_APPEARANCE_CHARGE_COUNTS
(
    APPEARANCE_CHARGE_COUNT_ID     NUMBER(15)   not null
        constraint APCC_PK
            primary key,
    COURT_APPEARANCE_ID            NUMBER(15)   not null
        constraint APCC_COAP_FK
            references TCO_COURT_APPEARANCES,
    DISPUTE_COUNT_ID               NUMBER(15)   not null
        constraint APCC_DICO_FK
            references TCO_DISPUTE_COUNTS,
    FINDING_RESULT_CD              VARCHAR2(2)
        constraint AVCON_1683305954_FINDI_000
            check (FINDING_RESULT_CD IN ('GU', 'NG', 'GL', 'CA', 'PP')),
    LESSER_CHARGE_DESC_TXT         VARCHAR2(500),
    SUSP_SNTC_PROBATION_DURTN_TXT  VARCHAR2(500),
    SUSP_SNTC_PROBATION_CONDS_TXT  VARCHAR2(500),
    JAIL_DURATION_TXT              VARCHAR2(500),
    JAIL_INTERMITTENT_YN           VARCHAR2(1)  not null
        constraint AVCON_1670886040_JAIL__000
            check (JAIL_INTERMITTENT_YN IN ('Y', 'N')),
    PROBATION_DURATION_TXT         VARCHAR2(500),
    PROBATION_CONDITIONS_TXT       VARCHAR2(1000),
    DRIVING_PROHIB_DURATION_TXT    VARCHAR2(500),
    DRIVING_PROHIB_MVA_SECTION_TXT VARCHAR2(240),
    DISMISSED_YN                   VARCHAR2(1)  not null
        constraint AVCON_1670886040_DISMI_000
            check (DISMISSED_YN IN ('Y', 'N')),
    DISMISSED_FOR_WANT_PROSEC_YN   VARCHAR2(1)  not null
        constraint AVCON_1670886040_DISMI_001
            check (DISMISSED_FOR_WANT_PROSEC_YN IN ('Y', 'N')),
    WITHDRAWN_YN                   VARCHAR2(1)  not null
        constraint AVCON_1670886040_WITHD_000
            check (WITHDRAWN_YN IN ('Y', 'N')),
    ABATEMENT_YN                   VARCHAR2(1)  not null
        constraint AVCON_1670886040_ABATE_000
            check (ABATEMENT_YN IN ('Y', 'N')),
    STAY_OF_PROCEEDINGS_BY_TXT     VARCHAR2(500),
    OTHER_TXT                      VARCHAR2(500),
    REMARKS_TXT                    VARCHAR2(500),
    ENT_DTM                        DATE         not null,
    ENT_USER_ID                    VARCHAR2(30) not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30),
    constraint APCO_UK1
        unique (DISPUTE_COUNT_ID, COURT_APPEARANCE_ID)
)
/

create index APCC_COAP_FK_I
    on TCO_APPEARANCE_CHARGE_COUNTS (COURT_APPEARANCE_ID)
/

create index APCC_DICO_FK_I
    on TCO_APPEARANCE_CHARGE_COUNTS (DISPUTE_COUNT_ID)
/

create table TCO_AUDIT_LOG_ENTRY_TYPES
(
    AUDIT_LOG_ENTRY_TYPE_CD        VARCHAR2(4)   not null
        constraint ALET_PK
            primary key,
    AUDIT_LOG_ENTRY_TYPE_DSC       VARCHAR2(150) not null
        constraint ALET_ALET_UK1
            unique,
    AUDIT_LOG_ENTRY_TYPE_SHORT_DSC VARCHAR2(100) not null
        constraint ALET_ALET_UK2
            unique,
    ACTIVE_USE_YN                  VARCHAR2(1)   not null
        constraint AVCON_1716835411_ACTIV_000
            check (ACTIVE_USE_YN IN ('Y', 'N')),
    COMMENT_ENTRY_ALLOWED_YN       VARCHAR2(1)   not null
        constraint AVCON_1716835411_COMME_000
            check (COMMENT_ENTRY_ALLOWED_YN IN ('Y', 'N')),
    ENT_DTM                        DATE          not null,
    ENT_USER_ID                    VARCHAR2(30)  not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30)
)
/

create table TCO_AUDIT_LOG_ENTRIES
(
    AUDIT_LOG_ENTRY_ID         NUMBER(15)   not null
        constraint ALE_PK
            primary key,
    DISPUTE_ID                 NUMBER(15)   not null
        constraint ALE_DISP_FK
            references TCO_DISPUTES,
    AUDIT_LOG_ENTRY_TYPE_CD    VARCHAR2(4)  not null
        constraint ALE_ALET_FK
            references TCO_AUDIT_LOG_ENTRY_TYPES,
    ACTION_BY_APPLICATION_USER VARCHAR2(240),
    COMMENT_TXT                VARCHAR2(500),
    ENT_DTM                    DATE         not null,
    ENT_USER_ID                VARCHAR2(30) not null,
    UPD_DTM                    DATE,
    UPD_USER_ID                VARCHAR2(30)
)
/

create index ALE_DISP_FK_I
    on TCO_AUDIT_LOG_ENTRIES (DISPUTE_ID)
/

create index ALE_ALET_FK_I
    on TCO_AUDIT_LOG_ENTRIES (AUDIT_LOG_ENTRY_TYPE_CD)
/

create table TCO_JJ_TEAMS
(
    JJ_TEAM_CD  VARCHAR2(4)             not null
        constraint JJTE_PK
            primary key,
    JJ_TEAM_NM  VARCHAR2(50)            not null
        constraint JJTE_UK
            unique,
    JJ_TEAM_DSC VARCHAR2(80)            not null,
    ACTIVE_YN   VARCHAR2(1) default 'Y' not null
        constraint AVCON_1735055388_ACTIV_001
            check (ACTIVE_YN IN ('Y', 'N')),
    ENT_DTM     DATE                    not null,
    ENT_USER_ID VARCHAR2(30)            not null,
    UPD_DTM     DATE,
    UPD_USER_ID VARCHAR2(30)
)
/

create table TCO_JJ_TEAM_AGENCY_XREF
(
    TEAM_AGENCY_ID NUMBER(15)              not null
        constraint JJTA_PK
            primary key,
    JJ_TEAM_CD     VARCHAR2(4)             not null
        constraint JJTA_JJTE_FK
            references TCO_JJ_TEAMS,
    AGEN_ID        NUMBER(15)              not null
        constraint JJTA_AGEN_FK
            references JUSTIN.JUSTIN_AGENCIES (),
    ACTIVE_YN      VARCHAR2(1) default 'Y' not null
        constraint AVCON_1735055388_ACTIV_000
            check (ACTIVE_YN IN ('Y', 'N')),
    ENT_DTM        DATE                    not null,
    ENT_USER_ID    VARCHAR2(30)            not null,
    UPD_DTM        DATE,
    UPD_USER_ID    VARCHAR2(30)
)
/

create index JJTA_AGEN_FK_I
    on TCO_JJ_TEAM_AGENCY_XREF (AGEN_ID)
/

create index JJTA_JJTE_FK_I
    on TCO_JJ_TEAM_AGENCY_XREF (JJ_TEAM_CD)
/

alter table TCO_JJ_TEAM_AGENCY_XREF
    add constraint JJTA_UK
        unique (AGEN_ID)
/

create table TCO_APPEARANCE_AMENDMENTS
(
    APPEARANCE_AMENDMENT_ID   NUMBER(15)   not null
        constraint APAM_PK
            primary key,
    COURT_APPEARANCE_ID       NUMBER(15)   not null
        constraint APAM_COAP_FK
            references TCO_COURT_APPEARANCES,
    DISPUTANT_SURNAME_NM      VARCHAR2(30),
    DISPUTANT_GIVEN_NAMES_NM  VARCHAR2(100),
    VIOLATION_DATE_DTM        DATE,
    OTHER_NOTES_TXT           VARCHAR2(500),
    COUNT_1_ACT_SECT_DESC_TXT VARCHAR2(500),
    COUNT_1_OTHER_TXT         VARCHAR2(500),
    COUNT_2_ACT_SECT_DESC_TXT VARCHAR2(500),
    COUNT_2_OTHER_TXT         VARCHAR2(500),
    COUNT_3_ACT_SECT_DESC_TXT VARCHAR2(500),
    COUNT_3_OTHER_TXT         VARCHAR2(500),
    ENT_DTM                   DATE         not null,
    ENT_USER_ID               VARCHAR2(30) not null,
    UPD_DTM                   DATE,
    UPD_USER_ID               VARCHAR2(30)
)
/

create index APAM_COAP_FK_I
    on TCO_APPEARANCE_AMENDMENTS (COURT_APPEARANCE_ID)
/

create PACKAGE     tco_dispute IS

  PROCEDURE pDisputeSave(pDisputeId                  OUT NUMBER,
                         pDisputeStatusTypeCd        IN tco_disputes.dispute_status_type_cd%TYPE,
                         pTicketNumberTxt            IN tco_disputes.ticket_number_txt%TYPE,
                         pOfficerFromAgenId          IN tco_disputes.officer_from_agen_id%TYPE,
                         pToBeHeardAtAgenId          IN tco_disputes.to_be_heard_at_agen_id%TYPE,
                         pJustinRccId                IN tco_disputes.justin_rcc_id%TYPE,
                         pTicketInboxEntryId         IN tco_disputes.ticket_inbox_entry_id%TYPE,
                         pOccamViolationTicketId     IN tco_disputes.occam_violation_ticket_upld_id%TYPE,
                         pOccamDisputeId             IN tco_disputes.occam_dispute_id%TYPE,
                         pViolationDt                IN tco_disputes.violation_dt%TYPE,
                         pSubmittedDt                IN tco_disputes.submitted_dt%TYPE,
                         pIcbcRecievedDt             IN tco_disputes.icbc_received_dt%TYPE,
                         pVtcAssignedTo              IN tco_disputes.vtc_assigned_to%TYPE,
                         pVtcAssignedDtm             IN tco_disputes.vtc_assigned_dtm%TYPE,
                         pJjAssignedTo               IN tco_disputes.jj_assigned_to%TYPE,
                         pJjDecisionDt               IN tco_disputes.jj_decision_dt%TYPE,
                         pRemarksTxt                 IN tco_disputes.remarks_txt%TYPE,
                         pHearingTypeCd              IN tco_disputes.hearing_type_cd%TYPE,
                         pEntDtm                     IN tco_disputes.ent_dtm%TYPE,
                         pEntUserId                  IN tco_disputes.ent_user_id%TYPE,
                         pUpdDtm                     IN tco_disputes.upd_dtm%TYPE,
                         pUpdUserId                  IN tco_disputes.upd_user_id%TYPE);


  PROCEDURE pDisputeUpdate(pDisputeId                  IN tco_disputes.dispute_id%TYPE,
                           pDisputeStatusTypeCd        IN tco_disputes.dispute_status_type_cd%TYPE,
                           pTicketNumberTxt            IN tco_disputes.ticket_number_txt%TYPE,
                           pOfficerFromAgenId          IN tco_disputes.officer_from_agen_id%TYPE,
                           pToBeHeardAtAgenId          IN tco_disputes.to_be_heard_at_agen_id%TYPE,
                           pJustinRccId                IN tco_disputes.justin_rcc_id%TYPE,
                           pTicketInboxEntryId         IN tco_disputes.ticket_inbox_entry_id%TYPE,
                           pOccamViolationTicketId     IN tco_disputes.occam_violation_ticket_upld_id%TYPE,
                           pOccamDisputeId             IN tco_disputes.occam_dispute_id%TYPE,
                           pViolationDt                IN tco_disputes.violation_dt%TYPE,
                           pSubmittedDt                IN tco_disputes.submitted_dt%TYPE,
                           pIcbcRecievedDt             IN tco_disputes.icbc_received_dt%TYPE,
                           pVtcAssignedTo              IN tco_disputes.vtc_assigned_to%TYPE,
                           pVtcAssignedDtm             IN tco_disputes.vtc_assigned_dtm%TYPE,
                           pJjAssignedTo               IN tco_disputes.jj_assigned_to%TYPE,
                           pJjDecisionDt               IN tco_disputes.jj_decision_dt%TYPE,
                           pRemarksTxt                 IN tco_disputes.remarks_txt%TYPE,
                           pHearingTypeCd              IN tco_disputes.hearing_type_cd%TYPE,
                           pEntDtm                     IN tco_disputes.ent_dtm%TYPE,
                           pEntUserId                  IN tco_disputes.ent_user_id%TYPE,
                           pUpdDtm                     IN tco_disputes.upd_dtm%TYPE,
                           pUpdUserId                  IN tco_disputes.upd_user_id%TYPE);


  PROCEDURE pDisputeUpdateSubset(pDisputeId                  IN tco_disputes.dispute_id%TYPE,
                                 pDisputeStatusTypeCd        IN tco_disputes.dispute_status_type_cd%TYPE,
                                 pViolationDt                IN tco_disputes.violation_dt%TYPE,
                                 pSubmittedDt                IN tco_disputes.submitted_dt%TYPE,
                                 pIcbcRecievedDt             IN tco_disputes.icbc_received_dt%TYPE,
                                 pVtcAssignedTo              IN tco_disputes.vtc_assigned_to%TYPE,
                                 pVtcAssignedDtm             IN tco_disputes.vtc_assigned_dtm%TYPE,
                                 pJjAssignedTo               IN tco_disputes.jj_assigned_to%TYPE,
                                 pJjDecisionDt               IN tco_disputes.jj_decision_dt%TYPE,
                                 pRemarksTxt                 IN tco_disputes.remarks_txt%TYPE,
                                 pEntDtm                     IN tco_disputes.ent_dtm%TYPE,
                                 pEntUserId                  IN tco_disputes.ent_user_id%TYPE,
                                 pUpdDtm                     IN tco_disputes.upd_dtm%TYPE,
                                 pUpdUserId                  IN tco_disputes.upd_user_id%TYPE);

  PROCEDURE pDisputeCountSave(pDisputeCountId           OUT NUMBER,
                              pDisputeId                IN tco_dispute_counts.dispute_id%TYPE,
                              pCountNo                  IN tco_dispute_counts.count_no%TYPE,
                              pViolationTicketCountId   IN tco_dispute_counts.violation_ticket_count_id%TYPE,
                              pTicketInboxCountSeqNo    IN tco_dispute_counts.ticket_inbox_count_seq_no%TYPE,
                              pTicketInboxEntryId       IN tco_dispute_counts.ticket_inbox_entry_id%TYPE,
                              pJustinStatId             IN tco_dispute_counts.justin_stat_id%TYPE,
                              pInitialPleaCd            IN tco_dispute_counts.initial_plea_cd%TYPE,
                              pFineDueDt                IN tco_dispute_counts.fine_due_dt%TYPE,
                              pAdjustedAmt              IN tco_dispute_counts.adjusted_amt%TYPE,
                              pIncludesSurchargeYn      IN tco_dispute_counts.includes_surcharge_yn%TYPE,
                              pRevisedDueDt             IN tco_dispute_counts.revised_due_dt%TYPE,
                              pTotalFineAmt             IN tco_dispute_counts.total_fine_amt%TYPE,
                              pCommentsTxt              IN tco_dispute_counts.comments_txt%TYPE,
                              pSystemMessageTxt         IN tco_dispute_counts.system_message_txt%TYPE,
                              pEntDtm                   IN tco_dispute_counts.ent_dtm%TYPE,
                              pEntUserId                IN tco_dispute_counts.ent_user_id%TYPE,
                              pUpdDtm                   IN tco_dispute_counts.upd_dtm%TYPE,
                              pUpdUserId                IN tco_dispute_counts.upd_user_id%TYPE);


  PROCEDURE pDisputeCountUpdate(pDisputeCountId           IN tco_dispute_counts.dispute_count_id%TYPE,
                                pDisputeId                IN tco_dispute_counts.dispute_id%TYPE,
                                pCountNo                  IN tco_dispute_counts.count_no%TYPE,
                                pViolationTicketCountId   IN tco_dispute_counts.violation_ticket_count_id%TYPE,
                                pTicketInboxCountSeqNo    IN tco_dispute_counts.ticket_inbox_count_seq_no%TYPE,
                                pTicketInboxEntryId       IN tco_dispute_counts.ticket_inbox_entry_id%TYPE,
                                pJustinStatId             IN tco_dispute_counts.justin_stat_id%TYPE,
                                pInitialPleaCd            IN tco_dispute_counts.initial_plea_cd%TYPE,
                                pFineDueDt                IN tco_dispute_counts.fine_due_dt%TYPE,
                                pAdjustedAmt              IN tco_dispute_counts.adjusted_amt%TYPE,
                                pIncludesSurchargeYn      IN tco_dispute_counts.includes_surcharge_yn%TYPE,
                                pRevisedDueDt             IN tco_dispute_counts.revised_due_dt%TYPE,
                                pTotalFineAmt             IN tco_dispute_counts.total_fine_amt%TYPE,
                                pCommentsTxt              IN tco_dispute_counts.comments_txt%TYPE,
                                pSystemMessageTxt         IN tco_dispute_counts.system_message_txt%TYPE,
                                pEntDtm                   IN tco_dispute_counts.ent_dtm%TYPE,
                                pEntUserId                IN tco_dispute_counts.ent_user_id%TYPE,
                                pUpdDtm                   IN tco_dispute_counts.upd_dtm%TYPE,
                                pUpdUserId                IN tco_dispute_counts.upd_user_id%TYPE);

  PROCEDURE pDisputeCountUpdateSubset(pDisputeCountId           IN tco_dispute_counts.dispute_count_id%TYPE,
                                      pAdjustedAmt              IN tco_dispute_counts.adjusted_amt%TYPE,
                                      pIncludesSurchargeYn      IN tco_dispute_counts.includes_surcharge_yn%TYPE,
                                      pGrantTimeToPayYn         IN tco_dispute_counts.grant_time_to_pay_yn%TYPE,
                                      pRevisedDueDt             IN tco_dispute_counts.revised_due_dt%TYPE,
                                      pTotalFineAmt             IN tco_dispute_counts.total_fine_amt%TYPE,
                                      pCommentsTxt              IN tco_dispute_counts.comments_txt%TYPE,
									                    pLatestPleaCd             IN tco_dispute_counts.latest_plea_cd%TYPE,
									                    pLatestPleaUpdateDtm      IN tco_dispute_counts.latest_plea_update_dtm%TYPE,
                                      pEntDtm                   IN tco_dispute_counts.ent_dtm%TYPE,
                                      pEntUserId                IN tco_dispute_counts.ent_user_id%TYPE,
                                      pUpdDtm                   IN tco_dispute_counts.upd_dtm%TYPE,
                                      pUpdUserId                IN tco_dispute_counts.upd_user_id%TYPE);


  PROCEDURE pDisputeRemarkSave(pDisputeRemarkId          OUT NUMBER,
                               pDisputeId                IN tco_dispute_remarks.dispute_id%TYPE,
                               pDisputeRemarkTxt         IN tco_dispute_remarks.dispute_remark_txt%TYPE,
                               pFullUserNameTxt          IN tco_dispute_remarks.full_user_name_txt%TYPE,
                               pRemarksMadeDtm           IN tco_dispute_remarks.remarks_made_dtm%TYPE,
                               pEntDtm                   IN tco_dispute_remarks.ent_dtm%TYPE,
                               pEntUserId                IN tco_dispute_remarks.ent_user_id%TYPE,
                               pUpdDtm                   IN tco_dispute_remarks.upd_dtm%TYPE,
                               pUpdUserId                IN tco_dispute_remarks.upd_user_id%TYPE);


  PROCEDURE pDisputeRemarkUpdate(pDisputeRemarkId         IN tco_dispute_remarks.dispute_remark_id%TYPE,
                                 pDisputeId               IN tco_dispute_remarks.dispute_id%TYPE,
                                 pDisputeRemarkTxt        IN tco_dispute_remarks.dispute_remark_txt%TYPE,
                                 pFullUserNameTxt         IN tco_dispute_remarks.full_user_name_txt%TYPE,
                                 pRemarksMadeDtm          IN tco_dispute_remarks.remarks_made_dtm%TYPE,
                                 pEntDtm                  IN tco_dispute_remarks.ent_dtm%TYPE,
                                 pEntUserId               IN tco_dispute_remarks.ent_user_id%TYPE,
                                 pUpdDtm                  IN tco_dispute_remarks.upd_dtm%TYPE,
                                 pUpdUserId               IN tco_dispute_remarks.upd_user_id%TYPE);


  PROCEDURE pDisputeDelete(pDisputeId   IN NUMBER
                         , pViolationTicketNumber IN VARCHAR2);


  PROCEDURE pDisputeUpdateStatus(pDisputeId           IN NUMBER
                               , pStatusCd            IN VARCHAR2
                               , pUserId              IN VARCHAR2);


  PROCEDURE pDisputeAssignVtc(pDisputeId           IN NUMBER
                            , pTicketNumber        IN VARCHAR2
                            , pUserId              IN VARCHAR2);


  PROCEDURE pDisputeUnassignVtc(pAssignedBeforeTs IN DATE
                              , pTicketNumber     IN VARCHAR2);


  PROCEDURE pDisputeAssignJj(pDisputeId           IN NUMBER
                           , pTicketNumber        IN VARCHAR2
                           , pUserId              IN VARCHAR2);

  PROCEDURE pDisputeUnassignJj(pAssignedBeforeTs IN DATE);


END;
/

create PACKAGE     tco_appearance_data_modify IS

  PROCEDURE prCreateDispute(pApprId                     IN justin_appearances.appr_id%TYPE
                          , pJustinRccId                IN tco_disputes.justin_rcc_id%TYPE
                          , pOccamViolationTicketId     IN tco_disputes.occam_violation_ticket_upld_id%TYPE
                          , pViolationTicketNumber      IN tco_disputes.ticket_number_txt%TYPE
                          , pHearingReasonCd            IN VARCHAR2 DEFAULT NULL);

END; 
/

create PACKAGE     tco_rest IS

  FUNCTION isNoticeOfHearingDocExistsYn(p_rcc_id IN NUMBER) return varchar2;
  FUNCTION isTicketImageExistsYn(p_rcc_id IN NUMBER) return varchar2;
  FUNCTION getAdjudicatorName(p_appr_id IN NUMBER) return varchar2;
  FUNCTION isMultipleOfficersYn(p_rcc_id IN NUMBER) return varchar2;

  PROCEDURE ping;

  PROCEDURE prDisputeGet(pViolationTicketNumber IN VARCHAR2);

  PROCEDURE prGetDisputeValuesArray(pJustinRccId                IN NUMBER
                                  , pOccamViolationTicketId     IN NUMBER
                                  , pViolationTicketNumber      IN VARCHAR2
                                  , pJjAssignedTo               IN VARCHAR2
                                  , pVtcAssignedTo              IN VARCHAR2);

  PROCEDURE prGetDisputeCountsArray(piDisputeId tco_dispute_counts.dispute_id%TYPE);

  PROCEDURE prGetDisputeRemarksArray(piDisputeId tco_dispute_counts.dispute_id%TYPE);

  PROCEDURE prModifyDispute(p_data IN CLOB);

  PROCEDURE prDeleteDispute(p_dispute_id   IN NUMBER
                          , p_violation_ticket_number IN VARCHAR2);

  PROCEDURE prDisputeUpdateStatus(p_dispute_id              IN NUMBER
                                , p_court_appearance_id     IN NUMBER
                                , p_status_cd               IN VARCHAR2
                                , p_seized_yn               IN VARCHAR2
                                , p_adjudicator_part_id     IN VARCHAR2
                                , p_aatt_cd                 IN VARCHAR2 -- A/N/P
                                , p_datt_cd                 IN VARCHAR2 -- A/C/D/N/S
                                , p_staff_part_id           IN VARCHAR2
                                , p_user_id                 IN VARCHAR2);

  PROCEDURE prDisputeAssignVtc(p_dispute_id              IN NUMBER
                             , p_ticket_number           IN VARCHAR2
                             , p_user_id                 IN VARCHAR2);

  PROCEDURE prDisputeUnassignVtc(p_assigned_before_ts IN VARCHAR2
                               , p_ticket_number      IN VARCHAR2);

  PROCEDURE pDisputeAssignJj(p_dispute_id           IN NUMBER
                           , p_ticket_number           IN VARCHAR2
                           , p_user_id              IN VARCHAR2);

  PROCEDURE prDisputeUnassignJj(p_assigned_before_ts IN VARCHAR2);

  PROCEDURE prGetCourtAppearancesArray(piDisputeId tco_court_appearances.dispute_id%TYPE);

  PROCEDURE prProcessDisputeRemark(p_data IN CLOB);

  PROCEDURE prGetDisputeCourtApprArray(p_dispute_id  IN NUMBER);
  PROCEDURE prGetDisputeApprChrgCountArray(p_dispute_id  IN NUMBER);

  PROCEDURE prModifyCourtAppearance(p_data IN CLOB);

  PROCEDURE prModifyAppearanceChargeCount(p_data IN CLOB);

  PROCEDURE prImageListGet(p_data IN clob);

  PROCEDURE prAuditLogEntrySearch(piTicketNumber IN VARCHAR2);
  PROCEDURE prProcessAuditLogEntry(p_data IN CLOB);
END;
/

create package tco_error is

  type typerecResult is record                                   -- Error record
    ( bStatus         boolean                                    -- The Result Status (True=Pass; False Fail)
    , vPgmnam         TCO_ERROR_LOGS.PROGRAM_NM%type             -- The Name of the Program Unit
    , vPgmVer         TCO_ERROR_LOGS.PROGRAM_VERSION_TXT%type    -- Version Format is '00000'
    , nSqlCode        TCO_ERROR_LOGS.ERROR_MESSAGE_NO%type       -- Oracle Error Code Variable sqlcode
    , vSqlErrm        TCO_ERROR_LOGS.ERROR_MESSAGE_TXT%type      -- Oracle Error Msg Variable sqlerm
    , vMsgTxt         TCO_ERROR_LOGS.ERROR_DETAIL_TXT%type);     -- Any additional debug information  


  PROCEDURE prDebugger (pnNo IN NUMBER
                       ,pvMessage IN VARCHAR2
                       ,pvPgmNam IN VARCHAR2 DEFAULT NULL
                       ,pvPgmVer IN VARCHAR2 DEFAULT NULL);

  PROCEDURE prErrsInsert (prResult IN typerecResult);

  PROCEDURE prExceptionHandle( pvPgmNam     VARCHAR2
                             , pvPgmVer     VARCHAR2
                             , pvMsgTxt     CLOB DEFAULT NULL
                             , pnAppErrCode PLS_INTEGER DEFAULT NULL
                             ) ;

  PROCEDURE prExceptionHandleNoFail( pvPgmNam     VARCHAR2
                                   , pvPgmVer     VARCHAR2
                                   , pvMsgTxt     CLOB DEFAULT NULL
                                   , pnAppErrCode PLS_INTEGER DEFAULT NULL
                                   ) ;

end tco_error; 
/

create PACKAGE     tco_court_appearance IS

  PROCEDURE pCourtAppearanceSave(pCourtAppearanceId         OUT NUMBER,
                                 pDisputeId                 IN tco_court_appearances.dispute_id%TYPE,
                                 pAppearanceDtm             IN tco_court_appearances.appearance_dtm%TYPE,
                                 pCourtroomNumberTxt        IN tco_court_appearances.courtroom_number_txt%TYPE,
                                 pAppearanceReasonTxt       IN tco_court_appearances.appearance_reason_txt%TYPE,
                                 pDisputantPresenceCd       IN tco_court_appearances.disputant_presence_cd%TYPE,
                                 pDisputantNotPresentDtm    IN tco_court_appearances.disputant_not_present_dtm%TYPE,
                                 pRecordingClerkNameTxt     IN tco_court_appearances.recording_clerk_name_txt%TYPE,
                                 pDefenceCounselNameTxt     IN tco_court_appearances.defence_counsel_name_txt%TYPE,
                                 pCrownPresenceCd           IN tco_court_appearances.crown_presence_cd%TYPE,
                                 pSeizedYn                  IN tco_court_appearances.seized_yn%TYPE,
                                 pJudgeOrJjNameTxt          IN tco_court_appearances.judge_or_jj_name_txt%TYPE,
                                 pCommentsTxt               IN tco_court_appearances.comments_txt%TYPE,
                                 pJustinApprId              IN tco_court_appearances.justin_appr_id%TYPE,
                                 pDefenceCounselPresenceCd  IN tco_court_appearances.defence_counsel_presence_cd%TYPE,
                                 pEntDtm                    IN tco_court_appearances.ent_dtm%TYPE,
                                 pEntUserId                 IN tco_court_appearances.ent_user_id%TYPE,
                                 pUpdDtm                    IN tco_court_appearances.upd_dtm%TYPE,
                                 pUpdUserId                 IN tco_court_appearances.upd_user_id%TYPE);


  PROCEDURE pCourtAppearanceUpdate(pCourtAppearanceId         IN tco_court_appearances.court_appearance_id%TYPE,
                                   pAppearanceDtm             IN tco_court_appearances.appearance_dtm%TYPE,
                                   pCourtroomNumberTxt        IN tco_court_appearances.courtroom_number_txt%TYPE,
                                   pAppearanceReasonTxt       IN tco_court_appearances.appearance_reason_txt%TYPE,
                                   pDisputantPresenceCd       IN tco_court_appearances.disputant_presence_cd%TYPE,
                                   pDisputantNotPresentDtm    IN tco_court_appearances.disputant_not_present_dtm%TYPE,
                                   pRecordingClerkNameTxt     IN tco_court_appearances.recording_clerk_name_txt%TYPE,
                                   pDefenceCounselNameTxt     IN tco_court_appearances.defence_counsel_name_txt%TYPE,
                                   pCrownPresenceCd           IN tco_court_appearances.crown_presence_cd%TYPE,
                                   pSeizedYn                  IN tco_court_appearances.seized_yn%TYPE,
                                   pJudgeOrJjNameTxt          IN tco_court_appearances.judge_or_jj_name_txt%TYPE,
                                   pDefenceCounselPresenceCd  IN tco_court_appearances.defence_counsel_presence_cd%TYPE,
                                   pCommentsTxt               IN tco_court_appearances.comments_txt%TYPE);

  PROCEDURE pCourtAppearanceRoomTmUpdate(pJustinApprId         IN justin_appearances.appr_id%TYPE,
                                         pAppearanceDtm        IN tco_court_appearances.appearance_dtm%TYPE,
                                         pCourtroomNumberTxt   IN tco_court_appearances.courtroom_number_txt%TYPE); 

  PROCEDURE pAppearanceChargeCountSave(pAppearanceChargeCountId     OUT NUMBER,
                                       pCourtAppearanceId           IN tco_appearance_charge_counts.court_appearance_id%TYPE,
                                       pDisputeCountId              IN tco_appearance_charge_counts.dispute_count_id%TYPE,
                                       pFindingResultCd             IN tco_appearance_charge_counts.finding_result_cd%TYPE,
                                       pLesserChargeDescTxt         IN tco_appearance_charge_counts.lesser_charge_desc_txt%TYPE,
                                       pSuspSntcProbationDurtnTxt   IN tco_appearance_charge_counts.susp_sntc_probation_durtn_txt%TYPE,
                                       pSuspSntcProbationCondsTxt   IN tco_appearance_charge_counts.susp_sntc_probation_conds_txt%TYPE,
                                       pJailDurationTxt             IN tco_appearance_charge_counts.jail_duration_txt%TYPE,
                                       pJailIntermittentYn          IN tco_appearance_charge_counts.jail_intermittent_yn%TYPE,
                                       pProbationDurationTxt        IN tco_appearance_charge_counts.probation_duration_txt%TYPE,
                                       pProbationConditionsTxt      IN tco_appearance_charge_counts.probation_conditions_txt%TYPE,
                                       pDrivingProhibDurationTxt    IN tco_appearance_charge_counts.driving_prohib_duration_txt%TYPE,
                                       pDrivingProhibMvaSectionTxt  IN tco_appearance_charge_counts.driving_prohib_mva_section_txt%TYPE,
                                       pDismissedYn                 IN tco_appearance_charge_counts.dismissed_yn%TYPE,
                                       pDismissedForWantProsecYn    IN tco_appearance_charge_counts.dismissed_for_want_prosec_yn%TYPE,
                                       pWithdrawnYn                 IN tco_appearance_charge_counts.withdrawn_yn%TYPE,
                                       pAbatementYn                 IN tco_appearance_charge_counts.abatement_yn%TYPE,
                                       pStayOfProceedingsByTxt      IN tco_appearance_charge_counts.stay_of_proceedings_by_txt%TYPE,
                                       pOtherTxt                    IN tco_appearance_charge_counts.other_txt%TYPE,
                                       pRemarksTxt                  IN tco_appearance_charge_counts.remarks_txt%TYPE,
                                       pEntDtm                      IN tco_appearance_charge_counts.ent_dtm%TYPE,
                                       pEntUserId                   IN tco_appearance_charge_counts.ent_user_id%TYPE,
                                       pUpdDtm                      IN tco_appearance_charge_counts.upd_dtm%TYPE,
                                       pUpdUserId                   IN tco_appearance_charge_counts.upd_user_id%TYPE);


  PROCEDURE pAppearanceChargeCountUpdate(pAppearanceChargeCountId     IN tco_appearance_charge_counts.appearance_charge_count_id%TYPE,
                                         pCourtAppearanceId           IN tco_appearance_charge_counts.court_appearance_id%TYPE,
                                         pFindingResultCd             IN tco_appearance_charge_counts.finding_result_cd%TYPE,
                                         pLesserChargeDescTxt         IN tco_appearance_charge_counts.lesser_charge_desc_txt%TYPE,
                                         pSuspSntcProbationDurtnTxt   IN tco_appearance_charge_counts.susp_sntc_probation_durtn_txt%TYPE,
                                         pSuspSntcProbationCondsTxt   IN tco_appearance_charge_counts.susp_sntc_probation_conds_txt%TYPE,
                                         pJailDurationTxt             IN tco_appearance_charge_counts.jail_duration_txt%TYPE,
                                         pJailIntermittentYn          IN tco_appearance_charge_counts.jail_intermittent_yn%TYPE,
                                         pProbationDurationTxt        IN tco_appearance_charge_counts.probation_duration_txt%TYPE,
                                         pProbationConditionsTxt      IN tco_appearance_charge_counts.probation_conditions_txt%TYPE,
                                         pDrivingProhibDurationTxt    IN tco_appearance_charge_counts.driving_prohib_duration_txt%TYPE,
                                         pDrivingProhibMvaSectionTxt  IN tco_appearance_charge_counts.driving_prohib_mva_section_txt%TYPE,
                                         pDismissedYn                 IN tco_appearance_charge_counts.dismissed_yn%TYPE,
                                         pDismissedForWantProsecYn    IN tco_appearance_charge_counts.dismissed_for_want_prosec_yn%TYPE,
                                         pWithdrawnYn                 IN tco_appearance_charge_counts.withdrawn_yn%TYPE,
                                         pAbatementYn                 IN tco_appearance_charge_counts.abatement_yn%TYPE,
                                         pStayOfProceedingsByTxt      IN tco_appearance_charge_counts.stay_of_proceedings_by_txt%TYPE,
                                         pOtherTxt                    IN tco_appearance_charge_counts.other_txt%TYPE,
                                         pRemarksTxt                  IN tco_appearance_charge_counts.remarks_txt%TYPE,
                                         pEntDtm                      IN tco_appearance_charge_counts.ent_dtm%TYPE,
                                         pEntUserId                   IN tco_appearance_charge_counts.ent_user_id%TYPE,
                                         pUpdDtm                      IN tco_appearance_charge_counts.upd_dtm%TYPE,
                                         pUpdUserId                   IN tco_appearance_charge_counts.upd_user_id%TYPE);

  PROCEDURE pCourtAppearanceDelete(pJustinApprId   IN     tco_court_appearances.justin_appr_id%TYPE);

END;
/

create PACKAGE     tco_audit_log IS


  PROCEDURE pAuditLogEntrySave(pAuditLogEntryId         OUT tco_audit_log_entries.audit_log_entry_id%TYPE,
                               pDisputeId               IN tco_audit_log_entries.dispute_id%TYPE,
                               pAuditLogEntryTypeCd     IN tco_audit_log_entries.audit_log_entry_type_cd%TYPE,
                               pActionByApplicationUser IN tco_audit_log_entries.action_by_application_user%TYPE,
                               pComment                 IN tco_audit_log_entries.comment_txt%TYPE,
                               pEntDtm                  IN tco_audit_log_entries.ent_dtm%TYPE,
                               pEntUserId               IN tco_audit_log_entries.ent_user_id%TYPE,
                               pUpdDtm                  IN tco_audit_log_entries.upd_dtm%TYPE,
                               pUpdUserId               IN tco_audit_log_entries.upd_user_id%TYPE);


END; 
/

create package tco_sql_builder
as

  type dictionary_t is table of varchar2(32767) index by varchar2(32767);

  type t_query_type is record (
    f_formatted   boolean,
    f_cte         dictionary_t default dictionary_t(),
    f_select      varchar2(32767),
    f_distinct    boolean,
    f_from        varchar2(32767),
    f_join        varchar2(32767),
    f_where       varchar2(32767),
    f_group_by    varchar2(32767),
    f_order_by    varchar2(32767),
    f_offset_rows boolean,
    f_fetch_rows  boolean
  );

  procedure set_formatted (io_query in out t_query_type,
                           in_formatted  in boolean default true,
                           in_indent     in integer default 1);

  procedure add_cte (io_query     in out t_query_type,
                     in_cte_query in t_query_type,
                     in_name      in varchar2);

  procedure add_select (io_query in out t_query_type,
                       in_name    in varchar2,
                       in_alias   in varchar2 default null);


  procedure add_select_to_char (io_query in out t_query_type,
                              in_name   in varchar2,
                              in_format in varchar2,
                              in_alias  in varchar2);

  procedure set_distinct (io_query in out t_query_type);

  procedure set_from (io_query in out t_query_type,
                      in_name in varchar2);

  procedure add_join (io_query in out t_query_type,
                       in_name    in varchar2);

  procedure add_where (io_query in out t_query_type,
                       in_name in varchar2);

  procedure add_group_by(io_query in out t_query_type,
                       in_name in varchar2);

  procedure add_order_by(io_query in out t_query_type,
                       in_name in varchar2);

  procedure set_offset_rows (io_query in out t_query_type);
  procedure set_fetch_rows (io_query in out t_query_type);

  function get_sql (in_query in t_query_type,
                    in_select_count in boolean := false,
                    in_include_where in boolean := true,
                    in_include_group_by in boolean := true,
                    in_include_order_by in boolean := true,
                    in_include_paging in boolean := true) return varchar2;

end tco_sql_builder;
/

create package tco_bind_variables
as

    g_max_pl_varchar2_def          varchar2(32767);
    subtype t_max_pl_varchar2      is g_max_pl_varchar2_def%type;

    g_max_pl_nvarchar2_def          nvarchar2(32767);
    subtype t_max_pl_nvarchar2      is g_max_pl_nvarchar2_def%type;

    g_bind_variable_name_def          varchar2(30);
    subtype t_bind_variable_name_type is g_bind_variable_name_def%type;

    -- t_bind_variable holds the variable name and the value
    -- only one of the f_value fields will be written to, the rest will be null
    type r_bind_variable_type is record (
      f_name           t_bind_variable_name_type,
      --  1 = varchar
      --  2 = nvarchar
      --  3 = number, 
      --  4 = date, 
      -- 11 - table of varchar
      -- 12 - table of nvarchar
      -- 13 - table of number
      -- 14 - table of date
      f_type               integer,
      f_value_varchar      t_max_pl_varchar2,
      f_value_nvarchar     t_max_pl_nvarchar2,
      f_value_number       number,
      f_value_date         date,
      f_value_varchar_list dbms_sql.varchar2_table,
      f_value_number_list  dbms_sql.number_table
    );

    type t_bind_variables_type is table of r_bind_variable_type index by pls_integer;

    co_type_date      constant varchar(4 char) := 'date';
    co_type_number    constant varchar(6 char) := 'number';

    function add_bind_variable(
      io_bind_variables in out t_bind_variables_type, 
      in_value              in varchar2,
      in_type               in varchar2 default null,
      in_format             in varchar2 default null) return t_bind_variable_name_type;

    function add_nvarchar2_bind_variable(
      io_bind_variables in out t_bind_variables_type, 
      in_value          in nvarchar2) return t_bind_variable_name_type;

    function add_bind_variable(
      io_bind_variables in out t_bind_variables_type, 
      in_value          in number) return t_bind_variable_name_type;

    function add_bind_variable(
      io_bind_variables in out t_bind_variables_type, 
      in_value          in date) return t_bind_variable_name_type;

    -- not working, getting error: cannot access rows from a non-nested table item
    function add_bind_variable(
      io_bind_variables in out t_bind_variables_type, 
      in_value              in dbms_sql.varchar2_table) return t_bind_variable_name_type;

    -- not working, getting error: cannot access rows from a non-nested table item
    function add_bind_variable(
      io_bind_variables in out t_bind_variables_type, 
      in_value              in dbms_sql.number_table) return t_bind_variable_name_type;

    procedure bind_variables(
      in_cursor_id      in number,
      in_bind_variables in t_bind_variables_type,
      in_include_paging in boolean := true);

end tco_bind_variables;
/

create package tco_common
as

co_default_separator       constant varchar2(1) := ',';

subtype t_max_pl_varchar2 is varchar2(32767);
type t_varchar2_table_type is table of t_max_pl_varchar2 index by pls_integer; 

subtype t_object_name_type is varchar(128);
subtype t_table_name_type is t_object_name_type;
subtype t_column_name_type is t_object_name_type;

/* list of columns */
type t_column_name_table_type is table of t_column_name_type index by pls_integer; 

type t_order_by_type is record (
  column     t_column_name_type,
  descending boolean default false
);
type t_order_by_table_type is table of t_order_by_type index by pls_integer; 

type t_paging_type is record (
  offset_rows number default 0,   -- Description: the offset rows used to fetch the page
  fetch_rows number default 25    -- Description: the fetch rows used to fetch the page
);

type t_current_page_type is record (
  offset_rows number,   -- Description: the offset rows used to fetch the page
  fetch_rows  number,   -- Description: the fetch rows used to fetch the page
  total_rows  number    -- Description: the total number rows
);

type t_predicate_type is record (
  column   varchar2(128),   -- the name of column known to the specific query
  operator varchar2(30),    -- normal sql operators, validated
  value    varchar2(32767)  -- the value as a string, single value, list
);

type t_predicates_type is table of t_predicate_type index by pls_integer; 

subtype dictionary_key_t is varchar2(32767);
subtype dictionary_value_t is varchar2(32767);
type dictionary_t is table of dictionary_value_t index by dictionary_value_t;

function to_predicates(in_parameters in tco_common.dictionary_t)
  return tco_common.t_predicates_type;

function assert_simple_sql_name(in_name varchar2) return varchar2;

-- -----------------------------------------------------------------------------
-- parse_date - parses a date value with format 'YYYY-MM-DD'
-- -----------------------------------------------------------------------------
function parse_date(in_value varchar2, in_parameter_name in varchar) return date;

-- -----------------------------------------------------------------------------
-- parse_datetime - parses a datetime value with format 'YYYY-MM-DD"T"HH24:MI:SS'
-- -----------------------------------------------------------------------------
function parse_datetime(in_value varchar2, in_parameter_name in varchar) return date;

function parse_number(in_value varchar2, in_parameter_name in varchar) return number;

procedure on_exception(out_errors out sys_refcursor);

procedure create_refcursor(
  l_query          tco_sql_builder.t_query_type,              -- the query to open
  l_bind_variables tco_bind_variables.t_bind_variables_type,  -- the bind variables
  in_query_name    in varchar2,                               -- the name of query, used when tco_debug.enabled is true
  out_data         out sys_refcursor                          -- the opened sys_refcursor
);

procedure add_where_predicate( 
  in_query              in out tco_sql_builder.t_query_type,             -- the query to add the where clause to
  in_bind_variables     in out tco_bind_variables.t_bind_variables_type, -- the bind variable collection
  in_predicate          in t_predicate_type,                             -- the where predicate
  in_column_types       in dictionary_t,                                 -- the mapping to data type for non-varchar2 columns
  in_column_source      in dictionary_t,                                 -- the optional source (table.column) of the column
  in_column_alt_source  in dictionary_t default dictionary_t());         -- the optional alternate column source when doing where (ie upper column for name)

procedure add_where_predicates(
  in_query              in out tco_sql_builder.t_query_type,             -- the query to add the where clause to
  in_bind_variables     in out tco_bind_variables.t_bind_variables_type, -- the bind variable collection
  in_predicates         in t_predicates_type,                            -- the where predicates
  in_column_types       in dictionary_t default dictionary_t(),          -- the mapping to data type for non-varchar2 columns
  in_column_source      in dictionary_t default dictionary_t(),          -- the optional source (table.column) of the column
  in_column_alt_source  in dictionary_t default dictionary_t());         -- the optional alternate column source when doing where (ie upper column for name)

function add_order_by(
  in_query          in out tco_sql_builder.t_query_type,                 -- the query to add the order by clause to
  in_order_by       in tco_common.t_order_by_table_type,                 -- the order by expressions to add
  in_column_source  in dictionary_t default dictionary_t()               -- the optional source (table.column) of the column
) return boolean;

function select_count_query(
  l_query          tco_sql_builder.t_query_type,              -- the query to open
  l_bind_variables tco_bind_variables.t_bind_variables_type,  -- the bind variables
  in_query_name    in varchar2                                -- the name of query, used when tco_debug.enabled is true
) return number;

function contains(
  in_columns in t_column_name_table_type,
  in_column in t_column_name_type
) return boolean;

function parse_order_by(
  in_order_by t_max_pl_varchar2
) return t_order_by_table_type;

function parse_columns_list(
  in_column_list t_max_pl_varchar2
) return t_column_name_table_type;

function parse_number_table(
  in_value_list in t_max_pl_varchar2,
  in_name       in varchar2
) return dbms_sql.number_table;

function parse_varchar2_table(
  in_value_list in t_max_pl_varchar2
) return dbms_sql.varchar2_table;

function csv_to_table(
  in_csv_line in varchar2,
  in_separator in varchar2 := co_default_separator
) return t_varchar2_table_type;

procedure add_paging_bind_variables(
  io_bind_variables in out tco_bind_variables.t_bind_variables_type, 
  in_paging         in t_paging_type
);

end tco_common;
/

create package tco_query
as

type t_tco_audit_log_entry_types_get_args_type is record (
  filter_by                   tco_common.t_predicates_type default tco_common.t_predicates_type(),
  order_by                    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure tco_audit_log_entry_types_get(
  in_args               in t_tco_audit_log_entry_types_get_args_type,
  out_data              out sys_refcursor);

type t_tco_dispute_status_types_get_args_type is record (
  filter_by                   tco_common.t_predicates_type default tco_common.t_predicates_type(),
  order_by                    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure tco_dispute_status_types_get(
  in_args               in t_tco_dispute_status_types_get_args_type,
  out_data              out sys_refcursor);

type t_tco_dispute_get_filter_type is record (
  dispute_id tco_disputes.dispute_id%type
);

type t_get_tco_dispute_args_type is record (
  exclude_columns            tco_common.t_column_name_table_type default tco_common.t_column_name_table_type(),
  filter_by                  t_tco_dispute_get_filter_type
);

type t_tco_disputes_dispute_id_table_type is table of tco_disputes.dispute_id%type;

type t_tco_disputes_get_args_type is record (
  include_columns             tco_common.t_column_name_table_type default tco_common.t_column_name_table_type(),
  filter_by                   tco_common.t_predicates_type default tco_common.t_predicates_type(),
  order_by                    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type(),
  paging                      tco_common.t_paging_type
);

procedure tco_disputes_get(
  in_args               in t_tco_disputes_get_args_type,
  out_data              out sys_refcursor,
  out_page              out tco_common.t_current_page_type);

end tco_query;
/

create package tco_debug
as

  function enabled return boolean;

  procedure debug_off;

  procedure debug_on;

  procedure print (p_msg in varchar2);

  procedure print (p_msg in varchar2,
                   p_value in varchar2);

  procedure print (p_msg in varchar2,
                   p_value in number);

  procedure print (p_msg in varchar2,
                   p_value in date);

  procedure print (p_msg in varchar2,
                   p_value in boolean);

  procedure print (p_refcursor in sys_refcursor,
                   p_null_handling in number := 0);

  procedure print (p_xml in xmltype);

  procedure print (p_clob in clob);

  procedure printf (p_msg in varchar2,
                    p_value1 in varchar2 := null,
                    p_value2 in varchar2 := null,
                    p_value3 in varchar2 := null,
                    p_value4 in varchar2 := null,
                    p_value5 in varchar2 := null,
                    p_value6 in varchar2 := null,
                    p_value7 in varchar2 := null,
                    p_value8 in varchar2 := null);

  function get_fdate(p_date in date) return varchar2;

  procedure set_info (p_action in varchar2,
                      p_module in varchar2 := null);

  procedure clear_info;

end tco_debug;
/

create package tco_justin_query
as

type t_justin_agencies_get_args_type is record (
  filter_by   tco_common.t_predicates_type,
  order_by    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure justin_agencies_get(
  in_args    in t_justin_agencies_get_args_type default null,
  out_data   out sys_refcursor);

type t_justin_cities_get_args_type is record (
  filter_by   tco_common.t_predicates_type,
  order_by    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure justin_cities_get(
  in_args   in t_justin_cities_get_args_type default null,
  out_data  out sys_refcursor);

type t_justin_countries_get_args_type is record (
  filter_by   tco_common.t_predicates_type,
  order_by    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure justin_countries_get(
  in_args   in t_justin_countries_get_args_type default null,
  out_data  out sys_refcursor);

type t_justin_languages_get_args_type is record (
  filter_by   tco_common.t_predicates_type,
  order_by    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure justin_languages_get(
  in_args    in t_justin_languages_get_args_type default null,
  out_data   out sys_refcursor);

type t_justin_provinces_get_args_type is record (
  filter_by   tco_common.t_predicates_type,
  order_by    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure justin_provinces_get(
  in_args    in t_justin_provinces_get_args_type default null,
  out_data   out sys_refcursor);

type t_justin_statutes_get_args_type is record (
  distinct_rows                boolean default false,
  include_columns              tco_common.t_column_name_table_type default tco_common.t_column_name_table_type(),
  filter_by                    tco_common.t_predicates_type,
  order_by                     tco_common.t_order_by_table_type
);

procedure justin_statutes_get(
  in_args             in t_justin_statutes_get_args_type default null,
  out_data            out sys_refcursor);

end tco_justin_query;
/

create package tco_rest_v2
as

procedure justin_agencies_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure justin_cities_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure justin_countries_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure justin_languages_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure justin_provinces_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure justin_statutes_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure tco_audit_log_entry_types_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure tco_dispute_status_types_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure tco_disputes_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor,
  out_rows_total                       out number,
  out_rows_offset                      out number,
  out_rows_fetch                       out number);

end tco_rest_v2;
/

create package tco_audit as
  procedure set_fields (inserting   in boolean,
                        ent_dtm     in out date,
                        ent_user_id in out varchar2,
                        upd_dtm     in out date,
                        upd_user_id in out varchar2);
end tco_audit;
/

create PACKAGE     tco_appearance_amendment IS

    PROCEDURE pAppearanceAmendmentsSave(pAppearanceAmendmentId    OUT NUMBER,
                                        pCourtAppearanceId             IN tco_appearance_amendments.court_appearance_id%TYPE,
                                        pDisputantSurnameNm            IN tco_appearance_amendments.disputant_surname_nm%TYPE,
                                        pDisputantGivenNamesNm         IN tco_appearance_amendments.disputant_given_names_nm%TYPE,
                                        pViolationDateDtm              IN tco_appearance_amendments.violation_date_dtm%TYPE,
                                        pOtherNotesTxt                 IN tco_appearance_amendments.other_notes_txt%TYPE,
                                        pCount1ActSectDescTxt          IN tco_appearance_amendments.count_1_act_sect_desc_txt%TYPE,
                                        pCount1OtherTxt                IN tco_appearance_amendments.count_1_other_txt%TYPE,
                                        pCount2ActSectDescTxt          IN tco_appearance_amendments.count_2_act_sect_desc_txt%TYPE,
                                        pCount2OtherTxt                IN tco_appearance_amendments.count_2_other_txt%TYPE,
                                        pCount3ActSectDescTxt          IN tco_appearance_amendments.count_3_act_sect_desc_txt%TYPE,
                                        pCount3OtherTxt                IN tco_appearance_amendments.count_3_other_txt%TYPE);

    PROCEDURE pAppearanceAmendmentsUpdate(pAppearanceAmendmentId  IN tco_appearance_amendments.appearance_amendment_id%TYPE,
                                          pCourtAppearanceId             IN tco_appearance_amendments.court_appearance_id%TYPE,
                                          pDisputantSurnameNm            IN tco_appearance_amendments.disputant_surname_nm%TYPE,
                                          pDisputantGivenNamesNm         IN tco_appearance_amendments.disputant_given_names_nm%TYPE,
                                          pViolationDateDtm              IN tco_appearance_amendments.violation_date_dtm%TYPE,
                                          pOtherNotesTxt                 IN tco_appearance_amendments.other_notes_txt%TYPE,
                                          pCount1ActSectDescTxt          IN tco_appearance_amendments.count_1_act_sect_desc_txt%TYPE,
                                          pCount1OtherTxt                IN tco_appearance_amendments.count_1_other_txt%TYPE,
                                          pCount2ActSectDescTxt          IN tco_appearance_amendments.count_2_act_sect_desc_txt%TYPE,
                                          pCount2OtherTxt                IN tco_appearance_amendments.count_2_other_txt%TYPE,
                                          pCount3ActSectDescTxt          IN tco_appearance_amendments.count_3_act_sect_desc_txt%TYPE,
                                          pCount3OtherTxt                IN tco_appearance_amendments.count_3_other_txt%TYPE);

    PROCEDURE pAppearanceAmendmentsDelete(pCourtAppearanceId      IN tco_appearance_amendments.court_appearance_id%TYPE);

END;
/

