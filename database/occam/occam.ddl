create sequence OCCAM_VITU_SEQ
/

create sequence OCCAM_VITC_SEQ
/

create sequence OCCAM_DISP_SEQ
/

create sequence OCCAM_DICO_SEQ
/

create sequence OCCAM_ERLO_SEQ
/

create sequence OCCAM_ALEN_SEQ
/

create sequence OCCAM_OUEM_SEQ
/

create sequence OCCAM_DURE_SEQ
/

create table OCCAM_VIOLATION_TICKET_UPLOADS
(
    VIOLATION_TICKET_UPLOAD_ID     NUMBER(15)   not null
        constraint VITU_PK
            primary key,
    TICKET_NUMBER_TXT              VARCHAR2(50) not null,
    DISPUTANT_SURNAME_TXT          VARCHAR2(100),
    DISPUTANT_GIVEN_NAMES_TXT      VARCHAR2(200),
    DISPUTANT_ORGANIZATION_NM_TXT  VARCHAR2(150),
    IS_YOUNG_PERSON_YN             VARCHAR2(1)
        constraint AVCON_1670888403_IS_YO_000
            check (IS_YOUNG_PERSON_YN IN ('Y', 'N')),
    DISPUTANT_DRV_LIC_NUMBER_TXT   VARCHAR2(30),
    DISPUTANT_CLIENT_NUMBER_TXT    VARCHAR2(30),
    DRV_LIC_ISSUED_PROVINCE_TXT    VARCHAR2(100),
    DRV_LIC_ISSUED_COUNTRY_TXT     VARCHAR2(100),
    DRV_LIC_ISSUED_YEAR_NO         NUMBER(4),
    DRV_LIC_EXPIRY_YEAR_NO         NUMBER(4),
    DISPUTANT_BIRTH_DT             DATE,
    ADDRESS_TXT                    VARCHAR2(100),
    ADDRESS_CITY_TXT               VARCHAR2(100),
    ADDRESS_PROVINCE_TXT           VARCHAR2(100),
    ADDRESS_COUNTRY_TXT            VARCHAR2(100),
    ADDRESS_POSTAL_CODE_TXT        VARCHAR2(10),
    OFFICER_PIN_TXT                VARCHAR2(10),
    DETACHMENT_LOCATION_TXT        VARCHAR2(150),
    ISSUED_DT                      DATE,
    ISSUED_ON_ROAD_OR_HIGHWAY_TXT  VARCHAR2(100),
    ISSUED_AT_OR_NEAR_CITY_TXT     VARCHAR2(100),
    IS_CHANGE_OF_ADDRESS_YN        VARCHAR2(1)
        constraint AVCON_1670888403_IS_CH_000
            check (IS_CHANGE_OF_ADDRESS_YN IN ('Y', 'N')),
    IS_DRIVER_YN                   VARCHAR2(1)
        constraint AVCON_1670888403_IS_DR_000
            check (IS_DRIVER_YN IN ('Y', 'N')),
    IS_OWNER_YN                    VARCHAR2(1)
        constraint AVCON_1670888403_IS_OW_000
            check (IS_OWNER_YN IN ('Y', 'N')),
    COURT_LOCATION_TXT             VARCHAR2(150),
    ENT_DTM                        DATE         not null,
    ENT_USER_ID                    VARCHAR2(30) not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30),
    UPPER_DISPUTANT_SURNAME_TXT    VARCHAR2(100),
    UPPER_DISPUTANT_GIVEN_NAMS_TXT VARCHAR2(200),
    UPPER_DISPUTANT_ORGANIZ_NM_TXT VARCHAR2(150)
)
/

create table OCCAM_VIOLATION_TICKET_COUNTS
(
    VIOLATION_TICKET_COUNT_ID  NUMBER(15)   not null
        constraint VITC_PK
            primary key,
    VIOLATION_TICKET_UPLOAD_ID NUMBER(15)
        constraint VITC_VITU_FK
            references OCCAM_VIOLATION_TICKET_UPLOADS,
    COUNT_NO                   NUMBER(1)    not null,
    DESCRIPTION_TXT            VARCHAR2(4000),
    IS_ACT_YN                  VARCHAR2(1)
        constraint AVCON_1670890084_IS_AC_000
            check (IS_ACT_YN IN ('Y', 'N')),
    IS_REGULATION_YN           VARCHAR2(1)
        constraint AVCON_1670890084_IS_RE_000
            check (IS_REGULATION_YN IN ('Y', 'N')),
    ACT_OR_REGULATION_NAME_CD  VARCHAR2(5),
    STAT_SECTION_TXT           VARCHAR2(10),
    STAT_SUB_SECTION_TXT       VARCHAR2(4),
    STAT_PARAGRAPH_TXT         VARCHAR2(3),
    STAT_SUB_PARAGRAPH_TXT     VARCHAR2(5),
    TICKETED_AMT               NUMBER(8, 2),
    ENT_DTM                    DATE         not null,
    ENT_USER_ID                VARCHAR2(30) not null,
    UPD_DTM                    DATE,
    UPD_USER_ID                VARCHAR2(30),
    constraint VITC_UK
        unique (VIOLATION_TICKET_UPLOAD_ID, COUNT_NO)
)
/

create index VITC_VITU_FK_I
    on OCCAM_VIOLATION_TICKET_COUNTS (VIOLATION_TICKET_UPLOAD_ID)
/

create table OCCAM_DISPUTE_STATUS_TYPES
(
    DISPUTE_STATUS_TYPE_CD  VARCHAR2(4)  not null
        constraint DIST_PK
            primary key,
    DISPUTE_STATUS_TYPE_DSC VARCHAR2(50) not null
        constraint DIST_UK
            unique,
    ACTIVE_YN               VARCHAR2(1)  not null
        constraint AVCON_1670888403_ACTIV_001
            check (ACTIVE_YN IN ('Y', 'N')),
    ENT_DTM                 DATE         not null,
    ENT_USER_ID             VARCHAR2(30) not null,
    UPD_DTM                 DATE,
    UPD_USER_ID             VARCHAR2(30)
)
/

create table OCCAM_ERROR_LOGS
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

create table OCCAM_AUDIT_LOG_ENTRY_TYPES
(
    AUDIT_LOG_ENTRY_TYPE_CD        VARCHAR2(4)             not null
        constraint ALET_PK
            primary key,
    AUDIT_LOG_ENTRY_TYPE_DSC       VARCHAR2(500)           not null
        constraint ALET_ALET_UK1_UK
            unique,
    AUDIT_LOG_ENTRY_TYPE_SHORT_DSC VARCHAR2(150)           not null
        constraint ALET_ALET_UK2_UK
            unique,
    ACTIVE_USE_YN                  VARCHAR2(1)             not null
        constraint AVCON_1670888403_ACTIV_000
            check (ACTIVE_USE_YN IN ('Y', 'N')),
    ENT_DTM                        DATE                    not null,
    ENT_USER_ID                    VARCHAR2(30)            not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30),
    COMMENT_ENTRY_ALLOWED_YN       VARCHAR2(1) default 'N' not null
        constraint AVCON_1704394282_COMME_000
            check (COMMENT_ENTRY_ALLOWED_YN IN ('Y', 'N'))
)
/

create table OCCAM_DISPUTE_UPDATE_STAT_TYPS
(
    DISPUTE_UPDATE_STAT_TYPE_CD    VARCHAR2(4)  not null
        constraint DUST_PK
            primary key,
    DISPUTE_UPDATE_STAT_TYPE_DSC   VARCHAR2(80) not null
        constraint DUST_UK1
            unique,
    DISPUTE_UPDATE_STAT_TYP_SH_DSC VARCHAR2(30) not null
        constraint DUST_UK2
            unique,
    ACTIVE_USE_YN                  VARCHAR2(1)  not null
        constraint AVCON_1670890674_ACTIV_001
            check (ACTIVE_USE_YN IN ('Y', 'N')),
    ENT_DTM                        DATE         not null,
    ENT_USER_ID                    VARCHAR2(30) not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30)
)
/

create table OCCAM_DISPUTE_UPDATE_REQ_TYPES
(
    DISPUTE_UPDATE_REQ_TYPE_CD     VARCHAR2(4)  not null
        constraint DURT_PK
            primary key,
    DISPUTE_UPDATE_REQ_TYPE_DSC    VARCHAR2(80) not null
        constraint DURT_UK1
            unique,
    DISPUTE_UPDATE_REQ_TYP_SHR_DSC VARCHAR2(30) not null
        constraint DURT_UK2
            unique,
    ACTIVE_USE_YN                  VARCHAR2(1)  not null
        constraint AVCON_1670890674_ACTIV_000
            check (ACTIVE_USE_YN IN ('Y', 'N')),
    ENT_DTM                        DATE         not null,
    ENT_USER_ID                    VARCHAR2(30) not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30)
)
/

