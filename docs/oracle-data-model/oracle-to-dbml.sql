DECLARE

  l_table_name varchar(4000);
  l_column_name varchar(4000);
  l_data_type varchar(4000);
  l_primary_key varchar(4000);
  l_data_length NUMBER;
  l_nullable VARCHAR(1);
  l_line VARCHAR(4000);
  CURSOR tables_cur
  IS
     SELECT table_name FROM user_tables order by table_name;

  CURSOR columns_cur(p_table_name IN VARCHAR2)
  IS
     SELECT lower(column_name) column_name, 
            lower(data_type) data_type,
            data_length,
            nullable
       FROM user_tab_columns
      WHERE table_name = p_table_name
       order by column_id;
       
  CURSOR table_pk_cur(p_table_name IN VARCHAR2)
  IS
     SELECT cols.column_name
       FROM all_constraints cons, all_cons_columns cols
      WHERE cols.table_name = p_table_name
        AND cons.constraint_type = 'P'
        AND cons.constraint_name = cols.constraint_name
        AND cons.owner = cols.owner
      ORDER BY cols.table_name, cols.position;  

BEGIN

  OPEN tables_cur;
   
  LOOP
     FETCH tables_cur INTO l_table_name;
     EXIT WHEN tables_cur%NOTFOUND;
     
     DBMS_OUTPUT.PUT_LINE('Table '||lower(l_table_name)||' {');
     
     -- columns
     OPEN columns_cur(l_table_name);
     LOOP
       FETCH columns_cur INTO l_column_name, l_data_type,l_data_length,l_nullable;
       EXIT WHEN columns_cur%NOTFOUND;
       l_line := '  '||l_column_name||' '||l_data_type;
       IF l_data_length IS NOT NULL AND l_data_length > 0 THEN
        l_line := l_line || '('||TO_CHAR(l_data_length)||')';
       END IF;
       IF l_nullable = 'N' THEN
         l_line := l_line || ' [not null]';
       END IF;
       DBMS_OUTPUT.PUT_LINE(l_line);       
     END LOOP;
     CLOSE columns_cur;
     
     -- primary key
     l_primary_key := '';
     OPEN table_pk_cur(l_table_name);
     LOOP
       FETCH table_pk_cur INTO l_column_name;
       EXIT WHEN table_pk_cur%NOTFOUND;
       IF l_primary_key IS NULL THEN
          l_primary_key := '('||lower(l_column_name);
       ELSE
          l_primary_key := l_primary_key|| ','||lower(l_column_name);
       END IF;
     END LOOP;
     CLOSE table_pk_cur;
     IF l_primary_key IS NOT NULL THEN
       l_primary_key := l_primary_key || ') [pk]';
       DBMS_OUTPUT.PUT_LINE('  Indexes {');
       DBMS_OUTPUT.PUT_LINE('    '||l_primary_key);
       DBMS_OUTPUT.PUT_LINE('  }');
     END IF;

     -- fk

     
     -- indexes
     DBMS_OUTPUT.PUT_LINE('}');
  END LOOP;
  CLOSE tables_cur;
  
  -- hard coded relationships for occam, some of the relationships could be reversed
  DBMS_OUTPUT.PUT_LINE('Ref: occam_dispute_counts.dispute_id > occam_disputes.dispute_id');
  DBMS_OUTPUT.PUT_LINE('Ref: occam_outgoing_emails.dispute_id > occam_disputes.dispute_id');
  DBMS_OUTPUT.PUT_LINE('Ref: occam_dispute_update_requests.dispute_id > occam_disputes.dispute_id');
  DBMS_OUTPUT.PUT_LINE('Ref: occam_audit_log_entries.dispute_id > occam_disputes.dispute_id');

  DBMS_OUTPUT.PUT_LINE('Ref: occam_audit_log_entry_types.audit_log_entry_type_cd > occam_audit_log_entries.audit_log_entry_type_cd');

  DBMS_OUTPUT.PUT_LINE('Ref: occam_dispute_status_types.dispute_status_type_cd > occam_disputes.dispute_status_type_cd');
  DBMS_OUTPUT.PUT_LINE('Ref: occam_dispute_update_req_types.dispute_update_req_type_cd > occam_dispute_update_requests.dispute_update_req_type_cd');


  DBMS_OUTPUT.PUT_LINE('Ref: occam_dispute_update_stat_typs.dispute_update_stat_type_cd > occam_dispute_update_requests.dispute_update_stat_type_cd');

  DBMS_OUTPUT.PUT_LINE('Ref: occam_violation_ticket_uploads.violation_ticket_upload_id - occam_disputes.violation_ticket_upload_id');

  DBMS_OUTPUT.PUT_LINE('Ref: occam_violation_ticket_counts.violation_ticket_upload_id > occam_violation_ticket_uploads.violation_ticket_upload_id');


END;



