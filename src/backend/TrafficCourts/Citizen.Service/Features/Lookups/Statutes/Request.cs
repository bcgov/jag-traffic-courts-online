using MediatR;

namespace TrafficCourts.Citizen.Service.Features.Lookups.Statutes;

public class Request : IRequest<Response>
{
    public Request(string? section)
    {
        Section = section;
    }

    public string? Section { get; }
}
