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
        var items = await _repository.GetListAsync(cancellationToken);

        var buffer = new StringBuilder();
        List<Domain.Models.Statute> models = items.Select(_ => _.ToDomainModel(buffer)).ToList();

        if (!string.IsNullOrWhiteSpace(request.Section))
        {
            models = models.Where(_ => _.Code == request.Section).ToList();
        }

        return new Response(models);
    }
}
