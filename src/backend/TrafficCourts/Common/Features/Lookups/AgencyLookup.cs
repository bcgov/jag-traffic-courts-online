using MediatR;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public static class AgencyLookup
{
    public class Request : IRequest<Response>
    {
        public Request()
        {
        }
    }

    public class Response
    {
        public Response(IList<Agency> agencies)
        {
            Agencies = agencies;
        }

        public IList<Agency> Agencies { get; }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IAgencyLookupService _service;

        public Handler(IAgencyLookupService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            IList<Agency> agencies = await _service.GetListAsync();
            return new Response(agencies);
        }
    }
}
