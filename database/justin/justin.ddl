create sequence TOAD_SEQ
/

create table JUSTIN_APPEARANCES
(
    APPR_ID                      NUMBER(15, 4) not null
        constraint APPR_PK
            primary key,
    APPR_DT                      DATE          not null,
    APPR_TM                      DATE          not null,
    APPR_ENT_DT                  DATE          not null,
    APPR_ENT_USER_ID             VARCHAR2(30)  not null,
    CTRM_AGEN_ID                 NUMBER(15, 4) not null,
    CTRM_ROOM_CD                 VARCHAR2(6)   not null,
    APPR_ESTIMATED_DURATION_HH   NUMBER(2),
    APPR_ESTIMATED_DURATION_MI   NUMBER(2),
    APPR_ACTUAL_DURATION_MI      NUMBER(2),
    APPR_ACTUAL_DURATION_HH      NUMBER(2),
    APPR_UPD_DT                  DATE,
    APPR_UPD_USER_ID             VARCHAR2(30),
    CCPT_CCAS_CASE_NO            NUMBER(15, 4),
    CCPT_PART_ID                 NUMBER(15, 4),
    DATT_CD                      VARCHAR2(3),
    CCAS_CASE_NO                 NUMBER(15, 4),
    CLTP_TYPE_CD                 VARCHAR2(3),
    AATT_CD                      VARCHAR2(3),
    APPR_INTERPRETER_REQUIRED_YN VARCHAR2(1)   not null,
    CLST_ID                      NUMBER(15, 4),
    COURT_LIST_REFERENCE_NO      NUMBER(4),
    NOTE_TXT                     VARCHAR2(4000),
    ADDITIONS_LIST_YN            VARCHAR2(1),
    APPR_JUDGES_RECOMMEND_TXT    CLOB,
    APPR_PARTICIPANT_NOT_REQD_YN VARCHAR2(1)
        constraint AVCON_1679338915_APPR__001
            check (APPR_PARTICIPANT_NOT_REQD_YN IN ('Y', 'N')),
    BAPT_TYPE_CD                 VARCHAR2(6),
    BADT_TYPE_CD                 VARCHAR2(6),
    PROCEEDING_ELECTION_CD       VARCHAR2(3)
)
/

create index APPR_CCPT_FK
    on JUSTIN_APPEARANCES (CCPT_CCAS_CASE_NO, CCPT_PART_ID)
/

create index APPR_CCAS_FK
    on JUSTIN_APPEARANCES (CCAS_CASE_NO)
/

create index APPR_SK1
    on JUSTIN_APPEARANCES (CTRM_AGEN_ID, APPR_DT, CTRM_ROOM_CD, CLTP_TYPE_CD, APPR_TM, APPR_ID)
/

create index APPR_CTRM_FK
    on JUSTIN_APPEARANCES (CTRM_AGEN_ID, CTRM_ROOM_CD)
/

create index APPR_CLST_FK
    on JUSTIN_APPEARANCES (CLST_ID)
/

create index APPR_AATT_FK
    on JUSTIN_APPEARANCES (AATT_CD)
/

create index APPR_DATT_FK
    on JUSTIN_APPEARANCES (DATT_CD)
/

create index APPR_CLTP_FK
    on JUSTIN_APPEARANCES (CLTP_TYPE_CD)
/

create index APPR_SK2
    on JUSTIN_APPEARANCES (APPR_ID, APPR_DT)
/

create index APPR_BADT_FK_I
    on JUSTIN_APPEARANCES (BADT_TYPE_CD)
/

create index APPR_BAPT_FK_I
    on JUSTIN_APPEARANCES (BAPT_TYPE_CD)
/

create index APPR_CREL_FK_I
    on JUSTIN_APPEARANCES (PROCEEDING_ELECTION_CD)
/

create table JUSTIN_CD_LANGUAGES
(
    CDLN_LANGUAGE_CD  VARCHAR2(3)  not null
        constraint CDLN_PK
            primary key,
    CDLN_LANGUAGE_DSC VARCHAR2(50) not null,
    CDLN_ENT_DT       DATE         not null,
    CDLN_ENT_USER_ID  VARCHAR2(30) not null,
    CDLN_UPD_DT       DATE,
    CDLN_UPD_USER_ID  VARCHAR2(30),
    CDLN_ACTIVE_YN    VARCHAR2(1)  not null
        constraint AVCON_1296082402_CDLN__000
            check (CDLN_ACTIVE_YN IN ('Y', 'N'))
)
/

create table JUSTIN_CD_PLEAS
(
    PLEA_PLEA_CD     VARCHAR2(3)  not null
        constraint PLEA_PK
            primary key,
    PLEA_PLEA_DSC    VARCHAR2(60) not null,
    PLEA_ACTIVE_YN   VARCHAR2(1)  not null
        check (plea_active_yn IN ('Y', 'N')),
    PLEA_ENT_DT      DATE         not null,
    PLEA_ENT_USER_ID VARCHAR2(30) not null,
    PLEA_UPD_DT      DATE,
    PLEA_UPD_USER_ID VARCHAR2(30)
)
/

create table JUSTIN_COUNTRIES
(
    CTRY_ID          NUMBER(8)    not null
        constraint CTRY_PK
            primary key,
    CTRY_LONG_NM     VARCHAR2(30) not null
        constraint CTRY_UK1
            unique,
    CTRY_SHORT_NM    VARCHAR2(5)
        constraint CTRY_UK2
            unique,
    CTRY_ENT_DT      DATE         not null,
    CTRY_ENT_USER_ID VARCHAR2(30) not null,
    CTRY_UPD_DT      DATE,
    CTRY_UPD_USER_ID VARCHAR2(30)
)
/

create table JUSTIN_DOCUMENTS
(
    DOCM_ID                       NUMBER(15, 4)           not null
        constraint DOCM_PK
            primary key,
    DOCM_ISSUE_DT                 DATE                    not null,
    DOCM_POR_YN                   VARCHAR2(1)             not null
        constraint AVCON_210186_DOCM__000
            check (DOCM_POR_YN IN ('Y', 'N')),
    DOCM_FR_YN                    VARCHAR2(1)             not null
        constraint AVCON_210186_DOCM__001
            check (DOCM_FR_YN IN ('Y', 'N')),
    DOCM_PRINTED_YN               VARCHAR2(1)             not null
        constraint AVCON_210186_DOCM__002
            check (DOCM_PRINTED_YN IN ('Y', 'N')),
    DOCM_ENT_DT                   DATE                    not null,
    DOCM_ENT_USER_ID              VARCHAR2(30)            not null,
    FMTP_FORM_CD                  VARCHAR2(8)             not null,
    DOCM_CANCEL_DT                DATE,
    DOCM_RECITAL_NO               NUMBER(3),
    DOCM_COMMENT_TXT              VARCHAR2(2000),
    DOCM_HELD_YN                  VARCHAR2(1),
    DOCM_UPD_DT                   DATE,
    DOCM_UPD_USER_ID              VARCHAR2(30),
    APPR_ID                       NUMBER(15, 4)
        constraint DOCM_APPR_FK
            references JUSTIN_APPEARANCES,
    NEXT_APPR_ID                  NUMBER(15, 4)
        constraint DOCM_APPR_FK1
            references JUSTIN_APPEARANCES,
    SVCM_CD                       VARCHAR2(3),
    VERIFIED_BY_PART_ID           NUMBER(15, 4),
    DOCM_VERIFIED_DT              DATE,
    DOCM_SIGNED_DT                DATE,
    DOCM_WARRANT_ENDORSED_YN      VARCHAR2(1)
        constraint AVCON_1289254244_DOCM__003
            check (DOCM_WARRANT_ENDORSED_YN IN ('Y', 'N')),
    PREVIOUS_PROCESS_APPLIES_YN   VARCHAR2(1),
    DOCM_FINGERPRINTS_REQUIRED_YN VARCHAR2(1)
        constraint AVCON_1727462181_DOCM__002
            check (DOCM_FINGERPRINTS_REQUIRED_YN IN ('Y', 'N')),
    DOCM_ICBC_YN                  VARCHAR2(1) default 'N' not null
        constraint AVCON_1769537854_DOCM__006
            check (DOCM_ICBC_YN IN ('Y', 'N'))
)
/

create index DOCM_SK1
    on JUSTIN_DOCUMENTS (DOCM_ENT_DT)
/

create index DOCM_APPR_FK
    on JUSTIN_DOCUMENTS (APPR_ID)
/

create index DOCM_FMTP_FK
    on JUSTIN_DOCUMENTS (FMTP_FORM_CD)
/

create index DOCM_SK3
    on JUSTIN_DOCUMENTS (DOCM_ISSUE_DT)
/

create index DOCM_APPR_FK1
    on JUSTIN_DOCUMENTS (NEXT_APPR_ID)
/

create index DOCM_SVCM_FK
    on JUSTIN_DOCUMENTS (SVCM_CD)
/

create index DOCM_SK4
    on JUSTIN_DOCUMENTS (DOCM_UPD_DT)
/

create index DOCM_PART_FK
    on JUSTIN_DOCUMENTS (VERIFIED_BY_PART_ID)
/

create index DOCM_SK7
    on JUSTIN_DOCUMENTS (FMTP_FORM_CD, TRUNC("DOCM_ENT_DT"))
/

create index DOCM_SK8
    on JUSTIN_DOCUMENTS (FMTP_FORM_CD, TRUNC("DOCM_UPD_DT"))
/

create unique index DOCM_SK6
    on JUSTIN_DOCUMENTS (FMTP_FORM_CD, DOCM_HELD_YN, DOCM_ISSUE_DT, DOCM_ID, APPR_ID)
/

create table JUSTIN_IDENTIFICATION_DETAILS
(
    PART_ID                NUMBER(15, 4) not null,
    IDDT_SEQ_NO            NUMBER(3)     not null,
    IDDT_ENT_DT            DATE          not null,
    IDDT_ENT_USER_ID       VARCHAR2(30)  not null,
    IDDT_SURNAME_NM        VARCHAR2(30),
    IDDT_SURNAME_UPPER     VARCHAR2(30),
    IDDT_SURNAME_SOUNDEX   VARCHAR2(5),
    IDDT_GIVEN_1_NM        VARCHAR2(30),
    IDDT_UPPER_GIVEN_1_NM  VARCHAR2(30),
    IDDT_GIVEN_2_NM        VARCHAR2(30),
    IDDT_GIVEN_3_NM        VARCHAR2(30),
    IDDT_BIRTH_DT          DATE,
    IDDT_ORG_NM            VARCHAR2(150),
    IDDT_ORG_NM_UPPER      VARCHAR2(150),
    IDDT_ORG_NM_SOUNDEX    VARCHAR2(5),
    IDDT_UPD_DT            DATE,
    IDDT_UPD_USER_ID       VARCHAR2(30),
    CDNM_NAME_TITLE_CD     VARCHAR2(3),
    IDDT_GENDER_CD         VARCHAR2(3),
    IDDT_ORG_NM_TYPE_CD    VARCHAR2(3),
    IDDT_NAME_TYPE_CD      VARCHAR2(3),
    IDDT_ROTA_INITIALS_TXT VARCHAR2(4),
    constraint IDDT_PK
        primary key (PART_ID, IDDT_SEQ_NO)
)
/

create index IDDT_SK2
    on JUSTIN_IDENTIFICATION_DETAILS (IDDT_ORG_NM_UPPER)
/

create index IDDT_SK3
    on JUSTIN_IDENTIFICATION_DETAILS (IDDT_ORG_NM_SOUNDEX)
/

create index IDDT_SK4
    on JUSTIN_IDENTIFICATION_DETAILS (IDDT_SURNAME_SOUNDEX)
/

create unique index IDDT_SK6
    on JUSTIN_IDENTIFICATION_DETAILS (PART_ID, IDDT_SEQ_NO, IDDT_NAME_TYPE_CD)
/

create index IDDT_SK1
    on JUSTIN_IDENTIFICATION_DETAILS (IDDT_SURNAME_UPPER, IDDT_UPPER_GIVEN_1_NM)
/

create table JUSTIN_PROVINCES
(
    CTRY_ID              NUMBER(8)    not null
        constraint PROV_CTRY_FK
            references JUSTIN_COUNTRIES,
    PROV_SEQ_NO          NUMBER(4)    not null,
    PROV_NM              VARCHAR2(30) not null,
    PROV_ABBREVIATION_CD VARCHAR2(2)  not null,
    PROV_ENT_DT          DATE         not null,
    PROV_ENT_USER_ID     VARCHAR2(30) not null,
    PROV_UPD_DT          DATE,
    PROV_UPD_USER_ID     VARCHAR2(30),
    constraint PROV_PK
        primary key (CTRY_ID, PROV_SEQ_NO)
)
/

create table JUSTIN_CITIES
(
    CTRY_ID             NUMBER(8)     not null
        constraint CITY_CTRY_FK
            references JUSTIN_COUNTRIES,
    CITY_SEQ_NO         NUMBER(4)     not null,
    CITY_LONG_NM        VARCHAR2(100) not null,
    CITY_SHORT_NM       VARCHAR2(4)   not null,
    CITY_ENT_DT         DATE          not null,
    CITY_ENT_USER_ID    VARCHAR2(30)  not null,
    PROV_CTRY_ID        NUMBER(8),
    PROV_SEQ_NO         NUMBER(4),
    CITY_UPD_DT         DATE,
    CITY_UPD_USER_ID    VARCHAR2(30),
    CITY_DEACTIVATED_DT DATE,
    constraint CITY_PK
        primary key (CTRY_ID, CITY_SEQ_NO),
    constraint CITY_PROV_FK
        foreign key (PROV_CTRY_ID, PROV_SEQ_NO) references JUSTIN_PROVINCES
)
/

