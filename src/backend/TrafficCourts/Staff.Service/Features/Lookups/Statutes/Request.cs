using MediatR;

namespace TrafficCourts.Staff.Service.Features.Lookups.Statutes;

public class Request : IRequest<Response>
{
    public string? Section { get; set; }

    public DateTime? EffectiveOn { get; set; }
}
