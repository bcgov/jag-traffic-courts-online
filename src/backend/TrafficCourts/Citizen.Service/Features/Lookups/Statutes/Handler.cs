using MediatR;
using TrafficCourts.Citizen.Service.Services.Lookups;

namespace TrafficCourts.Citizen.Service.Features.Lookups.Statutes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IStatuteLookupService _service;

    public Handler(IStatuteLookupService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }
    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Api.Statutes(3);

        IEnumerable<Domain.Models.Statute> models = await _service.GetListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Section))
        {
            models = models.Where(_ => _.Code == request.Section);
        }

        return new Response(models);
    }
}