create table JUSTIN_AGENCIES
(
    AGEN_ID                       NUMBER(15, 4) not null
        constraint AGEN_PK
            primary key,
    AGEN_AGENCY_NM                VARCHAR2(150) not null,
    AGEN_ENT_DT                   DATE          not null,
    AGEN_ENT_USER_ID              VARCHAR2(30)  not null,
    CITY_CTRY_ID                  NUMBER(8)     not null,
    CITY_SEQ_NO                   NUMBER(4)     not null,
    AGEN_ADDRESS_LINE_1_TXT       VARCHAR2(40)  not null,
    CDAT_AGENCY_TYPE_CD           VARCHAR2(3)   not null,
    AGEN_AGENCY_SHORT_NM          VARCHAR2(150),
    AGEN_AGENCY_IDENTIFIER_CD     VARCHAR2(4),
    AGEN_ADDRESS_LINE_2_TXT       VARCHAR2(40),
    AGEN_ADDRESS_LINE_3_TXT       VARCHAR2(40),
    AGEN_POSTAL_CODE_TXT          VARCHAR2(10),
    AGEN_CONTACT_NAME             VARCHAR2(40),
    AGEN_CONTACT_PHONE            VARCHAR2(13),
    AGEN_UPD_DT                   DATE,
    AGEN_UPD_USER_ID              VARCHAR2(30),
    AGEN_CONTACT_FAX_NO           VARCHAR2(13),
    AGEN_WIT_NOTIF_UNIT_PHONE_NO  VARCHAR2(13),
    AGEN_JUSTIN_SITE_YN           VARCHAR2(1),
    AGSB_AGENCY_SUBTYPE_CD        VARCHAR2(3),
    AGEN_ACTIVE_YN                VARCHAR2(1)   not null
        constraint AVCON_208392_AGEN__001
            check (AGEN_ACTIVE_YN IN ('Y', 'N')),
    AGEN_EMAIL_ADDRESS_TXT        VARCHAR2(100),
    TIME_ZONE_CD                  VARCHAR2(1)
        constraint AVCON_1107814249_TIME__000
            check (TIME_ZONE_CD IN ('M', 'P')),
    MAILING_ADDRESS_TXT           VARCHAR2(100),
    DELIVERY_METHOD_CD            VARCHAR2(4),
    RECEIVE_FAX_COVER_PAGE_YN     VARCHAR2(1)
        constraint AVCON_1234916455_RECEI_000
            check (RECEIVE_FAX_COVER_PAGE_YN IN ('Y', 'N')),
    RCMP_DIVISION_CD              VARCHAR2(4),
    CROWN_REGION_CD               VARCHAR2(4),
    FILE_DESIGNATION_MANDATORY_YN VARCHAR2(1)
        constraint AVCON_1386287211_FILE__000
            check (FILE_DESIGNATION_MANDATORY_YN IN ('Y', 'N')),
    AGEN_OBSERVES_DST_YN          VARCHAR2(1)   not null,
    DFLT_PHYS_TICKET_SERIES_TXT   VARCHAR2(4),
    constraint AGEN_UK1
        unique (CDAT_AGENCY_TYPE_CD, AGEN_AGENCY_IDENTIFIER_CD),
    constraint AGEN_CITY_FK
        foreign key (CITY_CTRY_ID, CITY_SEQ_NO) references JUSTIN_CITIES,
    constraint AGEN_CK1
        check ((CROWN_REGION_CD IS NOT NULL AND CDAT_AGENCY_TYPE_CD = 'CO')
            OR
               CROWN_REGION_CD IS NULL),
    constraint AGEN_CK2
        check ((RCMP_DIVISION_CD IS NOT NULL AND AGSB_AGENCY_SUBTYPE_CD = 'POL')
            OR
               RCMP_DIVISION_CD IS NULL),
    constraint AGEN_CK3
        check ((CDAT_AGENCY_TYPE_CD != 'CO'
            AND FILE_DESIGNATION_MANDATORY_YN IS NULL)
            OR
               CDAT_AGENCY_TYPE_CD = 'CO')
)
/

create index AGEN_CITY_FK
    on JUSTIN_AGENCIES (CITY_CTRY_ID, CITY_SEQ_NO)
/

create index AGEN_CRGN_FK
    on JUSTIN_AGENCIES (CROWN_REGION_CD)
/

create index AGEN_RCMD_FK
    on JUSTIN_AGENCIES (RCMP_DIVISION_CD)
/

create index CITY_CTRY_FK
    on JUSTIN_CITIES (CTRY_ID)
/

create index CITY_PROV_FK
    on JUSTIN_CITIES (PROV_CTRY_ID, PROV_SEQ_NO)
/

create table JUSTIN_COURT_PARTICIPANTS
(
    CRTP_ID                        NUMBER(15, 4) not null
        constraint CRTP_PK
            primary key,
    CRTP_ENT_DT                    DATE          not null,
    CRTP_ENT_USER_ID               VARCHAR2(30)  not null,
    CTRL_ROLE_CD                   VARCHAR2(3)   not null,
    CRTP_PARTY_COUNSEL_ATTENDED_YN VARCHAR2(1),
    CRTP_PARTY_COUNSEL_NM          VARCHAR2(30),
    CRTP_PARTY_ATTENDED_YN         VARCHAR2(1),
    CRTP_UPD_DT                    DATE,
    CRTP_UPD_USER_ID               VARCHAR2(30),
    CLST_ID                        NUMBER(15, 4),
    SESS_ID                        NUMBER(15, 4),
    APPR_ID                        NUMBER(15, 4)
        constraint CRTP_APPR_FK
            references JUSTIN_APPEARANCES,
    PART_ID                        NUMBER(15, 4) not null,
    REMOTE_ATTENDANCE_YN           VARCHAR2(1)
        constraint AVCON_1621010256_REMOT_000
            check (REMOTE_ATTENDANCE_YN IN ('Y', 'N')),
    REMOTE_AGEN_ID                 NUMBER(15, 4)
        constraint CRTP_AGEN_FK
            references JUSTIN_AGENCIES,
    CTRM_REMOTE_AGEN_ID            NUMBER(15, 4),
    CTRM_REMOTE_ROOM_CD            VARCHAR2(6),
    constraint CRTP_CK
        check (DECODE(CLST_ID, NULL, 0, 1) + DECODE(SESS_ID, NULL, 0, 1) +
               DECODE(APPR_ID, 0, 1) = 1),
    constraint CRTP_CK1
        check (DECODE(REMOTE_AGEN_ID, NULL, 0, 1) +
               DECODE(CTRM_REMOTE_AGEN_ID, NULL, 0, 1) IN (0, 1)),
    constraint CRTP_CK2
        check (DECODE(CTRM_REMOTE_AGEN_ID, NULL, 0, 1) +
               DECODE(CTRM_REMOTE_ROOM_CD, NULL, 0, 1) IN (0, 2)),
    constraint CRTP_CK3
        check ((REMOTE_ATTENDANCE_YN != 'Y'
            AND
                REMOTE_AGEN_ID IS NULL
            AND
                CTRM_REMOTE_AGEN_ID IS NULL)
            OR
               REMOTE_ATTENDANCE_YN = 'Y')
)
/

create index CRTP_SK2
    on JUSTIN_COURT_PARTICIPANTS (CLST_ID, CTRL_ROLE_CD)
/

create index CRTP_SK1
    on JUSTIN_COURT_PARTICIPANTS (APPR_ID, CTRL_ROLE_CD)
/

create index CRTP_SESS_FK
    on JUSTIN_COURT_PARTICIPANTS (SESS_ID)
/

create index CRTP_APPR_FK
    on JUSTIN_COURT_PARTICIPANTS (APPR_ID)
/

create index CRTP_CLST_FK
    on JUSTIN_COURT_PARTICIPANTS (CLST_ID)
/

create index CRTP_PART_FK
    on JUSTIN_COURT_PARTICIPANTS (PART_ID)
/

create index PROV_CTRY_FK
    on JUSTIN_PROVINCES (CTRY_ID)
/

create table JUSTIN_RCCS
(
    RCC_ID                        NUMBER(15, 4) not null
        constraint RCC_PK
            primary key,
    RCC_STATE_CD                  VARCHAR2(3)   not null
        constraint RCC_CK1
            check (RCC_STATE_CD IN ('ACT', 'BLD', 'CLS', 'FIN', 'RET', 'SUB', 'APR')),
    RCC_STATE_CHG_DT              DATE          not null,
    RCC_ORG_CREATION_DT           DATE          not null,
    RCC_PMA_CD                    VARCHAR2(1)   not null
        constraint RCC_CK4
            check (RCC_PMA_CD IN ('A', 'B', 'C', 'T')),
    RCC_ENT_DT                    DATE          not null,
    RCC_ENT_USER_ID               VARCHAR2(30)  not null,
    RCC_AGENCY_FILE_NO            VARCHAR2(17),
    RCC_CATEGORY_CD               VARCHAR2(3)
        constraint RCC_CK3
            check (RCC_CATEGORY_CD IN ('1', '2', '3', '4', '5', '6')),
    RCC_SUB_CATEGORY_CD           VARCHAR2(6)
        constraint RCC_CK2
            check (RCC_SUB_CATEGORY_CD
                IN ('COM', 'ENV', 'FAV', 'FDN', 'FFV', 'FGV', 'FMC', 'FMV', 'FOT', 'FVW', 'GRP', 'MAJ', 'OTH', 'PPY',
                    'VAW', 'FHI', 'HMI', 'HAT', 'FHC', 'AAP', 'VRT')),
    RCC_MOST_RECENT_BUILD_DT      DATE,
    RCC_MOST_RECENT_SUBMIT_DT     DATE,
    RCC_MOST_RECENT_REJECT_DT     DATE,
    RCC_DECISION_DT               DATE,
    RCC_REJECTING_USER_ID         VARCHAR2(30),
    RCC_ACCEPT_DT                 DATE,
    RCC_ACCEPTING_USER_ID         VARCHAR2(30),
    RCC_PUBLICATION_BAN_YN        VARCHAR2(1)
        constraint AVCON_1109109343_RCC_P_000
            check (RCC_PUBLICATION_BAN_YN IN ('Y', 'N')),
    RCC_LOCKED_DOWN_YN            VARCHAR2(1)
        constraint AVCON_1109109343_RCC_L_000
            check (RCC_LOCKED_DOWN_YN IN ('Y', 'N')),
    RCC_CROWN_CVWS_YN             VARCHAR2(1)
        constraint AVCON_1109109343_RCC_C_000
            check (RCC_CROWN_CVWS_YN IN ('Y', 'N')),
    RCC_UPD_DT                    DATE,
    RCC_UPD_USER_ID               VARCHAR2(30),
    RCC_FILE_FOLDER_HOME_AGEN_ID  NUMBER(15, 4)
        constraint RCC_AGEN_FK1
            references JUSTIN_AGENCIES,
    RCC_PRIORITY_CD               VARCHAR2(3),
    PROPOSED_CRT_LOC_AGEN_ID      NUMBER(15, 4)
        constraint RCC_AGEN_FK
            references JUSTIN_AGENCIES,
    RCC_SECURITY_CLEARANCE_LVL_NO NUMBER(2),
    PART_SUBM_APPROVED_BY_ID      NUMBER(15, 4),
    PART_APPROVED_BY_ID           NUMBER(15, 4),
    PART_INITIATED_BY_ID          NUMBER(15, 4),
    RCC_DECISION_CD               VARCHAR2(3),
    RCC_DECISION_CROWN_PART_ID    NUMBER(15, 4),
    RCC_DECISION_LOCN_AGEN_ID     NUMBER(15, 4)
        constraint RCC_AGEN_FK2
            references JUSTIN_AGENCIES,
    VCSS_SERVICE_STATE_CD         VARCHAR2(3),
    APPROVAL_OFFICER_PART_ID      NUMBER(15, 4),
    JURISDICTION_AGEN_ID          NUMBER(15, 4)
        constraint RCC_AGEN_FK3
            references JUSTIN_AGENCIES,
    RCC_PROBATION_ACCESS_YN       VARCHAR2(1)
        constraint AVCON_1109109343_RCC_P_001
            check (RCC_PROBATION_ACCESS_YN IN ('Y', 'N')),
    RCCT_RCC_TYPE_CD              VARCHAR2(3),
    RCC_INTIMATE_RELATIONSHIP_YN  VARCHAR2(1)
        constraint AVCON_1203117519_INTIM_000
            check (RCC_INTIMATE_RELATIONSHIP_YN IN ('Y', 'N')),
    CLEARANCE_PAAS_AGEN_ID        NUMBER(15, 4),
    CLEARANCE_PAAS_PART_ID        NUMBER(15, 4),
    CLEARANCE_PAAS_SEQ_NO         NUMBER(3),
    FLA_ORDER_BREACH_YN           VARCHAR2(1)
        constraint AVCON_1373651200_FLA_O_000
            check (FLA_ORDER_BREACH_YN IN ('Y', 'N')),
    CFC_ORDER_BREACH_YN           VARCHAR2(1)
        constraint AVCON_1373651200_CFC_O_000
            check (CFC_ORDER_BREACH_YN IN ('Y', 'N')),
    FILE_DESIGNATION_CD           VARCHAR2(4),
    FRA_ORDER_BREACH_YN           VARCHAR2(1)
        constraint AVCON_1439234637_FRA_O_000
            check (FRA_ORDER_BREACH_YN IN ('Y', 'N')),
    OTHER_ORDER_BREACH_YN         VARCHAR2(1)
        constraint AVCON_1439234637_OTHER_000
            check (OTHER_ORDER_BREACH_YN IN ('Y', 'N')),
    ORDER_BREACH_REVIEWED_YN      VARCHAR2(1)
        constraint AVCON_1439234637_ORDER_000
            check (ORDER_BREACH_REVIEWED_YN IN ('Y', 'N')),
    AFTERHOURS_PROCESS_YN         VARCHAR2(1)
        constraint AVCON_1548183258_AFTER_000
            check (AFTERHOURS_PROCESS_YN IN ('Y', 'N')),
    INITIAL_CHARGE_ASMT_AGEN_ID   NUMBER(15, 4)
        constraint RCC_AGEN_FK4
            references JUSTIN_AGENCIES,
    POL_DESIGNATED_HATE_CRIME_YN  VARCHAR2(1)
        constraint AVCON_1697480953_POL_D_000
            check (POL_DESIGNATED_HATE_CRIME_YN IN ('Y', 'N')),
    PRFSNL_STAFF_AGEN_ID          NUMBER(15, 4),
    PRFSNL_STAFF_PART_ID          NUMBER(15, 4),
    PRFSNL_STAFF_PAAS_SEQ_NO      NUMBER(3),
    constraint RCC_CK5
        check ((CLEARANCE_PAAS_AGEN_ID IS NOT NULL
            AND CLEARANCE_PAAS_PART_ID IS NOT NULL
            AND CLEARANCE_PAAS_SEQ_NO IS NOT NULL)
            OR
               (CLEARANCE_PAAS_AGEN_ID IS NULL
                   AND CLEARANCE_PAAS_PART_ID IS NULL AND CLEARANCE_PAAS_SEQ_NO IS NULL)),
    constraint RCC_CK6
        check ((CLEARANCE_PAAS_PART_ID IS NOT NULL
            AND RCC_SECURITY_CLEARANCE_LVL_NO IS NOT NULL)
            OR
               (CLEARANCE_PAAS_PART_ID IS NULL)),
    constraint RCC_CK7
        check (DECODE(FLA_ORDER_BREACH_YN, 'Y', 1, 0) +
               DECODE(CFC_ORDER_BREACH_YN, 'Y', 1, 0) +
               DECODE(FRA_ORDER_BREACH_YN, 'Y', 1, 0) +
               DECODE(OTHER_ORDER_BREACH_YN, 'Y', 1, 0)
            IN (0, 1)),
    constraint RCC_CK8
        check ((PRFSNL_STAFF_AGEN_ID IS NOT NULL
            AND PRFSNL_STAFF_PART_ID IS NOT NULL
            AND PRFSNL_STAFF_PAAS_SEQ_NO IS NOT NULL)
            OR (PRFSNL_STAFF_AGEN_ID IS NULL
                AND PRFSNL_STAFF_PART_ID IS NULL AND PRFSNL_STAFF_PAAS_SEQ_NO IS NULL))
)
/

