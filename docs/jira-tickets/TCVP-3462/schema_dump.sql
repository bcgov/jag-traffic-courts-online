SELECT owner,
			 table_name,
			 column_name,
			 data_type,
			 data_length,
			 data_precision,
			 data_scale,
			 nullable
FROM   all_tab_columns
WHERE  owner IN ('TCO', 'OCCAM', 'JUSTIN')
AND    data_type IN ('DATE', 'TIMESTAMP', 'TIMESTAMP WITH TIME ZONE', 'TIMESTAMP WITH LOCAL TIME ZONE')
ORDER BY owner, table_name, column_id;