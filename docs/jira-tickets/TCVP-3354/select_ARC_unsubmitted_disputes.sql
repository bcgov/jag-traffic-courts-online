SELECT
    count(*)
FROM
    OCCAM.OCCAM_DISPUTES OD
WHERE 
    OD.DISPUTE_STATUS_TYPE_CD = 'PROC'
    -- Disputes that do not have a 'SPRC' (Submitted to ARC) audit log entry
    AND (SELECT 
            COUNT(*) 
        FROM 
            OCCAM.OCCAM_AUDIT_LOG_ENTRIES OL
        WHERE 
            OL.DISPUTE_ID = OD.DISPUTE_ID
            AND OL.AUDIT_LOG_ENTRY_TYPE_CD = 'SPRC'
        ) < 1
;

SELECT
    OD.DISPUTE_ID,
    OV.TICKET_NUMBER_TXT,
    OD.DISPUTE_STATUS_TYPE_CD,
    OD.ISSUED_DT,
    OD.SUBMITTED_DT
FROM
    OCCAM.OCCAM_DISPUTES OD
        LEFT JOIN OCCAM.OCCAM_VIOLATION_TICKET_UPLOADS OV 
            ON OD.VIOLATION_TICKET_UPLOAD_ID = OV.VIOLATION_TICKET_UPLOAD_ID
WHERE 
    OD.DISPUTE_STATUS_TYPE_CD = 'PROC'
    -- Disputes that do not have a 'SPRC' (Submitted to ARC) audit log entry
    AND (SELECT 
            COUNT(*) 
        FROM 
            OCCAM.OCCAM_AUDIT_LOG_ENTRIES OL
        WHERE 
            OL.DISPUTE_ID = OD.DISPUTE_ID
            AND OL.AUDIT_LOG_ENTRY_TYPE_CD = 'SPRC'
        ) < 1
    ORDER BY
        OD.SUBMITTED_DT DESC
FETCH 
    NEXT 20 ROWS ONLY;
