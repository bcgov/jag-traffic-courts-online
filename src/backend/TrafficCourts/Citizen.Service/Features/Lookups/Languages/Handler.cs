using MediatR;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;
namespace TrafficCourts.Citizen.Service.Features.Lookups.Languages;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly ICachedLookupService<Language> _service;
    public Handler(ICachedLookupService<Language> service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Api.Languages(2);

        var items = await _service.GetListAsync(key, TimeSpan.FromDays(1), cancellationToken);

        var models = items.Select(item => item.ToDomainModel())
            .ToList();

        return new Response(models);
    }
}
