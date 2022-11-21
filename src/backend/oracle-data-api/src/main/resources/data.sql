DELETE FROM DISPUTANT_CONTACT_INFORMATION;
DELETE FROM JJDISPUTED_COUNT;
DELETE FROM JJDISPUTE_REMARK;
DELETE FROM JJDISPUTE;

INSERT INTO JJDISPUTE (TICKET_NUMBER, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, COURTHOUSE_LOCATION, GIVEN_NAMES, SURNAME, OFFENCE_LOCATION, FINE_REDUCTION_REASON, TIME_TO_PAY_REASON, 
	ENFORCEMENT_OFFICER, JJ_ASSIGNED_TO, POLICE_DETACHMENT, VIOLATION_DATE, STATUS, SUBMITTED_TS, ICBC_RECEIVED_DATE, HEARING_TYPE, LAW_FIRM_NAME, LAWYER_SURNAME, LAWYER_GIVEN_NAME1, LAWYER_GIVEN_NAME2, 
	LAWYER_GIVEN_NAME3, INTERPRETER_LANGUAGE_CD, WITNESS_NO, DISPUTANT_ATTENDANCE_TYPE) VALUES
  ('EA07789631', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'John', 'Doe', 'Victoria', '', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA01115469', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA04788952', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA05888445', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('AO38375804', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('AU46117415', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('ST04605407', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA04591312', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Victoria Law Courts', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA06555313', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Victoria Law Courts', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA02314445', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Victoria Law Courts', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA55588777', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Victoria Law Courts', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA02000460', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Victoria Law Courts', 'John', 'Doe', 'Victoria', '', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA02216655', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Abbotsford Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('AE44379079', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Dease Lake Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('ST04635407', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Pemberton Provincial Court', 'John', 'Doe', 'Victoria', '', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA04567891', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Pemberton Provincial Court', 'John', 'Doe', 'Victoria', '', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA09998887', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Pemberton Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA01288745', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Pemberton Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', '', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA03665215', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Pemberton Provincial Court', 'John', 'Doe', 'Victoria', 'Just started a new job', 'Need my salary to be deposited', 'Steven Allan', null, 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP, 'WRITTEN_REASONS', '', '', '', '', '', '', 0, 'WRITTEN_REASONS'),
  ('EA00241528', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Robson Square Provincial Court', 'Francois Robert', 'Hodge', 'Victoria', '', '', 'Steven Allan', 'ldame', 'West Shore', LOCALTIMESTAMP, 'HEARING_SCHEDULED', LOCALTIMESTAMP, LOCALTIMESTAMP, 'COURT_APPEARANCE', 'Bartwell Law & Associates', 'BARTWELL', 'BART', '', '', 'Polish', 4, 'IN_PERSON');
  
 
INSERT INTO DISPUTANT_CONTACT_INFORMATION (ID, JJDISPUTE_ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, ADDRESS, BIRTHDATE, DRIVERS_LICENCE_NUMBER, EMAIL_ADDRESS, GIVEN_NAMES, PROVINCE, SURNAME) VALUES 
	(1, 'EA07789631', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(2, 'EA01115469', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(3, 'EA04788952', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(4, 'EA05888445', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(5, 'AO38375804', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(6, 'AU46117415', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(7, 'ST04605407', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(8, 'EA04591312', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(9, 'EA06555313', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(10, 'EA02314445', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(11, 'EA55588777', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(12, 'EA02000460', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(13, 'EA02216655', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(14, 'AE44379079', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(15, 'ST04635407', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(16, 'EA04567891', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(17, 'EA09998887', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(18, 'EA01288745', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(19, 'EA03665215', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'colm.ohiggins@nttdata.com', 'John', 'BC', 'Doe'),
	(20, 'EA00241528', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '1298 Mapleview Place', '1976-03-15', '6455895', 'colm.ohiggins@nttdata.com', 'Francois Robert', 'BC', 'Hodge');
	
INSERT INTO JJDISPUTED_COUNT (ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, JJ_DISPUTE_TICKET_NUMBER, JJDISPUTE_ID, COUNT, APPEAR_IN_COURT, COMMENTS, DESCRIPTION, PLEA, REQUEST_REDUCTION, TICKETED_FINE_AMOUNT, LESSER_OR_GREATER_AMOUNT, INCLUDES_SURCHARGE, REQUEST_TIME_TO_PAY, DUE_DATE, REVISED_DUE_DATE) VALUES 
(1, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA07789631', 'EA07789631', 1, 'N', '', '146(1) Speed in/outside municipality', 0, 'N', 196.00, 196.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(2, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA01115469', 'EA01115469', 1, 'N', '', '146(3) Speed against highway sign', 0, 'Y', 196.00, 196.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(3, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA04788952', 'EA04788952', 1, 'N', '', '146(3) Speed against highway sign', 0, 'Y', 196.00, 196.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(4, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA04788952', 'EA04788952', 2, 'N', '', '214.2(1) Using Electronic device while driving', 0, 'Y', 368.00, 368.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(5, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA04788952', 'EA04788952', 3, 'N', '', '214.2(2) Emailing or texting while driving', 0, 'Y', 368.00, 368.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(6, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA05888445', 'EA05888445', 1, 'N', '', '146(3) Speed against highway sign', 0, 'Y', 138.00, 138.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(7, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AO38375804', 'AO38375804', 1, 'N', '', '194(3) Ride motorcycle without required helmet', 0, 'Y', 138.00, 138.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(8, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AO38375804', 'AO38375804', 2, 'N', '', '148(1) Excessive speed', 0, 'Y', 483.00, 483.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(9, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AU46117415', 'AU46117415', 1, 'N', '', '147(1) Speed in School zone', 0, 'Y', 253.00, 253.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(10, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AU46117415', 'AU46117415', 2, 'N', '', '220(4) Fail to wear seatbelt', 0, 'Y', 167.00, 167.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(11, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST04605407', 'ST04605407', 1, 'N', '', '129(1) Red light at intersection', 0, 'Y', 167.00, 167.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(12, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA04591312', 'EA04591312', 2, 'N', '', '214.2(1) Using Electronic device while driving', 0, 'Y', 368.00, 368.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(13, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA04591312', 'EA04591312', 3, 'N', '', '214.2(2) Emailing or texting while driving', 0, 'Y', 368.00, 368.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(14, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA06555313', 'EA06555313', 1, 'N', '', '146(1) Speed in/outside Municipality', 0, 'Y', 196.00, 196.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(15, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA02314445', 'EA02314445', 2, 'N', '', '214.2(2) Emailing or texting while driving', 0, 'Y', 368.00, 368.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(16, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA55588777', 'EA55588777', 1, 'N', '', '220(2) Operate vehicle without seatbelts', 0, 'Y', 167.00, 167.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(17, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA02000460', 'EA02000460', 1, 'N', '', '220(4) Fail to wear seatbelt', 0, 'N', 167.00, 167.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(18, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA02000460', 'EA02000460', 2, 'N', '', '220(6) Permit passenger without seatbelt', 0, 'N', 167.00, 167.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(19, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA02216655', 'EA02216655', 1, 'N', '', '24(1) No drivers licence/wrong class', 0, 'Y', 276.00, 276.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(20, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AE44379079', 'AE44379079', 2, 'N', '', '146(3) Spped against highway sign', 0, 'Y', 138.00, 138.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(21, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST04635407', 'ST04635407', 1, 'N', '', '24(1) No drivers license/wrong class', 0, 'N', 276.00, 276.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(22, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST04635407', 'ST04635407', 2, 'N', '', '146(3) Spped against highway sign', 0, 'N', 368.00, 368.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(23, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST04635407', 'ST04635407', 3, 'N', '', '214.2(1) Using Electronic device while driving', 0, 'N', 368.00, 368.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(24, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA04567891', 'EA04567891', 1, 'N', '', '146(1) Speed in/outside Municipality', 0, 'N', 150.00, 150.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(25, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA09998887', 'EA09998887', 1, 'N', '', '146(3) Spped against highway sign', 0, 'Y', 196.00, 196.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(26, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA01288745', 'EA01288745', 1, 'N', '', '146(3) Spped against highway sign', 0, 'Y', 196.00, 196.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(27, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA03665215', 'EA03665215', 2, 'N', '', '146(3) Spped against highway sign', 0, 'Y', 368.00, 368.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(28, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA00241528', 'EA00241528', 1, 'Y', '', '92.1(1) MVA - Fail to stop resulting in pursuit', 1, 'N', 380.00, 380.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(29, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA00241528', 'EA00241528', 2, 'Y', '', '146(5) MVA 0 Speed against area sign', 1, 'Y', 196.00, 196.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null),
(30, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA00241528', 'EA00241528', 3, 'Y', '', '92.1(1) MVA - Fail to stop resulting in pursuit', 1, 'N', 380.00, 380.00, 'Y', 'N', DATEADD('DAY', 30, CURRENT_DATE), null);

INSERT INTO JJDISPUTE_COURT_APPEARANCE_ROP (ID, JJ_DISPUTE_TICKET_NUMBER, JJDISPUTE_ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, APPEARANCE_TS, ROOM, REASON, APP, NO_APP_TS, CLERK_RECORD, 
DEFENSE_COUNSEL, CROWN, JJ_SEIZED, ADJUDICATOR, COMMENTS, DURATION) VALUES 
(1, 'EA00241528', 'EA00241528', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, LOCALTIMESTAMP, '304', 'HR', 'N', LOCALTIMESTAMP, '', '', 'P', '', 'I.BLACKSTONE', '', 15);

INSERT INTO JJDISPUTED_COUNT_ROP (ID, JJDISPUTEDCOUNT_ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, FINDING, LESSER_DESCRIPTION, SS_PROBATION_DURATION, SS_PROBATION_CONDITIONS,
JAIL_DURATION, JAIL_INTERMITTENT, PROBATION_DURATION, PROBATION_CONDITIONS, DRIVING_PROHIBITION, DRIVING_PROHIBITIONMVASECTION, DISMISSED, FOR_WANT_OF_PROSECUTION, WITHDRAWN,
ABATEMENT, STAY_OF_PROCEEDINGS_BY, OTHER) VALUES
(1, 28, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, null, '', '', '', '', 'N', '', '', '', '', 'N', 'N', 'N', 'N', '', ''),
(2, 29, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, null, '', '', '', '', 'N', '', '', '', '', 'N', 'N', 'N', 'N', '', ''),
(3, 30, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, null, '', '', '', '', 'N', '', '', '', '', 'N', 'N', 'N', 'N', '', '');

