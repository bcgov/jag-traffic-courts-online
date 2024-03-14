using X.PagedList;

namespace TrafficCourts.Domain.Models;

/// <summary>
/// 
/// </summary>
/// <param name="values"></param>
public abstract class PagedCollection<TValue>(IPagedList<TValue> values)
{
    /// <summary>
    /// The paged items.
    /// </summary>
    public IReadOnlyCollection<TValue> Items { get; set; } = values;

    /// <summary>
    /// The one-based index of this page.
    /// </summary>
    public int PageNumber { get; } = values.PageNumber;

    /// <summary>
    /// Them maximum size of a page.
    /// </summary>
    public int PageSize { get; } = values.PageSize;

    /// <summary>
    /// The total number of pages.
    /// </summary>
    public int PageCount { get; } = values.PageCount;

    /// <summary>
    /// The total number of items.
    /// </summary>
    public int TotalItemCount { get; } = values.TotalItemCount;

    /// <summary>
    /// True if there is a previous page, otherwise false.
    /// </summary>
    public bool HasPreviousPage { get; } = values.HasPreviousPage;

    /// <summary>
    /// True if there is a next page, otherwise false.
    /// </summary>
    public bool HasNextPage { get; } = values.HasNextPage;

    /// <summary>
    /// True if this is the first page, otherwise false.
    /// </summary>
    public bool IsFirstPage { get; } = values.IsFirstPage;

    /// <summary>
    /// True if this is the last page, otherwise false.
    /// </summary>
    public bool IsLastPage { get; } = values.IsLastPage;
}
