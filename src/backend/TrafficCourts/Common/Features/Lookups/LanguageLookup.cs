using MediatR;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public static class LanguageLookup
{
    public class Request : IRequest<Response>
    {
        public Request()
        {
        }
    }

    public class Response
    {
        public Response(IList<Language> languages)
        {
            Languages = languages;
        }

        public IList<Language> Languages { get; }
    }

    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly ILanguageLookupService _service;

        public Handler(ILanguageLookupService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            IList<Language> langugages = await _service.GetListAsync();
            return new Response(langugages);
        }
    }
}
