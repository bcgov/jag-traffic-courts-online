using CodegenCS;
using Oracle.DataDictionary;

namespace Oracle.DbmlGeneration;

public class DbmlGenerator : ICodegenMultifileTemplate
{
    public void Render(ICodegenContext context)
    {

        // collect the target tables
        List<Table> tables = new List<Table>();


        ICodegenOutputFile writer = context[""];
        
        foreach (var table in tables)
        {
            Render(context, table);
        }
    }

    private void Render(ICodegenContext context, ICodegenOutputFile writer, Table table)
    {
        writer.WriteLine($"Table {TableName(table)} {{");

        using (writer.WithIndent("", ""))
        {
            // render the columns
            foreach (var column in table.Columns)
            {
                Render(context, writer, column);
            }
        }


        // render the primary key

        // render indexes
    }

    private string TableName(Table table) => table.Name.ToLower();

    private void Render(ICodegenContext context, ICodegenOutputFile writer, TableColumn column)
    {
    }
}

public class DbmlGeneratorOptions
{

}