create table JUSTIN_PHYSICAL_FILES
(
    PHYS_ID                        NUMBER(15, 4) not null
        constraint PHYS_PK
            primary key,
    PHYS_ENT_DT                    DATE          not null,
    PHYS_ENT_USER_ID               VARCHAR2(30)  not null,
    PHYS_FILE_FOLDER_NO            NUMBER(12)    not null,
    PHYS_FILE_FOLDER_NO_CHAR       VARCHAR2(12)  not null,
    PHYS_COURT_CLASS_CD            VARCHAR2(3)   not null,
    PHYS_COURT_DIVISION_CD         VARCHAR2(3)   not null,
    PHYS_COURT_LEVEL_CD            VARCHAR2(3)   not null,
    PHYS_FILE_FOLDER_HOME_LOCATION NUMBER(15, 4) not null
        constraint PHYS_AGEN_FK
            references JUSTIN_AGENCIES,
    PHYS_FILE_OPENING_DT           DATE,
    PHYS_TICKET_SERIES_TXT         VARCHAR2(4),
    PHYS_UPD_DT                    DATE,
    PHYS_UPD_USER_ID               VARCHAR2(30),
    CCAS_CASE_NO                   NUMBER(15, 4),
    RCC_ID                         NUMBER(15, 4)
        constraint PHYS_RCC_FK
            references JUSTIN_RCCS,
    PHYS_SHERIFF_SECURITY_REQD_YN  VARCHAR2(1),
    constraint PHYS_UK2
        unique (PHYS_FILE_FOLDER_HOME_LOCATION, PHYS_FILE_FOLDER_NO, PHYS_COURT_DIVISION_CD, PHYS_COURT_LEVEL_CD,
                PHYS_COURT_CLASS_CD, PHYS_TICKET_SERIES_TXT),
    constraint PHYS_CK1
        check (PHYS_COURT_DIVISION_CD = 'R'
            OR
               (PHYS_COURT_DIVISION_CD = 'I'
                   AND PHYS_ENT_DT < TO_DATE('2004-07-01', 'YYYY-MM-DD')))
)
/

create table JUSTIN_MATTER_DOCS
(
    MDOC_JUSTIN_NO                 NUMBER(12)              not null
        constraint MDOC_PK
            primary key,
    MDOC_JUSTIN_NO_CHAR            VARCHAR2(12)            not null,
    MDOC_PMA_CD                    VARCHAR2(1)             not null
        constraint MDOC_CK3
            check (MDOC_PMA_CD IN ('A', 'B', 'C', 'T')),
    MDOC_INFO_SEQ_NO               NUMBER(4)               not null,
    MDOC_SWORN_OR_FILED_YN         VARCHAR2(1)             not null
        constraint AVCON_208481_MDOC__000
            check (MDOC_SWORN_OR_FILED_YN IN ('Y', 'N')),
    MDOC_ENT_DT                    DATE                    not null,
    MDOC_ENT_USER_ID               VARCHAR2(30)            not null,
    PHYS_ID                        NUMBER(15, 4)           not null
        constraint MDOC_PHYS_FK
            references JUSTIN_PHYSICAL_FILES,
    MDOC_DOCUMENT_CD               VARCHAR2(3)             not null,
    CREATING_AGEN_ID               NUMBER(15, 4)           not null
        constraint MDOC_AGEN_FK
            references JUSTIN_AGENCIES,
    MDOC_PRINTED_YN                VARCHAR2(1),
    MDOC_REF_TYPE_CD               VARCHAR2(6),
    MDOC_PROCESS_ISSUED_DT         DATE,
    MDOC_SWORN_DT                  DATE,
    MDOC_APPROVAL_DT               DATE,
    MDOC_FILED_DT                  DATE,
    MDOC_FILED_BY_TXT              VARCHAR2(30),
    MDOC_PRESENTER_OCCUPATION_TXT  VARCHAR2(240),
    MDOC_ADAP_SEQ_NO               NUMBER(4),
    MDOC_CROWN_ELECTION_DT         DATE,
    MDOC_DOCUMENT_AMENDED_YN       VARCHAR2(1),
    MDOC_DISCLOSURE_COURT_YN       VARCHAR2(1),
    MDOC_CASE_ESTIMATED_LENGTH_QTY NUMBER(5, 2),
    MDOC_CATEGORY_CD               VARCHAR2(3)
        constraint MDOC_CK2
            check (MDOC_CATEGORY_CD IN ('1', '2', '3', '4', '5', '6')),
    MDOC_SUB_CATEGORY_CD           VARCHAR2(6)
        constraint MDOC_CK1
            check (MDOC_SUB_CATEGORY_CD
                IN ('COM', 'ENV', 'FAV', 'FDN', 'FFV', 'FGV', 'FMC', 'FMV', 'FOT', 'FVW', 'GRP', 'MAJ', 'OTH', 'PPY',
                    'VAW', 'FHI', 'HMI', 'HAT', 'FHC', 'AAP', 'VRT')),
    MDOC_APPROVING_PART_ID         NUMBER(15, 4),
    MDOC_APPROVING_LOCN_AGEN_ID    NUMBER(15, 4)
        constraint MDOC_AGEN_FK1
            references JUSTIN_AGENCIES,
    MDOC_PRESENTING_PART_ID        NUMBER(15, 4),
    MDOC_AMENDMENT_DETAIL_TXT      VARCHAR2(4000),
    MDOC_TICKET_ISSUE_DT           DATE,
    MDOC_TICKET_COMPLETE_DT        DATE,
    MDOC_UPD_DT                    DATE,
    MDOC_UPD_USER_ID               VARCHAR2(30),
    MDOC_PRESENTING_AGEN_ID        NUMBER(15, 4)
        constraint MDOC_AGEN_FK2
            references JUSTIN_AGENCIES,
    MDOC_SWORN_BY_PART_ID          NUMBER(15, 4),
    MDOC_CROWN_ELECTION_CD         VARCHAR2(3),
    MDOC_CASE_ESTIMATED_LENGTH_CD  VARCHAR2(4),
    TRIL_ID                        NUMBER(15, 4),
    CIDF_DOCUMENT_FORM_CD          VARCHAR2(3),
    MDOC_CROWN_COMMENT_TXT         VARCHAR2(2000),
    SWORN_AT_AGEN_ID               NUMBER(15, 4)
        constraint MDOC_AGEN_FK3
            references JUSTIN_AGENCIES,
    ACCIDENT_YN                    VARCHAR2(1)
        constraint AVCON_1167947370_MDOC__001
            check (ACCIDENT_YN IN ('Y', 'N')),
    SIGNIFICANT_STATUS_START_DTM   DATE,
    SIGNIFICANT_STATUS_EXPIRY_DTM  DATE,
    SIGNIFICANT_STATUS_COMMENT_TXT VARCHAR2(4000),
    SUBMISSION_MEANS_CD            VARCHAR2(4)
        constraint AVCON_1307647498_SUBMI_000
            check (SUBMISSION_MEANS_CD IN ('TELC')),
    LOCKING_AGEN_ID                NUMBER(15, 4),
    LOCKING_PART_ID                NUMBER(15, 4),
    LOCKING_PAAS_SEQ_NO            NUMBER(3),
    LOCKING_COURT_AGEN_ID          NUMBER(15, 4),
    LOCKING_CTRM_ROOM_CD           VARCHAR2(6),
    FILE_DESIGNATION_CD            VARCHAR2(4),
    TICKET_DISPUTE_FILED_DT        DATE,
    PRIMARY_RCC_ID                 NUMBER(15, 4)
        constraint MDOC_RCC_FK
            references JUSTIN_RCCS,
    RETENTION_PERIOD_CD            VARCHAR2(4),
    EXHIBIT_ENVELOPE_YN            VARCHAR2(1)
        constraint AVCON_1747071894_EXHIB_000
            check (EXHIBIT_ENVELOPE_YN IN ('Y', 'N')),
    LOCKED_DOWN_YN                 VARCHAR2(1) default 'N' not null
        constraint AVCON_1754089759_LOCKE_000
            check (LOCKED_DOWN_YN IN ('Y', 'N')),
    constraint MDOC_CK4
        check ((SIGNIFICANT_STATUS_COMMENT_TXT IS NOT NULL
            AND SIGNIFICANT_STATUS_START_DTM IS NOT NULL)
            OR
               SIGNIFICANT_STATUS_COMMENT_TXT IS NULL),
    constraint MDOC_CK6
        check (DECODE(LOCKING_AGEN_ID, NULL, 0, 1) + DECODE(LOCKING_PART_ID, NULL, 0, 1)
                   + DECODE(LOCKING_PAAS_SEQ_NO, NULL, 0, 1) IN (0, 3)),
    constraint MDOC_CK7
        check (DECODE(LOCKING_COURT_AGEN_ID, NULL, 0, 1) +
               DECODE(LOCKING_CTRM_ROOM_CD, NULL, 0, 1) IN (0, 2)),
    constraint MDOC_CK8
        check (DECODE(LOCKING_AGEN_ID, NULL, 0, 1) +
               DECODE(LOCKING_COURT_AGEN_ID, NULL, 0, 1) IN (0, 1))
)
/

create index MDOC_PART_FK1
    on JUSTIN_MATTER_DOCS (MDOC_APPROVING_PART_ID)
/

create index MDOC_SK1
    on JUSTIN_MATTER_DOCS (MDOC_APPROVING_LOCN_AGEN_ID, MDOC_APPROVAL_DT)
/

create index MDOC_PART_FK2
    on JUSTIN_MATTER_DOCS (MDOC_PRESENTING_PART_ID)
/

create index MDOC_SK
    on JUSTIN_MATTER_DOCS (PHYS_ID, MDOC_SWORN_OR_FILED_YN)
/

create index MDOC_TRIL_FK
    on JUSTIN_MATTER_DOCS (TRIL_ID)
/

create index MDOC_AGEN_FK
    on JUSTIN_MATTER_DOCS (CREATING_AGEN_ID)
/

create index MDOC_PART_FK
    on JUSTIN_MATTER_DOCS (MDOC_SWORN_BY_PART_ID)
/

create index MDOC_SK2
    on JUSTIN_MATTER_DOCS (MDOC_APPROVING_LOCN_AGEN_ID, MDOC_APPROVAL_DT, MDOC_SWORN_OR_FILED_YN, MDOC_REF_TYPE_CD,
                           MDOC_INFO_SEQ_NO, PHYS_ID, MDOC_JUSTIN_NO)
/

create unique index MDOC_SK3
    on JUSTIN_MATTER_DOCS (PHYS_ID, MDOC_REF_TYPE_CD, MDOC_DOCUMENT_CD, MDOC_JUSTIN_NO, MDOC_INFO_SEQ_NO, MDOC_SWORN_DT,
                           MDOC_FILED_DT)
/

create index MDOC_AGEN_FK3
    on JUSTIN_MATTER_DOCS (SWORN_AT_AGEN_ID)
/

create index MDOC_AGEN_FK2
    on JUSTIN_MATTER_DOCS (MDOC_PRESENTING_AGEN_ID)
/

create index MDOC_SK4
    on JUSTIN_MATTER_DOCS (SIGNIFICANT_STATUS_START_DTM)
/

create index MDOC_AGEN_FK1
    on JUSTIN_MATTER_DOCS (MDOC_APPROVING_LOCN_AGEN_ID)
/

create index MDOC_SK5
    on JUSTIN_MATTER_DOCS (SUBMISSION_MEANS_CD)
/

create index MDOC_PAAS_FK
    on JUSTIN_MATTER_DOCS (LOCKING_AGEN_ID, LOCKING_PART_ID, LOCKING_PAAS_SEQ_NO)
/

create index MDOC_CTRM_FK
    on JUSTIN_MATTER_DOCS (LOCKING_COURT_AGEN_ID, LOCKING_CTRM_ROOM_CD)
/

create index MDOC_FLDS_FK
    on JUSTIN_MATTER_DOCS (FILE_DESIGNATION_CD)
/

create index MDOC_RCC_FK
    on JUSTIN_MATTER_DOCS (PRIMARY_RCC_ID)
/

