DELETE FROM DISPUTANT_CONTACT_INFORMATION;
DELETE FROM JJDISPUTED_COUNT;
DELETE FROM JJDISPUTE;

INSERT INTO JJDISPUTE (TICKET_NUMBER, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, COURTHOUSE_LOCATION, GIVEN_NAMES, SURNAME, OFFENCE_LOCATION, FINE_REDUCTION_REASON, TIME_TO_PAY_REASON, 
	ENFORCEMENT_OFFICER, JJ_ASSIGNED_TO, JJ_GROUP_ASSIGNED_TO, POLICE_DETACHMENT, VIOLATION_DATE, STATUS, SUBMITTED_DATE, ICBC_RECEIVED_DATE) VALUES
  ('AL89933084', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Victoria', 'John', 'Doe', 'Victoria', 'I have lost my job and I am looking for another one', 'Need my salary to be deposited', 'Steven Allan', 'JJ1', 'JJGroup1', 'West Shore', LOCALTIMESTAMP, 'NEW', LOCALTIMESTAMP, LOCALTIMESTAMP),
  ('ST046334431', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Vancouver', 'Jane', 'Doe', 'Burnaby', 'I have lost my job and I am looking for another one', 'Need my salary to be deposited', 'Alison Kerr', 'JJ2', 'JJGroup2', 'Valemount', DATEADD('DAY',-1, CURRENT_DATE), 'NEW', DATEADD('DAY',-1, CURRENT_DATE), DATEADD('DAY',-1, CURRENT_DATE)),
  ('AJ201092461', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Vancouver', 'Simon', 'Young', 'New Westminister', 'I have lost my job and I am looking for another one', 'Need my salary to be deposited', 'Adrian Peake', NULL, 'JJGroup2', 'University', DATEADD('DAY',-2, CURRENT_DATE), 'IN_PROGRESS', DATEADD('DAY',-2, CURRENT_DATE), DATEADD('DAY',-2, CURRENT_DATE)),
  ('EA022166552', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Whistler', 'Matt', 'Vaughan', 'Golden', 'I have lost my job and I am looking for another one', '', 'Steven Allan', 'JJ3', 'JJGroup1', 'Whistler', DATEADD('DAY',-3, CURRENT_DATE), 'IN_PROGRESS', DATEADD('DAY',-3, CURRENT_DATE), DATEADD('DAY',-3, CURRENT_DATE)),
  ('SG008537641', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Squamish', 'Gavin', 'Glover', 'Brentwood Bay', 'I have lost my job and I am looking for another one', '', 'Harry Reid', 'JJ3', 'JJGroup3', 'Ladysmith', DATEADD('DAY',-4, CURRENT_DATE), 'REVIEW', DATEADD('DAY',-4, CURRENT_DATE), DATEADD('DAY',-4, CURRENT_DATE)),
  ('AQ221856981', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Squamish', 'Gavin', 'Glover', 'Cache Creek', 'I have lost my job and I am looking for another one', '', 'Harry Reid', NULL, NULL, 'Ladysmith', DATEADD('DAY',-5, CURRENT_DATE), 'COMPLETED', DATEADD('DAY',-5, CURRENT_DATE), DATEADD('DAY',-5, CURRENT_DATE));
  
 
INSERT INTO DISPUTANT_CONTACT_INFORMATION (ID, JJDISPUTE_ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, ADDRESS, BIRTHDATE, DRIVERS_LICENCE_NUMBER, EMAIL_ADDRESS, GIVEN_NAMES, PROVINCE, SURNAME) VALUES 
	(1, 'AL89933084', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '1234567', 'Lorraine.dame@nttdata.com', 'John', 'BC', 'Doe'),
	(2, 'ST046334431', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '202-1306 Stellys Cross Road', '1965-07-01', '876655', 'Lorraine.dame@nttdata.com', 'Jane', 'BC', 'Doe'),
	(3, 'AJ201092461', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '3-1409 Camosun Street', '1965-07-01', '3838383', 'Lorraine.dame@nttdata.com', 'Simon', 'BC', 'Young'),
	(4, 'EA022166552', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '429 Raynor Avenue', '1965-07-01', '8765432', 'Lorraine.dame@nttdata.com', 'Matt', 'BC', 'Vaughan'),
	(5, 'SG008537641', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '425 Raynor Avenue', '1965-07-01', '8787873', 'Lorraine.dame@nttdata.com', 'Gavin', 'BC', 'Glover'),
	(6, 'AQ221856981', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP,  '1236 Mackenzie Street', '1965-07-01', '9890833', 'Lorraine.dame@nttdata.com', 'Gavin', 'BC', 'Glover');
	
INSERT INTO JJDISPUTED_COUNT (ID, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, JJ_DISPUTE_TICKET_NUMBER, JJDISPUTE_ID, COUNT, APPEAR_IN_COURT, COMMENTS, DESCRIPTION, PLEA, REQUEST_REDUCTION, TICKETED_FINE_AMOUNT, LESSER_OR_GREATER_AMOUNT, INCLUDES_SURCHARGE, REQUEST_TIME_TO_PAY, DUE_DATE, REVISED_DUE_DATE) VALUES 
(1, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AL89933084', 'AL89933084', 1, FALSE, '', '124(c)(1) Driving with burned out break lights', 0, TRUE, 5.00, 5.00, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null),
(2, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AL89933084', 'AL89933084', 2, FALSE, '', '67(b) Excessive speeding', 0, TRUE, 350.0, 345.0, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null),
(3, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AL89933084', 'AL89933084', 3, FALSE, '', '45(a) Driving without licence', 0, TRUE, 45.00, 40.00, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null),
(4, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST046334431', 'ST046334431', 1, FALSE, '', '124(c)(1) Driving with burned out break lights', 0, TRUE, 5.00, 10.00, TRUE, TRUE, DATEADD('DAY', 45, CURRENT_DATE), null),
(5, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST046334431', 'ST046334431', 2, FALSE, '', '67(b) Excessive speeding', 0, TRUE, 350.0, 300.00, TRUE, TRUE, DATEADD('DAY', 45, CURRENT_DATE), null),
(6, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'ST046334431', 'ST046334431', 3, FALSE, '', '45(a) Driving without licence', 0, TRUE, 45.00, 47.00, TRUE, TRUE, DATEADD('DAY', 45, CURRENT_DATE), null),
(7, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AJ201092461', 'AJ201092461', 1, FALSE, '', '124(c)(1) Driving with burned out break lights', 0, TRUE, 5.00, 3.00, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null),
(8, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AJ201092461', 'AJ201092461', 2, FALSE, '', '67(b) Excessive speeding', 0, TRUE, 350.0, 300.00, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null),
(9, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'EA022166552', 'EA022166552', 1, FALSE, '', '45(a) Driving without licence', 0, TRUE, 45.00, 40.00, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null),
(10, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'SG008537641', 'SG008537641', 1, FALSE, '', '124(c)(1) Driving with burned out break lights', 0, TRUE, 5.00, 5.00, TRUE, TRUE, DATEADD('DAY', 45, CURRENT_DATE), null),
(11, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'SG008537641', 'SG008537641', 2, FALSE, '', '67(b) Excessive speeding', 0, TRUE, 350.0, 200.00, TRUE, TRUE, DATEADD('DAY', 45, CURRENT_DATE), null),
(12, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'SG008537641', 'SG008537641', 3, FALSE, '', '45(a) Driving without licence', 0, TRUE, 45.00, 20.00, TRUE, TRUE, DATEADD('DAY', 45, CURRENT_DATE), null),
(13, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AQ221856981', 'AQ221856981', 1, FALSE, '', '124(c)(1) Driving with burned out break lights', 0, TRUE, 5.00, 20.00, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null),
(14, 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'AQ221856981', 'AQ221856981', 2, FALSE, '', '67(b) Excessive speeding', 0, TRUE, 350.0, 475.00, TRUE, TRUE, DATEADD('DAY', 30, CURRENT_DATE), null);