create table OCCAM_DISPUTES
(
    DISPUTE_ID                     NUMBER(15)    not null
        constraint DISP_PK
            primary key,
    COURT_AGEN_ID                  NUMBER(15, 4)
        constraint DISP_AGEN_FK
            references JUSTIN.JUSTIN_AGENCIES (),
    DISPUTE_STATUS_TYPE_CD         VARCHAR2(4)   not null
        constraint DISP_DIST_FK
            references OCCAM_DISPUTE_STATUS_TYPES,
    CONTACT_TYPE_CD                VARCHAR2(1)   not null
        constraint AVCON_1674239981_CONTA_000
            check (CONTACT_TYPE_CD IN ('I', 'L', 'O')),
    REQUEST_COURT_APPEARANCE_YN    VARCHAR2(1)   not null
        constraint AVCON_1674239981_REQUE_000
            check (REQUEST_COURT_APPEARANCE_YN IN ('Y', 'N')),
    VIOLATION_TICKET_UPLOAD_ID     NUMBER(15)    not null
        constraint DISP_VITU_FK
            references OCCAM_VIOLATION_TICKET_UPLOADS,
    ISSUED_DT                      DATE,
    SUBMITTED_DT                   DATE,
    DISPUTANT_SURNAME_NM           VARCHAR2(30),
    DISPUTANT_GIVEN_1_NM           VARCHAR2(30),
    DISPUTANT_GIVEN_2_NM           VARCHAR2(30),
    DISPUTANT_GIVEN_3_NM           VARCHAR2(30),
    DISPUTANT_BIRTH_DT             DATE,
    DISPUTANT_ORGANIZATION_NM      VARCHAR2(150),
    DISPUTANT_DRV_LIC_NUMBER_TXT   VARCHAR2(30),
    DRV_LIC_ISSUED_PROV_CTRY_ID    NUMBER(8),
    DRV_LIC_ISSUED_PROV_SEQ_NO     NUMBER(4),
    DRV_LIC_ISSUED_CTRY_CTRY_ID    NUMBER(8)
        constraint DISP_CTRY_FK3
            references JUSTIN.JUSTIN_COUNTRIES (),
    DRV_LIC_ISSUED_OTHER_PROV_TXT  VARCHAR2(30),
    DISPUTANT_CLIENT_ID            VARCHAR2(30),
    ADDRESS_LINE_1_TXT             VARCHAR2(100) not null
        constraint DISP_CK9_CONT_ADDR
            check (ADDRESS_LINE_1_TXT is not null),
    ADDRESS_LINE_2_TXT             VARCHAR2(100),
    ADDRESS_LINE_3_TXT             VARCHAR2(100),
    ADDRESS_CITY_CTRY_ID           NUMBER(8),
    ADDRESS_CITY_SEQ_NO            NUMBER(4),
    ADDRESS_PROV_CTRY_ID           NUMBER(8),
    ADDRESS_PROV_SEQ_NO            NUMBER(4),
    ADDRESS_CTRY_CTRY_ID           NUMBER(8)
        constraint DISP_CTRY_FK1
            references JUSTIN.JUSTIN_COUNTRIES (),
    ADDRESS_OTHER_CITY_TXT         VARCHAR2(30),
    ADDRESS_OTHER_PROV_TXT         VARCHAR2(30),
    POSTAL_CODE_TXT                VARCHAR2(10),
    HOME_PHONE_NUMBER_TXT          VARCHAR2(20),
    CONTACT_LAW_FIRM_NM            VARCHAR2(200),
    CONTACT_SURNAME_NM             VARCHAR2(30),
    CONTACT_GIVEN_1_NM             VARCHAR2(30),
    CONTACT_GIVEN_2_NM             VARCHAR2(30),
    CONTACT_GIVEN_3_NM             VARCHAR2(30),
    WORK_PHONE_NUMBER_TXT          VARCHAR2(20),
    EMAIL_ADDRESS_TXT              VARCHAR2(100),
    EMAIL_VERIFIED_YN              VARCHAR2(1)
        constraint AVCON_1674239981_EMAIL_000
            check (EMAIL_VERIFIED_YN IN ('Y', 'N')),
    NOTICE_OF_DISPUTE_GUID         VARCHAR2(36)
        constraint DISP_UK1
            unique,
    FILING_DT                      DATE,
    REPRESENTED_BY_LAWYER_YN       VARCHAR2(1)
        constraint AVCON_1674239981_REPRE_000
            check (REPRESENTED_BY_LAWYER_YN IN ('Y', 'N')),
    LAW_FIRM_NM                    VARCHAR2(200),
    LAW_FIRM_ADDR_LINE_1_TXT       VARCHAR2(100),
    LAW_FIRM_ADDR_LINE_2_TXT       VARCHAR2(100),
    LAW_FIRM_ADDR_LINE_3_TXT       VARCHAR2(100),
    LAW_FIRM_ADDR_CITY_CTRY_ID     NUMBER(8),
    LAW_FIRM_ADDR_CITY_SEQ_NO      NUMBER(4),
    LAW_FIRM_ADDR_PROV_CTRY_ID     NUMBER(8),
    LAW_FIRM_ADDR_PROV_SEQ_NO      NUMBER(4),
    LAW_FIRM_ADDR_CTRY_CTRY_ID     NUMBER(8)
        constraint DISP_CTRY_FK2
            references JUSTIN.JUSTIN_COUNTRIES (),
    LAW_FIRM_ADDR_OTHER_CITY_TXT   VARCHAR2(30),
    LAW_FIRM_ADDR_OTHER_PROV_TXT   VARCHAR2(30),
    LAW_FIRM_ADDR_POSTAL_CODE_TXT  VARCHAR2(10),
    LAWYER_SURNAME_NM              VARCHAR2(30),
    LAWYER_GIVEN_1_NM              VARCHAR2(30),
    LAWYER_GIVEN_2_NM              VARCHAR2(30),
    LAWYER_GIVEN_3_NM              VARCHAR2(30),
    LAWYER_PHONE_NUMBER_TXT        VARCHAR2(20),
    LAWYER_EMAIL_ADDRESS_TXT       VARCHAR2(100),
    OFFICER_PIN_TXT                VARCHAR2(10),
    DETACHMENT_LOCATION_TXT        VARCHAR2(150),
    LANGUAGE_CD                    VARCHAR2(3),
    INTERPRETER_REQUIRED_YN        VARCHAR2(1)
        constraint AVCON_1674239981_INTER_000
            check (INTERPRETER_REQUIRED_YN IN ('Y', 'N')),
    WITNESS_NO                     NUMBER(3),
    FINE_REDUCTION_REASON_TXT      VARCHAR2(500),
    TIME_TO_PAY_REASON_TXT         VARCHAR2(500),
    DISPUTANT_COMMENT_TXT          VARCHAR2(4000),
    REJECTED_REASON_TXT            VARCHAR2(500),
    JJ_ASSIGNED_TO                 VARCHAR2(30),
    USER_ASSIGNED_TO               VARCHAR2(30),
    USER_ASSIGNED_DTM              DATE,
    DISPUTANT_DETECT_OCR_ISSUES_YN VARCHAR2(1)   not null
        constraint AVCON_1674239981_DISPU_000
            check (DISPUTANT_DETECT_OCR_ISSUES_YN IN ('Y', 'N')),
    DISPUTANT_OCR_ISSUES_TXT       VARCHAR2(500),
    SYSTEM_DETECT_OCR_ISSUES_YN    VARCHAR2(1)   not null
        constraint AVCON_1674239981_SYSTE_000
            check (SYSTEM_DETECT_OCR_ISSUES_YN IN ('Y', 'N')),
    OCR_TICKET_JSON_FILENAME_TXT   VARCHAR2(100),
    ENT_DTM                        DATE          not null,
    ENT_USER_ID                    VARCHAR2(30)  not null,
    UPD_DTM                        DATE,
    UPD_USER_ID                    VARCHAR2(30),
    SIGNATORY_TYPE_CD              VARCHAR2(1)
        constraint AVCON_1704408247_SIGNA_000
            check (SIGNATORY_TYPE_CD IN ('D', 'A')),
    SIGNATORY_NAME_TXT             VARCHAR2(100),
    UPPER_DISPUTANT_SURNAME_NM     VARCHAR2(30),
    UPPER_DISPUTANT_GIVEN_1_NM     VARCHAR2(30),
    UPPER_DISPUTANT_GIVEN_2_NM     VARCHAR2(30),
    UPPER_DISPUTANT_GIVEN_3_NM     VARCHAR2(30),
    UPPER_DISPUTANT_ORGANIZ_NM     VARCHAR2(150),
    constraint DISP_CITY_FK1
        foreign key (ADDRESS_CITY_CTRY_ID, ADDRESS_CITY_SEQ_NO) references JUSTIN.JUSTIN_CITIES (),
    constraint DISP_CITY_FK2
        foreign key (LAW_FIRM_ADDR_CITY_CTRY_ID, LAW_FIRM_ADDR_CITY_SEQ_NO) references JUSTIN.JUSTIN_CITIES (),
    constraint DISP_PROV_FK1
        foreign key (ADDRESS_PROV_CTRY_ID, ADDRESS_PROV_SEQ_NO) references JUSTIN.JUSTIN_PROVINCES (),
    constraint DISP_PROV_FK2
        foreign key (LAW_FIRM_ADDR_PROV_CTRY_ID, LAW_FIRM_ADDR_PROV_SEQ_NO) references JUSTIN.JUSTIN_PROVINCES (),
    constraint DISP_PROV_FK3
        foreign key (DRV_LIC_ISSUED_PROV_CTRY_ID, DRV_LIC_ISSUED_PROV_SEQ_NO) references JUSTIN.JUSTIN_PROVINCES (),
    constraint DISP_CK1_CONTACT_ADDR
        check (( -- city fk is used, don't populate anything else
                   ADDRESS_CITY_CTRY_ID is not null
                       and ADDRESS_CITY_SEQ_NO is not null
                       and ADDRESS_OTHER_CITY_TXT is null
                       and ADDRESS_PROV_CTRY_ID is null
                       and ADDRESS_PROV_SEQ_NO is null
                       and ADDRESS_OTHER_PROV_TXT is null
                       and ADDRESS_CTRY_CTRY_ID is null
                   )
            or
               ( -- intl city txt is used,
                   --     populate only either (prov) or (country and intl prov)
                   ADDRESS_CITY_CTRY_ID is null
                       and ADDRESS_CITY_SEQ_NO is null
                       and ADDRESS_OTHER_CITY_TXT is not null
                       and ((ADDRESS_PROV_CTRY_ID is not null
                       and ADDRESS_PROV_SEQ_NO is not null
                       and ADDRESS_OTHER_PROV_TXT is null
                       and ADDRESS_CTRY_CTRY_ID is null
                                )
                       or (ADDRESS_PROV_CTRY_ID is null
                           and ADDRESS_PROV_SEQ_NO is null
                           and ADDRESS_OTHER_PROV_TXT is not null
                           and ADDRESS_CTRY_CTRY_ID is not null
                                )
                       )
                   )
            or
               ( -- all fields are empty
                   ADDRESS_CITY_CTRY_ID is null
                       and ADDRESS_CITY_SEQ_NO is null
                       and ADDRESS_OTHER_CITY_TXT is null
                       and ADDRESS_PROV_CTRY_ID is null
                       and ADDRESS_PROV_SEQ_NO is null
                       and ADDRESS_OTHER_PROV_TXT is null
                       and ADDRESS_CTRY_CTRY_ID is null
                   )),
    constraint DISP_CK2_LAW_FIRM_ADDR
        check (( -- city fk is used, don't populate anything else
                   LAW_FIRM_ADDR_CITY_CTRY_ID is not null
                       and LAW_FIRM_ADDR_CITY_SEQ_NO is not null
                       and LAW_FIRM_ADDR_OTHER_CITY_TXT is null
                       and LAW_FIRM_ADDR_PROV_CTRY_ID is null
                       and LAW_FIRM_ADDR_PROV_SEQ_NO is null
                       and LAW_FIRM_ADDR_OTHER_PROV_TXT is null
                       and LAW_FIRM_ADDR_CTRY_CTRY_ID is null
                   )
            or
               ( -- intl city txt is used,
                   --     populate only either (prov) or (country and intl prov)
                   LAW_FIRM_ADDR_CITY_CTRY_ID is null
                       and LAW_FIRM_ADDR_CITY_SEQ_NO is null
                       and LAW_FIRM_ADDR_OTHER_CITY_TXT is not null
                       and ((LAW_FIRM_ADDR_PROV_CTRY_ID is not null
                       and LAW_FIRM_ADDR_PROV_SEQ_NO is not null
                       and LAW_FIRM_ADDR_OTHER_PROV_TXT is null
                       and LAW_FIRM_ADDR_CTRY_CTRY_ID is null
                                )
                       or (LAW_FIRM_ADDR_PROV_CTRY_ID is null
                           and LAW_FIRM_ADDR_PROV_SEQ_NO is null
                           and LAW_FIRM_ADDR_OTHER_PROV_TXT is not null
                           and LAW_FIRM_ADDR_CTRY_CTRY_ID is not null
                                )
                       )
                   )
            or
               ( -- all fields are empty
                   LAW_FIRM_ADDR_CITY_CTRY_ID is null
                       and LAW_FIRM_ADDR_CITY_SEQ_NO is null
                       and LAW_FIRM_ADDR_OTHER_CITY_TXT is null
                       and LAW_FIRM_ADDR_PROV_CTRY_ID is null
                       and LAW_FIRM_ADDR_PROV_SEQ_NO is null
                       and LAW_FIRM_ADDR_OTHER_PROV_TXT is null
                       and LAW_FIRM_ADDR_CTRY_CTRY_ID is null
                   )),
    constraint DISP_CK3_DL_ISSUED
        check (( -- prov fk is used, don't populate anything else
                   DRV_LIC_ISSUED_PROV_CTRY_ID is not null
                       and DRV_LIC_ISSUED_PROV_SEQ_NO is not null
                       and DRV_LIC_ISSUED_OTHER_PROV_TXT is null
                       and DRV_LIC_ISSUED_CTRY_CTRY_ID is null
                   )
            or
               ( -- ctry fk is used - do not populate prov fk, but other prov MAY be
                   DRV_LIC_ISSUED_CTRY_CTRY_ID is not null
                       and DRV_LIC_ISSUED_PROV_CTRY_ID is null
                       and DRV_LIC_ISSUED_PROV_SEQ_NO is null
                   )
            or
               ( -- all fields are empty
                   DRV_LIC_ISSUED_PROV_CTRY_ID is null
                       and DRV_LIC_ISSUED_PROV_SEQ_NO is null
                       and DRV_LIC_ISSUED_OTHER_PROV_TXT is null
                       and DRV_LIC_ISSUED_CTRY_CTRY_ID is null
                   )),
    constraint DISP_CK4_CONT_LAW_FIRM_NM
        check (( -- Contact is a lawyer
                   CONTACT_TYPE_CD = 'L'
                   )
            or
               ( -- Contact is not a lawyer
                   CONTACT_TYPE_CD in ('O', 'I')
                       and CONTACT_LAW_FIRM_NM is null
                   )),
    constraint DISP_CK5_CONT_SURNAME
        check (( -- Contact is a lawyer or other
                   CONTACT_TYPE_CD in ('L', 'O')
                   )
            or
               ( -- Contact is an individual
                   CONTACT_TYPE_CD = 'I'
                       and CONTACT_SURNAME_NM is null
                   )),
    constraint DISP_CK6_CONT_GIVEN1
        check (( -- Contact is a lawyer or other
                   CONTACT_TYPE_CD in ('L', 'O')
                   )
            or
               ( -- Contact is an individual
                   CONTACT_TYPE_CD = 'I'
                       and CONTACT_GIVEN_1_NM is null
                   )),
    constraint DISP_CK7_CONT_GIVEN2
        check (( -- Contact is a lawyer or other
                   CONTACT_TYPE_CD in ('L', 'O')
                   )
            or
               ( -- Contact is an individual
                   CONTACT_TYPE_CD = 'I'
                       and CONTACT_GIVEN_2_NM is null
                   )),
    constraint DISP_CK8_CONT_GIVEN3
        check (( -- Contact is a lawyer or other
                   CONTACT_TYPE_CD in ('L', 'O')
                   )
            or
               ( -- Contact is an individual
                   CONTACT_TYPE_CD = 'I'
                       and CONTACT_GIVEN_3_NM is null
                   ))
)
/