create index MDOC_RETP_FK_I
    on JUSTIN_MATTER_DOCS (RETENTION_PERIOD_CD)
/

create table JUSTIN_PARTICIPANT_PROFILES
(
    PART_ID                        NUMBER(15, 4) not null,
    PROF_SEQ_NO                    NUMBER(10)    not null,
    PROF_ENT_DT                    DATE          not null,
    PROF_ENT_USER_ID               VARCHAR2(30)  not null,
    PROF_ASSOCIATION_CD            VARCHAR2(3)   not null
        constraint AVCON_1109098417_PROF__000
            check (PROF_ASSOCIATION_CD IN ('CIV', 'RCC', 'INF'))
        constraint PROF_CK1
            check (PROF_ASSOCIATION_CD IN ('CIV', 'INF', 'RCC')),
    PROF_RCC_PART_ID               NUMBER(15, 4),
    RCC_ID                         NUMBER(15, 4)
        constraint PROF_RCC_FK
            references JUSTIN_RCCS,
    PROF_INFO_PART_ID              NUMBER(15, 4),
    PROF_TOMBSTONE_STATIC_YN       VARCHAR2(1),
    PROF_SURNAME_NM                VARCHAR2(30),
    PROF_SURNAME_UPPER_NM          VARCHAR2(30),
    PROF_GIVEN_1_NM                VARCHAR2(30),
    PROF_UPPER_GIVEN_1_NM          VARCHAR2(30),
    PROF_GIVEN_2_NM                VARCHAR2(30),
    PROF_GIVEN_3_NM                VARCHAR2(30),
    PROF_BIRTH_DT                  DATE,
    PROF_DECEASED_YN               VARCHAR2(1)
        constraint AVCON_1109098417_PROF__001
            check (PROF_DECEASED_YN IN ('Y', 'N')),
    PROF_DECEASED_DT               DATE,
    PROF_INTERPRETER_REQUIRED_YN   VARCHAR2(1),
    PROF_DRV_LIC_NUMBER_TXT        VARCHAR2(30),
    MDOC_JUSTIN_NO                 NUMBER(12)
        constraint PROF_MDOC_FK
            references JUSTIN_MATTER_DOCS,
    PROF_DRV_LIC_SUSPENDED_YN      VARCHAR2(1),
    PROF_DRV_LIC_ISSUE_DT          DATE,
    PROF_DRV_LIC_EXPIRY_DT         DATE,
    PROF_PASSPORT_NUMBER_TXT       VARCHAR2(30),
    PROF_STATEMENT_TAKEN_YN        VARCHAR2(1)
        constraint AVCON_1109098417_PROF__002
            check (PROF_STATEMENT_TAKEN_YN IN ('Y', 'N')),
    PROF_AC_HEIGHT_PRIMARY_VAL     NUMBER(3),
    PROF_AC_WEIGHT_VAL             NUMBER(3),
    PROF_AC_DURATION_AREA_VAL      NUMBER(6, 2),
    PROF_AC_CC_SECTION_10A_YN      VARCHAR2(1),
    PROF_AC_CC_SECTION_10B_YN      VARCHAR2(1),
    PROF_AC_RELEASE_COND_TXT       VARCHAR2(500),
    PROF_AC_PROPOSED_APPEARANCE_DT DATE,
    PROF_AC_IN_CUSTODY_YN          VARCHAR2(1),
    PROF_AC_VISEN_NOT_KNOWN_YN     VARCHAR2(1)
        constraint AVCON_1109098417_PROF__003
            check (PROF_AC_VISEN_NOT_KNOWN_YN IN ('Y', 'N')),
    PROF_AC_VISEN_VIOLENT_YN       VARCHAR2(1),
    PROF_AC_VISEN_INFECTIOUS_YN    VARCHAR2(2),
    PROF_AC_VISEN_SUICIDAL_YN      VARCHAR2(1),
    PROF_AC_VISEN_ESCAPEE_YN       VARCHAR2(1),
    PROF_AC_VISEN_AS_NORMAL_YN     VARCHAR2(1),
    PROF_AC_VISEN_DRUG_USER_YN     VARCHAR2(1),
    PROF_AC_VISEN_MENTAL_UNSTBL_YN VARCHAR2(1),
    PROF_AC_GANG_MEMBER_YN         VARCHAR2(1),
    PROF_AC_YOUNG_OFFENDER_YN      VARCHAR2(1),
    PROF_AC_CRIMINAL_RECORD_YN     VARCHAR2(1)
        constraint AVCON_1109098417_PROF__004
            check (PROF_AC_CRIMINAL_RECORD_YN IN ('Y', 'N')),
    PROF_AC_DRIVING_RECORD_YN      VARCHAR2(1)
        constraint AVCON_1109098417_PROF__005
            check (PROF_AC_DRIVING_RECORD_YN IN ('Y', 'N')),
    PROF_AC_FINGERPRINTED_YN       VARCHAR2(1)
        constraint AVCON_1109098417_PROF__006
            check (PROF_AC_FINGERPRINTED_YN IN ('Y', 'N')),
    PROF_AC_ICA_YN                 VARCHAR2(1)
        constraint AVCON_1109098417_PROF__007
            check (PROF_AC_ICA_YN IN ('Y', 'N')),
    PROF_AC_PARDONED_YN            VARCHAR2(1)
        constraint AVCON_1109098417_PROF__008
            check (PROF_AC_PARDONED_YN IN ('Y', 'N')),
    PROF_AC_PARDONED_DT            DATE,
    PROF_AC_CURRENT_BAIL_PROC_TXT  VARCHAR2(3),
    PROF_AC_PRINT_NAME_YN          VARCHAR2(1)
        constraint AVCON_1109098417_PROF__009
            check (PROF_AC_PRINT_NAME_YN IN ('Y', 'N')),
    PROF_AC_PSYCHIATIC_EXAM_YN     VARCHAR2(1)
        constraint AVCON_1109098417_PROF__010
            check (PROF_AC_PSYCHIATIC_EXAM_YN IN ('Y', 'N')),
    PROF_NA_SIN_NO                 NUMBER(9),
    PROF_NA_WILLING_ID_ACCUSED_YN  VARCHAR2(1),
    PROF_NA_PHOTO_LINEUP_GIVEN_YN  VARCHAR2(1),
    PROF_NA_RELATED_TO_ACCUSED_YN  VARCHAR2(1),
    PROF_NA_RELATED_TO_NOTIFIED_YN VARCHAR2(1),
    PROF_NA_VICTIM_SERVICES_REQ_YN VARCHAR2(1),
    PROF_NA_REFER_TO_VIC_SER_YN    VARCHAR2(1),
    PROF_NA_NOTIFIED_VIC_SER_YN    VARCHAR2(1),
    PROF_NA_NOTIFIED_VIC_YN        VARCHAR2(1),
    PROF_NA_RECVD_IMPACT_STMT_YN   VARCHAR2(1),
    PROF_NA_REQUESTED_AMT          NUMBER(10, 2),
    PROF_NA_REQ_COMPENSATION_AMT   NUMBER(6, 2),
    PROF_NA_VICTIM_SVCS_SET_DT     DATE,
    PROF_WITNESS_CONFIDENTIAL_YN   VARCHAR2(1)
        constraint AVCON_1109098417_PROF__011
            check (PROF_WITNESS_CONFIDENTIAL_YN IN ('Y', 'N')),
    PROF_WITNESS_REQUIRED_YN       VARCHAR2(1),
    PROF_WITNESS_NOT_REQUIRED_YN   VARCHAR2(1),
    PROF_WITNESS_NOTIFIED_YN       VARCHAR2(1),
    PROF_WITNESS_PERS_SERVICE_YN   VARCHAR2(1),
    PROF_WITNESS_CONFIRMED_YN      VARCHAR2(1),
    PROF_WITNESS_CONFIRMED_DT      DATE,
    PROF_CROWN_DECISION_DT         DATE,
    PROF_DOC_PRODUCED_YN           VARCHAR2(1),
    PROF_COMMENT_TXT               VARCHAR2(4000),
    PROF_MEDICAL_DISORDER_TXT      VARCHAR2(2000),
    PROF_AC_LOCATION_TXT           VARCHAR2(40),
    PROF_ORG_NM                    VARCHAR2(150),
    PROF_UPPER_ORG_NM              VARCHAR2(150),
    PROF_UPD_DT                    DATE,
    PROF_UPD_USER_ID               VARCHAR2(30),
    PROF_CROWN_DECISION_AGEN_ID    NUMBER(15, 4)
        constraint PROF_AGEN_FK1
            references JUSTIN_AGENCIES,
    PROF_GENDER_CD                 VARCHAR2(3),
    CDCX_COMPLEXION_CD             VARCHAR2(3),
    CDHR_HAIR_COLOUR_CD            VARCHAR2(3),
    CDNM_NAME_TITLE_CD             VARCHAR2(3),
    PROF_DEFAULT_NOTIF_METHOD_CD   VARCHAR2(3),
    CDBT_BUSINESS_TYPE_CD          VARCHAR2(3),
    PROF_CROWN_DECISION_CD         VARCHAR2(3),
    CDBL_BUILD_CD                  VARCHAR2(3),
    CDRC_RACE_CD                   VARCHAR2(3),
    PROF_MARITAL_STATUS_CD         VARCHAR2(3),
    CDCZ_CITIZEN_CD                VARCHAR2(3),
    CDEY_EYE_COLOUR_CD             VARCHAR2(3),
    CDLN_LANGUAGE_CD               VARCHAR2(3)
        constraint PROF_CDLN_FK
            references JUSTIN_CD_LANGUAGES,
    CDCO_COOPERATION_CD            VARCHAR2(3),
    CDPR_PROCESS_TYPE_CD           VARCHAR2(3),
    PROF_AC_DURATION_AREA_UNIT_CD  VARCHAR2(4),
    PROF_AC_HEIGHT_PRIMARY_UNIT_CD VARCHAR2(4),
    PROF_AC_WEIGHT_UNIT_CD         VARCHAR2(4),
    PASSPORT_ISSUED_CTRY_ID        NUMBER(8)
        constraint PROF_CTRY_FK
            references JUSTIN_COUNTRIES,
    PROF_CROWN_DECISION_PART_ID    NUMBER(15, 4),
    DRIVER_LIC_ISSUE_CTRY_ID       NUMBER(8)
        constraint PROF_CTRY_FK2
            references JUSTIN_COUNTRIES,
    DRIVER_LIC_ISSUE_PROV_SEQ_NO   NUMBER(4),
    DACL_ACCESS_LEVEL_NO           NUMBER(2),
    POLICE_WITNESS_AGEN_ID         NUMBER(15, 4),
    POLICE_WITNESS_PAAS_SEQ_NO     NUMBER(3),
    PROF_STYLE_OF_CAUSE_SEQ_NO     NUMBER(3),
    PROF_PRELIM_INQUIRY_YN         VARCHAR2(1)
        constraint AVCON_1109098417_PROF__012
            check (PROF_PRELIM_INQUIRY_YN IN ('Y', 'N')),
    PROF_BIRTH_PLACE_TXT           VARCHAR2(150),
    PROF_OCCUPATION_TXT            VARCHAR2(50),
    PROF_DIVERSION_EXISTS_YN       VARCHAR2(1)
        constraint AVCON_1109098417_PROF__013
            check (PROF_DIVERSION_EXISTS_YN IN ('Y', 'N')),
    SEAL_DT                        DATE,
    SEAL_TYPE_CD                   VARCHAR2(4),
    ASTP_ASSET_TYPE_CD             VARCHAR2(3),
    EXCLUDE_FROM_DNA_PROCESSING_YN VARCHAR2(1)
        constraint AVCON_1195157193_EXCLU_000
            check (EXCLUDE_FROM_DNA_PROCESSING_YN IN ('Y', 'N')),
    FRENCH_TRIAL_GRANTED_YN        VARCHAR2(1)
        constraint AVCON_1203196324_FRENC_000
            check (FRENCH_TRIAL_GRANTED_YN IN ('Y', 'N')),
    EXCLUDE_FROM_SOR_PROCESSING_YN VARCHAR2(1)
        constraint AVCON_1318538389_EXCLU_001
            check (EXCLUDE_FROM_SOR_PROCESSING_YN IN ('Y', 'N')),
    CROWN_DECISION_UPD_AGEN_ID     NUMBER(15, 4),
    CROWN_DECISION_UPD_PART_ID     NUMBER(15, 4),
    CROWN_DECISION_UPD_PAAS_SEQ_NO NUMBER(3),
    POTENTIAL_DO_LTO_YN            VARCHAR2(1)
        constraint AVCON_1557863206_POTEN_000
            check (POTENTIAL_DO_LTO_YN IN ('Y', 'N')),
    HROIP_REFERRAL_YN              VARCHAR2(1)
        constraint AVCON_1557863206_HROIP_000
            check (HROIP_REFERRAL_YN IN ('Y', 'N')),
    ELIGIBLE_FOR_YCJA_SEAL_DT      DATE,
    ELIGIBLE_FOR_YCJA_SEAL_TYPE_CD VARCHAR2(4)
        constraint PROF_CK4
            check (ELIGIBLE_FOR_YCJA_SEAL_TYPE_CD IS NULL
                OR ELIGIBLE_FOR_YCJA_SEAL_TYPE_CD IN ('NDIS', 'NDSP')),
    constraint PROF_PK
        primary key (PART_ID, PROF_SEQ_NO),
    constraint PROF_UK1
        unique (PROF_INFO_PART_ID, MDOC_JUSTIN_NO),
    constraint PROF_UK2
        unique (PROF_RCC_PART_ID, RCC_ID),
    constraint PROF_PROV_FK
        foreign key (DRIVER_LIC_ISSUE_CTRY_ID, DRIVER_LIC_ISSUE_PROV_SEQ_NO) references JUSTIN_PROVINCES,
    constraint PROF_CK2
        check ((PROF_DIVERSION_EXISTS_YN IS NOT NULL AND PROF_ASSOCIATION_CD = 'RCC')
            OR (PROF_DIVERSION_EXISTS_YN IS NULL)),
    constraint PROF_CK3
        check (-- multi-column FK from PARTICIPANT ASSIGNMENT to PARTICIPANT PROFILE
            DECODE(CROWN_DECISION_UPD_PART_ID, null, 0, 1) +
            DECODE(CROWN_DECISION_UPD_AGEN_ID, null, 0, 1) +
            DECODE(CROWN_DECISION_UPD_PAAS_SEQ_NO, null, 0, 1)
                in (0, 3))
)
/

