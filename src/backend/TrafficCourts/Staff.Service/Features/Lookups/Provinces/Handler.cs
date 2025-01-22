using MediatR;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Features.Lookups.Provinces;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IProvinceLookupService _lookupService;

    public Handler(IProvinceLookupService lookupService)
    {
        _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _lookupService.GetListAsync(cancellationToken);

        return new Response(items);
    }
}
