namespace TrafficCourts.Hotfix.DataMigration.Hotfixes
{
    /// <summary>
    /// Service for managing and executing hotfixes
    /// </summary>
    public interface IHotfixManager
    {
        /// <summary>
        /// Gets all available hotfixes
        /// </summary>
        /// <returns>A list of available hotfixes</returns>
        IEnumerable<IHotfix> GetAllHotfixes();

        /// <summary>
        /// Gets a specific hotfix by name
        /// </summary>
        /// <param name="name">The hotfix name</param>
        /// <returns>The hotfix if found, null otherwise</returns>
        IHotfix? GetHotfixByName(string name);

        /// <summary>
        /// Executes a hotfix by name
        /// </summary>
        /// <param name="name">The hotfix name</param>
        /// <param name="fixVersion">The version of the hotfix to execute</param>
        /// <param name="dryRun">Whether to run in dry-run mode</param>
        /// <param name="environment">The target environment</param>
        /// <param name="batchSize">The batch size for processing</param>
        /// <param name="additionalData">Additional hotfix-specific parameters as key-value pairs</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the hotfix execution</returns>
        Task<dynamic> ExecuteHotfixAsync(string name, string fixVersion, bool dryRun, string environment, int batchSize, int? pageNumber, int? pageSize, Dictionary<string, object> additionalData, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the list of hotfix names
        /// </summary>
        /// <returns>A list of hotfix names</returns>
        List<string> GetHotfixNames();
    }
}