create table JUSTIN_ADDRESSES
(
    PART_ID                 NUMBER(15, 4) not null,
    ADRS_SEQ_NO             NUMBER(4)     not null,
    CDAD_ADDRESS_TYPE_CD    VARCHAR2(3)   not null,
    ADRS_CONFIDENTIAL_YN    VARCHAR2(1)   not null,
    ADRS_START_DT           DATE          not null,
    ADRS_ENT_DT             DATE          not null,
    ADRS_ENT_USER_ID        VARCHAR2(30)  not null,
    ADRS_ADDRESS_LINE_1_TXT VARCHAR2(40),
    ADRS_ADDRESS_LINE_2_TXT VARCHAR2(40),
    ADRS_ADDRESS_LINE_3_TXT VARCHAR2(40),
    ADRS_POSTAL_CODE_TXT    VARCHAR2(10),
    CITY_CTRY_ID            NUMBER(8),
    CITY_SEQ_NO             NUMBER(4),
    PROV_CTRY_ID            NUMBER(8),
    CTRY_ID                 NUMBER(8)
        constraint ADRS_CTRY_FK
            references JUSTIN_COUNTRIES,
    PROV_SEQ_NO             NUMBER(4),
    ADRS_CONTACT_NM         VARCHAR2(40),
    ADRS_PHONE              VARCHAR2(13),
    ADRS_UPD_DT             DATE,
    ADRS_UPD_USER_ID        VARCHAR2(30),
    ADRS_END_DT             DATE,
    PROF_SEQ_NO             NUMBER(10),
    constraint ADRS_PK
        primary key (PART_ID, ADRS_SEQ_NO),
    constraint ADRS_CITY_FK
        foreign key (CITY_CTRY_ID, CITY_SEQ_NO) references JUSTIN_CITIES,
    constraint ADRS_PROF_FK
        foreign key (PART_ID, PROF_SEQ_NO) references JUSTIN_PARTICIPANT_PROFILES,
    constraint ADRS_PROV_FK
        foreign key (PROV_CTRY_ID, PROV_SEQ_NO) references JUSTIN_PROVINCES,
    constraint ADRS_CK1
        check (ADRS_START_DT <= ADRS_END_DT)
)
/

create index ADRS_PROF_FK
    on JUSTIN_ADDRESSES (PART_ID, PROF_SEQ_NO)
/

create index ADRS_CTRY_FK
    on JUSTIN_ADDRESSES (CTRY_ID)
/

create index ADRS_CITY_FK
    on JUSTIN_ADDRESSES (CITY_CTRY_ID, CITY_SEQ_NO)
/

create index ADRS_PROV_FK
    on JUSTIN_ADDRESSES (PROV_CTRY_ID, PROV_SEQ_NO)
/

create table JUSTIN_PARTICIPANT_DOCUMENTS
(
    DOCM_ID                     NUMBER(15, 4) not null
        constraint PADO_DOCM_FK
            references JUSTIN_DOCUMENTS,
    PROF_PART_ID                NUMBER(15, 4) not null,
    PROF_SEQ_NO                 NUMBER(10)    not null,
    PADO_PRIMARY_YN             VARCHAR2(1)   not null,
    PADO_ENT_DT                 DATE          not null,
    PADO_ENT_USER_ID            VARCHAR2(30)  not null,
    PADO_UPD_DT                 DATE,
    PADO_UPD_USER_ID            VARCHAR2(30),
    DDSP_CD                     VARCHAR2(3),
    PADO_DISPOSITION_DT         DATE,
    ASSESSED_BY_COMM_PROCESS_YN VARCHAR2(1)
        constraint AVCON_1573066366_ASSES_000
            check (ASSESSED_BY_COMM_PROCESS_YN IN ('Y', 'N')),
    constraint PADO_PK
        primary key (DOCM_ID, PROF_PART_ID, PROF_SEQ_NO),
    constraint PADO_PROF_FK
        foreign key (PROF_PART_ID, PROF_SEQ_NO) references JUSTIN_PARTICIPANT_PROFILES
)
/

create unique index PADO_SK1
    on JUSTIN_PARTICIPANT_DOCUMENTS (PROF_PART_ID, PROF_SEQ_NO, DOCM_ID)
/

create index PADO_SK2
    on JUSTIN_PARTICIPANT_DOCUMENTS (DOCM_ID, PADO_PRIMARY_YN)
/

create index PADO_DDSP_FK
    on JUSTIN_PARTICIPANT_DOCUMENTS (DDSP_CD)
/

create index PADO_PROF_FK
    on JUSTIN_PARTICIPANT_DOCUMENTS (PROF_PART_ID, PROF_SEQ_NO)
/

create index PADO_SK3
    on JUSTIN_PARTICIPANT_DOCUMENTS (DECODE("DDSP_CD", NULL, 'Y', NULL))
/

create index PADO_DOCM_FK
    on JUSTIN_PARTICIPANT_DOCUMENTS (DOCM_ID)
/

create index PROF_SK2
    on JUSTIN_PARTICIPANT_PROFILES (RCC_ID)
/

create index PROF_PAAS_FK
    on JUSTIN_PARTICIPANT_PROFILES (POLICE_WITNESS_AGEN_ID, PART_ID, POLICE_WITNESS_PAAS_SEQ_NO)
/

create index PROF_SK1
    on JUSTIN_PARTICIPANT_PROFILES (MDOC_JUSTIN_NO)
/

create index PROF_PART_FK1
    on JUSTIN_PARTICIPANT_PROFILES (PROF_CROWN_DECISION_PART_ID)
/

create index PROF_SK3
    on JUSTIN_PARTICIPANT_PROFILES (PROF_CROWN_DECISION_CD)
/

create index PROF_AGEN_FK1
    on JUSTIN_PARTICIPANT_PROFILES (PROF_CROWN_DECISION_AGEN_ID)
/

create index PROF_SK4
    on JUSTIN_PARTICIPANT_PROFILES (PROF_SURNAME_UPPER_NM, PROF_UPPER_GIVEN_1_NM, MDOC_JUSTIN_NO, PROF_AC_PARDONED_YN)
/

create index PROF_SK5
    on JUSTIN_PARTICIPANT_PROFILES (PROF_UPPER_ORG_NM, MDOC_JUSTIN_NO, PROF_AC_PARDONED_YN)
/

create unique index PROF_SK6
    on JUSTIN_PARTICIPANT_PROFILES (MDOC_JUSTIN_NO, PROF_INFO_PART_ID, PART_ID, PROF_SEQ_NO)
/

create index PROF_SK8
    on JUSTIN_PARTICIPANT_PROFILES (PROF_AC_PARDONED_YN, SEAL_DT)
/

create index PROF_PROV_FK
    on JUSTIN_PARTICIPANT_PROFILES (DRIVER_LIC_ISSUE_CTRY_ID, DRIVER_LIC_ISSUE_PROV_SEQ_NO)
/

create index PROF_CTRY_FK2
    on JUSTIN_PARTICIPANT_PROFILES (DRIVER_LIC_ISSUE_CTRY_ID)
/

create index PROF_CTRY_FK
    on JUSTIN_PARTICIPANT_PROFILES (PASSPORT_ISSUED_CTRY_ID)
/

create index PROF_PAAS_FK1
    on JUSTIN_PARTICIPANT_PROFILES (CROWN_DECISION_UPD_AGEN_ID, CROWN_DECISION_UPD_PART_ID,
                                    CROWN_DECISION_UPD_PAAS_SEQ_NO)
/

create index PROF_SLTP_FK
    on JUSTIN_PARTICIPANT_PROFILES (SEAL_TYPE_CD)
/

create index PROF_SLTP_FK1
    on JUSTIN_PARTICIPANT_PROFILES (ELIGIBLE_FOR_YCJA_SEAL_TYPE_CD)
/

create table JUSTIN_PARTICIPANT_ROLES
(
    PROF_PART_ID         NUMBER(15, 4) not null,
    PROF_SEQ_NO          NUMBER(10)    not null,
    PRLT_ROLE_TYPE_CD    VARCHAR2(3)   not null,
    PROL_PMA_CD          VARCHAR2(1)   not null
        constraint PROL_CK1
            check (PROL_PMA_CD IN ('A', 'B', 'C', 'T')),
    PROL_ENT_DT          DATE          not null,
    PROL_ENT_USER_ID     VARCHAR2(30)  not null,
    PROL_UPD_DT          DATE,
    PROL_UPD_USER_ID     VARCHAR2(30),
    SRLT_SUBROLE_TYPE_CD VARCHAR2(3),
    EXPERT_WITNESS_YN    VARCHAR2(1)
        constraint AVCON_1652461641_EXPER_000
            check (EXPERT_WITNESS_YN IN ('Y', 'N')),
    constraint PROL_PK
        primary key (PROF_PART_ID, PROF_SEQ_NO, PRLT_ROLE_TYPE_CD),
    constraint PROL_PROF_FK
        foreign key (PROF_PART_ID, PROF_SEQ_NO) references JUSTIN_PARTICIPANT_PROFILES,
    constraint PROL_CK2
        check ((PRLT_ROLE_TYPE_CD <> 'WIT'
            AND NVL(EXPERT_WITNESS_YN, 'N') = 'N'
                   )
            OR
               (PRLT_ROLE_TYPE_CD = 'WIT'
                   AND SRLT_SUBROLE_TYPE_CD not in ('PO', 'PAR', 'PRO')
                   AND NVL(EXPERT_WITNESS_YN, 'N') = 'N'
                   )
            OR
               (PRLT_ROLE_TYPE_CD = 'WIT'
                   AND SRLT_SUBROLE_TYPE_CD in ('PO', 'PAR', 'PRO')
                   ))
)
/

create index PHYS_RCC_FK
    on JUSTIN_PHYSICAL_FILES (RCC_ID)
/

create index PHYS_CCAS_FK
    on JUSTIN_PHYSICAL_FILES (CCAS_CASE_NO)
/

create unique index PHYS_SK2
    on JUSTIN_PHYSICAL_FILES (PHYS_COURT_CLASS_CD, PHYS_COURT_LEVEL_CD, PHYS_COURT_DIVISION_CD,
                              PHYS_FILE_FOLDER_HOME_LOCATION, PHYS_TICKET_SERIES_TXT, PHYS_FILE_FOLDER_NO, PHYS_ID)
/

create index PHYS_AGEN_FK
    on JUSTIN_PHYSICAL_FILES (PHYS_FILE_FOLDER_HOME_LOCATION)
/

create index RCC_SK3
    on JUSTIN_RCCS (RCC_MOST_RECENT_SUBMIT_DT, RCC_DECISION_LOCN_AGEN_ID)
/

create index RCC_SK2
    on JUSTIN_RCCS (RCC_STATE_CD)
/

create index RCC_PART_FK
    on JUSTIN_RCCS (PART_APPROVED_BY_ID)
/

create index RCC_SK1
    on JUSTIN_RCCS (RCC_AGENCY_FILE_NO)
/

create index RCC_PART_FK4
    on JUSTIN_RCCS (APPROVAL_OFFICER_PART_ID)
/

create index RCC_PART_FK3
    on JUSTIN_RCCS (RCC_DECISION_CROWN_PART_ID)
/

create index RCC_PART_FK2
    on JUSTIN_RCCS (PART_SUBM_APPROVED_BY_ID)
/

create index RCC_PART_FK1
    on JUSTIN_RCCS (PART_INITIATED_BY_ID)
/

create index RCC_SK4
    on JUSTIN_RCCS (RCC_DECISION_DT, RCC_DECISION_LOCN_AGEN_ID)
/

create unique index RCC_SK5
    on JUSTIN_RCCS (RCC_FILE_FOLDER_HOME_AGEN_ID, RCC_AGENCY_FILE_NO, RCC_ID)
/

create index RCC_AGEN_FK
    on JUSTIN_RCCS (PROPOSED_CRT_LOC_AGEN_ID)
/

create index RCC_AGEN_FK2
    on JUSTIN_RCCS (RCC_DECISION_LOCN_AGEN_ID)
/

create index RCC_AGEN_FK3
    on JUSTIN_RCCS (JURISDICTION_AGEN_ID)
/

create index RCC_PAAS_FK
    on JUSTIN_RCCS (CLEARANCE_PAAS_AGEN_ID, CLEARANCE_PAAS_PART_ID, CLEARANCE_PAAS_SEQ_NO)
/

create index RCC_SK6
    on JUSTIN_RCCS (RCC_DECISION_LOCN_AGEN_ID, RCC_STATE_CD)
/

create index RCC_SK7
    on JUSTIN_RCCS (RCC_FILE_FOLDER_HOME_AGEN_ID, RCC_STATE_CD)
/

create index RCC_FLDS_FK
    on JUSTIN_RCCS (FILE_DESIGNATION_CD)
/

create index RCC_AGEN_FK4
    on JUSTIN_RCCS (INITIAL_CHARGE_ASMT_AGEN_ID)
/

create index RCC_PAAS_FK2_I
    on JUSTIN_RCCS (PRFSNL_STAFF_AGEN_ID, PRFSNL_STAFF_PART_ID, PRFSNL_STAFF_PAAS_SEQ_NO)
/

