using MediatR;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups
{
    public static class StatuteLookup
    {
        public class Request : IRequest<Response>
        {
            public string Section { get; init; }

            public Request(string? section)
            {
                Section = section ?? string.Empty;
            }
        }

        public class Response
        {
            public Response(IList<Statute> statutes)
            {
                Statutes = statutes;
            }

            public IList<Statute> Statutes { get; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IStatuteLookupService _service;

            public Handler(IStatuteLookupService service)
            {
                _service = service ?? throw new ArgumentNullException(nameof(service));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                // get one?
                if (!string.IsNullOrEmpty(request.Section))
                {
                    var statute = await _service.GetBySectionAsync(request.Section);
                    if (statute is not null)
                    {
                        return new Response(new Statute[] { statute });
                    }

                    return new Response(Array.Empty<Statute>());
                }

                // get all
                IList<Statute> statutes = await _service.GetListAsync();
                return new Response(statutes);
            }
        }
    }
}