create table OCCAM_DISPUTE_COUNTS
(
    DISPUTE_COUNT_ID            NUMBER(15)   not null
        constraint DICO_PK
            primary key,
    DISPUTE_ID                  NUMBER(15)   not null
        constraint DICO_DISP_FK
            references OCCAM_DISPUTES,
    VIOLATION_TICKET_COUNT_ID   NUMBER(15)   not null
        constraint DICO_VITC_FK
            references OCCAM_VIOLATION_TICKET_COUNTS,
    STAT_ID                     NUMBER(8)
        constraint DICO_STAT_FK
            references JUSTIN.JUSTIN_STATUTES (),
    PLEA_CD                     VARCHAR2(3)  not null
        constraint DICO_PLEA_FK
            references JUSTIN.JUSTIN_CD_PLEAS (),
    REQUEST_COURT_APPEARANCE_YN VARCHAR2(1)  not null
        constraint AVCON_1670888403_REQUE_000
            check (REQUEST_COURT_APPEARANCE_YN IN ('Y', 'N')),
    REQUEST_REDUCTION_YN        VARCHAR2(1)  not null
        constraint AVCON_1670888403_REQUE_001
            check (REQUEST_REDUCTION_YN IN ('Y', 'N')),
    REQUEST_TIME_TO_PAY_YN      VARCHAR2(1)  not null
        constraint AVCON_1670888403_REQUE_002
            check (REQUEST_TIME_TO_PAY_YN IN ('Y', 'N')),
    ENT_DTM                     DATE         not null,
    ENT_USER_ID                 VARCHAR2(30) not null,
    UPD_DTM                     DATE,
    UPD_USER_ID                 VARCHAR2(30),
    constraint DICO_UK
        unique (DISPUTE_ID, VIOLATION_TICKET_COUNT_ID)
)
/

create index DICO_PLEA_FK_I
    on OCCAM_DISPUTE_COUNTS (PLEA_CD)
/

create index DICO_DISP_FK_I
    on OCCAM_DISPUTE_COUNTS (DISPUTE_ID)
/

create index DICO_STAT_FK_I
    on OCCAM_DISPUTE_COUNTS (STAT_ID)
/

create index DICO_VITC_FK_I
    on OCCAM_DISPUTE_COUNTS (VIOLATION_TICKET_COUNT_ID)
/

create table OCCAM_AUDIT_LOG_ENTRIES
(
    AUDIT_LOG_ENTRY_ID         NUMBER(15)   not null
        constraint ALEN_PK
            primary key,
    DISPUTE_ID                 NUMBER(15)   not null
        constraint ALEN_DISP_FK
            references OCCAM_DISPUTES,
    AUDIT_LOG_ENTRY_TYPE_CD    VARCHAR2(4)  not null
        constraint ALEN_ALET_FK
            references OCCAM_AUDIT_LOG_ENTRY_TYPES,
    ENT_DTM                    DATE         not null,
    ENT_USER_ID                VARCHAR2(30) not null,
    UPD_DTM                    DATE,
    UPD_USER_ID                VARCHAR2(30),
    ACTION_BY_APPLICATION_USER VARCHAR2(240),
    COMMENT_TXT                VARCHAR2(500)
)
/

create index ALEN_DISP_FK_I
    on OCCAM_AUDIT_LOG_ENTRIES (DISPUTE_ID)
/

create index ALEN_ALET_FK_I
    on OCCAM_AUDIT_LOG_ENTRIES (AUDIT_LOG_ENTRY_TYPE_CD)
/

create table OCCAM_OUTGOING_EMAILS
(
    OUTGOING_EMAIL_ID    NUMBER(15)     not null
        constraint OUEM_PK
            primary key,
    DISPUTE_ID           NUMBER(15)     not null
        constraint OUEM_DISP_FK
            references OCCAM_DISPUTES,
    EMAIL_SENT_DTM       DATE           not null,
    EMAIL_SUBJECT_TXT    VARCHAR2(1000) not null,
    FROM_EMAIL_ADDRESS   VARCHAR2(100)  not null,
    TO_EMAIL_ADDRESS     VARCHAR2(4000) not null,
    CC_EMAIL_ADDRESS     VARCHAR2(4000),
    BCC_EMAIL_ADDRESS    VARCHAR2(4000),
    SUCCESSFULLY_SENT_YN VARCHAR2(1)    not null
        constraint AVCON_1670884735_SUCCE_000
            check (SUCCESSFULLY_SENT_YN IN ('Y', 'N')),
    HTML_CONTENT         VARCHAR2(4000),
    PLAIN_TEXT_CONTENT   VARCHAR2(4000),
    ENT_DTM              DATE           not null,
    ENT_USER_ID          VARCHAR2(30)   not null,
    UPD_DTM              DATE,
    UPD_USER_ID          VARCHAR2(30),
    constraint OUEM_CK1
        check ((HTML_CONTENT is null
            and PLAIN_TEXT_CONTENT is not null)
            or
               (HTML_CONTENT is not null
                   and PLAIN_TEXT_CONTENT is null)
            or
               (HTML_CONTENT is null
                   and PLAIN_TEXT_CONTENT is null))
)
/

create index OUEM_DISP_FK_I
    on OCCAM_OUTGOING_EMAILS (DISPUTE_ID)
/

create table OCCAM_DISPUTE_UPDATE_REQUESTS
(
    DISPUTE_UPDATE_REQUEST_ID   NUMBER(15)   not null
        constraint DURE_PK
            primary key,
    DISPUTE_ID                  NUMBER(15)   not null
        constraint DURE_DISP_FK
            references OCCAM_DISPUTES,
    DISPUTE_UPDATE_STAT_TYPE_CD VARCHAR2(4)  not null
        constraint DURE_DUST_FK
            references OCCAM_DISPUTE_UPDATE_STAT_TYPS,
    DISPUTE_UPDATE_REQ_TYPE_CD  VARCHAR2(4)  not null
        constraint DURE_DURT_FK
            references OCCAM_DISPUTE_UPDATE_REQ_TYPES,
    REQUEST_JSON_TXT            VARCHAR2(4000),
    ENT_DTM                     DATE         not null,
    ENT_USER_ID                 VARCHAR2(30) not null,
    UPD_DTM                     DATE,
    UPD_USER_ID                 VARCHAR2(30),
    CURRENT_JSON_TXT            VARCHAR2(4000),
    STATUS_UPDATE_DTM           DATE
)
/

create index DURE_DISP_FK_I
    on OCCAM_DISPUTE_UPDATE_REQUESTS (DISPUTE_ID)
/

create index DURE_DUST_FK_I
    on OCCAM_DISPUTE_UPDATE_REQUESTS (DISPUTE_UPDATE_STAT_TYPE_CD)
/

create index DURE_DURT_FK_I
    on OCCAM_DISPUTE_UPDATE_REQUESTS (DISPUTE_UPDATE_REQ_TYPE_CD)
/

create index DISP_CTRY_FK1
    on OCCAM_DISPUTES (ADDRESS_CTRY_CTRY_ID)
