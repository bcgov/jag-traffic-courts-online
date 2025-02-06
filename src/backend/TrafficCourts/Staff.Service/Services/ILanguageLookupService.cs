namespace TrafficCourts.Staff.Service.Services;

public interface ILanguageLookupService
{
    Task<List<Domain.Models.Language>> GetListAsync(CancellationToken cancellationToken);
}
