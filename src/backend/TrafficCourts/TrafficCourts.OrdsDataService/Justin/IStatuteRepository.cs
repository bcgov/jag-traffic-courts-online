
namespace TrafficCourts.OrdsDataService.Justin;

public interface IStatuteRepository
{
    /// <summary>
    /// Gets the statute by the given stat_id.
    /// </summary>
    /// <param name="stat_id"></param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
    /// <returns></returns>
    Task<Statute?> GetAsync(int stat_id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of MVA and MVR statutes
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Statute>> GetListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of MVA and MVR statutes that are effective on the given date.
    /// </summary>
    /// <param name="effectiveOn"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<Statute>> GetListAsync(DateTime effectiveOn, CancellationToken cancellationToken = default);
}