create table JUSTIN_STATUTES
(
    STAT_ID                     NUMBER(8)    not null
        constraint STAT_PK
            primary key,
    STAT_EFFECTIVE_DT           DATE         not null,
    STAT_SECTION_TXT            VARCHAR2(10) not null,
    STAT_VALID_REFERENCE_YN     VARCHAR2(1)  not null
        constraint AVCON_208458_STAT__000
            check (STAT_VALID_REFERENCE_YN IN ('Y', 'N')),
    STAT_PRESUMPTIVE_OFFENCE_YN VARCHAR2(1)  not null
        constraint AVCON_208458_STAT__001
            check (STAT_PRESUMPTIVE_OFFENCE_YN IN ('Y', 'N')),
    STAT_ENT_DT                 DATE         not null,
    STAT_ENT_USER_ID            VARCHAR2(30) not null,
    ACT_CD                      VARCHAR2(3)  not null,
    STAT_SUB_SECTION_TXT        VARCHAR2(4),
    STAT_PARAGRAPH_TXT          VARCHAR2(3),
    STAT_SUB_PARAGRAPH_TXT      VARCHAR2(5),
    STAT_CLAUSE_TXT             VARCHAR2(4),
    STAT_SHORT_DESCRIPTION_TXT  VARCHAR2(50),
    STAT_DESCRIPTION_TXT        VARCHAR2(2000),
    STAT_TERMINATION_DT         DATE,
    STAT_CCJIS_CODE_TXT         VARCHAR2(6),
    STAT_UCR_CODE_NO            NUMBER(5),
    STAT_LIMITATION_NO          NUMBER(4),
    STAT_UPD_DT                 DATE,
    STAT_UPD_USER_ID            VARCHAR2(30),
    STAT_CHARGE_TYPE_CD         VARCHAR2(3),
    STAT_CRRP_INTEREST_YN       VARCHAR2(1)
        constraint AVCON_1077920395_STAT__002
            check (STAT_CRRP_INTEREST_YN IN ('Y', 'N')),
    RETENTION_PERIOD_CD         VARCHAR2(4),
    SENTENCE_FOR_STAT_ID        NUMBER(8)
        constraint STAT_STAT_FK
            references JUSTIN_STATUTES,
    constraint STAT_UK1
        unique (ACT_CD, STAT_EFFECTIVE_DT, STAT_SECTION_TXT, STAT_SUB_SECTION_TXT, STAT_PARAGRAPH_TXT,
                STAT_SUB_PARAGRAPH_TXT, STAT_CLAUSE_TXT)
)
/

create table JUSTIN_MATTERS
(
    RCC_ID                      NUMBER(15, 4) not null
        constraint MATT_RCC_FK
            references JUSTIN_RCCS,
    MATT_SEQ_NO                 NUMBER(3)     not null,
    MATT_TYPE                   VARCHAR2(10)  not null
        constraint MATT_CK1
            check (matt_type in ('CNT')),
    MATT_PMA_CD                 VARCHAR2(1)   not null
        constraint MATT_CK2
            check (MATT_PMA_CD IN ('A', 'B', 'C', 'T')),
    MATT_ENT_DT                 DATE          not null,
    MATT_ENT_USER_ID            VARCHAR2(30)  not null,
    MATT_OFFENCE_DT             DATE,
    MATT_OFFENCE_END_DT         DATE,
    MATT_CATEGORY_CD            VARCHAR2(3)
        constraint MATT_CK3
            check (MATT_CATEGORY_CD IN ('1', '2', '3', '4', '5', '6')),
    MATT_SYNOPSIS               VARCHAR2(500),
    MATT_FPRED_COMPLETE_YN      VARCHAR2(1)
        constraint AVCON_208808_MATT__000
            check (MATT_FPRED_COMPLETE_YN IN ('Y', 'N')),
    MATT_SEARCH_WARRANT_USED_YN VARCHAR2(1)
        constraint AVCON_208808_MATT__001
            check (MATT_SEARCH_WARRANT_USED_YN IN ('Y', 'N')),
    MATT_EXHIBITS_INVOLVED_YN   VARCHAR2(1)
        constraint AVCON_208808_MATT__002
            check (MATT_EXHIBITS_INVOLVED_YN IN ('Y', 'N')),
    MATT_APPLICATION_DETAIL_TXT VARCHAR2(2000),
    MATT_UPD_DT                 DATE,
    MATT_UPD_USER_ID            VARCHAR2(30),
    STAT_ID                     NUMBER(8)
        constraint MATT_STAT_FK
            references JUSTIN_STATUTES,
    MATT_PRECEDENCE_NO          NUMBER(8),
    MATT_NATURE_CD              VARCHAR2(3),
    DISPLAY_ORDER_NBR           NUMBER(4),
    constraint MATT_PK
        primary key (RCC_ID, MATT_SEQ_NO)
)
/

create index MATT_STAT_FK
    on JUSTIN_MATTERS (STAT_ID)
/

create index MATT_CHGP_FK
    on JUSTIN_MATTERS (STAT_ID, MATT_PRECEDENCE_NO)
/

create table JUSTIN_MATTER_DOC_COUNTS
(
    MDOC_JUSTIN_NO             NUMBER(12)    not null
        constraint MDCT_MDOC_FK
            references JUSTIN_MATTER_DOCS,
    MDCT_SEQ_NO                NUMBER(3)     not null,
    MDCT_ENT_DT                DATE          not null,
    MDCT_ENT_USER_ID           VARCHAR2(30)  not null,
    MATT_RCC_ID                NUMBER(15, 4) not null,
    MATT_SEQ_NO                NUMBER(3)     not null,
    MDCT_PRINT_SEQ_NO          NUMBER(3),
    MDCT_DISPUTE_ALLEGATION_YN VARCHAR2(1)
        constraint AVCON_210025_MDCT__000
            check (MDCT_DISPUTE_ALLEGATION_YN IN ('Y', 'N')),
    MDCT_DISPUTE_FINE_YN       VARCHAR2(1)
        constraint AVCON_210025_MDCT__001
            check (MDCT_DISPUTE_FINE_YN IN ('Y', 'N')),
    MDCT_UPD_DT                DATE,
    MDCT_UPD_USER_ID           VARCHAR2(30),
    FPR_MATT_RCC_ID            NUMBER(15, 4),
    FPR_MATT_SEQ_NO            NUMBER(3),
    FPR_SEQ_NO                 NUMBER(4),
    MDCT_TICKET_AMT            NUMBER(7, 2),
    constraint MDCT_PK
        primary key (MDOC_JUSTIN_NO, MDCT_SEQ_NO),
    constraint MDCT_MATT_FK
        foreign key (MATT_RCC_ID, MATT_SEQ_NO) references JUSTIN_MATTERS
)
/

create index MDCT_MATT_FK
    on JUSTIN_MATTER_DOC_COUNTS (MATT_RCC_ID, MATT_SEQ_NO)
/

create index MDCT_SK1
    on JUSTIN_MATTER_DOC_COUNTS (MATT_RCC_ID)
/

create index MDCT_SK2
    on JUSTIN_MATTER_DOC_COUNTS (FPR_MATT_RCC_ID)
/

create index MDCT_FPR_FK
    on JUSTIN_MATTER_DOC_COUNTS (FPR_MATT_RCC_ID, FPR_MATT_SEQ_NO, FPR_SEQ_NO)
/

create index MDCT_SK3
    on JUSTIN_MATTER_DOC_COUNTS (MATT_RCC_ID, MATT_SEQ_NO, MDOC_JUSTIN_NO, MDCT_SEQ_NO)
/

create index MDCT_SK4
    on JUSTIN_MATTER_DOC_COUNTS (MDOC_JUSTIN_NO, MDCT_SEQ_NO, MATT_RCC_ID, MATT_SEQ_NO, MDCT_PRINT_SEQ_NO)
/

create table JUSTIN_MDPR_COUNTS
(
    MDCT_JUSTIN_NO              NUMBER(12)    not null,
    MDCT_SEQ_NO                 NUMBER(3)     not null,
    MDPR_SEQ_NO                 NUMBER(3)     not null,
    MDPR_ENT_DT                 DATE          not null,
    MDPR_ENT_USER_ID            VARCHAR2(30)  not null,
    PROF_PART_ID                NUMBER(15, 4) not null,
    PROF_SEQ_NO                 NUMBER(10)    not null,
    MDPR_PRINT_SEQ_NO           NUMBER(3),
    MDPR_UPD_DT                 DATE,
    MDPR_UPD_USER_ID            VARCHAR2(30),
    LESSER_INCLUDED_STAT_ID     NUMBER(8)
        constraint MDPR_STAT_FK
            references JUSTIN_STATUTES,
    MANA_NATURE_CD              VARCHAR2(3),
    PUBLIC_ACCESS_RESTRICTED_YN VARCHAR2(1)   not null,
    SENTENCE_END_DT             DATE,
    ELIGIBLE_FOR_YCJA_SEAL_DT   DATE,
    constraint MDPR_PK
        primary key (MDCT_JUSTIN_NO, MDCT_SEQ_NO, MDPR_SEQ_NO),
    constraint MDPR_MDCT_FK
        foreign key (MDCT_JUSTIN_NO, MDCT_SEQ_NO) references JUSTIN_MATTER_DOC_COUNTS,
    constraint MDPR_PROF_FK
        foreign key (PROF_PART_ID, PROF_SEQ_NO) references JUSTIN_PARTICIPANT_PROFILES
)
/

create table JUSTIN_APPEARANCE_COUNTS
(
    APPC_ID                   NUMBER(15, 4) not null
        constraint APPC_PK
            primary key,
    APPC_PART_OF_TRIAL_YN     VARCHAR2(1)   not null
        constraint AVCON_210323_APPC__000
            check (APPC_PART_OF_TRIAL_YN IN ('Y', 'N')),
    APPC_CONFIRMED_YN         VARCHAR2(1)   not null
        constraint AVCON_210323_APPC__001
            check (APPC_CONFIRMED_YN IN ('Y', 'N')),
    APPC_ENT_DT               DATE          not null,
    APPC_ENT_USER_ID          VARCHAR2(30)  not null,
    APPR_ID                   NUMBER(15, 4) not null
        constraint APPC_APPR_FK
            references JUSTIN_APPEARANCES,
    APPC_PROOF_OF_AGE_YN      VARCHAR2(1)
        constraint AVCON_210323_APPC__002
            check (APPC_PROOF_OF_AGE_YN IN ('Y', 'N')),
    APPC_ARRAIGNED_YN         VARCHAR2(1)
        constraint AVCON_210323_APPC__003
            check (APPC_ARRAIGNED_YN IN ('Y', 'N')),
    APPC_UPD_DT               DATE,
    APPC_UPD_USER_ID          VARCHAR2(30),
    MDPR_MDCT_JUSTIN_NO       NUMBER(12)    not null,
    MDPR_MDCT_SEQ_NO          NUMBER(3)     not null,
    MDPR_SEQ_NO               NUMBER(3)     not null,
    NEXT_APPC_ID              NUMBER(15, 4)
        constraint APPC_APPC_FK
            references JUSTIN_APPEARANCE_COUNTS,
    ELEC_ELECTION_CD          VARCHAR2(3),
    RSN_APPEARANCE_REASON_CD  VARCHAR2(3),
    RSLT_APPEARANCE_RESULT_CD VARCHAR2(3),
    FIND_FINDING_CD           VARCHAR2(3),
    PLEA_PLEA_CD              VARCHAR2(3)
        constraint APPC_PLEA_FK
            references JUSTIN_CD_PLEAS,
    APPC_CANCELLED_YN         VARCHAR2(1)
        constraint AVCON_1259096429_APPC__004
            check (APPC_CANCELLED_YN IN ('Y', 'N')),
    constraint APPC_MDPR_FK
        foreign key (MDPR_MDCT_JUSTIN_NO, MDPR_MDCT_SEQ_NO, MDPR_SEQ_NO) references JUSTIN_MDPR_COUNTS
)
/

create index APPC_APPR_FK
    on JUSTIN_APPEARANCE_COUNTS (APPR_ID)
/

create index APPC_MDPR_FK
    on JUSTIN_APPEARANCE_COUNTS (MDPR_MDCT_JUSTIN_NO, MDPR_MDCT_SEQ_NO, MDPR_SEQ_NO)
/

create index APPC_APPC_FK
    on JUSTIN_APPEARANCE_COUNTS (NEXT_APPC_ID)
/

create unique index APPC_SK1
    on JUSTIN_APPEARANCE_COUNTS (MDPR_MDCT_JUSTIN_NO, MDPR_MDCT_SEQ_NO, MDPR_SEQ_NO, RSLT_APPEARANCE_RESULT_CD,
                                 RSN_APPEARANCE_REASON_CD, APPR_ID, APPC_ID, NEXT_APPC_ID)
/

create index APPC_SK2
    on JUSTIN_APPEARANCE_COUNTS (DECODE("RSLT_APPEARANCE_RESULT_CD", 'SL', "APPC_ID", 'SOL', "APPC_ID", NULL))
/

create index APPC_SK3
    on JUSTIN_APPEARANCE_COUNTS (APPR_ID, RSLT_APPEARANCE_RESULT_CD, RSN_APPEARANCE_REASON_CD, APPC_CONFIRMED_YN,
                                 NVL("APPC_CANCELLED_YN", 'N'), MDPR_SEQ_NO, MDPR_MDCT_SEQ_NO, MDPR_MDCT_JUSTIN_NO,
                                 APPC_ID, NEXT_APPC_ID)
/

create index APPC_SK4
    on JUSTIN_APPEARANCE_COUNTS (RSN_APPEARANCE_REASON_CD, APPC_CONFIRMED_YN, NVL("APPC_CANCELLED_YN", 'N'), APPR_ID,
                                 MDPR_MDCT_JUSTIN_NO, MDPR_MDCT_SEQ_NO, MDPR_SEQ_NO)
/

create index APPC_RSLT_FK
    on JUSTIN_APPEARANCE_COUNTS (RSLT_APPEARANCE_RESULT_CD)
/

create unique index MDPR_SK1
    on JUSTIN_MDPR_COUNTS (PROF_PART_ID, PROF_SEQ_NO, MDCT_JUSTIN_NO, MDCT_SEQ_NO, MDPR_SEQ_NO)
/

create index MDPR_SK2
    on JUSTIN_MDPR_COUNTS (MDCT_JUSTIN_NO, MDCT_SEQ_NO, MDPR_SEQ_NO, PROF_PART_ID, PROF_SEQ_NO, LESSER_INCLUDED_STAT_ID)
/

