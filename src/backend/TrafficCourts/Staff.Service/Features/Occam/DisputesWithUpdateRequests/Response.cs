namespace TrafficCourts.Staff.Service.Features.Occam.DisputesWithUpdateRequests;

public class Response
{
    public Response(PagedOccamDisputeWithUpdateRequestListItemCollection data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public Response(string errorId)
    {
        ErrorId = errorId ?? throw new ArgumentNullException(nameof(errorId));
    }

    public PagedOccamDisputeWithUpdateRequestListItemCollection? Data { get; set; }

    public string? ErrorId { get; set; }
}
