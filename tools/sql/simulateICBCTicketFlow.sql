/*------------------------------------------------------------------
------  NOTE: This script can be run from occam@devj 
------------------------------------------------------------------*/

/* ------------------------------------------------------------------
  Step #1 push the data from OCCAM into the icbc messages table:
    - use the query below to find what tickets you want to push
------------------------------------------------------------------*/

/************************  FIND the Tickets to push with this query ********************************************
    -- user this one for automatically choosing new Tickets that are not in TCO
    select vitu.*, vitc.dispute_id 
      from occam_disputes vitc, occam_violation_ticket_uploads vitu
     where vitc.dispute_status_type_cd = 'PROC' 
       and vitc.violation_ticket_upload_id = vitu.violation_ticket_upload_id
      and not exists 
        (select 'x' from tco_disputes disp where disp.ticket_number_txt = vitu.ticket_number_txt 
        )
      and not exists 
        (select 'x' from justin_icbc_messages imsg where imsg.field_1_txt = vitu.ticket_number_txt 
        )
      and vitu.ticket_number_txt not in ('SG00003236','EZ02047195','EZ02047194','EZ02047193','EZ02047192',
                                         'EZ02047196','AK05827805','AK05827798','AK05827780','AK05827772',
                                         'AK05827764',
                                         -- Round 2
                                         'AK05827516','AK05827524','AK05827532','AK05827540','AK05827558','AK05827566',
                                         'EB02000001','EB02000002','EB02000003','EB02000004','EB02000005',
                                         -- Round 3
                                         'AK58033087','AK58033425','AK83416349','AK86580448','AK83416331')
    order by vitc.dispute_id 

*****************************************************************************************************************/

/* ------------------------------------------------------------------
  Step #2 is to push the data from OCCAM into the icbc Messages table:
    1. update the where clause below with the list of tickets to be pushed
    2. Run the script
------------------------------------------------------------------*/

