using MediatR;
using TrafficCourts.OrdsDataService.Justin;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Features.Lookups.Languages;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly ILanguageLookupService _service;

    public Handler(ILanguageLookupService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _service.GetListAsync(cancellationToken);
        return new Response(items);
    }
}
