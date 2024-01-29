using MediatR;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public static class CountryLookup
{
    public class Request : IRequest<Response>
    {
        public Request()
        {
        }
    }

    public class Response
    {
        public Response(IList<Country> countries)
        {
            Countries = countries;
        }

        public IList<Country> Countries { get; }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ICountryLookupService _service;

        public Handler(ICountryLookupService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            IList<Country> countries = await _service.GetListAsync();
            return new Response(countries);
        }
    }
}
