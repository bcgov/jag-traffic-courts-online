namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public class Response
{
    public Response(PagedOccamDisputeListItemCollection data)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public Response(string errorId)
    {
        ErrorId = errorId ?? throw new ArgumentNullException(nameof(errorId));
    }

    public PagedOccamDisputeListItemCollection? Data { get; set; }

    public string? ErrorId { get; set; }
}