/

create index DISP_CTRY_FK2
    on OCCAM_DISPUTES (LAW_FIRM_ADDR_CTRY_CTRY_ID)
/

create index DISP_CTRY_FK3
    on OCCAM_DISPUTES (DRV_LIC_ISSUED_CTRY_CTRY_ID)
/

create index DISP_AGEN_FK
    on OCCAM_DISPUTES (COURT_AGEN_ID)
/

create index DISP_PROV_FK1
    on OCCAM_DISPUTES (ADDRESS_PROV_CTRY_ID, ADDRESS_PROV_SEQ_NO)
/

create index DISP_PROV_FK2
    on OCCAM_DISPUTES (LAW_FIRM_ADDR_PROV_CTRY_ID, LAW_FIRM_ADDR_PROV_SEQ_NO)
/

create index DISP_PROV_FK3
    on OCCAM_DISPUTES (DRV_LIC_ISSUED_PROV_CTRY_ID, DRV_LIC_ISSUED_PROV_SEQ_NO)
/

create index DISP_CITY_FK1
    on OCCAM_DISPUTES (ADDRESS_CITY_CTRY_ID, ADDRESS_CITY_SEQ_NO)
/

create index DISP_CITY_FK2
    on OCCAM_DISPUTES (LAW_FIRM_ADDR_CITY_CTRY_ID, LAW_FIRM_ADDR_CITY_SEQ_NO)
/

create index DISP_DIST_FK
    on OCCAM_DISPUTES (DISPUTE_STATUS_TYPE_CD)
/

create index DISP_VITU_FK
    on OCCAM_DISPUTES (VIOLATION_TICKET_UPLOAD_ID)
/

create PACKAGE       occam_rest IS

  PROCEDURE ping;

  PROCEDURE prGetAgencyValues(pAgencyTypeCd IN VARCHAR2);
  PROCEDURE prGetAgencyValuesArray(pAgencyTypeCd IN VARCHAR2);

  PROCEDURE prGetLanguageValues;
  PROCEDURE prGetLanguageValuesArray;

  PROCEDURE prGetCountryValues;
  PROCEDURE prGetCountryValuesArray;

  PROCEDURE prGetProvinceValues(pCountryId IN NUMBER);
  PROCEDURE prGetProvinceValuesArray(pCountryId IN NUMBER);

  PROCEDURE prGetCityValues(pCountryId IN NUMBER, pProvSeqNo IN NUMBER);
  PROCEDURE prGetCityValuesArray(pCountryId IN NUMBER, pProvSeqNo IN NUMBER);

  PROCEDURE prGetStatuteValues;
  PROCEDURE prGetStatuteValuesArray;

  PROCEDURE prViolationTicketSearch(piFromDate            IN VARCHAR2
                                   ,piExcludedStatus      IN VARCHAR2
                                   ,piTicketNumber        IN VARCHAR2
                                   ,piNoticeOfDisputeGuid IN VARCHAR2
                                   ,piIssueTime           IN VARCHAR2);

  PROCEDURE prViolationTicketGet(piViolationTicketId    IN NUMBER
                               , piDisputeId            IN NUMBER
                               , piNoticeOfDisputeGuid  IN VARCHAR2);

  PROCEDURE prViolationTicket(piViolationTicketId   IN NUMBER
                            , piDisputeId           IN NUMBER
                            , piNoticeOfDisputeGuid IN VARCHAR2);

  PROCEDURE prViolationTicketCounts(piViolationTicketId occam_violation_ticket_counts.violation_ticket_upload_id%TYPE);

  PROCEDURE prProcessViolationTicket(p_data IN CLOB);

  PROCEDURE prModifyViolationTicket(p_data IN CLOB);

  PROCEDURE prDeleteViolationTicket(p_violation_ticket_id     IN NUMBER
                                  , p_dispute_id              IN NUMBER
                                  , p_notice_of_dispute_guid  IN VARCHAR2);

  PROCEDURE prViolationTicketAssign(p_violation_ticket_id     IN NUMBER
                                  , p_dispute_id              IN NUMBER
                                  , p_notice_of_dispute_guid  IN VARCHAR2
                                  , p_user_id                 IN VARCHAR2);

  PROCEDURE prViolationTicketUnassign(p_assigned_before_ts IN VARCHAR2);

  PROCEDURE prViolationTicketUpdateStatus(p_violation_ticket_id     IN NUMBER
                                        , p_dispute_id              IN NUMBER
                                        , p_notice_of_dispute_guid  IN VARCHAR2
                                        , p_status_cd               IN VARCHAR2
                                        , p_reject_reason           IN VARCHAR2
                                        , p_user_id                 IN VARCHAR2);


  PROCEDURE prGetAuditLogEntryTypeValues;
  PROCEDURE prGetAuditLogTypeValuesArray;

  PROCEDURE prAuditLogEntrySearch(piTicketNumber IN VARCHAR2);

  PROCEDURE prOutgoingEmailSearch(piTicketNumber IN VARCHAR2);

  PROCEDURE prProcessAuditLogEntryType(p_data IN CLOB);
  PROCEDURE prModifyAuditLogEntryType(p_data IN CLOB);

  PROCEDURE prProcessAuditLogEntry(p_data IN CLOB);

  PROCEDURE prProcessOutgoingEmail(p_data IN CLOB);

  PROCEDURE prDispUpdateRequestSearch(piStatusType          IN VARCHAR2
                                     ,piRequestType         IN VARCHAR2
                                     ,piDisputeId           IN VARCHAR2
                                     ,piNoticeOfDisputeGuid IN VARCHAR2);

  PROCEDURE prDispUpdateRequestGet(piDisputeUpdateRequestId    IN occam_dispute_update_requests.dispute_update_request_id%TYPE);

  PROCEDURE prDisputeUpdateRequest(piDisputeUpdateRequestId   IN occam_dispute_update_requests.dispute_update_request_id%TYPE);

  PROCEDURE prProcessDispUpdateRequest(p_data IN CLOB);

  PROCEDURE prModifyDispUpdateRequest(p_data IN CLOB);

  PROCEDURE prDispUpdateRequestStatus(p_dispute_update_request_id     IN NUMBER
                                    , p_status_cd                     IN VARCHAR2
                                    , p_user_id                       IN VARCHAR2);

  PROCEDURE prDeleteDispUpdateRequest(p_dispute_request_update_id     IN NUMBER
                                    , p_dispute_id              IN NUMBER);

  PROCEDURE prGetDisputeList(piFromDate            IN VARCHAR2
                            ,piExcludedStatus      IN VARCHAR2
                            ,piTicketNumber        IN VARCHAR2
                            ,piNoticeOfDisputeGuid IN VARCHAR2
                            ,piIssueTime           IN VARCHAR2
                            ,po_data               OUT SYS_REFCURSOR);

  PROCEDURE prDeleteViolationTicketCount(p_violation_ticket_count_id IN NUMBER);

END; 
/

