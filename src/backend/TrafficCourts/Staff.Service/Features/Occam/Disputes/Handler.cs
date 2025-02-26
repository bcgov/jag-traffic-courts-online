using MediatR;
using System.Text;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Occam;

namespace TrafficCourts.Staff.Service.Features.Occam.Disputes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IOccamDisputeRepository _repository;

    public Handler(IOccamDisputeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(null, cancellationToken);

        var buffer = new StringBuilder();
        List<Domain.Models.DisputeCaseFileSummary> models = items.Select(_ => _.ToDomainModel()).ToList();

        if (request.Parameters != null)
        {
            // Do filtering? Why aren't we doing this DB side??!
        }

        return new Response(models);
    }
}
