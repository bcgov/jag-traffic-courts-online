namespace TrafficCourts.Collections;

public static class SortableExtensions
{
    public static IQueryable<T> Sort<T>(this IQueryable<T> items, ISortable? parameters)
    {
        // TODO
        if (parameters?.SortBy is null || parameters.SortBy.Count == 0)
        {
            return items;
        }

        var sorted = items.OrderBy(_ => 0); // fake sort

        for (int i = 0; i < parameters.SortBy.Count; i++)
        {
            string sortBy = parameters.SortBy[i];
            SortDirection direction = GetSortDirection(parameters.SortDirection, i);

            sorted = direction == SortDirection.desc
                ? sorted.ThenByDescending(sortBy)
                : sorted.ThenBy(sortBy);
        }

        return sorted;
    }

    private static SortDirection GetSortDirection(List<SortDirection>? direction, int index)
    {
        if (direction is null) return SortDirection.asc;
        if (direction.Count <= index) return SortDirection.asc;
        return direction[index];
    }
}
