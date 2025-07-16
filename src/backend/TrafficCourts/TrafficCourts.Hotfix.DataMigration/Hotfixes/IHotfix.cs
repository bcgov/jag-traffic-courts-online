namespace TrafficCourts.Hotfix.DataMigration.Hotfixes
{
    /// <summary>
    /// Execution context passed to hotfixes containing all parameters
    /// </summary>
    public class HotfixExecutionContext
    {
        public string Step { get; set; }
        public bool DryRun { get; set; }
        public string Environment { get; set; } = "dev";
        public int BatchSize { get; set; } = 100;
        public Dictionary<string, object> Request { get; set; } = new();
        public CancellationToken CancellationToken { get; set; }
    }

    /// <summary>
    /// Interface for hotfix services that provide data migration and fix functionality
    /// </summary>
    public interface IHotfix
    {
        /// <summary>
        /// Unique identifier for the hotfix (e.g., "00001_Fix_Missing_Counts_On_OCCAM_Violation_Tickets")
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Human-readable description of what the hotfix does
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Version of the hotfix for tracking and rollback purposes
        /// </summary>
        string FixVersion { get; }

        /// <summary>
        /// Executes the hotfix logic asynchronously with execution context
        /// </summary>
        /// <param name="context">The execution context containing all parameters</param>
        /// <returns>A message describing the execution result</returns>
        Task<dynamic> ExecuteAsync(HotfixExecutionContext context);
    }
}
