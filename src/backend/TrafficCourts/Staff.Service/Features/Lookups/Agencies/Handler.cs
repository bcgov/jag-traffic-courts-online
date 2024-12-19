using MassTransit.Initializers;
using MediatR;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Features.Lookups.Agencies;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly ICachedLookupService<Agency> _service;
    private readonly ILogger<Handler> _logger;

    public Handler(ICachedLookupService<Agency> service, ILogger<Handler> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Api.Agencies(2);

        List<Agency> items = await _service.GetListAsync(key, TimeSpan.FromHours(1), cancellationToken);

        List<Domain.Models.Agency> models = request.Type is not null
            ? items.Where(_ => _.cdat_agency_type_cd == request.Type).Select(ToDomainModel).ToList()
            : items.Select(ToDomainModel).ToList();

        return new Response(models);
    }

    private Domain.Models.Agency ToDomainModel(Agency item)
    {
        var model = new Domain.Models.Agency
        (
        item.agen_id.ToString(),
        item.agen_agency_nm,
        item.cdat_agency_type_cd
        );

        return model;
    }
}