create index MDPR_STAT_FK
    on JUSTIN_MDPR_COUNTS (LESSER_INCLUDED_STAT_ID)
/

create index MDPR_SK3
    on JUSTIN_MDPR_COUNTS (MDCT_JUSTIN_NO, PUBLIC_ACCESS_RESTRICTED_YN, PROF_PART_ID, PROF_SEQ_NO, MDCT_SEQ_NO)
/

create index STAT_SK1
    on JUSTIN_STATUTES (ACT_CD, STAT_SECTION_TXT)
/

create index STAT_STAT_FK
    on JUSTIN_STATUTES (SENTENCE_FOR_STAT_ID)
/

create global temporary table TOAD_PLAN_TABLE
(
    STATEMENT_ID      VARCHAR2(30),
    PLAN_ID           NUMBER,
    TIMESTAMP         DATE,
    REMARKS           VARCHAR2(4000),
    OPERATION         VARCHAR2(30),
    OPTIONS           VARCHAR2(255),
    OBJECT_NODE       VARCHAR2(128),
    OBJECT_OWNER      VARCHAR2(30),
    OBJECT_NAME       VARCHAR2(30),
    OBJECT_ALIAS      VARCHAR2(65),
    OBJECT_INSTANCE   NUMBER,
    OBJECT_TYPE       VARCHAR2(30),
    OPTIMIZER         VARCHAR2(255),
    SEARCH_COLUMNS    NUMBER,
    ID                NUMBER,
    PARENT_ID         NUMBER,
    DEPTH             NUMBER,
    POSITION          NUMBER,
    COST              NUMBER,
    CARDINALITY       NUMBER,
    BYTES             NUMBER,
    OTHER_TAG         VARCHAR2(255),
    PARTITION_START   VARCHAR2(255),
    PARTITION_STOP    VARCHAR2(255),
    PARTITION_ID      NUMBER,
    OTHER             LONG,
    DISTRIBUTION      VARCHAR2(30),
    CPU_COST          NUMBER,
    IO_COST           NUMBER,
    TEMP_SPACE        NUMBER,
    ACCESS_PREDICATES VARCHAR2(4000),
    FILTER_PREDICATES VARCHAR2(4000),
    PROJECTION        VARCHAR2(4000),
    TIME              NUMBER,
    QBLOCK_NAME       VARCHAR2(30),
    OTHER_XML         CLOB
)
    on commit preserve rows
/

create table JUSTIN_IMAGES
(
    IMAGE_ID              NUMBER(15, 4) not null
        constraint IMAG_PK
            primary key,
    OBJECT_GUID           VARCHAR2(100) not null
        constraint IMAG_UK
            unique,
    CURRENT_VERSION_YN    VARCHAR2(1)   not null
        constraint AVCON_1305324251_CURRE_000
            check (CURRENT_VERSION_YN IN ('Y', 'N')),
    SCANNED_IMAGE_YN      VARCHAR2(1)   not null
        constraint AVCON_1305324251_SCANN_000
            check (SCANNED_IMAGE_YN IN ('Y', 'N')),
    SIGNED_IMAGE_YN       VARCHAR2(1)
        constraint AVCON_1305324251_SIGNE_000
            check (SIGNED_IMAGE_YN IN ('Y', 'N')),
    DOCUMENT_PAGE_QTY     NUMBER(4),
    ENT_DTM               DATE          not null,
    ENT_USER_ID           VARCHAR2(30)  not null,
    UPD_DTM               DATE,
    UPD_USER_ID           VARCHAR2(30),
    DOCM_ID               NUMBER(15, 4)
        constraint IMAG_DOCM_FK
            references JUSTIN_DOCUMENTS,
    MDOC_JUSTIN_NO        NUMBER(12)
        constraint IMAG_MDOC_FK
            references JUSTIN_MATTER_DOCS,
    PDF_VERSION_TXT       VARCHAR2(5),
    NOTI_PROF_PART_ID     NUMBER(15, 4),
    NOTI_PROF_SEQ_NO      NUMBER(10),
    NOTI_MDOC_JUSTIN_NO   NUMBER(12),
    NOTI_SEQ_NO           NUMBER(3),
    TELE_INFORMATION_ID   NUMBER(15),
    IMAGE_CONTENT_XML_TXT CLOB,
    AUTO_GENERATED_YN     VARCHAR2(1)
        constraint AVCON_1614192205_AUTO__000
            check (AUTO_GENERATED_YN IN ('Y', 'N')),
    constraint IMAG_CK
        check (DECODE(DOCM_ID, NULL, 0, 1) + DECODE(MDOC_JUSTIN_NO, NULL, 0, 1) +
               DECODE(NOTI_PROF_PART_ID, NULL, 0, 1) +
               DECODE(TELE_INFORMATION_ID, NULL, 0, 1) = 1),
    constraint IMAG_CK1
        check (DECODE(NOTI_PROF_PART_ID, NULL, 0, 1) +
               DECODE(NOTI_PROF_SEQ_NO, NULL, 0, 1) +
               DECODE(NOTI_MDOC_JUSTIN_NO, NULL, 0, 1) +
               DECODE(NOTI_SEQ_NO, NULL, 0, 1) IN (0, 4))
)
/

create index IMAG_DOCM_FK
    on JUSTIN_IMAGES (DOCM_ID)
/

create index IMAG_MDOC_FK
    on JUSTIN_IMAGES (MDOC_JUSTIN_NO)
/

create index IMAG_NOTI_FK
    on JUSTIN_IMAGES (NOTI_PROF_PART_ID, NOTI_PROF_SEQ_NO, NOTI_MDOC_JUSTIN_NO, NOTI_SEQ_NO)
/

create index IMAG_TINFO_FK
    on JUSTIN_IMAGES (TELE_INFORMATION_ID)
/

create table JUSTIN_OFFENCE_LOCATIONS
(
    MATT_RCC_ID             NUMBER(15, 4) not null,
    MATT_SEQ_NO             NUMBER(3)     not null,
    OFFENCE_LOCATION_SEQ_NO NUMBER(4)     not null,
    CITY_CTRY_ID            NUMBER(8),
    CITY_SEQ_NO             NUMBER(4),
    DETAIL_TXT              VARCHAR2(255),
    ENT_DTM                 DATE          not null,
    ENT_USER_ID             VARCHAR2(30)  not null,
    UPD_DTM                 DATE,
    UPD_USER_ID             VARCHAR2(30),
    DISPLAY_ORDER_NBR       NUMBER(4)     not null,
    constraint OFLO_PK
        primary key (MATT_RCC_ID, MATT_SEQ_NO, OFFENCE_LOCATION_SEQ_NO),
    constraint OFLO_CITY_FK
        foreign key (CITY_CTRY_ID, CITY_SEQ_NO) references JUSTIN_CITIES,
    constraint OFLO_MATT_FK
        foreign key (MATT_RCC_ID, MATT_SEQ_NO) references JUSTIN_MATTERS,
    constraint OFLO_CK1
        check ((CITY_CTRY_ID IS NOT NULL AND CITY_SEQ_NO IS NOT NULL)
            OR
               (CITY_CTRY_ID IS NULL
                   AND CITY_SEQ_NO IS NULL AND DETAIL_TXT IS NOT NULL))
)
/

create index OFLO_MATT_FK
    on JUSTIN_OFFENCE_LOCATIONS (MATT_RCC_ID, MATT_SEQ_NO)
/

create index OFLO_CITY_FK
    on JUSTIN_OFFENCE_LOCATIONS (CITY_CTRY_ID, CITY_SEQ_NO)
/

create table JUSTIN_STATUTE_DESIGNATIONS
(
    STAT_ID             NUMBER(8)    not null
        constraint STDS_STAT_FK
            references JUSTIN_STATUTES,
    STATUTE_INTEREST_CD VARCHAR2(4)  not null,
    ACTIVE_YN           VARCHAR2(1)  not null,
    ENT_DTM             DATE         not null,
    ENT_USER_ID         VARCHAR2(30) not null,
    UPD_DTM             DATE,
    UPD_USER_ID         VARCHAR2(30),
    constraint STDS_PK
        primary key (STAT_ID, STATUTE_INTEREST_CD)
)
/

create index STDS_STAT_FK
    on JUSTIN_STATUTE_DESIGNATIONS (STAT_ID)
/

create index STDS_STIN_FK
    on JUSTIN_STATUTE_DESIGNATIONS (STATUTE_INTEREST_CD)
/

create table JUSTIN_TICKET_INBOX_ENTRIES
(
    TICKET_INBOX_ENTRY_ID    NUMBER(15)   not null
        constraint TINE_PK
            primary key,
    TICKET_PROCESS_STATUS_CD VARCHAR2(10) not null
        constraint AVCON_1670872967_TICKE_000
            check (TICKET_PROCESS_STATUS_CD
                IN ('NEW', 'CLAIMED', 'ACCEPTED', 'REJECTED')),
    TRANSACTION_DTM          DATE         not null,
    TICKET_SERIES_TXT        VARCHAR2(2)  not null,
    FILE_FOLDER_NO           VARCHAR2(12) not null,
    FILED_DT                 DATE,
    VIOLATION_DT             DATE         not null,
    SURNAME_NM               VARCHAR2(30),
    GIVEN_1_NM               VARCHAR2(30),
    GIVEN_2_NM               VARCHAR2(30),
    COMMENT_TXT              VARCHAR2(4000),
    VALIDATED_YN             VARCHAR2(1),
    ENT_DTM                  DATE         not null,
    ENT_USER_ID              VARCHAR2(30) not null,
    UPD_DTM                  DATE,
    UPD_USER_ID              VARCHAR2(30),
    COURT_AGEN_ID            NUMBER(15, 4)
        constraint TINE_AGEN_FK1
            references JUSTIN_AGENCIES,
    ENFORCEMENT_AGEN_ID      NUMBER(15, 4)
        constraint TINE_AGEN_FK
            references JUSTIN_AGENCIES,
    PAAS_AGEN_ID             NUMBER(15, 4),
    PAAS_PART_ID             NUMBER(15, 4),
    PAAS_SEQ_NO              NUMBER(3),
    TICKET_MESSAGE_TYPE_CD   VARCHAR2(6)  not null,
    ADDRESS_LINE_1_TXT       VARCHAR2(40),
    ADDRESS_POSTAL_CODE_TXT  VARCHAR2(10),
    PROV_CTRY_ID             NUMBER(8),
    PROV_SEQ_NO              NUMBER(4),
    CITY_CTRY_ID             NUMBER(8),
    CITY_SEQ_NO              NUMBER(4),
    PART_TYPE_CD             VARCHAR2(3),
    GIVEN_3_NM               VARCHAR2(30),
    ORG_NM                   VARCHAR2(150),
    ADDRESS_LINE_2_TXT       VARCHAR2(40),
    ADDRESS_LINE_3_TXT       VARCHAR2(40),
    CTRY_ID                  NUMBER(8)
        constraint TINE_CTRY_FK
            references JUSTIN_COUNTRIES,
    SYSTEM_MESSAGE_TXT       VARCHAR2(4000),
    EMAIL_ADDRESS_TXT        VARCHAR2(100),
    constraint TINE_CITY_FK
        foreign key (CITY_CTRY_ID, CITY_SEQ_NO) references JUSTIN_CITIES,
    constraint TINE_PROV_FK
        foreign key (PROV_CTRY_ID, PROV_SEQ_NO) references JUSTIN_PROVINCES,
    constraint TINE_CK1
        check (DECODE(PROV_CTRY_ID, NULL, 0, 1) + DECODE(PROV_SEQ_NO, NULL, 0, 1) IN (0, 2)),
    constraint TINE_CK2
        check (DECODE(CITY_CTRY_ID, NULL, 0, 1) + DECODE(CITY_SEQ_NO, NULL, 0, 1) IN (0, 2))
)
/

create index TINE_AGEN_FK1
    on JUSTIN_TICKET_INBOX_ENTRIES (COURT_AGEN_ID)
/

create index TINE_AGEN_FK
    on JUSTIN_TICKET_INBOX_ENTRIES (ENFORCEMENT_AGEN_ID)
/

create index TINE_TMST_FK
    on JUSTIN_TICKET_INBOX_ENTRIES (TICKET_MESSAGE_TYPE_CD)
/

create index TINE_PAAS_FK
    on JUSTIN_TICKET_INBOX_ENTRIES (PAAS_AGEN_ID, PAAS_PART_ID, PAAS_SEQ_NO)
/

create index TINE_PROV_FK
    on JUSTIN_TICKET_INBOX_ENTRIES (PROV_CTRY_ID, PROV_SEQ_NO)
/

create index TINE_CITY_FK
    on JUSTIN_TICKET_INBOX_ENTRIES (CITY_CTRY_ID, CITY_SEQ_NO)
/

create index TINE_SK
    on JUSTIN_TICKET_INBOX_ENTRIES (TICKET_SERIES_TXT, FILE_FOLDER_NO)
/

create index TINE_CTRY_FK
    on JUSTIN_TICKET_INBOX_ENTRIES (CTRY_ID)
/

create table JUSTIN_TICKET_INBOX_COUNTS
(
    TICKET_INBOX_ENTRY_ID     NUMBER(15)   not null
        constraint TINC_TINE_FK
            references JUSTIN_TICKET_INBOX_ENTRIES,
    TICKET_INBOX_COUNT_SEQ_NO NUMBER(4)    not null,
    COUNT_SEQ_NO              NUMBER(4)    not null,
    SYSTEM_MESSAGE_TXT        VARCHAR2(4000),
    ENT_DTM                   DATE         not null,
    ENT_USER_ID               VARCHAR2(30) not null,
    UPD_DTM                   DATE,
    UPD_USER_ID               VARCHAR2(30),
    STAT_ID                   NUMBER(8)
        constraint TINC_STAT_FK
            references JUSTIN_STATUTES,
    ICBC_MESSAGE_ID           NUMBER(15)   not null,
    constraint TINC_PK
        primary key (TICKET_INBOX_ENTRY_ID, TICKET_INBOX_COUNT_SEQ_NO)
)
/

