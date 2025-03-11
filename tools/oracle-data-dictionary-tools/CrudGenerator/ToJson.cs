using Oracle.DataDictionary;
using System.Text;

namespace CrudGenerator;

public class ToJson
{
    public static string RowToJsonObject(Table table, List<string> auditColumns)
    {
        var columns = table.Columns
            .Where(column => !auditColumns.Contains(column.Name))
            .OrderBy(_ => _.ColumnId)
            .ToList();

        var buffer = new StringBuilder();
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Convert a row from the {table.Name} table to a json_object_t.");
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"FUNCTION to_json_object(p_row {table.Name.ToLower()}%rowtype) RETURN json_object_t IS");
        buffer.AppendLine("    l_json json_object_t;");
        buffer.AppendLine("BEGIN");
        buffer.AppendLine("    l_json := json_object_t();");
        foreach (var column in columns)
        {
            buffer.AppendLine($"    l_json.put('{column.Name.ToLower()}', p_row.{column.Name.ToLower()});");
        }
        buffer.AppendLine("    RETURN l_json;");
        buffer.AppendLine("END;");

        return buffer.ToString();
    }

    public static string SelectToJsonObject(Table table)
    {
        List<TableColumn> columns = table.Columns
            .OrderBy(_ => _.ColumnId)
            .ToList();

        var buffer = new StringBuilder();
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"-- Convert a row from the {table.Name} table to a json_object_t.");
        buffer.AppendLine("-- --------------------------------------------------------------------------------");
        buffer.AppendLine($"FUNCTION select_to_json_object(p_row {table.Name.ToLower()}%rowtype) RETURN json_object_t IS");
        buffer.AppendLine("    l_json json_object_t;");
        buffer.AppendLine("BEGIN");

        buffer.AppendLine("    SELECT JSON_OBJECT(");

        foreach (var column in columns)
        {
            string suffix = ",";
            if (columns.IsLastColumn(column))
            {
                suffix = string.Empty;
            }

            buffer.Append($"               '{column.Name.ToLower()}' VALUE {column.Name.ToLower()}");
            buffer.AppendLine(suffix);
        }
        buffer.AppendLine("       )");
        buffer.AppendLine("    INTO l_json");
        buffer.AppendLine($"    FROM {table.Name.ToLower()}");
        buffer.Append("    WHERE ");

        Constraint pk = new Constraint(); // todo

        foreach (var column in pk.Columns)
        {
            if (!pk.Columns.IsFirstColumn(column))
            {
                buffer.Append("      AND ");
            }

            buffer.Append($"{column.ColumnName.ToLower()} = p_row.{column.ColumnName.ToLower()}");

            if (!pk.Columns.IsLastColumn(column))
            {
                buffer.AppendLine();
            }
            else
            {
                buffer.AppendLine(";");
            }
        }

        buffer.AppendLine("    RETURN l_json;");

        buffer.AppendLine("END;");

        return buffer.ToString();

    }


}
