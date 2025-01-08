using MediatR;
using System.Text;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Staff.Service.Features.Lookups.Statutes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IStatuteRepository _repository;
    private readonly IFusionCache _cache;

    public Handler(IStatuteRepository repository, IFusionCache cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Api.Statutes(2);

        IEnumerable<Statute> items = await _cache.GetOrSetAsync<List<Statute>>(
            key,
            _repository.GetListAsync,
            options => options.SetDuration(TimeSpan.FromMinutes(15)),
            token: cancellationToken);

        var buffer = new StringBuilder();
        List<Domain.Models.Statute> models = items.Select(_ => _.ToDomainModel(buffer)).ToList();

        if (!string.IsNullOrWhiteSpace(request.Section))
        {
            models = models.Where(_ => _.Code == request.Section).ToList();
        }

        return new Response(models);
    }
}