create PACKAGE       occam_violation_ticket IS

  PROCEDURE pViolationTicketSave(pViolationTicketId           OUT NUMBER,
                                 pTicketNumberTxt             IN occam_violation_ticket_uploads.ticket_number_txt%TYPE,
                                 pDisputantSurnameTxt         IN occam_violation_ticket_uploads.disputant_surname_txt%TYPE,
                                 pDisputantGivenNamesTxt      IN occam_violation_ticket_uploads.disputant_given_names_txt%TYPE,
                                 pDisputantOrganizationNmTxt  IN occam_violation_ticket_uploads.disputant_organization_nm_txt%TYPE,
                                 pIsYoungPersonYn             IN occam_violation_ticket_uploads.is_young_person_yn%TYPE,
                                 pDisputantDrvLicNumberTxt    IN occam_violation_ticket_uploads.disputant_drv_lic_number_txt%TYPE,
                                 pDisputantClientNumberTxt    IN occam_violation_ticket_uploads.disputant_client_number_txt%TYPE,
                                 pDrvLicIssuedProvinceTxt     IN occam_violation_ticket_uploads.drv_lic_issued_province_txt%TYPE,
                                 pDrvLicIssuedCountryTxt      IN occam_violation_ticket_uploads.drv_lic_issued_country_txt%TYPE,
                                 pDrvLicIssuedYearNo          IN occam_violation_ticket_uploads.drv_lic_issued_year_no%TYPE,
                                 pDrvLicExpiryYearNo          IN occam_violation_ticket_uploads.drv_lic_expiry_year_no%TYPE,
                                 pDisputantBirthDt            IN occam_violation_ticket_uploads.disputant_birth_dt%TYPE,
                                 pAddressTxt                  IN occam_violation_ticket_uploads.address_txt%TYPE,
                                 pAddressCityTxt              IN occam_violation_ticket_uploads.address_city_txt%TYPE,
                                 pAddressProvinceTxt          IN occam_violation_ticket_uploads.address_province_txt%TYPE,
                                 pAddressCountryTxt           IN occam_violation_ticket_uploads.address_country_txt%TYPE,
                                 pAddressPostalCodeTxt        IN occam_violation_ticket_uploads.address_postal_code_txt%TYPE,
                                 pOfficerPinTxt               IN occam_violation_ticket_uploads.officer_pin_txt%TYPE,
                                 pDetachmentLocationTxt       IN occam_violation_ticket_uploads.detachment_location_txt%TYPE,
                                 pIssuedDt                    IN occam_violation_ticket_uploads.issued_dt%TYPE,
                                 pIssuedOnRoadOrHighwayTxt    IN occam_violation_ticket_uploads.issued_on_road_or_highway_txt%TYPE,
                                 pIssuedAtOrNearCityTxt       IN occam_violation_ticket_uploads.issued_at_or_near_city_txt%TYPE,
                                 pIsChangeOfAddressYn         IN occam_violation_ticket_uploads.is_change_of_address_yn%TYPE,
                                 pIsDriverYn                  IN occam_violation_ticket_uploads.is_driver_yn%TYPE,
                                 pIsOwnerYn                   IN occam_violation_ticket_uploads.is_owner_yn%TYPE,
                                 pCourtLocationTxt            IN occam_violation_ticket_uploads.court_location_txt%TYPE,
                                 pEntDtm                      IN occam_violation_ticket_uploads.ent_dtm%TYPE,
                                 pEntUserId                   IN occam_violation_ticket_uploads.ent_user_id%TYPE,
                                 pUpdDtm                      IN occam_violation_ticket_uploads.upd_dtm%TYPE,
                                 pUpdUserId                   IN occam_violation_ticket_uploads.upd_user_id%TYPE);


  PROCEDURE pViolationTicketUpdate(pViolationTicketId           IN occam_violation_ticket_uploads.violation_ticket_upload_id%TYPE,
                                   pTicketNumberTxt             IN occam_violation_ticket_uploads.ticket_number_txt%TYPE,
                                   pDisputantSurnameTxt         IN occam_violation_ticket_uploads.disputant_surname_txt%TYPE,
                                   pDisputantGivenNamesTxt      IN occam_violation_ticket_uploads.disputant_given_names_txt%TYPE,
                                   pDisputantOrganizationNmTxt  IN occam_violation_ticket_uploads.disputant_organization_nm_txt%TYPE,
                                   pIsYoungPersonYn             IN occam_violation_ticket_uploads.is_young_person_yn%TYPE,
                                   pDisputantDrvLicNumberTxt    IN occam_violation_ticket_uploads.disputant_drv_lic_number_txt%TYPE,
                                   pDisputantClientNumberTxt    IN occam_violation_ticket_uploads.disputant_client_number_txt%TYPE,
                                   pDrvLicIssuedProvinceTxt     IN occam_violation_ticket_uploads.drv_lic_issued_province_txt%TYPE,
                                   pDrvLicIssuedCountryTxt      IN occam_violation_ticket_uploads.drv_lic_issued_country_txt%TYPE,
                                   pDrvLicIssuedYearNo          IN occam_violation_ticket_uploads.drv_lic_issued_year_no%TYPE,
                                   pDrvLicExpiryYearNo          IN occam_violation_ticket_uploads.drv_lic_expiry_year_no%TYPE,
                                   pDisputantBirthDt            IN occam_violation_ticket_uploads.disputant_birth_dt%TYPE,
                                   pAddressTxt                  IN occam_violation_ticket_uploads.address_txt%TYPE,
                                   pAddressCityTxt              IN occam_violation_ticket_uploads.address_city_txt%TYPE,
                                   pAddressProvinceTxt          IN occam_violation_ticket_uploads.address_province_txt%TYPE,
                                   pAddressCountryTxt           IN occam_violation_ticket_uploads.address_country_txt%TYPE,
                                   pAddressPostalCodeTxt        IN occam_violation_ticket_uploads.address_postal_code_txt%TYPE,
                                   pOfficerPinTxt               IN occam_violation_ticket_uploads.officer_pin_txt%TYPE,
                                   pDetachmentLocationTxt       IN occam_violation_ticket_uploads.detachment_location_txt%TYPE,
                                   pIssuedDt                    IN occam_violation_ticket_uploads.issued_dt%TYPE,
                                   pIssuedOnRoadOrHighwayTxt    IN occam_violation_ticket_uploads.issued_on_road_or_highway_txt%TYPE,
                                   pIssuedAtOrNearCityTxt       IN occam_violation_ticket_uploads.issued_at_or_near_city_txt%TYPE,
                                   pIsChangeOfAddressYn         IN occam_violation_ticket_uploads.is_change_of_address_yn%TYPE,
                                   pIsDriverYn                  IN occam_violation_ticket_uploads.is_driver_yn%TYPE,
                                   pIsOwnerYn                   IN occam_violation_ticket_uploads.is_owner_yn%TYPE,
                                   pCourtLocationTxt            IN occam_violation_ticket_uploads.court_location_txt%TYPE,
                                   pEntDtm                      IN occam_violation_ticket_uploads.ent_dtm%TYPE,
                                   pEntUserId                   IN occam_violation_ticket_uploads.ent_user_id%TYPE,
                                   pUpdDtm                      IN occam_violation_ticket_uploads.upd_dtm%TYPE,
                                   pUpdUserId                   IN occam_violation_ticket_uploads.upd_user_id%TYPE);


  PROCEDURE pViolationTicketCountSave(pViolationTicketCountId OUT NUMBER,
                                      pViolationTicketId      IN occam_violation_ticket_counts.violation_ticket_upload_id%TYPE,
                                      pCountNo                IN occam_violation_ticket_counts.count_no%TYPE,
                                      pDescriptionTxt         IN occam_violation_ticket_counts.description_txt%TYPE,
                                      pActOrRegulationNameCd  IN occam_violation_ticket_counts.act_or_regulation_name_cd%TYPE,
                                      pIsActYn                IN occam_violation_ticket_counts.is_act_yn%TYPE,
                                      pIsRegulationYn         IN occam_violation_ticket_counts.is_regulation_yn%TYPE,
                                      pStatSectionTxt         IN occam_violation_ticket_counts.stat_section_txt%TYPE,
                                      pStatSubSectionTxt      IN occam_violation_ticket_counts.stat_sub_section_txt%TYPE,
                                      pStatParagraphTxt       IN occam_violation_ticket_counts.stat_paragraph_txt%TYPE,
                                      pStatSubParagraphTxt    IN occam_violation_ticket_counts.stat_sub_paragraph_txt%TYPE,
                                      pTicketedAmt            IN occam_violation_ticket_counts.ticketed_amt%TYPE,
                                      pEntDtm                 IN occam_violation_ticket_counts.ent_dtm%TYPE,
                                      pEntUserId              IN occam_violation_ticket_counts.ent_user_id%TYPE,
                                      pUpdDtm                 IN occam_violation_ticket_counts.upd_dtm%TYPE,
                                      pUpdUserId              IN occam_violation_ticket_counts.upd_user_id%TYPE);


  PROCEDURE pViolationTicketCountUpdate(pViolationTicketCountId IN occam_violation_ticket_counts.violation_ticket_count_id%TYPE,
                                        pViolationTicketId      IN occam_violation_ticket_counts.violation_ticket_upload_id%TYPE,
                                        pCountNo                IN occam_violation_ticket_counts.count_no%TYPE,
                                        pDescriptionTxt         IN occam_violation_ticket_counts.description_txt%TYPE,
                                        pActOrRegulationNameCd  IN occam_violation_ticket_counts.act_or_regulation_name_cd%TYPE,
                                        pIsActYn                IN occam_violation_ticket_counts.is_act_yn%TYPE,
                                        pIsRegulationYn         IN occam_violation_ticket_counts.is_regulation_yn%TYPE,
                                        pStatSectionTxt         IN occam_violation_ticket_counts.stat_section_txt%TYPE,
                                        pStatSubSectionTxt      IN occam_violation_ticket_counts.stat_sub_section_txt%TYPE,
                                        pStatParagraphTxt       IN occam_violation_ticket_counts.stat_paragraph_txt%TYPE,
                                        pStatSubParagraphTxt    IN occam_violation_ticket_counts.stat_sub_paragraph_txt%TYPE,
                                        pTicketedAmt            IN occam_violation_ticket_counts.ticketed_amt%TYPE,
                                        pEntDtm                 IN occam_violation_ticket_counts.ent_dtm%TYPE,
                                        pEntUserId              IN occam_violation_ticket_counts.ent_user_id%TYPE,
                                        pUpdDtm                 IN occam_violation_ticket_counts.upd_dtm%TYPE,
                                        pUpdUserId              IN occam_violation_ticket_counts.upd_user_id%TYPE);


  PROCEDURE pDisputeSave(pDisputeId                  OUT NUMBER,
                         pViolationTicketId          IN occam_disputes.violation_ticket_upload_id%TYPE,
                         pDisputeStatusTypeCd        IN occam_disputes.dispute_status_type_cd%TYPE,
                         pCourtAgenId                IN occam_disputes.court_agen_id%TYPE,
                         pContactTypeCd              IN occam_disputes.contact_type_cd%TYPE,
                         pRequestCourtAppearanceYn   IN occam_disputes.request_court_appearance_yn%TYPE,
                         pIssuedDt                   IN occam_disputes.issued_dt%TYPE,
                         pSubmittedDt                IN occam_disputes.submitted_dt%TYPE,
                         pDisputantSurnameNm         IN occam_disputes.disputant_surname_nm%TYPE,
                         pDisputantGiven1Nm          IN occam_disputes.disputant_given_1_nm%TYPE,
                         pDisputantGiven2Nm          IN occam_disputes.disputant_given_2_nm%TYPE,
                         pDisputantGiven3Nm          IN occam_disputes.disputant_given_3_nm%TYPE,
                         pDisputantBirthDt           IN occam_disputes.disputant_birth_dt%TYPE,
                         pDisputantOrganizationNm    IN occam_disputes.disputant_organization_nm%TYPE,
                         pDisputantDrvLicNumberTxt   IN occam_disputes.disputant_drv_lic_number_txt%TYPE,
                         pDrvLicIssuedCtryId         IN occam_disputes.drv_lic_issued_ctry_ctry_id%TYPE,
                         pDrvLicIssuedProvSeqNo      IN occam_disputes.drv_lic_issued_prov_seq_no%TYPE,
                         pDrvLicIssuedProvCtryId     IN occam_disputes.drv_lic_issued_prov_ctry_id%TYPE,
                         pDrvLicIssuedIntlProvTxt    IN occam_disputes.drv_lic_issued_other_prov_txt%TYPE,
                         pDisputantClientId          IN occam_disputes.disputant_client_id%TYPE,
                         pAddressLine1Txt            IN occam_disputes.address_line_1_txt%TYPE,
                         pAddressLine2Txt            IN occam_disputes.address_line_2_txt%TYPE,
                         pAddressLine3Txt            IN occam_disputes.address_line_3_txt%TYPE,
                         pAddressCityCtryId          IN occam_disputes.address_city_ctry_id%TYPE,
                         pAddressCitySeqNo           IN occam_disputes.address_city_seq_no%TYPE,
                         pAddressProvCtryId          IN occam_disputes.address_prov_ctry_id%TYPE,
                         pAddressProvSeqNo           IN occam_disputes.address_prov_seq_no%TYPE,
                         pAddressIntlCityTxt         IN occam_disputes.address_other_city_txt%TYPE,
                         pAddressIntlProvTxt         IN occam_disputes.address_other_prov_txt%TYPE,
                         pAddressCtryId              IN occam_disputes.address_ctry_ctry_id%TYPE,
                         pPostalCodeTxt              IN occam_disputes.postal_code_txt%TYPE,
                         pHomePhoneNumberTxt         IN occam_disputes.home_phone_number_txt%TYPE,
                         pWorkPhoneNumberTxt         IN occam_disputes.work_phone_number_txt%TYPE,
                         pContactLawFirmNm           IN occam_disputes.contact_law_firm_nm%TYPE,
                         pContactSurnameNm           IN occam_disputes.contact_surname_nm%TYPE,
                         pContactGiven1Nm            IN occam_disputes.contact_given_1_nm%TYPE,
                         pContactGiven2Nm            IN occam_disputes.contact_given_2_nm%TYPE,
                         pContactGiven3Nm            IN occam_disputes.contact_given_3_nm%TYPE,
                         pEmailAddressTxt            IN occam_disputes.email_address_txt%TYPE,
                         pEmailVerifiedYn            IN occam_disputes.email_verified_yn%TYPE,
                         pNoticeOfDisputeGuid        IN occam_disputes.notice_of_dispute_guid%TYPE,
                         pFilingDt                   IN occam_disputes.filing_dt%TYPE,
                         pRepresentedByLawyerYn      IN occam_disputes.represented_by_lawyer_yn%TYPE,
                         pLawFirmNm                  IN occam_disputes.law_firm_nm%TYPE,
                         pLawFirmAddrLine1Txt        IN occam_disputes.law_firm_addr_line_1_txt%TYPE,
                         pLawFirmAddrLine2Txt        IN occam_disputes.law_firm_addr_line_2_txt%TYPE,
                         pLawFirmAddrLine3Txt        IN occam_disputes.law_firm_addr_line_3_txt%TYPE,
                         pLawFirmAddrCityCtryId      IN occam_disputes.law_firm_addr_city_ctry_id%TYPE,
                         pLawFirmAddrCitySeqNo       IN occam_disputes.law_firm_addr_city_seq_no%TYPE,
                         pLawFirmAddrProvCtryId      IN occam_disputes.law_firm_addr_prov_ctry_id%TYPE,
                         pLawFirmAddrProvSeqNo       IN occam_disputes.law_firm_addr_prov_seq_no%TYPE,
                         pLawFirmAddrIntlCityTxt     IN occam_disputes.law_firm_addr_other_city_txt%TYPE,
                         pLawFirmAddrIntlProvTxt     IN occam_disputes.law_firm_addr_other_prov_txt%TYPE,
                         pLawFirmAddrCtryId          IN occam_disputes.law_firm_addr_ctry_ctry_id%TYPE,
                         pLawFirmAddrPostalCodeTxt   IN occam_disputes.law_firm_addr_postal_code_txt%TYPE,
                         pLawyerSurnameNm            IN occam_disputes.lawyer_surname_nm%TYPE,
                         pLawyerGiven1Nm             IN occam_disputes.lawyer_given_1_nm%TYPE,
                         pLawyerGiven2Nm             IN occam_disputes.lawyer_given_2_nm%TYPE,
                         pLawyerGiven3Nm             IN occam_disputes.lawyer_given_3_nm%TYPE,
                         pLawyerPhoneNumberTxt       IN occam_disputes.lawyer_phone_number_txt%TYPE,
                         pLawyerEmailAddressTxt      IN occam_disputes.lawyer_email_address_txt%TYPE,
                         pOfficerPinTxt              IN occam_disputes.officer_pin_txt%TYPE,
                         pDetachmentLocationTxt      IN occam_disputes.detachment_location_txt%TYPE,
                         pLanguageCd                 IN occam_disputes.language_cd%TYPE,
                         pInterpreterRequiredYn      IN occam_disputes.interpreter_required_yn%TYPE,
                         pWitnessNo                  IN occam_disputes.witness_no%TYPE,
                         pFineReductionReasonTxt     IN occam_disputes.fine_reduction_reason_txt%TYPE,
                         pTimeToPayReasonTxt         IN occam_disputes.time_to_pay_reason_txt%TYPE,
                         pDisputantCommentTxt        IN occam_disputes.disputant_comment_txt%TYPE,
                         pRejectedReasonTxt          IN occam_disputes.rejected_reason_txt%TYPE,
                         pJjAssignedTo               IN occam_disputes.jj_assigned_to%TYPE,
                         pUserAssignedTo             IN occam_disputes.user_assigned_to%TYPE,
                         pUserAssignedDtm            IN occam_disputes.user_assigned_dtm%TYPE,
                         pDisputantDetectOcrIssuesYn IN occam_disputes.disputant_detect_ocr_issues_yn%TYPE,
                         pDisputantOcrIssuesTxt      IN occam_disputes.disputant_ocr_issues_txt%TYPE,
                         pSystemDetectOcrIssuesYn    IN occam_disputes.system_detect_ocr_issues_yn%TYPE,
                         pOcrTicketJsonFilenameTxt   IN occam_disputes.ocr_ticket_json_filename_txt%TYPE,
                         pSignatoryTypeCd            IN occam_disputes.signatory_type_cd%TYPE,
                         pSignatoryNameTxt           IN occam_disputes.signatory_name_txt%TYPE,
                         pEntDtm                     IN occam_disputes.ent_dtm%TYPE,
                         pEntUserId                  IN occam_disputes.ent_user_id%TYPE,
                         pUpdDtm                     IN occam_disputes.upd_dtm%TYPE,
                         pUpdUserId                  IN occam_disputes.upd_user_id%TYPE);


  PROCEDURE pDisputeUpdate(pDisputeId                  IN occam_disputes.dispute_id%TYPE,
                           pViolationTicketId          IN occam_disputes.violation_ticket_upload_id%TYPE,
                           pDisputeStatusTypeCd        IN occam_disputes.dispute_status_type_cd%TYPE,
                           pCourtAgenId                IN occam_disputes.court_agen_id%TYPE,
                           pContactTypeCd              IN occam_disputes.contact_type_cd%TYPE,
                           pRequestCourtAppearanceYn   IN occam_disputes.request_court_appearance_yn%TYPE,
                           pIssuedDt                   IN occam_disputes.issued_dt%TYPE,
                           pSubmittedDt                IN occam_disputes.submitted_dt%TYPE,
                           pDisputantSurnameNm         IN occam_disputes.disputant_surname_nm%TYPE,
                           pDisputantGiven1Nm          IN occam_disputes.disputant_given_1_nm%TYPE,
                           pDisputantGiven2Nm          IN occam_disputes.disputant_given_2_nm%TYPE,
                           pDisputantGiven3Nm          IN occam_disputes.disputant_given_3_nm%TYPE,
                           pDisputantBirthDt           IN occam_disputes.disputant_birth_dt%TYPE,
                           pDisputantOrganizationNm    IN occam_disputes.disputant_organization_nm%TYPE,
                           pDisputantDrvLicNumberTxt   IN occam_disputes.disputant_drv_lic_number_txt%TYPE,
                           pDrvLicIssuedCtryId         IN occam_disputes.drv_lic_issued_ctry_ctry_id%TYPE,
                           pDrvLicIssuedProvSeqNo      IN occam_disputes.drv_lic_issued_prov_seq_no%TYPE,
                           pDrvLicIssuedProvCtryId     IN occam_disputes.drv_lic_issued_prov_ctry_id%TYPE,
                           pDrvLicIssuedIntlProvTxt    IN occam_disputes.drv_lic_issued_other_prov_txt%TYPE,
                           pDisputantClientId          IN occam_disputes.disputant_client_id%TYPE,
                           pAddressLine1Txt            IN occam_disputes.address_line_1_txt%TYPE,
                           pAddressLine2Txt            IN occam_disputes.address_line_2_txt%TYPE,
                           pAddressLine3Txt            IN occam_disputes.address_line_3_txt%TYPE,
                           pAddressCityCtryId          IN occam_disputes.address_city_ctry_id%TYPE,
                           pAddressCitySeqNo           IN occam_disputes.address_city_seq_no%TYPE,
                           pAddressProvCtryId          IN occam_disputes.address_prov_ctry_id%TYPE,
                           pAddressProvSeqNo           IN occam_disputes.address_prov_seq_no%TYPE,
                           pAddressIntlCityTxt         IN occam_disputes.address_other_city_txt%TYPE,
                           pAddressIntlProvTxt         IN occam_disputes.address_other_prov_txt%TYPE,
                           pAddressCtryId              IN occam_disputes.address_ctry_ctry_id%TYPE,
                           pPostalCodeTxt              IN occam_disputes.postal_code_txt%TYPE,
                           pHomePhoneNumberTxt         IN occam_disputes.home_phone_number_txt%TYPE,
                           pWorkPhoneNumberTxt         IN occam_disputes.work_phone_number_txt%TYPE,
                           pContactLawFirmNm           IN occam_disputes.contact_law_firm_nm%TYPE,
                           pContactSurnameNm           IN occam_disputes.contact_surname_nm%TYPE,
                           pContactGiven1Nm            IN occam_disputes.contact_given_1_nm%TYPE,
                           pContactGiven2Nm            IN occam_disputes.contact_given_2_nm%TYPE,
                           pContactGiven3Nm            IN occam_disputes.contact_given_3_nm%TYPE,
                           pEmailAddressTxt            IN occam_disputes.email_address_txt%TYPE,
                           pEmailVerifiedYn            IN occam_disputes.email_verified_yn%TYPE,
                           pNoticeOfDisputeGuid        IN occam_disputes.notice_of_dispute_guid%TYPE,
                           pFilingDt                   IN occam_disputes.filing_dt%TYPE,
                           pRepresentedByLawyerYn      IN occam_disputes.represented_by_lawyer_yn%TYPE,
                           pLawFirmNm                  IN occam_disputes.law_firm_nm%TYPE,
                           pLawFirmAddrLine1Txt        IN occam_disputes.law_firm_addr_line_1_txt%TYPE,
                           pLawFirmAddrLine2Txt        IN occam_disputes.law_firm_addr_line_2_txt%TYPE,
                           pLawFirmAddrLine3Txt        IN occam_disputes.law_firm_addr_line_3_txt%TYPE,
                           pLawFirmAddrCityCtryId      IN occam_disputes.law_firm_addr_city_ctry_id%TYPE,
                           pLawFirmAddrCitySeqNo       IN occam_disputes.law_firm_addr_city_seq_no%TYPE,
                           pLawFirmAddrProvCtryId      IN occam_disputes.law_firm_addr_prov_ctry_id%TYPE,
                           pLawFirmAddrProvSeqNo       IN occam_disputes.law_firm_addr_prov_seq_no%TYPE,
                           pLawFirmAddrIntlCityTxt     IN occam_disputes.law_firm_addr_other_city_txt%TYPE,
                           pLawFirmAddrIntlProvTxt     IN occam_disputes.law_firm_addr_other_prov_txt%TYPE,
                           pLawFirmAddrCtryId          IN occam_disputes.law_firm_addr_ctry_ctry_id%TYPE,
                           pLawFirmAddrPostalCodeTxt   IN occam_disputes.law_firm_addr_postal_code_txt%TYPE,
                           pLawyerSurnameNm            IN occam_disputes.lawyer_surname_nm%TYPE,
                           pLawyerGiven1Nm             IN occam_disputes.lawyer_given_1_nm%TYPE,
                           pLawyerGiven2Nm             IN occam_disputes.lawyer_given_2_nm%TYPE,
                           pLawyerGiven3Nm             IN occam_disputes.lawyer_given_3_nm%TYPE,
                           pLawyerPhoneNumberTxt       IN occam_disputes.lawyer_phone_number_txt%TYPE,
                           pLawyerEmailAddressTxt      IN occam_disputes.lawyer_email_address_txt%TYPE,
                           pOfficerPinTxt              IN occam_disputes.officer_pin_txt%TYPE,
                           pDetachmentLocationTxt      IN occam_disputes.detachment_location_txt%TYPE,
                           pLanguageCd                 IN occam_disputes.language_cd%TYPE,
                           pInterpreterRequiredYn      IN occam_disputes.interpreter_required_yn%TYPE,
                           pWitnessNo                  IN occam_disputes.witness_no%TYPE,
                           pFineReductionReasonTxt     IN occam_disputes.fine_reduction_reason_txt%TYPE,
                           pTimeToPayReasonTxt         IN occam_disputes.time_to_pay_reason_txt%TYPE,
                           pDisputantCommentTxt        IN occam_disputes.disputant_comment_txt%TYPE,
                           pRejectedReasonTxt          IN occam_disputes.rejected_reason_txt%TYPE,
                           pJjAssignedTo               IN occam_disputes.jj_assigned_to%TYPE,
                           pUserAssignedTo             IN occam_disputes.user_assigned_to%TYPE,
                           pUserAssignedDtm            IN occam_disputes.user_assigned_dtm%TYPE,
                           pDisputantDetectOcrIssuesYn IN occam_disputes.disputant_detect_ocr_issues_yn%TYPE,
                           pDisputantOcrIssuesTxt      IN occam_disputes.disputant_ocr_issues_txt%TYPE,
                           pSystemDetectOcrIssuesYn    IN occam_disputes.system_detect_ocr_issues_yn%TYPE,
                           pOcrTicketJsonFilenameTxt   IN occam_disputes.ocr_ticket_json_filename_txt%TYPE,
                           pSignatoryTypeCd            IN occam_disputes.signatory_type_cd%TYPE,
                           pSignatoryNameTxt           IN occam_disputes.signatory_name_txt%TYPE,
                           pEntDtm                     IN occam_disputes.ent_dtm%TYPE,
                           pEntUserId                  IN occam_disputes.ent_user_id%TYPE,
                           pUpdDtm                     IN occam_disputes.upd_dtm%TYPE,
                           pUpdUserId                  IN occam_disputes.upd_user_id%TYPE);



  PROCEDURE pDisputeCountSave(pViolationTicketCountId   IN occam_dispute_counts.violation_ticket_count_id%TYPE,
                              pDisputeId                IN occam_dispute_counts.dispute_id%TYPE,
                              pStatId                   IN occam_dispute_counts.stat_id%TYPE,
                              pPleaCd                   IN occam_dispute_counts.plea_cd%TYPE,
                              pRequestTimeToPayYn       IN occam_dispute_counts.request_time_to_pay_yn%TYPE,
                              pRequestReductionYn       IN occam_dispute_counts.request_reduction_yn%TYPE,
                              pRequestCourtAppearanceYn IN occam_dispute_counts.request_court_appearance_yn%TYPE,
                              pEntDtm                   IN occam_dispute_counts.ent_dtm%TYPE,
                              pEntUserId                IN occam_dispute_counts.ent_user_id%TYPE,
                              pUpdDtm                   IN occam_dispute_counts.upd_dtm%TYPE,
                              pUpdUserId                IN occam_dispute_counts.upd_user_id%TYPE);


  PROCEDURE pDisputeCountUpdate(pViolationTicketCountId   IN occam_dispute_counts.violation_ticket_count_id%TYPE,
                                pDisputeId                IN occam_dispute_counts.dispute_id%TYPE,
                                pStatId                   IN occam_dispute_counts.stat_id%TYPE,
                                pPleaCd                   IN occam_dispute_counts.plea_cd%TYPE,
                                pRequestTimeToPayYn       IN occam_dispute_counts.request_time_to_pay_yn%TYPE,
                                pRequestReductionYn       IN occam_dispute_counts.request_reduction_yn%TYPE,
                                pRequestCourtAppearanceYn IN occam_dispute_counts.request_court_appearance_yn%TYPE,
                                pEntDtm                   IN occam_dispute_counts.ent_dtm%TYPE,
                                pEntUserId                IN occam_dispute_counts.ent_user_id%TYPE,
                                pUpdDtm                   IN occam_dispute_counts.upd_dtm%TYPE,
                                pUpdUserId                IN occam_dispute_counts.upd_user_id%TYPE);


  PROCEDURE pViolationTicketDelete(pViolationTicketId   IN NUMBER
                                 , pDisputeId           IN NUMBER
                                 , pNoticeOfDisputeGuid IN VARCHAR2);

  PROCEDURE pViolationTicketAssign(pViolationTicketId   IN NUMBER
                                 , pDisputeId           IN NUMBER
                                 , pNoticeOfDisputeGuid IN VARCHAR2
                                 , pUserId              IN VARCHAR2);

  PROCEDURE pViolationTicketUnassign(pAssignedBeforeTs IN DATE);

  PROCEDURE pViolationTicketUpdateStatus(pViolationTicketId   IN NUMBER
                                       , pDisputeId           IN NUMBER
                                       , pNoticeOfDisputeGuid IN VARCHAR2
                                       , pStatusCd            IN VARCHAR2
                                       , pRejectReason        IN VARCHAR2
                                       , pUserId              IN VARCHAR2);

  PROCEDURE pDisputeStatusUpdate(pViolationTicketUploadId  IN occam_disputes.violation_ticket_upload_id%TYPE,
                                 pDisputeStatusTypeCd      IN occam_disputes.dispute_status_type_cd%TYPE);

  PROCEDURE pViolationTicketCountDelete(pViolationTicketCountId IN NUMBER);


