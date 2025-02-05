using MediatR;
using TrafficCourts.Common;
using TrafficCourts.OrdsDataService.Justin;
namespace TrafficCourts.Citizen.Service.Features.Lookups.Languages;

public class Handler : IRequestHandler<Request, Response>
{
    private readonly ILanguageRepository _repository;
    public Handler(ILanguageRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var models = items.Select(item => item.ToDomainModel())
            .ToList();

        return new Response(models);
    }
}

