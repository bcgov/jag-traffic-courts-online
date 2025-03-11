using System.Text;
using Microsoft.EntityFrameworkCore;
using Oracle.DataDictionary;
using Index = Oracle.DataDictionary.Index;

namespace Oracle;

public class OracleToDbml
{
    private readonly IDbmlGenerationOptions _options;

    public OracleToDbml(IDbmlGenerationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    public string Generate(bool dbml = true)
    {
        StringBuilder buffer = new StringBuilder();

        if (!dbml)
        {
            buffer.AppendLine("owner,table,column,column_id,type,length,scale,nullable");
        }

        List<Table> tables = new List<Table>();

        foreach (var owner in _options.Schemas)
        {
            tables.AddRange(GetTargetTables(owner));
        }

        tables = tables.Distinct(new TableComparer()).ToList();

        foreach (var table in tables.OrderBy(_ => _.Owner).ThenBy(_ => _.Name))
        {
            if (dbml)
            {
                GenerateTable(tables, table, buffer);
            }
            else
            {
                ExportTables(tables, table, buffer);
            }
        }

        return buffer.ToString();
    }

    private IList<Table> GetTargetTables(string owner)
    {
        var db = GetDbContext(owner);
        if (db is null) return Array.Empty<Table>();

        var tables = db.TablesOwnedBy(owner)
            .OrderBy(t => t.Name)
            .Select(t => new Table { Owner = t.Owner, Name = t.Name })
            .ToList();

        // foreach of these tables, find any references to tables that are not in our schema
        List<Table> additionalTables = new List<Table>();

        foreach (var table in tables)
        {
            List<Constraint> fks = db
                .ForeignKeysFor(table)
                .ToList();

            foreach (var fk in fks)
            {
                var referenced = db.ReferencedConstraintFor(fk)
                    .SingleOrDefault();

                if (referenced is null)
                {
                    continue; // the FK points to something outside our schema
                }

                if (referenced.Owner != table.Owner)
                {
                    // the referenced table is in a different schema
                    var referencedDb = GetDbContext(referenced.Owner);
                    if (referencedDb is null)
                    {
                        // references an object that we don't have a connection to
                        continue;
                    }

                    var otherTable = referencedDb.TableFor(referenced)
                        .Select(t => new Table { Owner = t.Owner, Name = t.Name })
                        .SingleOrDefault();

                    if (otherTable is not null && !additionalTables.Exists(t => t.Owner == otherTable.Owner && t.Name == otherTable.Name))
                    {
                        additionalTables.Add(otherTable);
                    }
                }
            }
        }

        if (additionalTables.Count != 0)
        {
            tables.AddRange(additionalTables);
        }

        return tables;
    }

    private OracleDataDictionaryDbContext? GetDbContext(string owner)
    {
        var context = _options.Connections.Where(kv => kv.Key == owner).Select(kv => kv.Value).FirstOrDefault();
        return context;
    }

    private void ExportTables(IList<Table> tables, Table table, StringBuilder buffer)
    {
        OracleDataDictionaryDbContext? db = GetDbContext(table.Owner);
        if (db is null)
        {
            return;
        }

        // get table with details
        table = db.Tables
            .Where(t => t.Owner == table.Owner && t.Name == table.Name)
            .Include(t => t.Columns.OrderBy(c => c.ColumnId))
            .First();

        // some tables, at least in the sample oracle schemas do not have columns?
        // see: OE.PRODUCT_REF_LIST_NESTEDTAB
        if (table.Columns.Count == 0)
        {
            return;
        }

        foreach (var column in table.Columns)
        {
            buffer.Append(table.Owner.ToLower());
            buffer.Append(',');

            buffer.Append(table.Name.ToLower());
            buffer.Append(',');

            buffer.Append(column.Name.ToLower());
            buffer.Append(',');

            buffer.Append(column.ColumnId);
            buffer.Append(',');

            buffer.Append(column.DataType.ToLower());
            buffer.Append(',');

            // dump out the length
            switch (column.DataType)
            {

                case "VARCHAR2":
                case "CLOB":
                    buffer.Append(column.DataLength);
                    buffer.Append(',');
                    break;
                case "NUMBER":
                    buffer.Append(column.DataPrecision);
                    buffer.Append(',');
                    buffer.Append(column.DataScale);
                    break;
                default: // DATE
                    buffer.Append(',');
                    break;
            }
            buffer.Append(',');

            buffer.Append(column.Nullable == "Y" ? "NULL" : "NOT NULL");
            buffer.AppendLine();
        }
    }

    private void GenerateTable(IList<Table> tables, Table table, StringBuilder buffer)
    {
        OracleDataDictionaryDbContext? db = GetDbContext(table.Owner);
        if (db is null)
        {
            return;
        }

        // get table with details
        table = db.Tables
            .Where(t => t.Owner == table.Owner && t.Name == table.Name)
            .Include(t => t.Columns.OrderBy(c => c.ColumnId))
            .First();

        // some tables, at least in the sample oracle schemas do not have columns?
        // see: OE.PRODUCT_REF_LIST_NESTEDTAB
        if (table.Columns.Count == 0)
        {
            return;
        }

        buffer.Append("Table ");
        if (_options.OutputSchemaName)
        {
            buffer.AppendName(table.Owner);
        }
        buffer.Append('.');
        buffer.AppendName(table.Name);
        buffer.AppendLine(" {");

        foreach (var column in table.Columns)
        {
            buffer.AppendColumnDeclaration(column);
            buffer.AppendLine();
        }

        GenerateIndexes(db, table, buffer);

        buffer.AppendLine("}");

        GenerateForeignKey(db, tables, table, buffer);
    }

    private void GenerateIndexes(OracleDataDictionaryDbContext db, Table table, StringBuilder buffer)
    {
        // get the primary key
        Constraint? pk = db
            .PrimaryKeyFor(table)
            .Include(c => c.Columns.OrderBy(c => c.Position))
            .FirstOrDefault();

        // get the normal indexes
        List<Index> indexes = db
             .IndexesFor(table)
             .Include(i => i.Columns.OrderBy(c => c.Position))
             .Where(i => i.Type == IndexType.Normal)
             .ToList();

        // no primary key or index
        if (pk is null && indexes.Count == 0)
        {
            return;
        }

        buffer.AppendLine("  Indexes {");

        if (pk is not null)
        {
            GeneratePrimaryKey(pk, buffer);
        }

        foreach (var index in indexes)
        {
            if (pk is not null && index.Owner == pk.IndexOwner && index.Name == pk.IndexName)
            {
                continue; // index is for the primary key
            }
            GenerateIndex(index, buffer);
        }

        buffer.AppendLine("  }");
    }

    private void GenerateIndex(Index index, StringBuilder buffer)
    {
        buffer.Append("    (");

        foreach (var column in index.Columns)
        {
            if (column.Position != 1)
            {
                buffer.Append(',');
            }
            buffer.AppendName(column.ColumnName);
        }

        buffer.Append(')');

        if (index.Uniqueness == "UNIQUE")
        {
            buffer.Append(" [unique]");
        }

        buffer.AppendLine();

    }
    private string GeneratePrimaryKey(Constraint pk, StringBuilder buffer)
    {        
        if (pk is null)
        {
            return string.Empty;
        }

        buffer.Append("    (");
        foreach (var column in pk.Columns)
        {
            if (column.Position != 1)
            {
                buffer.Append(',');
            }
            buffer.AppendName(column.ColumnName);
        }

        buffer.AppendLine(") [pk]");

        return pk.Name;
    }

    private void GenerateForeignKey(OracleDataDictionaryDbContext db, IList<Table> tables, Table table, StringBuilder buffer)
    {
        var length = buffer.Length;

        List<Constraint> fks = db
            .ForeignKeysFor(table)
            .Include(c => c.Columns.OrderBy(c => c.Position))
            .ToList();

        foreach (var fk in fks)
        {
            var referenced = db.ReferencedConstraintFor(fk)
                .Include(c => c.Columns.OrderBy(c => c.Position))
                .SingleOrDefault();

            if (referenced is null)
            {
                continue; // the FK points to something outside our schema
            }

            // only add references to tables we know about
            if (tables.Any(t => t.Owner == referenced.Owner && t.Name == referenced.TableName))
            {
                buffer.Append($"Ref: ");
                GenerateTableAndColumns(referenced, buffer);
                buffer.Append(" < ");
                GenerateTableAndColumns(fk, buffer);
                buffer.AppendLine();
            }

        }

        if (buffer.Length != length)
        {
            // we adding something
            buffer.AppendLine();
        }
    }

    private void GenerateTableAndColumns(Constraint constraint, StringBuilder buffer)
    {
        buffer.AppendName(constraint.Owner);
        buffer.Append('.');
        buffer.AppendName(constraint.TableName);
        buffer.Append('.');

        if (constraint.Columns.Count > 1)
        {
            buffer.Append('(');
        }

        foreach (var column in constraint.Columns)
        {
            if (column.Position != 1) { buffer.Append(','); }
            buffer.AppendName(column.ColumnName);
        }

        if (constraint.Columns.Count > 1)
        {
            buffer.Append(')');
        }
    }

}

public static class StringBuilderExtensions
{
    public static StringBuilder AppendName(this StringBuilder builder, string name)
    {
        if (name.IndexOf('$') == -1)
        {
            builder.Append(name.ToLower());
        }
        else
        {
            builder.Append('"');
            builder.Append(name.ToLower());
            builder.Append('"');
        }

        return builder;
    }

