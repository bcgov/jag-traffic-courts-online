using System.Text.Json;

namespace TrafficCourts.Hotfix.DataMigration.Hotfixes
{
    /// <summary>
    /// Manages hotfix execution and discovery
    /// </summary>
    public class HotfixManager : IHotfixManager
    {
        private readonly IEnumerable<IHotfix> _hotfixes;
        private readonly ILogger<HotfixManager> _logger;

        public HotfixManager(IEnumerable<IHotfix> hotfixes, ILogger<HotfixManager> logger)
        {
            _hotfixes = hotfixes;
            _logger = logger;
        }

        public IEnumerable<IHotfix> GetAllHotfixes()
        {
            return _hotfixes;
        }

        public IHotfix? GetHotfixByName(string name)
        {
            return _hotfixes.FirstOrDefault(h => h.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<dynamic> ExecuteHotfixAsync(string name, string fixVersion, bool dryRun, bool skipCache, string environment, int batchSize, int? pageNumber, int? pageSize, Dictionary<string, object> additionalData, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to execute hotfix");

            var hotfix = GetHotfixByName(name);
            if (hotfix == null)
            {
                var errorMessage = "Hotfix not found.";
                _logger.LogWarning(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Validate fix version matches the hotfix version
            if (!hotfix.FixVersion.Equals(fixVersion, StringComparison.OrdinalIgnoreCase))
            {
                var errorMessage = "Fix version mismatch for hotfix";
                _logger.LogWarning(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            try
            {
                // Log additional data if present
                if (additionalData.Any())
                {
                    _logger.LogInformation("Additional data provided");
                }

                var context = new HotfixExecutionContext
                {
                    DryRun = dryRun,
                    SkipCache = skipCache,
                    Environment = environment,
                    BatchSize = batchSize,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Request = additionalData,
                    CancellationToken = cancellationToken
                };

                var result = await hotfix.ExecuteAsync(context);
                _logger.LogInformation("Hotfix execution completed.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute hotfix");
                throw;
            }
        }
        public List<string> GetHotfixNames()
        {
            return _hotfixes.Select(h => h.Name).OrderBy(name => name).ToList();
        }
    }
}