END;
/

create package occam_error is

  type typerecResult is record                                   -- Error record
    ( bStatus         boolean                                    -- The Result Status (True=Pass; False Fail)
    , vPgmnam         OCCAM_ERROR_LOGS.PROGRAM_NM%type             -- The Name of the Program Unit
    , vPgmVer         OCCAM_ERROR_LOGS.PROGRAM_VERSION_TXT%type    -- Version Format is '00000'
    , nSqlCode        OCCAM_ERROR_LOGS.ERROR_MESSAGE_NO%type       -- Oracle Error Code Variable sqlcode
    , vSqlErrm        OCCAM_ERROR_LOGS.ERROR_MESSAGE_TXT%type      -- Oracle Error Msg Variable sqlerm
    , vMsgTxt         OCCAM_ERROR_LOGS.ERROR_DETAIL_TXT%type);     -- Any additional debug information  


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

  FUNCTION extract_json_clob (p_json_clob IN CLOB, p_json_keyname IN VARCHAR2)  RETURN CLOB;

  FUNCTION unescape_chars(val in VARCHAR2) RETURN VARCHAR2;
end occam_error; 
/

create PACKAGE       occam_audit_log IS

  PROCEDURE pAuditLogEntrySave(pAuditLogEntryId         OUT occam_audit_log_entries.audit_log_entry_id%TYPE,
                               pDisputeId               IN occam_audit_log_entries.dispute_id%TYPE,
                               pAuditLogEntryTypeCd     IN occam_audit_log_entries.audit_log_entry_type_cd%TYPE,
                               pActionByApplicationUser IN occam_audit_log_entries.action_by_application_user%TYPE,
                               pComment                 IN occam_audit_log_entries.comment_txt%TYPE,
                               pEntDtm                  IN occam_audit_log_entries.ent_dtm%TYPE,
                               pEntUserId               IN occam_audit_log_entries.ent_user_id%TYPE,
                               pUpdDtm                  IN occam_audit_log_entries.upd_dtm%TYPE,
                               pUpdUserId               IN occam_audit_log_entries.upd_user_id%TYPE);


  PROCEDURE pAuditLogEntrySave(pAuditLogEntryId         OUT occam_audit_log_entries.audit_log_entry_id%TYPE,
                               pDisputeId               IN occam_audit_log_entries.dispute_id%TYPE,
                               pViolationTicketId       IN occam_violation_ticket_uploads.violation_ticket_upload_id%TYPE,
                               pAuditLogEntryTypeCd     IN occam_audit_log_entries.audit_log_entry_type_cd%TYPE,
                               pActionByApplicationUser IN occam_audit_log_entries.action_by_application_user%TYPE,
                               pComment                 IN occam_audit_log_entries.comment_txt%TYPE,
                               pEntDtm                  IN occam_audit_log_entries.ent_dtm%TYPE,
                               pEntUserId               IN occam_audit_log_entries.ent_user_id%TYPE,
                               pUpdDtm                  IN occam_audit_log_entries.upd_dtm%TYPE,
                               pUpdUserId               IN occam_audit_log_entries.upd_user_id%TYPE);


  PROCEDURE pOutgoingEmailSave(pOutgoingEmailId        OUT occam_outgoing_emails.outgoing_email_id%TYPE,
                               pDisputeId              IN occam_outgoing_emails.dispute_id%TYPE,
                               pEmailSentDtm           IN occam_outgoing_emails.email_sent_dtm%TYPE,
                               pEmailSubjectTxt        IN occam_outgoing_emails.email_subject_txt%TYPE,
                               pFromEmailAddress       IN occam_outgoing_emails.from_email_address%TYPE,
                               pToEmailAddress         IN occam_outgoing_emails.to_email_address%TYPE,
                               pCcEmailAddress         IN occam_outgoing_emails.cc_email_address%TYPE,
                               pBccEmailAddress        IN occam_outgoing_emails.bcc_email_address%TYPE,
                               pSuccessfullySentYn     IN occam_outgoing_emails.successfully_sent_yn%TYPE,
                               pHtmlContent            IN occam_outgoing_emails.html_content%TYPE,
                               pPlainTextContent       IN occam_outgoing_emails.plain_text_content%TYPE,
                               pEntDtm                 IN occam_outgoing_emails.ent_dtm%TYPE,
                               pEntUserId              IN occam_outgoing_emails.ent_user_id%TYPE,
                               pUpdDtm                 IN occam_outgoing_emails.upd_dtm%TYPE,
                               pUpdUserId              IN occam_outgoing_emails.upd_user_id%TYPE);


  PROCEDURE pAuditLogEntryTypeSave(pAuditLogEntryTypeCd       IN occam_audit_log_entry_types.audit_log_entry_type_cd%TYPE,
                                   pAuditLogEntryTypeDsc      IN occam_audit_log_entry_types.audit_log_entry_type_dsc%TYPE,
                                   pAuditLogEntryTypeShortDsc IN occam_audit_log_entry_types.audit_log_entry_type_short_dsc%TYPE,
                                   pActiveUseYn               IN occam_audit_log_entry_types.active_use_yn%TYPE,
                                   pEntDtm                    IN occam_audit_log_entry_types.ent_dtm%TYPE,
                                   pEntUserId                 IN occam_audit_log_entry_types.ent_user_id%TYPE,
                                   pUpdDtm                    IN occam_audit_log_entry_types.upd_dtm%TYPE,
                                   pUpdUserId                 IN occam_audit_log_entry_types.upd_user_id%TYPE);


  PROCEDURE pAuditLogEntryTypeUpdate(pAuditLogEntryTypeCd       IN occam_audit_log_entry_types.audit_log_entry_type_cd%TYPE,
                                     pAuditLogEntryTypeDsc      IN occam_audit_log_entry_types.audit_log_entry_type_dsc%TYPE,
                                     pAuditLogEntryTypeShortDsc IN occam_audit_log_entry_types.audit_log_entry_type_short_dsc%TYPE,
                                     pActiveUseYn               IN occam_audit_log_entry_types.active_use_yn%TYPE,
                                     pEntDtm                    IN occam_audit_log_entry_types.ent_dtm%TYPE,
                                     pEntUserId                 IN occam_audit_log_entry_types.ent_user_id%TYPE,
                                     pUpdDtm                    IN occam_audit_log_entry_types.upd_dtm%TYPE,
                                     pUpdUserId                 IN occam_audit_log_entry_types.upd_user_id%TYPE);


