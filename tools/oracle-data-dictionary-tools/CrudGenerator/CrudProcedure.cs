using Microsoft.EntityFrameworkCore;
using Oracle.DataDictionary;
using System.Text;

namespace CrudGenerator;

public class CrudProcedure
{
    private readonly OracleDataDictionaryDbContext _database;

    public CrudProcedure(OracleDataDictionaryDbContext database)
    {
        _database = database;
    }

    public string Select(SourceTable sourceTable, List<string> auditColumns, bool specification = false)
    {
        Table table = _database.Tables
            .Where(table => table.Name == sourceTable.Name)
            .Include(table => table.Columns)
            .Single();

        List<TableColumn> columns = table.Columns
            .Where(_ => !auditColumns.Contains(_.Name))
            .OrderBy(_ => _.ColumnId)
            .ToList();

        string direction = !string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName)
            ? "in out"
            : "in";

        // assumption - single column primary key
        string primaryKeyColumnName = _database.PrimaryKeyFor(table)
            .SelectMany(constraint => constraint.Columns)
            .Single()
            .ColumnName;

        TableColumn primaryKeyColumn = table.Columns.Single(_ => _.Name == primaryKeyColumnName);

        var buffer = new StringBuilder();

        buffer.AppendLine($"-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Select a row from {table.Name} table using its rowtype.");
        buffer.AppendLine($"-- --------------------------------------------------------------------------------");

        buffer.Append($"procedure select_row(p_row in out {table.Name.ToLower()}%rowtype)");

        if (specification)
        {
            buffer.AppendLine(";");
            return buffer.ToString(); //  select_row(p_json json_object_t overload is private
        }
        else
        {
            buffer.AppendLine();
            buffer.AppendLine("is");
            buffer.AppendLine("begin");
            buffer.AppendLine($"  if p_row.{primaryKeyColumn.Name.ToLower()} is null then");
            buffer.AppendLine($"    raise_application_error(-20400, 'Primary key column {primaryKeyColumn.Name.ToLower()} was not supplied. Cannot select {table.Name.ToLower()} row');"); ;
            buffer.AppendLine($"  end if;");

            buffer.AppendLine();
            buffer.AppendLine($"  SELECT *");
            buffer.AppendLine($"    INTO p_row");
            buffer.AppendLine($"    FROM {table.Name.ToLower()}");
            buffer.AppendLine($"   WHERE {primaryKeyColumn.Name.ToLower()} = p_row.{primaryKeyColumn.Name.ToLower()};");

            buffer.AppendLine();
            buffer.AppendLine("end select_row;");
        }

        buffer.AppendLine();
        buffer.AppendLine($"-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Select a row from {table.Name} table using its rowtype.");
        buffer.AppendLine($"-- The primary key {primaryKeyColumnName.ToLower()}");
        buffer.AppendLine($"-- will be extracted from the json object.");
        buffer.AppendLine($"-- This is an private procedure.");
        buffer.AppendLine($"-- --------------------------------------------------------------------------------");

        buffer.AppendLine("procedure select_row(p_json json_object_t,");
        buffer.Append($"                     p_row in out {table.Name.ToLower()}%rowtype)");

        buffer.AppendLine();
        buffer.AppendLine("is");
        buffer.AppendLine("begin");

        ReadColumnFromJson(buffer, primaryKeyColumn, auditColumns, "p_row", "p_json");

        buffer.AppendLine();
        buffer.AppendLine($"  if p_row.{primaryKeyColumn.Name.ToLower()} is null then");
        buffer.AppendLine($"    raise_application_error(-20400, 'Primary key column {primaryKeyColumn.Name.ToLower()} was not supplied in the json object.');"); ;
        buffer.AppendLine($"  end if;");

        buffer.AppendLine();
        buffer.AppendLine($"  select_row(p_row);");
        buffer.AppendLine("end;");

