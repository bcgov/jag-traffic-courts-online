using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Collections;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Staff.Service.Models.Disputes;

public class GetAllDisputesParameters : IPagable, ISortable
{
    #region Filtering
    /// <summary>
    /// The status to exclude
    /// </summary>
    [FromQuery(Name = "excludeStatus")]
    public List<ExcludeStatus>? ExcludeStatus { get; set; }

    /// <summary>
    /// The optional ticket number to search on. The value will be searched using contains.
    /// </summary>
    [FromQuery(Name = "ticket")]
    public string? TicketNumber { get; set; }

    /// <summary>
    /// The optional surname to search on. The value will be searched using contains.
    /// </summary>
    [FromQuery(Name = "surname")]
    public string? Surname { get; set; }

    /// <summary>
    /// The optional status to find.
    /// </summary>
    [FromQuery(Name = "status")]
    public List<DisputeStatus>? Status { get; set; }

    /// <summary>
    /// The optional from date to search. The submitted date will be filtered where greater or equal to this value.
    /// </summary>
    [FromQuery(Name = "from")]
    public DateTimeOffset? From { get; set; }

    /// <summary>
    /// The optional thru date to search. The submitted date will be filtered where less than or equal to this value.
    /// </summary>
    [FromQuery(Name = "thru")]
    public DateTimeOffset? Thru { get; set; }

    /// <summary>
    /// The optional court house location to search. The value will be searched using contains.
    /// </summary>
    [FromQuery(Name = "courtHouse")]
    public string? CourtHouse { get; set; }

    #endregion

    #region Sorting
    [FromQuery(Name = "sortBy")]
    public List<string>? SortBy { get; set; }

    [FromQuery(Name = "direction")]
    public List<SortDirection>? SortDirection { get; set; }
    #endregion

    #region Paging
    public int DefaultPageSize => 25;

    [FromQuery(Name = "pageNumber")]
    public int? PageNumber { get; set; }

    [FromQuery(Name = "pageSize")]
    public int? PageSize { get; set; }

    #endregion

    /// <summary>
    /// Sets the default sort order if no sort order specified
    /// </summary>
    public void SetDefaultSortIfNotSpecified()
    {
        if (SortBy is null || SortBy?.Count == 0)
        {
            SortBy = [nameof(DisputeListItem.SubmittedTs), nameof(DisputeListItem.TicketNumber)];
            SortDirection = [Collections.SortDirection.desc, Collections.SortDirection.asc];
        }
    }
}
