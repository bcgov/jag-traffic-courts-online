using MediatR;
using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Features.Lookups.Provinces;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly IProvinceRepository _repository;

    public Handler(IProvinceRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var models = items.Select(_ => new Domain.Models.Province
        (
            _.ctry_id.ToString(),
            _.prov_seq_no.ToString(),
            _.prov_nm,
            _.prov_abbreviation_cd
        )).ToList();

        return new Response(models);
    }
}