DECLARE
  vReturn VARCHAR2(10);
  vDisputeId NUMBER;

  CURSOR cDisputeCountList(cpDisputeId  IN NUMBER)
  IS
   
   SELECT vt.ticket_number_txt,
           TO_CHAR(d.SUBMITTED_DT, 'YYYY-MM-DD') AS SUBMITTED_DT,
           TO_CHAR(d.ISSUED_DT, 'YYYY-MM-DD') AS ISSUED_DT,
           'I' AS client_type_code, -- client type code
           d.DISPUTANT_SURNAME_NM,
           d.DISPUTANT_GIVEN_1_NM,
           d.DISPUTANT_GIVEN_2_NM,
           d.DISPUTANT_GIVEN_3_NM,
           d.DISPUTANT_ORGANIZATION_NM AS organization_name,
           d.DISPUTANT_DRV_LIC_NUMBER_TXT,
           NVL(jpdl.PROV_ABBREVIATION_CD, d.DRV_LIC_ISSUED_OTHER_PROV_TXT) driver_jurisdiction_code,
           NULL AS driver_mvb_client_no,
           NULL AS driver_gender,
           TO_CHAR(d.disputant_birth_dt, 'YYYY-MM-DD') AS disputant_birth_dt,
           d.ADDRESS_LINE_1_TXT,
           d.address_line_2_txt,
           d.address_line_3_txt,
           d.ADDRESS_OTHER_CITY_TXT,
           NVL(jp.PROV_ABBREVIATION_CD, d.ADDRESS_OTHER_PROV_TXT) AS address_jurisdiction_code,
           NVL(jc.ctry_short_nm, jcp.ctry_short_nm) address_country_code_txt,
           d.POSTAL_CODE_TXT,
           vt.ISSUED_ON_ROAD_OR_HIGHWAY_TXT,
           vt.ISSUED_AT_OR_NEAR_CITY_TXT,
           vtc.COUNT_NO,
           vtc.ACT_OR_REGULATION_NAME_CD,
           vtc.STAT_SECTION_TXT,
           vtc.STAT_SUB_SECTION_TXT,
           vtc.STAT_PARAGRAPH_TXT,
           vtc.STAT_SUB_PARAGRAPH_TXT,
           NULL AS csb_count_clause,
           vtc.TICKETED_AMT,
           'A' AS dispute_type,
           NULL AS vehicle_juridiction_txt,
           NULL AS vehicle_plate_number,
           NULL AS vehicle_owner_nm,
           NULL AS vehicle_type,
           NULL AS vehicle_colour,
           NULL AS notice_of_dispute_addr_txt,
           ja.AGEN_AGENCY_NM AS hearing_location_txt,
           ja.agen_agency_identifier_cd,
           vt.OFFICER_PIN_TXT,
           NULL enforcement_officer_name_txt,
           vt.DETACHMENT_LOCATION_TXT,
           NULL enforcement_agency_code_txt,
           'false' AS accident_flag_txt,
           NULL AS witness_officer_pin_txt,
           NULL AS witness_officer_name_txt,
           NULL AS cos_form_number_txt,
           NULL AS evt_form_number_txt,
           NULL AS mre_minor_version_txt,
           NULL AS ticket_xml_clob
      FROM occam.OCCAM_VIOLATION_TICKET_UPLOADS vt
      JOIN occam.OCCAM_VIOLATION_TICKET_COUNTS vtc ON vtc.VIOLATION_TICKET_UPLOAD_ID = vt.VIOLATION_TICKET_UPLOAD_ID
      JOIN occam.OCCAM_DISPUTES d ON d.VIOLATION_TICKET_UPLOAD_ID = vt.VIOLATION_TICKET_UPLOAD_ID
      LEFT JOIN occam.OCCAM_DISPUTE_COUNTS dc ON dc.DISPUTE_ID = d.DISPUTE_ID AND dc.VIOLATION_TICKET_COUNT_ID = vtc.VIOLATION_TICKET_COUNT_ID
      LEFT JOIN justin.JUSTIN_PROVINCES jpdl ON jpdl.CTRY_ID = d.DRV_LIC_ISSUED_PROV_CTRY_ID AND jpdl.PROV_SEQ_NO = d.DRV_LIC_ISSUED_PROV_SEQ_NO
      LEFT JOIN justin.JUSTIN_PROVINCES jp ON jp.CTRY_ID = d.ADDRESS_PROV_CTRY_ID AND jp.PROV_SEQ_NO = d.ADDRESS_PROV_SEQ_NO
      LEFT JOIN justin.JUSTIN_COUNTRIES jcp ON jcp.ctry_id = d.address_prov_ctry_id
      LEFT JOIN justin.JUSTIN_COUNTRIES jc ON jc.ctry_id = d.address_ctry_ctry_id
      LEFT JOIN JUSTIN.JUSTIN_AGENCIES ja ON ja.AGEN_ID = d.COURT_AGEN_ID
     WHERE d.dispute_id = cpDisputeId
       AND ( nvl(dc.PLEA_CD,'G') = 'N' or dc.REQUEST_COURT_APPEARANCE_YN = 'Y' or REQUEST_REDUCTION_YN = 'Y' or 
            REQUEST_TIME_TO_PAY_YN = 'Y');

  cursor cVitu is
    select dispute_id from occam_disputes vitc 
     where vitc.violation_ticket_upload_id in 
             (select vitu.violation_ticket_upload_id from occam_violation_ticket_uploads vitu 
               where vitu.ticket_number_txt in ('AM11223344', 'AM11223347', 'AS00000152', 'AS00000153', 'AS00000154', 
                                                'AT99990007', 'AT99990009', 'AT99990010','AM11223345','AM11223346' )
--                 and vitu.ticket_number_txt not in ('SG00003236','EZ02047195','EZ02047194','EZ02047193','EZ02047192',
--                                                    'EZ02047196','AK05827805','AK05827798','AK05827780','AK05827772',
--                                                    'AK05827764',
--                                                    -- Round 2
--                                                    'AK05827516','AK05827524','AK05827532','AK05827540','AK05827558','AK05827566',
--                                                    'EB02000001','EB02000002','EB02000003','EB02000004','EB02000005',
--                                                    -- Round 3
--                                                    'AK58033087','AK58033425','AK83416349','AK86580448','AK83416331','EB02000005')
--                 and vitu.disputant_surname_txt is not null
             )
      and dispute_status_type_cd = 'PROC'
    order by violation_ticket_upload_id;