    public static StringBuilder AppendColumnDeclaration(this StringBuilder builder, TableColumn column)
    {
        if (column.DataType is null)
        {
            return builder;
        }

        builder.AppendName(column.Name);
        builder.Append(' ');

        if (column.DataType == "VARCHAR" ||
            column.DataType == "VARCHAR2" ||
            column.DataType == "CHAR" ||
            column.DataType == "NVARCHAR" ||
            column.DataType == "NVARCHAR2" ||
            column.DataType == "NCHAR" ||
            column.DataType == "RAW")
        {
            builder.AppendName(column.DataType);
            builder.Append('(');
            builder.Append(column.DataLength);
            builder.Append(')');
        }
        else if (column.DataType == "NUMBER")
        {
            builder.AppendName(column.DataType);

            if (column.DataPrecision is not null)
            {
                builder.Append('(');

                builder.Append(column.DataPrecision);
                if (column.DataScale is not null && column.DataScale != 0)
                {
                    builder.Append(',');
                    builder.Append(column.DataScale);
                }
                builder.Append(')');
            }
        }
        else if (column.DataType == "FLOAT")
        {
            builder.AppendName(column.DataType);

            builder.Append('(');
            builder.Append(column.DataPrecision);
            builder.Append(')');
        }
        else if (column.DataType == "TIMESTAMP")
        {
            builder.AppendName(column.DataType);
        }
        else
        {
            if (column.DataType.IndexOf(' ') != -1)
            {
                builder.Append('"');
                builder.AppendName(column.DataType);
                builder.Append('"');
            }
            else
            {
                builder.AppendName(column.DataType);
            }
        }

        if (column.Nullable == "N")
        {
            builder.Append(" [not null]");
        }

        return builder;
    }
}

public interface IDbmlGenerationOptions
{
    public bool OutputSchemaName { get; }
    IEnumerable<string> Schemas { get; }

