namespace TrafficCourts.OrdsDataService.Justin;

public class StatuteIdComparer : IComparer<Statute>
{
    public int Compare(Statute? x, Statute? y)
    {
        if (x is null)
        {
            return y is null ? 0 : -1;
        }

        if (y is null)
        {
            return 1;
        }

        return x.stat_id.CompareTo(y.stat_id);
    }
}
