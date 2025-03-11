using CommandLine;
using CommandLine.Text;
using CrudGenerator;
using Microsoft.EntityFrameworkCore;
using Oracle.DataDictionary;
using Oracle.ManagedDataAccess.Client;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static OracleDataDictionaryDbContext CreateDbContext(Options options)
    {
        OracleConfiguration.OracleDataSources.Add(options.Sid, $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={options.Host})(PORT=1521))(CONNECT_DATA=(SID={options.Sid})(SERVER=dedicated)))");

        var optionsBuilder = new DbContextOptionsBuilder<OracleDataDictionaryDbContext>();
        optionsBuilder.UseOracle($"Data Source={options.Sid};User ID={options.Username};Password={options.Password};");

        var dbContext = new OracleDataDictionaryDbContext(optionsBuilder.Options);
        return dbContext;
    }

    private static void Main(string[] args)
    {
        var parserResult = Parser.Default.ParseArguments<Options>(args);

        parserResult
            .WithParsed(Run)
            .WithNotParsed(errs => DisplayHelp(parserResult));
    }

    private static void Run(Options options)
    {
        if (!File.Exists(options.File))
        {
            Console.Error.WriteLine($"Configuration file {options.File} does not exist.");
            return;
        }

        if (!Directory.Exists(options.OutputPath))
        {
            Console.Error.WriteLine($"Output directory {options.OutputPath} does not exist.");
            return;
        }


        string json = File.ReadAllText(options.File);


        SourceTableCollection? source = JsonSerializer.Deserialize<SourceTableCollection>(json, new JsonSerializerOptions
        {
            Converters = { new DateTimeKindConverter() }
        });

        if (source is null)
        {
            return;
        }

        if (options.Crud)
        {
            CreateCrudPackage(options, source);
        }
        else if (options.Triggers)
        {
            CreateTriggers(options, source);
        }
        else if (options.Model)
        {
            CreateModels(options, source);
        }
    }

    private static string CreateRest(OracleDataDictionaryDbContext context, Options options, SourceTableCollection source, bool specification = false)
    {
        StringBuilder buffer = new StringBuilder();

        buffer.AppendLine("-- ================================================================================");
        buffer.AppendLine($"-- Execute one or more operations against the database.");
        buffer.AppendLine("--");
        buffer.AppendLine($"-- The input is a JSON array of objects. Each object must contain the following properties:");
        buffer.AppendLine("--");
        buffer.AppendLine("--   $operation: The operation to perform. Must be one of 'insert', 'update' or 'delete'.");
        buffer.AppendLine("--   $table: The table to perform the operation on.");
        buffer.AppendLine("--   $data: An object containing the data to insert, update or delete.");
        buffer.AppendLine("-- ================================================================================");
        buffer.Append($"procedure execute_operations(p_clob in clob, po_json out varchar2)");

        if (specification)
        {
            buffer.AppendLine(";");
            return buffer.ToString();
        }

        buffer.AppendLine();
        buffer.AppendLine("is");

        int maxTableNameLength = source.Tables.Max(table => table.Name.Length);
        string padding = string.Empty;
        foreach (var sourceTable in source.Tables)
        {
            padding = new string(' ', maxTableNameLength - sourceTable.Name.Length);
            buffer.AppendLine($"  l_{sourceTable.Name.ToLower()}_row {padding}{sourceTable.Name.ToLower()}%rowtype;");
        }

        padding = new string(' ', maxTableNameLength - 9);
        buffer.AppendLine($"  l_json_element  {padding}json_element_t;");
        buffer.AppendLine($"  l_json_object   {padding}json_object_t;");
        buffer.AppendLine($"  l_json_array    {padding}json_array_t;");
        buffer.AppendLine($"  l_size          {padding}pls_integer;");
        buffer.AppendLine($"  l_operation     {padding}varchar2(128); -- insert, update or delete");
        buffer.AppendLine($"  l_table         {padding}varchar2(128);");
        buffer.AppendLine($"  l_data          {padding}json_object_t;");
        buffer.AppendLine($"  -- return value which is an array and variable for creating the items");
        buffer.AppendLine($"  l_return_array  {padding}json_array_t;  -- return object");
        buffer.AppendLine($"  l_return_object {padding}json_object_t; -- return object containg $operation, $table and $data");
        buffer.AppendLine($"  l_return_data   {padding}json_object_t;");

        buffer.AppendLine("begin");
        buffer.AppendLine("  l_return_array := json_array_t();");
        buffer.AppendLine("  l_json_element := json_element_t.parse(p_clob);");
        buffer.AppendLine("  if not l_json_element.is_array then");
        buffer.AppendLine("    raise_application_error(-20001, 'Expected a JSON array');");
        buffer.AppendLine("  end if;");
        buffer.AppendLine();
        buffer.AppendLine("  l_json_array := treat(l_json_element as json_array_t);");
        buffer.AppendLine("  l_size := l_json_array.get_size;");
        buffer.AppendLine("  for i in 0 .. l_size - 1 loop");
        buffer.AppendLine("    l_json_element := l_json_array.get(i);");
        buffer.AppendLine();
        buffer.AppendLine("    if not l_json_element.is_object then");
        buffer.AppendLine("      raise_application_error(-20001, 'Expected array index '|| TO_CHAR(i) || ' to contain a JSON object');");
        buffer.AppendLine("    end if;");
        buffer.AppendLine("    l_json_object := treat(l_json_element as json_object_t);");
        buffer.AppendLine();
        buffer.AppendLine("    -- ensure the structure of the JSON object is correct");
        buffer.AppendLine("    l_operation := l_json_object.get_string('$operation');");
        buffer.AppendLine("    if l_operation is null then");
        buffer.AppendLine("      raise_application_error(-20001, 'Expected string property ''$operation'' at array index '|| TO_CHAR(i));");
        buffer.AppendLine("    end if;");
        buffer.AppendLine();
        buffer.AppendLine("    l_table := l_json_object.get_string('$table');");
        buffer.AppendLine("    if l_table is null then");
        buffer.AppendLine("      raise_application_error(-20001, 'Expected string property ''$table'' at array index '|| TO_CHAR(i));");
        buffer.AppendLine("    end if;");
        buffer.AppendLine();
        buffer.AppendLine("    l_data := l_json_object.get_object('$data');");
        buffer.AppendLine("    if l_data is null then");
        buffer.AppendLine("      raise_application_error(-20001, 'Expected json object property ''$data'' at array index '|| TO_CHAR(i));");
        buffer.AppendLine("    end if;");
        buffer.AppendLine();

        buffer.AppendLine("    l_return_object := json_object_t();");
        buffer.AppendLine("    l_return_object.put('$operation', l_operation);");
        buffer.AppendLine("    l_return_object.put('$table', l_table);");
        buffer.AppendLine("    l_return_data := json_object_t();");
        buffer.AppendLine();

        foreach (var sourceTable in source.Tables)
        {
            var table = context.Tables
                .Where(table => table.Name == sourceTable.Name)
                .Include(table => table.Columns)
                .Include(table => table.Constraints.Where(constraint => constraint.Type == ConstraintType.PrimaryKey))
                    .ThenInclude(constraint => constraint.Columns)
                .Single();

            string primaryKeyColumn = table.Constraints.Single(constraint => constraint.Type == ConstraintType.PrimaryKey).Columns.Single().ColumnName.ToLower();

            string prefix = sourceTable == source.Tables.First() ? "if" : "elsif";
            buffer.AppendLine($"    {prefix} l_table = '{sourceTable.Name.ToLower()}' then");
            buffer.AppendLine($"      execute_operation(l_operation, l_json_object, l_{sourceTable.Name.ToLower()}_row);");
            buffer.AppendLine($"      l_return_data.put('{primaryKeyColumn}', l_{sourceTable.Name.ToLower()}_row.{primaryKeyColumn});");
        }

        buffer.AppendLine("    end if;");
        buffer.AppendLine();
        buffer.AppendLine("  -- append the object to the array");
        buffer.AppendLine("    l_return_object.put('$data', l_return_data);");
        buffer.AppendLine("    l_return_array.append(l_return_object);");
        buffer.AppendLine($"  end loop;");

        buffer.AppendLine();
        buffer.AppendLine("  po_json := l_return_array.to_string;");
        buffer.AppendLine($"end execute_operations;");
        buffer.AppendLine();

        return buffer.ToString();
    }

    private static void CreateModels(Options options, SourceTableCollection source)
    {
        OracleDataDictionaryDbContext context = context = CreateDbContext(options);
        string owner = source.Schema.ToLower();

        StringBuilder buffer = new StringBuilder();

        buffer.AppendLine("//------------------------------------------------------------------------------");
        buffer.AppendLine("// <auto-generated>");
        buffer.AppendLine("//     This code was generated by a tool.");
        buffer.AppendLine("//");
        buffer.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
        buffer.AppendLine("//     the code is regenerated.");
        buffer.AppendLine("// </auto-generated>");
        buffer.AppendLine("//------------------------------------------------------------------------------");
        buffer.AppendLine();
        buffer.AppendLine("#nullable enable");
        buffer.AppendLine();
        buffer.AppendLine($"namespace TrafficCourts.OrdsDataService.{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(owner)};");
        buffer.AppendLine();
        buffer.AppendLine("#nullable enable");

        foreach (SourceTable sourceTable in source.Tables)
        {
            Table table = context.Tables
                .Where(table => table.Name == sourceTable.Name)
                .Include(table => table.Columns)
                .Include(table => table.Constraints.Where(constraint => constraint.Type == ConstraintType.PrimaryKey))
                    .ThenInclude(constraint => constraint.Columns)
                .Single();

            buffer.Append(Model.Create(table, sourceTable));
            buffer.AppendLine();
        }

        File.WriteAllText($"{options.OutputPath}/Models.cs", buffer.ToString());

    }

    private static void CreateTriggers(Options options, SourceTableCollection source)
    {
        OracleDataDictionaryDbContext context = context = CreateDbContext(options);
        string owner = source.Schema.ToLower();

        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        foreach (var sourceTable in source.Tables)
        {
            StringBuilder buffer = new StringBuilder();

            Table table = context.Tables
                .Where(table => table.Name == sourceTable.Name)
                .Include(table => table.Columns)
                .Include(table => table.Constraints)
                .Single();

            // skip this table if it doesn't have the audit columns
            if (!(table.Columns.Any(column => column.Name == "ENT_DTM") &&
                  table.Columns.Any(column => column.Name == "ENT_USER_ID") &&
                  table.Columns.Any(column => column.Name == "UPD_DTM") &&
                  table.Columns.Any(column => column.Name == "UPD_USER_ID")))
            {
                continue;
            }

            string triggerFileName = $"{owner}_{sourceTable.Abbreviation}_TRIGGER".ToUpper();
            string triggerName = $"{owner}_{sourceTable.Abbreviation}_R_B_IU".ToLower();

            // assumption - single column primary key
            string primaryKeyColumnName = context.PrimaryKeyFor(table)
                .SelectMany(constraint => constraint.Columns)
                .Single()
                .ColumnName
                .ToLower();

            buffer.AppendLine("-- ------------------------------------------------------------------------------");
            buffer.AppendLine("-- AUTO-GENERATED");
            buffer.AppendLine("--");
            buffer.AppendLine("--     This code was generated by a tool.");
            buffer.AppendLine("--");
            buffer.AppendLine("--     Changes to this file may cause incorrect behavior and will be lost if");
            buffer.AppendLine("--     the code is regenerated.");
            buffer.AppendLine("-- ------------------------------------------------------------------------------");
            buffer.AppendLine();
            buffer.AppendLine($"create or replace trigger {triggerName}");
            buffer.AppendLine($"before insert or update on {table.Name.ToLower()}");
            buffer.AppendLine($"for each row");
            buffer.AppendLine($"begin");
            buffer.AppendLine();
            buffer.AppendLine($"-- $Date$");
            buffer.AppendLine($"-- $Revision$");
            buffer.AppendLine($"-- $Author$");
            buffer.AppendLine($"-- $HeadURL$");
            buffer.AppendLine();

            // does this table use a sequence for the PK?
            if (!string.IsNullOrEmpty(sourceTable.PrimaryKeySequenceName))
            {
                buffer.AppendLine($"  if inserting then");
                buffer.AppendLine($"    -- get the next value from the sequence");
                buffer.AppendLine($"    select {sourceTable.PrimaryKeySequenceName}.nextval");
                buffer.AppendLine($"      into :new.{primaryKeyColumnName}");
                buffer.AppendLine($"      from dual;");

                // for each fk that is generated by a sequence
                foreach (var foreignKey in context.ForeignKeysFor(table).Include(fk => fk.Columns).OrderBy(fk => fk.Name))
                {   
                    // get the constraint name (ie the PK, of the other table)
                    var pkConstraint = context.Constraints
                        .Where(constraint => constraint.Owner == foreignKey.Owner && constraint.Name == foreignKey.ReferencedConstraintName)
                        .Include(constraint => constraint.Columns)
                        .SingleOrDefault();

                    if (pkConstraint is null)
                    {
                        continue;
                    }

                    var otherSourceTable = source.Tables.Where(table => table.Name == pkConstraint.TableName).SingleOrDefault();
                    if (!string.IsNullOrEmpty(otherSourceTable?.PrimaryKeySequenceName))
                    {
                        string columnName = foreignKey.Columns.Single().ColumnName.ToLower();

                        buffer.AppendLine();
                        buffer.AppendLine($"    -- get the value for {columnName} from current value of sequence {otherSourceTable.PrimaryKeySequenceName}");
                        buffer.AppendLine($"    if :new.{columnName} is null then");
                        buffer.AppendLine($"      select {otherSourceTable.PrimaryKeySequenceName}.currval");
                        buffer.AppendLine($"        into :new.{columnName}");
                        buffer.AppendLine($"        from dual;");
                        buffer.AppendLine($"    end if;");
                    }
                }

                buffer.AppendLine($"  end if;");
            }

            buffer.AppendLine();
            buffer.AppendLine("  -- set the audit fields");
            buffer.AppendLine($"  {owner}_audit.set_fields(inserting, :new.ent_dtm, :new.ent_user_id, :new.upd_dtm, :new.upd_user_id);");
            buffer.AppendLine($"end {triggerName};");
            buffer.AppendLine("/");

            File.WriteAllText($"{options.OutputPath}/{triggerFileName}.sql", buffer.ToString());
        }


    }

    private static void CreateCrudPackage(Options options, SourceTableCollection source)
    {
        void AppendAutoGeneratedHeader(StringBuilder buffer)
        {
            buffer.AppendLine("-- ------------------------------------------------------------------------------");
            buffer.AppendLine("-- AUTO-GENERATED");
            buffer.AppendLine("--");
            buffer.AppendLine("--     This code was generated by a tool.");
            buffer.AppendLine("--");
            buffer.AppendLine("--     Changes to this file may cause incorrect behavior and will be lost if");
            buffer.AppendLine("--     the code is regenerated.");
            buffer.AppendLine("-- ------------------------------------------------------------------------------");
            buffer.AppendLine();
        }

        OracleDataDictionaryDbContext context = context = CreateDbContext(options);
        var crud = new CrudProcedure(context);

        string owner = source.Schema;

        string packageName = $"{owner.ToLower()}_tables";

        StringBuilder buffer = new StringBuilder();

        AppendAutoGeneratedHeader(buffer);

        buffer.AppendLine($"CREATE OR REPLACE PACKAGE {packageName}");
        buffer.AppendLine("AS");
        buffer.AppendLine();
        buffer.AppendLine("-- This package contains procedures for reading, creating, updating and deleting");
        buffer.AppendLine($"-- rows from the {owner.ToLower()} schema.");
        buffer.AppendLine();
        buffer.AppendLine($"-- $Date$");
        buffer.AppendLine($"-- $Revision$");
        buffer.AppendLine($"-- $Author$");
        buffer.AppendLine($"-- $HeadURL$");
        buffer.AppendLine();

        foreach (var table in source.Tables)
        {
            buffer.AppendLine("-- ================================================================================");
            buffer.AppendLine($"-- Operations on the {table.Name} table");
            buffer.AppendLine("-- ================================================================================");
            buffer.AppendLine();

            if (table.CreateOperation(CrudOperation.Insert))
            {
                buffer.AppendLine(crud.Insert(source, table, source.AuditColumns, specification: true));
            }

            if (table.CreateOperation(CrudOperation.Update))
            {
                buffer.AppendLine(crud.Update(table, source.AuditColumns, specification: true));
            }

            if (table.CreateOperation(CrudOperation.Delete))
            {
                buffer.AppendLine(crud.Delete(table, source.AuditColumns, specification: true));
            }

            if (table.CreateOperation(CrudOperation.Select))
            {
                buffer.AppendLine(crud.Select(table, source.AuditColumns, specification: true));
            }
        }

        buffer.AppendLine(CreateRest(context, options, source, specification: true));

        buffer.AppendLine($"END {packageName};");
        buffer.AppendLine("/");
        buffer.AppendLine();

        File.WriteAllText($"{options.OutputPath}/{packageName}.sql", buffer.ToString());
        buffer.Clear();

        AppendAutoGeneratedHeader(buffer);

        buffer.AppendLine($"CREATE OR REPLACE PACKAGE BODY {packageName}");
        buffer.AppendLine("AS");
        buffer.AppendLine();
        buffer.AppendLine("-- This package contains procedures for creating, updating and deleting");
        buffer.AppendLine($"-- rows from the {owner.ToLower()} schema.");
        buffer.AppendLine();
        buffer.AppendLine($"-- $Date$");
        buffer.AppendLine($"-- $Revision$");
        buffer.AppendLine($"-- $Author$");
        buffer.AppendLine($"-- $HeadURL$");
        buffer.AppendLine();

        foreach (var table in source.Tables)
        {
            buffer.AppendLine("-- ================================================================================");
            buffer.AppendLine($"-- Operations on the {table.Name} table");
            buffer.AppendLine("-- ================================================================================");
            buffer.AppendLine();
            //buffer.AppendLine(crud.Select(table));
            if (table.CreateOperation(CrudOperation.Insert))
            {
                buffer.AppendLine(crud.Insert(source, table, source.AuditColumns));
            }

            if (table.CreateOperation(CrudOperation.Update))
            {
                buffer.AppendLine(crud.Update(table, source.AuditColumns));
            }

            if (table.CreateOperation(CrudOperation.Delete))
            {
                buffer.AppendLine(crud.Delete(table, source.AuditColumns));
            }

            if (table.CreateOperation(CrudOperation.Select))
            {
                buffer.AppendLine(crud.Select(table, source.AuditColumns));
            }

            if (table.CreateOperation(CrudOperation.Insert) || table.CreateOperation(CrudOperation.Update))
            {
                buffer.AppendLine(crud.JsonToRowType(table, source.AuditColumns));
            }

            buffer.AppendLine(crud.ExecuteOperation(table, source.AuditColumns));
        }

        buffer.AppendLine(CreateRest(context, options, source));


        buffer.AppendLine();
        buffer.AppendLine($"END {packageName};");
        buffer.AppendLine("/");

        File.WriteAllText($"{options.OutputPath}/{packageName}_body.sql", buffer.ToString());
    }

    private static void DisplayHelp<T>(ParserResult<T> result)
    {
        var helpText = HelpText.AutoBuild(result, h =>
        {
            h.AdditionalNewLineAfterOption = false;
            h.Copyright = string.Empty;
            return HelpText.DefaultParsingErrorsHandler(result, h);
        }, e => e);
    }
}

public class DateTimeKindConverter : JsonConverter<DateTimeKind>
{
    public override DateTimeKind Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        return value switch
        {
            "Utc" => DateTimeKind.Utc,
            "Local" => DateTimeKind.Local,
            "Unspecified" => DateTimeKind.Unspecified,
            _ => throw new JsonException($"Invalid DateTimeKind value: {value}")
        };
    }

    public override void Write(Utf8JsonWriter writer, DateTimeKind value, JsonSerializerOptions options)
    {
        string stringValue = value switch
        {
            DateTimeKind.Utc => "Utc",
            DateTimeKind.Local => "Local",
            DateTimeKind.Unspecified => "Unspecified",
            _ => throw new JsonException($"Invalid DateTimeKind value: {value}")
        };
        writer.WriteStringValue(stringValue);
    }
}