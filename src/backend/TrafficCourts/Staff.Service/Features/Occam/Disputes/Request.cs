using MediatR;
using TrafficCourts.Staff.Service.Models.Disputes;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public class Request : IRequest<Response>
{
    public GetAllDisputesParameters Parameters;
}
