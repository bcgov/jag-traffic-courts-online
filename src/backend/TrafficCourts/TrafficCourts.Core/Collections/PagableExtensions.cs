using X.PagedList;

namespace TrafficCourts.Collections;

public static class PagableExtensions
{
    public static IPagedList<T> Page<T>(this IQueryable<T> items, IPagable? parameters, int defaultPageSize = 25)
    {
        int page = parameters?.PageNumber ?? 1;
        int size = parameters?.PageSize ?? defaultPageSize;

        var paged = new PagedList<T>(items, page, size);
        return paged;
    }
}