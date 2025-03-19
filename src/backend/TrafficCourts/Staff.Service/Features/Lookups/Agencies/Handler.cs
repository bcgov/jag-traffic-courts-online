using MediatR;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Features.Lookups.Agencies;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IAgencyLookupService _service;

    public Handler(IAgencyLookupService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _service.GetListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            items = items.Where(_ => _.TypeCode == request.Type).ToList();
        }

        return new Response(items);
    }
}
