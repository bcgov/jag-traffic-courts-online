using MediatR;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public static class ProvinceLookup
{
    public class Request : IRequest<Response>
    {
        public Request()
        {
        }
    }

    public class Response
    {
        public Response(IList<Province> provinces)
        {
            Provinces = provinces;
        }

        public IList<Province> Provinces { get; }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IProvinceLookupService _service;

        public Handler(IProvinceLookupService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            IList<Province> provinces = await _service.GetListAsync();
            return new Response(provinces);
        }
    }
}
