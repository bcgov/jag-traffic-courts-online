using AutoMapper;
using TrafficCourts.TicketSearch;
using TrafficCourts.OrdsDataService.Occam;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Hotfix.DataMigration.Data;
using System.Text.Json;

namespace TrafficCourts.Hotfix.DataMigration.Hotfixes
{
    public class Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix : IHotfix
    {
        private readonly ITicketSearchService _ticketSearchService;
        private readonly ILogger<Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix> _logger;
        private readonly IMapper _mapper;

        private readonly IOccamDisputeRepository _occamDisputeRepository;

        public Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix(
            ITicketSearchService ticketSearchService,
            IOccamDisputeRepository occamDisputeRepository,
            ILogger<Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix> logger, IMapper mapper)
        {
            _ticketSearchService = ticketSearchService;
            _occamDisputeRepository = occamDisputeRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public string Name { get; } = "Fix_Missing_Counts_On_OCCAM_Violation_Tickets";

        public string Description { get; } = "Fixes missing counts on OCCAM violation tickets by searching and updating ticket data.";

        public string FixVersion { get; } = "2.13.3";
        
        public string Env { get; } = "dev"; // Default environment, can be overridden

        // Entity Framework SQLite caching methods
        private async Task<List<string>> LoadOrFetchDisputeDataAsync(Dictionary<string, string> disputesRequest, CancellationToken cancellationToken)
        {
            using var context = new HotfixSqliteContext(Name, Env);

            // Ensure database and tables are created
            await context.EnsureDatabaseCreatedAsync();

            var cachedData = await context.OccamDisputes    
                .Select(d => d.TicketNumber)
                .ToListAsync(cancellationToken);

            if (cachedData.Any())
            {
                _logger.LogInformation("Loading {Count} dispute records from SQLite cache: {DbPath}",
                    cachedData.Count, $"{Name}.db");
                return cachedData;
            }

            // Fetch fresh data from OCCAM
            _logger.LogInformation("Fetching fresh dispute data from OCCAM database");
            var response = await _occamDisputeRepository.GetListAsync(disputesRequest, cancellationToken);

            if (response?.Rows == null)
            {
                _logger.LogWarning("No data received from OCCAM repository");
                return new List<string>();
            }

            var disputeData = response.Rows.Select(d => d.ticket_number_txt).ToList();

            // Cache the data
            await CacheDisputeDataAsync(context, disputeData, cancellationToken);

            return disputeData;
        }

        private async Task CacheDisputeDataAsync(HotfixSqliteContext context, List<string> disputeData, CancellationToken cancellationToken)
        {
            // Clear old cache entries
            var oldEntries = await context.OccamDisputes.ToListAsync(cancellationToken);
            context.OccamDisputes.RemoveRange(oldEntries);

            // Add new cache entries
            var cacheEntries = disputeData.Select(d => new Data.OccamDispute
            {
                TicketNumber = d,
            });

            await context.OccamDisputes.AddRangeAsync(cacheEntries, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cached {Count} dispute records in SQLite database: {DbPath}", 
                disputeData.Count, $"{Name}.db");
        }

        // Implementation of IHotfix interface
        public async Task<dynamic> ExecuteAsync(HotfixExecutionContext context)
        {
            _logger.LogInformation("Starting execution of hotfix: {HotfixName} with DryRun={DryRun}, Environment={Environment}",
                Name, context.DryRun, context.Environment);

            try
            {
                var timeZone = "America/Vancouver";
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

                // Cast to the specific request type we expect
                var disputesRequest = new Dictionary<string, string>();

                DateTime startDate = DateTime.ParseExact("2025-06-25", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                startDate = TimeZoneInfo.ConvertTimeToUtc(startDate, tz);
                disputesRequest.Add("submitted_dt_ge", startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                
                DateTime endDate = DateTime.ParseExact("2025-07-09", "yyyy-MM-dd", CultureInfo.InvariantCulture);
                endDate = TimeZoneInfo.ConvertTimeToUtc(endDate, tz); 
                disputesRequest.Add("submitted_dt_lt", endDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                // Step 1: Get disputes with caching from OCCAM database for the given date range
                var ticketsToProcess = await LoadOrFetchDisputeDataAsync(disputesRequest, context.CancellationToken);

                // Step 2: Process the tickets
                _logger.LogInformation("Step 2: Get ViolationTicket from RSI");

                // Step 3: For each ticket check if the data is missing or mismatching data in RSI database 

                // foreach (var ticketNumber in ticketsToProcess) 
                // {
                //     // TODO: Add your RSI database comparison logic here
                    
                // }

                // Step 4: If data is missing, Update ticket in OCCAM with RSI Counts
                if (!context.DryRun && ticketsToProcess.Any())
                {
                    _logger.LogInformation("Would update {Count} tickets in OCCAM", ticketsToProcess.Count);
                    // TODO: Add your OCCAM update logic here
                }

                return ticketsToProcess;            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing hotfix: {HotfixName}", Name);
                throw;
            }
        }
    }

    /// <summary>
    /// SQLite DbContext specific to this hotfix - inherits base functionality
    /// Can add hotfix-specific tables and configurations here
    /// </summary>
    public class HotfixSqliteContext : BaseHotfixDbContext
    {
        public HotfixSqliteContext(string hotfixName, string env) : base(hotfixName, env)
        {
        }

        // Add hotfix-specific DbSets here if needed
        // Example: public DbSet<SpecificHotfixModel> SpecificData { get; set; } = null!;

        protected override void ConfigureHotfixSpecificModels(ModelBuilder modelBuilder)
        {
            // Add hotfix-specific model configurations here
        }
    }
}