END; 
/

create PACKAGE       occam_dispute_update_request IS

  PROCEDURE pDisputeUpdateRequestSave(pDisputeUpdateRequestId       OUT NUMBER,
                                      pDisputeId                    IN occam_dispute_update_requests.dispute_id%TYPE,
                                      pDisputeUpdateStatTypeCd      IN occam_dispute_update_requests.dispute_update_stat_type_cd%TYPE,
                                      pDisputeUpdateReqTypeCd       IN occam_dispute_update_requests.dispute_update_req_type_cd%TYPE,
                                      pRequestJsonTxt               IN occam_dispute_update_requests.request_json_txt%TYPE,
                                      pCurrentJsonTxt               IN occam_dispute_update_requests.current_json_txt%TYPE,
                                      pEntDtm                       IN occam_dispute_update_requests.ent_dtm%TYPE,
                                      pEntUserId                    IN occam_dispute_update_requests.ent_user_id%TYPE,
                                      pUpdDtm                       IN occam_dispute_update_requests.upd_dtm%TYPE,
                                      pUpdUserId                    IN occam_dispute_update_requests.upd_user_id%TYPE);


  PROCEDURE pDisputeUpdateRequestUpdate(pDisputeUpdateRequestId       IN occam_dispute_update_requests.dispute_update_request_id%TYPE,
                                        pDisputeId                    IN occam_dispute_update_requests.dispute_id%TYPE,
                                        pDisputeUpdateStatTypeCd      IN occam_dispute_update_requests.dispute_update_stat_type_cd%TYPE,
                                        pDisputeUpdateReqTypeCd       IN occam_dispute_update_requests.dispute_update_req_type_cd%TYPE,
                                        pRequestJsonTxt               IN occam_dispute_update_requests.request_json_txt%TYPE,
                                        pStatusUpdateDtm              IN occam_dispute_update_requests.status_update_dtm%TYPE,
                                        pEntDtm                       IN occam_dispute_update_requests.ent_dtm%TYPE,
                                        pEntUserId                    IN occam_dispute_update_requests.ent_user_id%TYPE,
                                        pUpdDtm                       IN occam_dispute_update_requests.upd_dtm%TYPE,
                                        pUpdUserId                    IN occam_dispute_update_requests.upd_user_id%TYPE);


  PROCEDURE pDisputeUpdateRequestDelete(pDisputeUpdateRequestId   IN occam_dispute_update_requests.dispute_update_request_id%TYPE
                                      , pDisputeId                IN occam_dispute_update_requests.dispute_id%TYPE);

  PROCEDURE pDisputeUpdateRequestStatus(pDisputeUpdateRequestId IN occam_dispute_update_requests.dispute_update_request_id%TYPE
                                      , pStatusCd               IN VARCHAR2
                                      , pUserId                 IN VARCHAR2);
