using Oracle.DataDictionary;
using System.Globalization;
using System.Text;

namespace CrudGenerator;

public class Model
{
    public static string Create(Table table, SourceTable sourceTable)
    {
        Constraint primaryKey = table.Constraints.Single(_ => _.Type == ConstraintType.PrimaryKey);

        List<TableColumn> columns = table.Columns
            .OrderBy(_ => _.ColumnId)
            .ToList();

        var buffer = new StringBuilder();
        buffer.AppendLine("/// <summary>");
        buffer.AppendLine($"/// Represents a row in the {table.Name.ToLower()} table.");
        buffer.AppendLine("/// </summary>");
        buffer.AppendLine($"public partial class {ToCSharpPropertyName(table.Name)} : DatabaseEntity");
        buffer.AppendLine("{");
        buffer.AppendLine("    [System.Text.Json.Serialization.JsonIgnore]");
        buffer.AppendLine($"    public override string Name => \"{table.Name.ToLower()}\";");
        buffer.AppendLine();

        foreach (var column in columns)
        {
            var type = GetCSharpType(column);
            var name = column.Name.ToLower();

            bool isPrimaryKey = primaryKey.Columns.Any(_ => _.ColumnName == column.Name);
            string extraSetParameters = isPrimaryKey ? ", alwaysDirty: true" : "";

            if (isPrimaryKey)
            {
                buffer.AppendLine($"    /// <summary>");
                buffer.AppendLine($"    /// Primary Key");
                buffer.AppendLine($"    /// </summary>");
            }

            buffer.AppendLine($"    [System.Text.Json.Serialization.JsonPropertyName(\"{name}\")]");
            buffer.AppendLine($"    public {type} {ToCSharpPropertyName(column.Name)}");
            buffer.AppendLine("    {");

            if (type == "DateTime?")
            {
                ColumnProperty property = sourceTable.Properties?.FirstOrDefault(_ => _.Name == column.Name) ?? new ColumnProperty();

                // default is Unspecified
                string extraGetArguments = property.DateTimeKind == DateTimeKind.Unspecified
                    ? $"format: \"{property.Format}\""
                    : $"format: \"{property.Format}\", kind: DateTimeKind.{property.DateTimeKind}";

                buffer.AppendLine($"        get {{ return BackingStore.GetDateTime(\"{name}\", {extraGetArguments}); }}");
                buffer.AppendLine($"        set {{ BackingStore.SetDateTime(\"{name}\", value, format: \"{property.Format}\"); }}");
            }
            else
            {
                buffer.AppendLine($"        get {{ return BackingStore.Get<{GetCSharpType(column)}>(\"{name}\"); }}");
                buffer.AppendLine($"        set {{ BackingStore.Set<{GetCSharpType(column)}>(\"{name}\", value{extraSetParameters}); }}");
            }

            buffer.AppendLine("    }");

            if (!columns.IsLastColumn(column))
            {
                buffer.AppendLine();
            }

        }
        buffer.AppendLine("}");
        return buffer.ToString();
    }

    private static string ToCSharpPropertyName(string name)
    {
        // Split the column name by underscores
        var words = name.ToLower().Split('_');

        // Capitalize the first letter of each word and concatenate them
        var result = new StringBuilder();
        foreach (var word in words)
        {
            if (word.Length > 0)
            {
                result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word));
            }
        }

        return result.ToString();
    }

    private static string GetCSharpType(TableColumn column)
    {
        string GetNumberType(TableColumn column)
        {
            var scale = column.DataScale ?? 0;
            if (scale == 0)
            {
                return column.DataPrecision switch
                {
                    <= 4 => "short?",   // 4 digits or less, values greater than 32,767 will not fit
                    <= 9 => "int?",     // 9 digits or less, values greater than 2,147,483,647 will not fit
                    _ => "long?"
                };
            }
            return "decimal?";
        }

        return column.DataType switch
        {
            "NUMBER" => GetNumberType(column),
            "DATE" => "DateTime?",
            "TIMESTAMP" => "DateTime?",
            _ => "string?"
        };
    }
}