        return buffer.ToString();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="sequenceName"></param>
    /// <param name="skipColumns">The columns that are skipped on insert.</param>
    /// <returns></returns>
    public string Insert(SourceTableCollection sourceTables, SourceTable sourceTable, List<string> auditColumns, bool specification = false)
    {
        Table table = _database.Tables
            .Where(table => table.Name == sourceTable.Name)
            .Include(table => table.Columns)
            .Single();

        // we do not insert the audit columns or the update user id
        List<TableColumn> columns = table.Columns
            .Where(column => !auditColumns.Contains(column.Name) && column.Name != "UPD_USER_ID")
            .OrderBy(column => column.ColumnId)
            .ToList();

        string direction = !string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName)
            ? "in out"
            : "in";

        // assumption - single column primary key
        string primaryKeyColumnName = _database.PrimaryKeyFor(table)
            .SelectMany(constraint => constraint.Columns)
            .Single()
            .ColumnName;

        TableColumn primaryKeyColumn = table.Columns.Single(_ => _.Name == primaryKeyColumnName);

        var buffer = new StringBuilder();

        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Insert a row into the {table.Name} table using its rowtype.");
        buffer.AppendLine("-- --------------------------------------------------------------------------------");

        buffer.Append($"procedure insert_row(p_row {direction} {table.Name.ToLower()}%rowtype)");

        if (specification)
        {
            buffer.AppendLine(";");
            return buffer.ToString();
        }

        buffer.AppendLine();
        buffer.AppendLine("is");
        buffer.AppendLine("begin");

