namespace TrafficCourts.Citizen.Service.Services.Lookups;

public interface IStatuteLookupService
{
    /// <summary>
    /// Returns a specific Statute based on the provided section
    /// </summary>
    /// <param name="section"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TrafficCourts.Domain.Models.Statute?> GetBySectionAsync(string section, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a specific Statute based on the provided ID
    /// </summary>
    /// <param name="statuteId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TrafficCourts.Domain.Models.Statute?> GetByIdAsync(int statuteId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a list of statutes that are effective today.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<TrafficCourts.Domain.Models.Statute>> GetListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns a list of statutes that are effective on the specified date.
    /// </summary>
    /// <param name="effectiveOn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IList<TrafficCourts.Domain.Models.Statute>> GetListAsync(DateTime effectiveOn, CancellationToken cancellationToken);
}
