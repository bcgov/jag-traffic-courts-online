using TrafficCourts.OrdsDataService.Justin;

namespace TrafficCourts.Staff.Service.Services;

public class LanguageLookupService : ILanguageLookupService
{
    private readonly ILanguageRepository _repository;

    public LanguageLookupService(ILanguageRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<List<Domain.Models.Language>> GetListAsync(CancellationToken cancellationToken)
    {
        var items = await _repository.GetListAsync(cancellationToken);

        var models = items.Select(_ => new Domain.Models.Language
        (
            _.cdln_language_cd,
            _.cdln_language_dsc
        )).ToList();

        return models;
    }
}