    IEnumerable<KeyValuePair<string, OracleDataDictionaryDbContext>> Connections { get; }
}

public class DbmlGenerationOptionsBuilder
{
    private Dictionary<string, OracleDataDictionaryDbContext> _databases = new Dictionary<string, OracleDataDictionaryDbContext>();

    private HashSet<string> _includedSchemas = new HashSet<string>();
    private HashSet<string> _excludedSchemas = new HashSet<string>();

    private bool _outputSchemaName;
    public DbmlGenerationOptionsBuilder UseDbContext(string schema, OracleDataDictionaryDbContext context)
    {
        _databases.Add(schema, context);
        return this;
    }


    public DbmlGenerationOptionsBuilder OutputSchemaName(bool output)
    {
        _outputSchemaName = output;
        return this;
    }

    public DbmlGenerationOptionsBuilder AddSchema(string schema)
    {
        schema = schema.ToUpper();

        _excludedSchemas.Remove(schema);
        _includedSchemas.Add(schema);

        return this;
    }

    public DbmlGenerationOptionsBuilder ExcludeSchema(string schema)
    {
        schema = schema.ToUpper();

        _includedSchemas.Remove(schema);
        _excludedSchemas.Add(schema);

        return this;
    }

    public DbmlGenerationOptions Build()
    {
        var options = new DbmlGenerationOptions();
        options._includedSchemas.AddRange(_includedSchemas);
        options._excludedSchemas.AddRange(_excludedSchemas);

        options._databases = _databases;
        options.OutputSchemaName = _outputSchemaName;

        return options;
    }

    public class DbmlGenerationOptions : IDbmlGenerationOptions
    {
        public Dictionary<string, OracleDataDictionaryDbContext> _databases = new Dictionary<string, OracleDataDictionaryDbContext>();

        public List<string> _includedSchemas = new List<string>();
        public List<string> _excludedSchemas = new List<string>();

        public List<TableWithOwner> _excludedTables = new List<TableWithOwner>();
        public List<TableWithOwner> _includedTables = new List<TableWithOwner>();

        public bool OutputSchemaName { get; set; }

        public IEnumerable<string> Schemas
        {
            get 
            { 
                foreach (var schema in _includedSchemas)
                {
                    yield return schema;
                }
            }
        }

        public IEnumerable<KeyValuePair<string, OracleDataDictionaryDbContext>> Connections
        {
            get
            {
                foreach (var item in _databases)
                {
                    yield return item;
                }
            }
        }

        public record TableWithOwner(string Owner, string TableName);
    }
}