END; 
/

create package occam_query
as

type t_occam_dispute_list_get_args_type is record (
  select_columns     tco.tco_common.t_column_name_table_type default tco.tco_common.t_column_name_table_type(),
  include_columns    tco.tco_common.t_column_name_table_type default tco.tco_common.t_column_name_table_type(),
  filter_by          tco.tco_common.t_predicates_type,
  order_by           tco.tco_common.t_order_by_table_type default tco.tco_common.t_order_by_table_type(),
  paging             tco_common.t_paging_type
);

procedure occam_dispute_list_get(
  in_args     in t_occam_dispute_list_get_args_type default null,
  out_data   out sys_refcursor,
  out_page   out tco_common.t_current_page_type);

type t_occam_violation_ticket_list_get_args_type is record (
  select_columns     tco.tco_common.t_column_name_table_type default tco.tco_common.t_column_name_table_type(),
  include_columns    tco.tco_common.t_column_name_table_type default tco.tco_common.t_column_name_table_type(),
  filter_by          tco.tco_common.t_predicates_type,
  order_by           tco.tco_common.t_order_by_table_type default tco.tco_common.t_order_by_table_type(),
  paging             tco_common.t_paging_type
);

procedure occam_violation_ticket_list_get(
  in_args    in t_occam_violation_ticket_list_get_args_type default null,
  out_data   out sys_refcursor,
  out_page   out tco_common.t_current_page_type);

type t_occam_violation_ticket_get_args_type is record (
  violation_ticket_upload_id        occam_violation_ticket_uploads.violation_ticket_upload_id%type,
  dispute_id                        occam_disputes.dispute_id%type,
  notice_of_dispute_guid            occam_disputes.notice_of_dispute_guid%type,
  include                           tco.tco_common.t_column_name_table_type default tco.tco_common.t_column_name_table_type()
);

procedure occam_violation_ticket_get(
  in_args                             in t_occam_violation_ticket_get_args_type,
  out_occam_violation_ticket_uploads out sys_refcursor,
  out_occam_violation_ticket_counts  out sys_refcursor,
  out_occam_disputes                 out sys_refcursor,
  out_occam_dispute_counts           out sys_refcursor,
  out_occam_audit_log_entries        out sys_refcursor,
  out_occam_dispute_update_requests  out sys_refcursor,
  out_occam_outgoing_emails          out sys_refcursor);

procedure occam_violation_ticket_delete(
  p_violation_ticket_upload_id        in occam_violation_ticket_uploads.violation_ticket_upload_id%type,
  p_dispute_id                        in occam_disputes.dispute_id%type,
  p_notice_of_dispute_guid            in occam_disputes.notice_of_dispute_guid%type);

procedure occam_violation_ticket_assign(
  p_violation_ticket_upload_id         in occam_disputes.violation_ticket_upload_id%type,
  p_dispute_id                         in occam_disputes.dispute_id%type,
  p_notice_of_dispute_guid             in occam_disputes.notice_of_dispute_guid%type,
  p_user_assigned_to                   in occam_disputes.user_assigned_to%type);

procedure occam_violation_ticket_unassign(
  p_user_assigned_dtm                  in occam_disputes.user_assigned_dtm%type);

procedure occam_violation_ticket_count_delete(
  p_violation_ticket_count_id          in occam_violation_ticket_counts.violation_ticket_count_id%type);

type t_occam_dispute_update_requests_list_get_args_type is record (
  select_columns     tco.tco_common.t_column_name_table_type default tco.tco_common.t_column_name_table_type(),
  include_columns    tco.tco_common.t_column_name_table_type default tco.tco_common.t_column_name_table_type(),
  filter_by          tco.tco_common.t_predicates_type,
  order_by           tco.tco_common.t_order_by_table_type default tco.tco_common.t_order_by_table_type(),
  paging             tco_common.t_paging_type
);

procedure occam_dispute_update_requests_list_get(
  in_args    in t_occam_dispute_update_requests_list_get_args_type default null,
  out_data   out sys_refcursor,
  out_page   out tco_common.t_current_page_type);

procedure occam_dispute_update_request_delete(
  p_dispute_update_request_id         in occam_dispute_update_requests.dispute_update_request_id%type,
  p_dispute_id                        in occam_disputes.dispute_id%type);

type t_occam_audit_log_entry_types_get_args_type is record (
  filter_by   tco_common.t_predicates_type,
  order_by    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure occam_audit_log_entry_types_get (
  in_args    in t_occam_audit_log_entry_types_get_args_type default null,
  out_data   out sys_refcursor);
    
type t_occam_audit_log_entries_list_get_args_type is record (
  filter_by   tco_common.t_predicates_type,
  order_by    tco_common.t_order_by_table_type default tco_common.t_order_by_table_type()
);

procedure occam_audit_log_entries_list_get(
  in_args    in t_occam_audit_log_entries_list_get_args_type default null,
  out_data   out sys_refcursor);

end occam_query;
/

create package occam_rest_v2
as

procedure occam_dispute_list_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor,
  out_rows_total                       out number,
  out_rows_offset                      out number,
  out_rows_fetch                       out number); 

procedure occam_violation_ticket_list_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor,
  out_rows_total                       out number,
  out_rows_offset                      out number,
  out_rows_fetch                       out number);

procedure occam_violation_ticket_get(
  in_parameters                        in tco_common.dictionary_t,
  out_occam_violation_ticket_uploads   out sys_refcursor,
  out_occam_violation_ticket_counts    out sys_refcursor,
  out_occam_disputes                   out sys_refcursor,
  out_occam_dispute_counts             out sys_refcursor,
  out_occam_audit_log_entries          out sys_refcursor,
  out_occam_dispute_update_requests    out sys_refcursor,
  out_occam_outgoing_emails            out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure occam_violation_ticket_delete(
  in_parameters                        in tco_common.dictionary_t,
  out_errors                           out sys_refcursor);

procedure occam_violation_ticket_assign(
  in_parameters                        in tco_common.dictionary_t,
  out_errors                           out sys_refcursor);

procedure occam_violation_ticket_unassign(
  in_parameters                        in tco_common.dictionary_t,
  out_errors                           out sys_refcursor);

procedure occam_violation_ticket_count_delete(
  in_parameters                        in tco_common.dictionary_t,
  out_errors                           out sys_refcursor);

procedure occam_dispute_update_requests_list_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor,
  out_rows_total                       out number,
  out_rows_offset                      out number,
  out_rows_fetch                       out number);

procedure occam_dispute_update_request_delete(
  in_parameters                        in tco_common.dictionary_t,
  out_errors                           out sys_refcursor);

procedure occam_audit_log_entry_types_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

procedure occam_audit_log_entries_list_get(
  in_parameters                        in tco_common.dictionary_t,
  out_data                             out sys_refcursor,
  out_errors                           out sys_refcursor);

end occam_rest_v2;
/

create package occam_audit as
  procedure set_fields (inserting   in boolean,
                        ent_dtm     in out date,
                        ent_user_id in out varchar2,
                        upd_dtm     in out date,
                        upd_user_id in out varchar2);
end occam_audit;
/

