namespace TrafficCourts.Staff.Service.Services;

public interface ICountryLookupService
{
    Task<Domain.Models.Country?> GetByIdAsync(int ctryId, CancellationToken cancellationToken);
    Task<List<Domain.Models.Country>> GetListAsync(CancellationToken cancellationToken);
}
