using MediatR;

namespace TrafficCourts.Staff.Service.Features.Occam.DisputesWithUpdateRequests;

public class Request : IRequest<Response>
{
    public OccamDisputeWithUpdateRequestsListingParameters Parameters { get; set; }
}