BEGIN

  FOR recVitu in cVitu loop
    FOR recDisputeCount in cDisputeCountList(recVitu.dispute_id) LOOP
      
      dbms_output.put_line('Create : ' || recDisputeCount.ticket_number_txt ||  ' count '||TO_CHAR(recDisputeCount.COUNT_NO));

      justin_rest.vt_dispute_post(ViolationTicketNo             =>  recDisputeCount.ticket_number_txt,
                                      DisputeFiledDate          =>  recDisputeCount.SUBMITTED_DT,
                                      ServiceDate               =>  recDisputeCount.ISSUED_DT,
                                      ClientTypeCode            =>  recDisputeCount.client_type_code,
                                      DriverSurname             =>  recDisputeCount.DISPUTANT_SURNAME_NM,
                                      DriverGiven1Name          =>  recDisputeCount.DISPUTANT_GIVEN_1_NM,
                                      DriverGiven2Name          =>  recDisputeCount.DISPUTANT_GIVEN_2_NM,
                                      DriverGiven3Name          =>  recDisputeCount.DISPUTANT_GIVEN_3_NM,
                                      OrganizationName          =>  recDisputeCount.organization_name,
                                      DriverLicenseNo           =>  recDisputeCount.DISPUTANT_DRV_LIC_NUMBER_TXT ,
                                      DriverJurisdictionCode    =>  recDisputeCount.driver_jurisdiction_code,
                                      DriverMvbClientNo         =>  recDisputeCount.driver_mvb_client_no,
                                      DriverGender              =>  recDisputeCount.driver_gender,
                                      DriverBirthdate           =>  recDisputeCount.disputant_birth_dt,
                                      AddressLine               =>  recDisputeCount.ADDRESS_LINE_1_TXT,
                                      AddressLine2              =>  recDisputeCount.address_line_2_txt,
                                      AddressLine3              =>  recDisputeCount.address_line_3_txt,
                                      AddressCity               =>  upper(recDisputeCount.ADDRESS_OTHER_CITY_TXT),
                                      AddressJurisdictionCode   =>  recDisputeCount.address_jurisdiction_code,
                                      AddressCountryCode        =>  recDisputeCount.address_country_code_txt,
                                      AddressPostalCode         =>  recDisputeCount.POSTAL_CODE_TXT,
                                      ViolationDate             =>  recDisputeCount.ISSUED_DT,
                                      ViolationCity             =>  recDisputeCount.ISSUED_AT_OR_NEAR_CITY_TXT,
                                      CountNumber               =>  recDisputeCount.COUNT_NO,
                                      IcbcAct                   =>  recDisputeCount.ACT_OR_REGULATION_NAME_CD,
                                      IcbcCountSection          =>  recDisputeCount.STAT_SECTION_TXT,
                                      CsbAct                    =>  recDisputeCount.ACT_OR_REGULATION_NAME_CD,
                                      CsbSection                =>  recDisputeCount.STAT_SECTION_TXT,
                                      CsbSubsection             =>  recDisputeCount.STAT_SUB_SECTION_TXT,
                                      CsbParagraph              =>  recDisputeCount.STAT_PARAGRAPH_TXT,
                                      CsbSubparagraph           =>  recDisputeCount.STAT_SUB_PARAGRAPH_TXT,
                                      CsbCountClause            =>  recDisputeCount.csb_count_clause,
                                      TicketedAmt               =>  recDisputeCount.TICKETED_AMT,
                                      DisputeType               =>  recDisputeCount.dispute_type,
                                      VehicleJuridiction        =>  recDisputeCount.vehicle_juridiction_txt,
                                      VehiclePlateNumber        =>  recDisputeCount.vehicle_plate_number,
                                      VehicleOwnerNm            =>  recDisputeCount.vehicle_owner_nm,
                                      VehicleType               =>  recDisputeCount.vehicle_type,
                                      VehicleColour             =>  recDisputeCount.vehicle_colour,
                                      NoticeOfDisputeAddr       =>  recDisputeCount.notice_of_dispute_addr_txt,
                                      HearingLocation           =>  recDisputeCount.hearing_location_txt,
                                      HearingLocationCode       =>  recDisputeCount.agen_agency_identifier_cd,
                                      EnforcementOfficerPin     =>  recDisputeCount.OFFICER_PIN_TXT,
                                      EnforcementOfficerName    =>  recDisputeCount.enforcement_officer_name_txt,
                                      EnforcementAgency         =>  recDisputeCount.DETACHMENT_LOCATION_TXT,
                                      EnforcementAgencyCode     =>  recDisputeCount.enforcement_agency_code_txt,
                                      AccidentFlag              =>  recDisputeCount.accident_flag_txt,
                                      WitnessOfficerPin         =>  recDisputeCount.witness_officer_pin_txt,
                                      WitnessOfficerName        =>  recDisputeCount.witness_officer_name_txt,
                                      CosFormNumber             =>  recDisputeCount.cos_form_number_txt,
                                      EvtFormNumber             =>  recDisputeCount.evt_form_number_txt,
                                      MreMinorVersion           =>  recDisputeCount.mre_minor_version_txt,
                                      TicketXml                 =>  recDisputeCount.ticket_xml_clob,
                                      ReturnCode                =>  vReturn  
                          );


    END LOOP;
  END LOOP;

  COMMIT;

END;
/

/* ------------------------------------------------------------------
Step #3 is to run the batch job to push the tickets to the VT Inbox
------------------------------------------------------------------*/

--begin
--  justin_vt_interface.processDisputeTickets;
--end;

--COMMIT;

