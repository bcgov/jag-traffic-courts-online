namespace Oracle.DataDictionary;

public class TableComparer : IEqualityComparer<Table>
{
    public bool Equals(Table x, Table y)
    {
        if (x == null || y == null)
            return false;

        return x.Owner == y.Owner && x.Name == y.Name;
    }

    public int GetHashCode(Table obj)
    {
        if (obj == null)
            return 0;

        int hashOwner = obj.Owner == null ? 0 : obj.Owner.GetHashCode();
        int hashName = obj.Name == null ? 0 : obj.Name.GetHashCode();

        return hashOwner ^ hashName;
    }
}