        if (!string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName))
        {
            buffer.AppendLine($"  -- Primary key column {primaryKeyColumn.Name.ToLower()} value");
            buffer.AppendLine($"  -- comes from the sequence {sourceTable.PrimaryKeySequenceName} and");
            buffer.AppendLine($"  -- is generated by a database trigger");
        }
        buffer.AppendLine($"  INSERT INTO {table.Name.ToLower()}");
        buffer.AppendLine("  (");

        for (var i = 0; i < columns.Count; i++)
        {
            if (columns[i].Name != primaryKeyColumn.Name || string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName))
            {
                buffer.Append($"    {columns[i].Name.ToLower()}");
                buffer.AppendLine(i < columns.Count - 1 ? "," : string.Empty);
            }
        }
        buffer.AppendLine("  ) VALUES (");

        if (!string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName))
        {
            // ky value is generated by the sequence in trigger
            //buffer.AppendLine($"        {primaryKeyStrategy.Sequence}.nextval,");
        }
        else
        {
            buffer.Append($"  p_row.{primaryKeyColumn.Name.ToLower()},");
        }

        for (var i = 0; i < columns.Count; i++)
        {
            if (primaryKeyColumn.Name != columns[i].Name)
            {
                buffer.Append($"    p_row.{columns[i].Name.ToLower()}");
                buffer.AppendLine(i < columns.Count - 1 ? "," : string.Empty);
            }
        }

        IList<string> GetReturningColumns()
        {
            List<string> columns = new List<string>();

            if (!string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName))
            {
                columns.Add(primaryKeyColumn.Name);
            }

            // for each fk that is generated by a sequence
            foreach (var foreignKey in _database.ForeignKeysFor(table).Include(fk => fk.Columns).OrderBy(fk => fk.Name))
            {
                // get the constraint name (ie the PK, of the other table)
                var pkConstraint = _database.Constraints
                    .Where(constraint => constraint.Owner == foreignKey.Owner && constraint.Name == foreignKey.ReferencedConstraintName)
                    .Include(constraint => constraint.Columns)
                    .SingleOrDefault();

                if (pkConstraint is null)
                {
                    continue;
                }

                var otherSourceTable = sourceTables.Tables.Where(table => table.Name == pkConstraint.TableName).SingleOrDefault();
                if (!string.IsNullOrEmpty(otherSourceTable?.PrimaryKeySequenceName))
                {
                    string columnName = foreignKey.Columns.Single().ColumnName.ToLower();
                    columns.Add(columnName);
                }
            }



            return columns;
        }

        var returningColumns = GetReturningColumns();

        if (returningColumns.Count != 0)
        {
            buffer.AppendLine("  )");
            buffer.Append("  returning ");
            foreach (var column in returningColumns)
            {
                buffer.Append($"{column.ToLower()}");
                buffer.Append(column != returningColumns.Last() ? ", " : string.Empty);
            }
            buffer.AppendLine();
            buffer.Append("       into ");
            foreach (var column in returningColumns)
            {
                buffer.Append($"p_row.{column.ToLower()}");
                buffer.Append(column != returningColumns.Last() ? ", " : string.Empty);
            }

            buffer.AppendLine(";");
        }
        else
        {
            buffer.AppendLine("    );");

        }

        buffer.AppendLine();
        buffer.AppendLine("end insert_row;");
        return buffer.ToString();
    }

    public string JsonToRowType(SourceTable sourceTable, List<string> auditColumns)
    {
        Table table = _database.Tables
            .Where(table => table.Name == sourceTable.Name)
            .Include(table => table.Columns)
            .Single();

        List<TableColumn> columns = table.Columns
            .Where(_ => !auditColumns.Contains(_.Name))
            .OrderBy(_ => _.ColumnId)
            .ToList();

        string direction = !string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName)
            ? "in out"
            : "in";

        // assumption - single column primary key
        string primaryKeyColumnName = _database.PrimaryKeyFor(table)
            .SelectMany(constraint => constraint.Columns)
            .Single()
            .ColumnName;

        TableColumn primaryKeyColumn = table.Columns.Single(_ => _.Name == primaryKeyColumnName);

        string tableName = table.Name;

        StringBuilder buffer = new StringBuilder();

        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Extract the fields from a json_object_t into record of type {table.Name}");
        buffer.AppendLine("-- Only the fields that exist in the JSON object will be written to the record.");
        buffer.AppendLine("-- Fields not in the JSON object will not be modified.");
        buffer.AppendLine("-- This is an private procedure.");
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine("procedure json_object_to_rowtype(p_json json_object_t,");
        buffer.Append($"                                 p_row in out {tableName.ToLower()}%rowtype)");

        buffer.AppendLine();
        buffer.AppendLine("is");
        buffer.AppendLine("begin");

        ReadColumnsFromJson(buffer, table, auditColumns, "p_row", "p_json");

        buffer.AppendLine();
        buffer.AppendLine("end json_object_to_rowtype;");

        return buffer.ToString();
    }

    public string Update(SourceTable sourceTable, List<string> auditColumns, bool specification = false)
    {
        Table table = _database.Tables
            .Where(table => table.Name == sourceTable.Name)
            .Include(table => table.Columns)
            .Single();

        // we dont update the ENT_USER_ID column or PK
        List<TableColumn> columns = table.Columns
            .Where(column => !auditColumns.Contains(column.Name) && column.Name != "ENT_USER_ID")
            .OrderBy(column => column.ColumnId)
            .ToList();

        string direction = !string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName)
            ? "in out"
            : "in";

        // assumption - single column primary key
        string primaryKeyColumnName = _database.PrimaryKeyFor(table)
            .SelectMany(constraint => constraint.Columns)
            .Single()
            .ColumnName;

        TableColumn primaryKeyColumn = table.Columns.Single(_ => _.Name == primaryKeyColumnName);

        var buffer = new StringBuilder();
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Update a row in the {table.Name} table using its rowtype.");
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.Append($"procedure update_row(p_row in {table.Name.ToLower()}%rowtype)");

        if (specification)
        {
            buffer.AppendLine(";");
            return buffer.ToString();
        }

        buffer.AppendLine();
        buffer.AppendLine("is");
        buffer.AppendLine("begin");
        buffer.AppendLine($"  if p_row.{primaryKeyColumn.Name.ToLower()} is null then");
        buffer.AppendLine($"    raise_application_error(-20400,'{primaryKeyColumn.Name.ToLower()} is null. Cannot update {table.Name.ToLower()} row.');");
        buffer.AppendLine($"  end if;");
        buffer.AppendLine();
        buffer.AppendLine($"  UPDATE {table.Name.ToLower()}");
        buffer.Append("     SET");
        bool first = true;

        var maxLength = columns
            .Where(column => column.Name != primaryKeyColumn.Name && column.Name != "ENT_USER_ID")
            .Max(column => column.Name.Length);

        for (var i = 0; i < columns.Count; i++)
        {
            if (primaryKeyColumn.Name != columns[i].Name)
            {
                string padding = " ";
                if (first)
                {
                    first = false;
                }
                else
                {
                    padding = "         ";
                }

                buffer.Append($"{padding}{columns[i].Name.ToLower()} ");
                if (columns[i].Name.Length < maxLength)
                {
                    buffer.Append(new string(' ', maxLength - columns[i].Name.Length));
                }
                buffer.Append($"= p_row.{columns[i].Name.ToLower()}");
                buffer.AppendLine(i < columns.Count - 1 ? "," : string.Empty);
            }
        }

        buffer.AppendLine($"   WHERE {primaryKeyColumn.Name.ToLower()} = p_row.{primaryKeyColumn.Name.ToLower()};");
        buffer.AppendLine();
        buffer.AppendLine("end update_row;");
        return buffer.ToString();
    }

    public string Delete(SourceTable sourceTable, List<string> auditColumns, bool specification = false)
    {
        Table table = _database.Tables
            .Where(table => table.Name == sourceTable.Name)
            .Include(table => table.Columns)
            .Single();

        List<TableColumn> columns = table.Columns
            .Where(_ => !auditColumns.Contains(_.Name))
            .OrderBy(_ => _.ColumnId)
            .ToList();

        string direction = !string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName)
            ? "in out"
            : "in";

        // assumption - single column primary key
        string primaryKeyColumnName = _database.PrimaryKeyFor(table)
            .SelectMany(constraint => constraint.Columns)
            .Single()
            .ColumnName;

        TableColumn primaryKeyColumn = table.Columns.Single(_ => _.Name == primaryKeyColumnName);

        var buffer = new StringBuilder();
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Delete a row from the {table.Name} table using its rowtype.");
        buffer.AppendLine($"-- Only the primary key columns are used in the WHERE clause.");
        buffer.AppendLine("-- --------------------------------------------------------------------------------");

        buffer.Append($"procedure delete_row(p_row in {table.Name.ToLower()}%rowtype)");

        if (specification)
        {
            buffer.AppendLine(";");
            return buffer.ToString();
        }

        buffer.AppendLine();
        buffer.AppendLine("is");
        buffer.AppendLine("begin");
        buffer.AppendLine($"  if p_row.{primaryKeyColumn.Name.ToLower()} is null then");
        buffer.AppendLine($"    raise_application_error(-20400,'{primaryKeyColumn.Name.ToLower()} is null. Cannot delete {table.Name.ToLower()} row.');");
        buffer.AppendLine($"  end if;");
        buffer.AppendLine();
        buffer.AppendLine($"  DELETE");
        buffer.AppendLine($"    FROM {table.Name.ToLower()}");
        buffer.AppendLine($"   WHERE {primaryKeyColumn.Name.ToLower()} = p_row.{primaryKeyColumn.Name.ToLower()};");
        buffer.AppendLine();
        buffer.AppendLine("end delete_row;");
        return buffer.ToString();
    }

    /// <summary>
    /// Generates the execute_operation procedure.
    /// </summary>
    /// <param name="sourceTable"></param>
    /// <param name="auditColumns"></param>
    /// <returns></returns>
    public string ExecuteOperation(SourceTable sourceTable, List<string> auditColumns, bool specification = false)
    {
        Table table = _database.Tables
            .Where(table => table.Name == sourceTable.Name)
            .Include(table => table.Columns)
            .Single();

        List<TableColumn> columns = table.Columns
            .Where(_ => !auditColumns.Contains(_.Name))
            .OrderBy(_ => _.ColumnId)
            .ToList();

        string direction = !string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName)
            ? "in out"
            : "in";

        // assumption - single column primary key
        string primaryKeyColumnName = _database.PrimaryKeyFor(table)
            .SelectMany(constraint => constraint.Columns)
            .Single()
            .ColumnName;

        TableColumn primaryKeyColumn = table.Columns.Single(_ => _.Name == primaryKeyColumnName);

        string schema = table.Owner.ToLower();

        var buffer = new StringBuilder();
        buffer.AppendLine($"-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Executes the operation to a row on the {table.Name} table.");
        buffer.AppendLine($"-- This is an private procedure.");
        buffer.AppendLine($"-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"procedure execute_operation(p_operation in     varchar2,");
        buffer.AppendLine($"                            p_data      in     json_object_t,");
        buffer.Append($"                            p_row       in out {table.Name.ToLower()}%rowtype)");

        if (specification)
        {
            buffer.AppendLine(";");
            return buffer.ToString();
        }
        else
        {
            buffer.AppendLine();
        }
        
        buffer.AppendLine("is");
        buffer.AppendLine("begin");

        if (sourceTable.CreateOperation(CrudOperation.Update))
        {
            buffer.AppendLine("     -- Updating, need to select the row first before loading the data");
            buffer.AppendLine("     if p_operation = 'update' then");
            buffer.AppendLine("         select_row(p_data, p_row);");
            buffer.AppendLine("     end if;");
            buffer.AppendLine();
        }
        buffer.AppendLine("     -- load the data from json");
        buffer.AppendLine($"     json_object_to_rowtype(p_data, p_row);");
        buffer.AppendLine();
        buffer.AppendLine("     if p_operation = 'insert' then");
        buffer.AppendLine($"         insert_row(p_row);");
        if (sourceTable.CreateOperation(CrudOperation.Update))
        {
            buffer.AppendLine("     elsif p_operation = 'update' then");
            buffer.AppendLine($"         update_row(p_row);");
        }
        if (sourceTable.CreateOperation(CrudOperation.Delete))
        {
            buffer.AppendLine("     elsif p_operation = 'delete' then");
            buffer.AppendLine($"         delete_row(p_row);");
        }
        buffer.AppendLine("     else");
        buffer.AppendLine($"         raise_application_error(-20400, 'Invalid operation: ''' || p_operation || ''' on table {table.Name.ToLower()}');");
        buffer.AppendLine("     end if;");
        buffer.AppendLine();
        buffer.AppendLine("end execute_operation;");
        return buffer.ToString();
    }

    public string CreateRestProc(SourceTable sourceTable)
    {
        Table table = _database.Tables
            .Where(table => table.Name == sourceTable.Name)
            .Single();

        var buffer = new StringBuilder();
        buffer.AppendLine($"        else if l_table = '{table.Name.ToLower()}' then");
        buffer.AppendLine($"            execute_operation(l_operation, l_data, l_{table.Name.ToLower()}_row);");
        return buffer.ToString();
    }

    private void ReadColumnsFromJson(StringBuilder buffer, Table table, List<string> auditColumns, string recordVariableName, string jsonVariableName)
    {
        var columns = table.Columns
            .OrderBy(column => column.ColumnId)
            .ToList();

        foreach (var column in columns)
        {
            ReadColumnFromJson(buffer, column, auditColumns, recordVariableName, jsonVariableName);
        }
    }

    private void ReadColumnFromJson(StringBuilder buffer, TableColumn column, List<string> auditColumns, string recordVariableName, string jsonVariableName)
    {
        if (!auditColumns.Contains(column.Name))
        {
            var name = column.Name.ToLower();
            string function = "get_string";
            if (column.DataType == "NUMBER")
            {
                function = "get_number";
            }
            else if (column.DataType == "DATE")
            {
                function = "get_date";
            }

            buffer.AppendLine($"    if {jsonVariableName}.has('{name}') then");
            buffer.AppendLine($"        {recordVariableName}.{name} := {jsonVariableName}.{function}('{name}');");
            buffer.AppendLine($"    end if;");
        }
    }
}
