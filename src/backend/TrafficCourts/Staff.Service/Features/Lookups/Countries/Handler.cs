using MediatR;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Features.Lookups.Countries;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly ICountryLookupService _service;

    public Handler(ICountryLookupService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _service.GetListAsync(cancellationToken);
        return new Response(items);
    }
}