create index TINC_IMSG_FK
    on JUSTIN_TICKET_INBOX_COUNTS (ICBC_MESSAGE_ID)
/

create index TINC_TINE_FK
    on JUSTIN_TICKET_INBOX_COUNTS (TICKET_INBOX_ENTRY_ID)
/

create index TINC_STAT_FK
    on JUSTIN_TICKET_INBOX_COUNTS (STAT_ID)
/

create table JUSTIN_REPORT_OBJECTS
(
    REPORT_OBJECT_ID NUMBER(15)   not null
        constraint REOB_PK
            primary key,
    ENT_DTM          DATE         not null,
    ENT_USER_ID      VARCHAR2(30) not null,
    UPD_DTM          DATE,
    UPD_USER_ID      VARCHAR2(30)
)
/

create package justin_error is
  g_code varchar2(6) := 'PCR008';

  type typerecResult is record                                   -- Error record
    ( bStatus         boolean                                    -- The Result Status (True=Pass; False Fail)
    , vPgmnam         JUSTIN_ERROR_LOGS.PROGRAM_NM%type             -- The Name of the Program Unit
    , vPgmVer         JUSTIN_ERROR_LOGS.PROGRAM_VERSION_TXT%type    -- Version Format is '00000'
    , nSqlCode        JUSTIN_ERROR_LOGS.ERROR_MESSAGE_NO%type       -- Oracle Error Code Variable sqlcode
    , vSqlErrm        JUSTIN_ERROR_LOGS.ERROR_MESSAGE_TXT%type      -- Oracle Error Msg Variable sqlerm
    , vMsgTxt         JUSTIN_ERROR_LOGS.ERROR_DETAIL_TXT%type);     -- Any additional debug information  

  PROCEDURE prDebuggerRonna (pnNo IN NUMBER
                           ,pvMessage IN clob
                       ,pvPgmNam IN VARCHAR2 DEFAULT NULL
                       ,pvPgmVer IN VARCHAR2 DEFAULT NULL);

  PROCEDURE prDebugger (pnNo IN NUMBER
                           ,pvMessage IN VARCHAR2
                       ,pvPgmNam IN VARCHAR2 DEFAULT NULL
                       ,pvPgmVer IN VARCHAR2 DEFAULT NULL);

  PROCEDURE prErrsInsert (prResult IN typerecResult);

  PROCEDURE prExceptionHandle
     ( pvPgmNam     VARCHAR2
     , pvPgmVer     VARCHAR2
     , pvMsgTxt     VARCHAR2 DEFAULT NULL
     , pnAppErrCode PLS_INTEGER DEFAULT NULL
     ) ;

  PROCEDURE prExceptionHandleNoFail
     ( pvPgmNam     VARCHAR2
     , pvPgmVer     VARCHAR2
     , pvMsgTxt     VARCHAR2 DEFAULT NULL
     , pnAppErrCode PLS_INTEGER DEFAULT NULL
     ) ;

  PROCEDURE prErrorLogRecordsDelete 
     (pnNumberDaysToKeep number default 60
     );

end justin_error;
/

create PACKAGE justin_image IS
  -- user object_guid for images in the object repository, use object_url for url based images, eg aem generated reports
  -- Note: each record, in the image_record for the DEMS and Disclosure export, represents a single document. For a Witness_Statement, for example,
  --       the part_id / prof_seq_no would be those of the witness, same with the participant names, etc.
  TYPE image_record is record (selected_yn varchar2(1),
                               object_guid varchar2(100), 
                               object_url varchar2(2000), 
                               bookmark_text varchar2(200), 
                               mime_type varchar2(10),
                               report_type varchar2(100),
                               report_data_found_yn varchar2(1),
                               image_index number, 
                               report_index number,
                               rcc_id justin_rccs.rcc_id%type,
                               sequence_nos varchar2(1000),
                               rcc_file_no varchar2(100),
                               primary_rcc_id justin_matter_docs.primary_rcc_id%type,
                               mdoc_justin_no justin_matter_docs.mdoc_justin_no%type,
                               image_id justin_images.image_id%type,
                               docm_id justin_documents.docm_id%type,
                               court_file_no varchar2(100),
                               court_agency_name justin_agencies.agen_agency_nm%type,
                               sworn_date varchar2(25),
                               docm_issue_date varchar2(25),
                               court_services_form_no justin_cd_form_types.court_services_form_no%type,
                               fmtp_form_cd justin_cd_form_types.fmtp_form_cd%type,
                               fmtp_form_dsc justin_cd_form_types.fmtp_form_dsc%type,
                               report_submit_date varchar2(25),
                               filed_by_name varchar2(200),
                               part_id justin_participant_profiles.part_id%type,
                               prof_seq_no justin_participant_profiles.prof_seq_no%type,
                               prof_org_nm justin_participant_profiles.prof_org_nm%type,
                               prof_surname_nm justin_participant_profiles.prof_surname_nm%type,
                               prof_given_1_nm justin_participant_profiles.prof_given_1_nm%type,
                               participant_names varchar2(4000),
                               part_ids varchar2(2000),
                               officer_pin_txt justin_partic_assignments.paas_code_txt%type,
                               witness_yn varchar2(1),
                               expert_yn varchar2(1),
                               police_officer_yn varchar2(1),
                               victim_yn varchar2(1),
                               filtered_yn varchar2(1));


  TYPE image_type is table of image_record index by binary_integer;

  -- this is the collection of ids used to create the image_type above. For example, rcc_id 123, report_types NARRATIVE, WITNESS_STATEMENTS
  -- NOTE: with each key_data_record, it could represent multiple documents because report_types is a comma separated list. 
  --       This makes it easier on the select side. Increased flexibility.
  TYPE key_data_record is record (rcc_id justin_rccs.rcc_id%type,
                                  part_id justin_participant_profiles.part_id%type,
                                  mdoc_justin_no justin_matter_docs.mdoc_justin_no%type,
                                  image_id justin_images.image_id%type,
                                  court_agen_id justin_agencies.agen_id%type,
                                  sequence_numbers varchar2(1000),
                                  report_types varchar2(4000),
                                  report_title varchar2(1000),
                                  filtered_yn varchar2(1),
                                  report_object_url varchar2(2000),
                                  active_yn varchar2(1));

  TYPE key_data_type is table of key_data_record index by binary_integer;

  procedure json_data_to_key_data(pSourceData varchar2, pKeyData in out key_data_type);

  procedure image_data_get(pKeyData in key_data_type, pImages in out image_type);

  FUNCTION encode64_clob(pclob CLOB) RETURN CLOB;

  procedure image_url_to_clob(vUrl varchar2, po_image_clob in out clob);

END;
/

create PACKAGE justin_rest_utilities 
IS

  FUNCTION  escape_chars (val in VARCHAR2) RETURN VARCHAR2;  

  PROCEDURE json_header_start; 

  PROCEDURE json_end; 

  PROCEDURE json_object_start (pcomma IN BOOLEAN DEFAULT FALSE); 

  PROCEDURE json_append_value (pvalue IN VARCHAR2, pcomma IN BOOLEAN DEFAULT FALSE, pquotes IN BOOLEAN DEFAULT TRUE);   

  PROCEDURE json_append_kv (pname IN VARCHAR2, pvalue IN VARCHAR2, pcomma IN BOOLEAN DEFAULT FALSE, pquotes IN BOOLEAN DEFAULT TRUE, nullable IN BOOLEAN DEFAULT TRUE);   

  PROCEDURE json_append_clob_kv (pname IN VARCHAR2, pvalue IN CLOB, pcomma IN BOOLEAN DEFAULT FALSE, pquotes IN BOOLEAN DEFAULT TRUE); 

  PROCEDURE json_append_bool (pname IN VARCHAR2, pvalue IN BOOLEAN, pcomma IN BOOLEAN DEFAULT FALSE);    

  PROCEDURE json_append_object (pname IN VARCHAR2, pempty IN BOOLEAN DEFAULT FALSE, pcomma IN BOOLEAN DEFAULT FALSE);  

  PROCEDURE json_append_array (pname IN VARCHAR2, pempty IN BOOLEAN DEFAULT FALSE, pcomma IN BOOLEAN DEFAULT FALSE); 

  PROCEDURE json_object_end (pcomma IN BOOLEAN DEFAULT FALSE);  

  PROCEDURE json_array_end; 

  PROCEDURE json_print; 

  PROCEDURE json_reset;

  PROCEDURE json_return_error_object (pResponseCd IN NUMBER, pResponseMsg IN VARCHAR2); 

  FUNCTION escape_clob (pclob IN CLOB) RETURN CLOB;

  FUNCTION encode64_clob (pclob CLOB) RETURN CLOB; 

  FUNCTION decode64_clob (pClob IN CLOB ) RETURN CLOB;

  FUNCTION extract_clob (p_json_clob IN CLOB, p_json_keyname IN VARCHAR2) RETURN CLOB;

  FUNCTION clob_to_blob (p_data IN CLOB) RETURN BLOB;

  FUNCTION return_jsondata return clob;

END justin_rest_utilities;
/

create PACKAGE        justin_tco_interface IS

  TYPE tco_appr_rec IS RECORD (
    rcc_id                     JUSTIN_RCCs.rcc_id%TYPE,
    mdoc_justin_no             JUSTIN_APPEARANCE_COUNTS.mdpr_mdct_justin_no%TYPE,
    part_id                    JUSTIN_PARTICIPANTS.part_id%TYPE,
    appr_id                    JUSTIN_APPEARANCES.appr_id%TYPE,
    rsn_appearance_reason_cd   JUSTIN_APPEARANCE_COUNTS.rsn_appearance_reason_cd%TYPE,
    find_finding_cd            JUSTIN_APPEARANCE_COUNTS.find_finding_cd%TYPE
  );

  TYPE tco_table IS TABLE OF tco_appr_rec index by binary_integer;

  tcoTab   tco_table;

  PROCEDURE prReconcileTicketAppearance(p_appr_id IN JUSTIN_APPEARANCES.APPR_ID%TYPE
                                      , p_rcc_id IN JUSTIN_RCCS.RCC_ID%TYPE
                                      , p_hearing_reason_cd IN VARCHAR2 DEFAULT NULL);

  PROCEDURE prJustinAppearanceUpdate(p_appr_id             IN JUSTIN_APPEARANCES.APPR_ID%TYPE
                                   , p_seized_yn           IN VARCHAR2
                                   , p_adjudicator_part_id IN JUSTIN_PARTICIPANTS.PART_ID%TYPE
                                   , p_aatt_cd             IN JUSTIN_APPEARANCES.AATT_CD%TYPE
                                   , p_datt_cd             IN JUSTIN_APPEARANCES.DATT_CD%TYPE
                                   , p_staff_part_id       IN JUSTIN_PARTICIPANTS.PART_ID%TYPE
                                  );

  PROCEDURE prJustinAppearanceResult(p_appr_id             IN JUSTIN_APPEARANCES.APPR_ID%TYPE
                                   , p_adjudicator_part_id IN JUSTIN_PARTICIPANTS.PART_ID%TYPE
                                   , p_staff_part_id       IN JUSTIN_PARTICIPANTS.PART_ID%TYPE
                                  );

  PROCEDURE prJustinAppearanceDelete(p_appr_id  IN JUSTIN_APPEARANCES.APPR_ID%TYPE, p_ccas_case_no justin_appearances.ccpt_ccas_case_no%TYPE); 

  PROCEDURE prTcoDisputeReset(p_appr_id justin_appearances.appr_id%TYPE, p_ccas_case_no justin_appearances.ccpt_ccas_case_no%TYPE); 

  procedure ticket_data_insert(pvTicketNumber varchar2
                              ,pnTicketCourtAgenId number
                              ,pnTicketEnfAgenId number
                              ,pvCurrentUser varchar2
                              ,pnJustinUserAgenId number 
                              ,pnJustinUserPartId number 
                              ,pnJustinUserPaasSeqNo number
                              ,pvResponseCode in out varchar2
                              ,pvResponseText in out varchar2);

  procedure appearance_data_insert(pnCourtFileNo number
                                  ,pnCourtFileSeqNo number
                                  ,pnCourtFileLocationAgenId number
                                  ,pvCourtLevelCd varchar2
                                  ,pvCourtClassCd varchar2
                                  ,pvAppearanceDate varchar2
                                  ,pvAppearanceTime varchar2
                                  ,pvAppearanceReason varchar2
                                  ,pvCourtRoomCd varchar2
                                  ,pvCourtListCd varchar2
                                  ,pvResponseCode in out varchar2
                                  ,pvResponseText in out varchar2);

  procedure appearance_result_data_insert(pnCourtFileNo number
                                         ,pnCourtFileSeqNo number
                                         ,pnCourtFileLocationAgenId number
                                         ,pvCourtLevelCd varchar2
                                         ,pvCourtClassCd varchar2
                                         ,pvAppearanceDate varchar2
                                         ,pvAppearanceTime varchar2
                                         ,pvAppearanceReason varchar2
                                         ,pvCourtRoomCd varchar2
                                         ,pvPleaCd varchar2
                                         ,pvFindingCd varchar2
                                         ,pvResultCd varchar2
                                         ,pvCourtListCd varchar2
                                         ,pvResponseCode in out varchar2
                                         ,pvResponseText in out varchar2);

  PROCEDURE pDisputeUpdateStatus(pDisputeId           IN NUMBER
                               , pStatusCd            IN VARCHAR2
                               , pUserId              IN VARCHAR2);

  FUNCTION fnTcoDisputeIdGet(pi_rcc_id justin_rccs.rcc_id%TYPE) RETURN NUMBER;

  PROCEDURE prAppearanceEventTrigger (pi_appr_id justin_appearances.appr_id%TYPE, 
                                      pi_rcc_id  justin_rccs.rcc_id%TYPE,
                                      pi_mdoc_justin_no justin_matter_docs.mdoc_justin_no%TYPE,
                                      pi_part_id        justin_participants.part_id%TYPE,
                                      pi_operation_cd   varchar2);

  FUNCTION fnTcoEntryExists(pi_rcc_id IN justin_rccs.rcc_id%TYPE) RETURN NUMBER;

END;
/
