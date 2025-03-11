using Oracle.DataDictionary;

namespace CrudGenerator;

public static class Extensions
{
    public static bool IsLastColumn(this List<TableColumn> columns, TableColumn column)
    {
        return columns.IndexOf(column) == columns.Count - 1;
    }

    public static bool IsFirstColumn(this ICollection<ConstraintColumn> columns, ConstraintColumn column)
    {
        return columns.First() == column;
    }

    public static bool IsLastColumn(this ICollection<ConstraintColumn> columns, ConstraintColumn column)
    {
        return columns.Last() == column;
    }
}