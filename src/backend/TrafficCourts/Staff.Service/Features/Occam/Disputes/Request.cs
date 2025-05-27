using MediatR;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public class Request : IRequest<Response>
{
    public OccamDisputeListingParameters Parameters { get; set; }
}
