using MediatR;
using System.Text;
using TrafficCourts.OrdsDataService.Justin;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Staff.Service.Features.Lookups.Statutes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IStatuteRepository _repository;
    private readonly IFusionCache _cache;
    private readonly ILogger<Endpoint> _logger;

    public Handler(IStatuteRepository repository, IFusionCache cache, ILogger<Endpoint> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger;
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var key = Caching.Cache.Api.Statutes(2);

        var statutes = await _cache.GetOrSetAsync<List<Domain.Models.Statute>>(
            key,
            ct => GetItems(ct),
            options => options.SetDuration(TimeSpan.FromMinutes(10)),
            token: cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Section))
        {
            statutes = statutes.Where(_ => _.Code == request.Section).ToList();
        }

        return new Response(statutes);
    }

    private async Task<List<Domain.Models.Statute>> GetItems(CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        StringBuilder buffer = new StringBuilder();

        List<Domain.Models.Statute> models = items.Select(_ => new Domain.Models.Statute
        (
            _.stat_id.ToString(),
            _.act_cd,
            _.stat_section_txt,
            _.stat_sub_section_txt ?? string.Empty,
            _.stat_paragraph_txt ?? string.Empty,
            _.stat_sub_paragraph_txt ?? string.Empty,
            GetCode(_, buffer),
            _.stat_short_description_txt ?? _.stat_description_txt,
            _.stat_description_txt
        )).ToList();

        return models;
    }

    private static string GetCode(Statute statute, StringBuilder buffer)
    {
        buffer.Append(statute.stat_section_txt);

        if (!string.IsNullOrWhiteSpace(statute.stat_sub_section_txt))
        {

            buffer.Append('(');
            buffer.Append(statute.stat_sub_section_txt);
            buffer.Append(')');
        }

        if (!string.IsNullOrWhiteSpace(statute.stat_paragraph_txt))
        {

            buffer.Append('(');
            buffer.Append(statute.stat_paragraph_txt);
            buffer.Append(')');
        }

        var code = buffer.ToString();
        buffer.Clear();
        return code;
    }
}
