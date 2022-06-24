DELETE FROM JJDISPUTE;

INSERT INTO JJDISPUTE (TICKET_NUMBER, CREATED_BY, CREATED_TS, MODIFIED_BY, MODIFIED_TS, COURTHOUSE_LOCATION, 
	DISPUTANT_NAME, ENFORCEMENT_OFFICER, JJ_ASSIGNED_TO, JJ_GROUP_ASSIGNED_TO, POLICE_DETACHMENT, VIOLATION_DATE) VALUES
  ('1000001', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Victoria', 'John Doe', 'Steven Allan', 'JJ1', 'JJGroup1', 'West Shore', LOCALTIMESTAMP),
  ('1000002', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Vancouver', 'Jane Doe', 'Alison Kerr', 'JJ2', 'JJGroup2', 'Valemount', DATEADD('DAY',-1, CURRENT_DATE)),
  ('1000003', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Vancouver', 'Simon Young', 'Adrian Peake', NULL, 'JJGroup2', 'University', DATEADD('DAY',-2, CURRENT_DATE)),
  ('1000004', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Whistler', 'Matt Vaughan', 'Steven Allan', 'JJ3', 'JJGroup1', 'Whistler', DATEADD('DAY',-3, CURRENT_DATE)),
  ('1000005', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Squamish', 'Gavin Glover', 'Harry Reid', 'JJ3', 'JJGroup3', 'Ladysmith', DATEADD('DAY',-4, CURRENT_DATE)),
  ('1000006', 'System', LOCALTIMESTAMP, 'System', LOCALTIMESTAMP, 'Squamish', 'Gavin Glover', 'Harry Reid', NULL, NULL, 'Ladysmith', DATEADD('DAY',-5, CURRENT_DATE));