using MediatR;
using System.Text;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Features.Lookups.Statutes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IStatuteRepository _repository;

    public Handler(IStatuteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var effectiveOn = request.EffectiveOn ?? DateTime.Today;

        var items = await _repository.GetListAsync(effectiveOn, cancellationToken);

        var buffer = new StringBuilder();

        IEnumerable<Domain.Models.Statute> models = items.Select(_ => _.ToDomainModel(buffer));

        if (!string.IsNullOrWhiteSpace(request.Section))
        {
            models = models.Where(_ => _.Code == request.Section);
        }

        return new Response([.. models]);
    }
}
