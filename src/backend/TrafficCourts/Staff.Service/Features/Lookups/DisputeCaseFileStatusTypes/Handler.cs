using MediatR;
using TrafficCourts.Domain.Models;
using TrafficCourts.OrdsDataService.Tco;

namespace TrafficCourts.Staff.Service.Features.Lookups.DisputeCaseFileStatusTypes;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IDisputeStatusTypeRepository _repository;

    public Handler(IDisputeStatusTypeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var models = items.Select(item => new DisputeCaseFileStatus
        {
            Code = item.dispute_status_type_cd,
            Description = item.dispute_status_type_dsc
        }).ToList();

        return new Response(models);
    }
}
