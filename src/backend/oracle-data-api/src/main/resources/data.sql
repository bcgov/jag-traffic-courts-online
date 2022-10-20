DELETE FROM DISPUTANT_CONTACT_INFORMATION;
DELETE FROM JJDISPUTED_COUNT;
DELETE FROM JJDISPUTE_REMARK;
DELETE FROM JJDISPUTE;

INSERT INTO JJDISPUTE (TICKET_NUMBER, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, COURTHOUSE_LOCATION, GIVEN_NAMES, SURNAME, OFFENCE_LOCATION, FINE_REDUCTION_REASON, TIME_TO_PAY_REASON, 
	ENFORCEMENT_OFFICER, JJ_ASSIGNED_TO, POLICE_DETACHMENT, VIOLATION_DATE, STATUS, SUBMITTED_DATE, ICBC_RECEIVED_DATE) VALUES
  ('AL89933084', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Pemberton Provincial Court', 'John', 'Doe', 'Victoria', 'I have lost my job and I am looking for another one', 'Need my salary to be deposited', 'Steven Allan', 'ldame@idir', 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP),
  ('ST04633443', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Vancouver Provincial Court', 'Jane', 'Doe', 'Burnaby', 'I have lost my job and I am looking for another one', 'Need my salary to be deposited', 'Alison Kerr', 'ldame@idir', 'Valemount', DATEADD('DAY',-1, CURRENT_DATE), 'NEW', DATEADD('DAY',-1, CURRENT_DATE), DATEADD('DAY',-1, CURRENT_DATE)),
  ('AJ20109246', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Dease Lake Provincial Court', 'Simon', 'Young', 'New Westminister', 'I have lost my job and I am looking for another one', 'Need my salary to be deposited', 'Adrian Peake', NULL, 'University', DATEADD('DAY',-2, CURRENT_DATE), 'NEW', DATEADD('DAY',-2, CURRENT_DATE), DATEADD('DAY',-2, CURRENT_DATE)),
  ('EA02216655', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Pemberton Provincial Court', 'Matt', 'Vaughan', 'Golden', 'I have lost my job and I am looking for another one', '', 'Steven Allan', 'ldame@idir', 'Whistler', DATEADD('DAY',-3, CURRENT_DATE), 'NEW', DATEADD('DAY',-3, CURRENT_DATE), DATEADD('DAY',-3, CURRENT_DATE)),
  ('SG00853764', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Abbotsford Provincial Court', 'Gavin', 'Glover', 'Brentwood Bay', 'I have lost my job and I am looking for another one', '', 'Harry Reid', 'ldame@idir', 'Ladysmith', DATEADD('DAY',-4, CURRENT_DATE), 'NEW', DATEADD('DAY',-4, CURRENT_DATE), DATEADD('DAY',-4, CURRENT_DATE)),
  ('AQ22185698', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Chilliwack Provincial Court', 'Gavin', 'Glover', 'Cache Creek', 'I have lost my job and I am looking for another one', '', 'Harry Reid', NULL, 'Ladysmith', DATEADD('DAY',-5, CURRENT_DATE), 'NEW', DATEADD('DAY',-5, CURRENT_DATE), DATEADD('DAY',-5, CURRENT_DATE));
  
 
INSERT INTO DISPUTANT_CONTACT_INFORMATION (ID, JJDISPUTE_ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, ADDRESS, BIRTHDATE, DRIVERS_LICENCE_NUMBER, EMAIL_ADDRESS, GIVEN_NAMES, PROVINCE, SURNAME) VALUES 
	(1, 'AL89933084', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'Lorraine.dame@nttdata.com', 'John', 'BC', 'Doe'),
	(2, 'ST04633443', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '202-1306 Stellys Cross Road', '1965-07-01', '876655', 'Lorraine.dame@nttdata.com', 'Jane', 'BC', 'Doe'),
	(3, 'AJ20109246', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '3838383', 'Lorraine.dame@nttdata.com', 'Simon', 'BC', 'Young'),
	(4, 'EA02216655', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '429 Raynor Avenue', '1965-07-01', '8765432', 'Lorraine.dame@nttdata.com', 'Matt', 'BC', 'Vaughan'),
	(5, 'SG00853764', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '425 Raynor Avenue', '1965-07-01', '8787873', 'Lorraine.dame@nttdata.com', 'Gavin', 'BC', 'Glover'),
	(6, 'AQ22185698', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '1236 Mackenzie Street', '1965-07-01', '9890833', 'Lorraine.dame@nttdata.com', 'Gavin', 'BC', 'Glover');
	
INSERT INTO JJDISPUTED_COUNT (ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, JJ_DISPUTE_TICKET_NUMBER, JJDISPUTE_ID, COUNT, APPEAR_IN_COURT, COMMENTS, DESCRIPTION, PLEA, REQUEST_REDUCTION, TICKETED_FINE_AMOUNT, LESSER_OR_GREATER_AMOUNT, INCLUDES_SURCHARGE, REQUEST_TIME_TO_PAY, DUE_DATE, REVISED_DUE_DATE) VALUES 
(1, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AL89933084', 'AL89933084', 1, 'N', '', '124(c)(1) Driving with burned out break lights', 0, 'Y', 5.00, 5.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(2, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AL89933084', 'AL89933084', 2, 'N', '', '67(b) Excessive speeding', 0, 'Y', 350.0, 350.0, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(3, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AL89933084', 'AL89933084', 3, 'N', '', '45(a) Driving without licence', 0, 'Y', 45.00, 45.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(4, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST04633443', 'ST04633443', 1, 'N', '', '124(c)(1) Driving with burned out break lights', 0, 'Y', 5.00, 5.00, 'Y', 'Y', DATEADD('DAY', 45, CURRENT_DATE), null),
(5, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST04633443', 'ST04633443', 2, 'N', '', '67(b) Excessive speeding', 0, 'Y', 350.0, 350.0, 'Y', 'Y', DATEADD('DAY', 45, CURRENT_DATE), null),
(6, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST04633443', 'ST04633443', 3, 'N', '', '45(a) Driving without licence', 0, 'Y', 45.00, 45.00, 'Y', 'Y', DATEADD('DAY', 45, CURRENT_DATE), null),
(7, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AJ20109246', 'AJ20109246', 1, 'N', '', '124(c)(1) Driving with burned out break lights', 0, 'Y', 5.00, 5.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(8, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AJ20109246', 'AJ20109246', 2, 'N', '', '67(b) Excessive speeding', 0, 'Y', 350.0, 350.0, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(9, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA02216655', 'EA02216655', 1, 'N', '', '45(a) Driving without licence', 0, 'Y', 45.00, 45.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(10, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'SG00853764', 'SG00853764', 1, 'N', '', '124(c)(1) Driving with burned out break lights', 0, 'Y', 5.00, 5.00, 'Y', 'Y', DATEADD('DAY', 45, CURRENT_DATE), null),
(11, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'SG00853764', 'SG00853764', 2, 'N', '', '67(b) Excessive speeding', 0, 'Y', 350.0, 350.00, 'Y', 'Y', DATEADD('DAY', 45, CURRENT_DATE), null),
(12, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'SG00853764', 'SG00853764', 3, 'N', '', '45(a) Driving without licence', 0, 'Y', 45.00, 45.00, 'Y', 'Y', DATEADD('DAY', 45, CURRENT_DATE), null),
(13, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AQ22185698', 'AQ22185698', 1, 'N', '', '124(c)(1) Driving with burned out break lights', 0, 'Y', 5.00, 5.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null),
(14, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AQ22185698', 'AQ22185698', 2, 'N', '', '67(b) Excessive speeding', 0, 'Y', 350.0, 350.00, 'Y', 'Y', DATEADD('DAY', 30, CURRENT_DATE), null);
