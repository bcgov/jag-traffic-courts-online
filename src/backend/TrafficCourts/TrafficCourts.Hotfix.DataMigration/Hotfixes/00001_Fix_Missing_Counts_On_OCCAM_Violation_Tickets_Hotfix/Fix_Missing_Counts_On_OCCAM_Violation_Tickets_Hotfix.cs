using AutoMapper;
using TrafficCourts.TicketSearch;
using TrafficCourts.OrdsDataService;
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
        private async Task<List<OccamDispute>> LoadOrFetchDisputeDataAsync(Dictionary<string, string> disputesRequest, CancellationToken cancellationToken)
        {
            using var context = new HotfixSqliteContext(Name, Env);

            try
            {
                // Test if database is accessible for writing
                await context.Database.OpenConnectionAsync(cancellationToken);
                await context.Database.CloseConnectionAsync();
                
                // Ensure database and tables are created
                await context.EnsureDatabaseCreatedAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("database is locked"))
            {
                _logger.LogError("SQLite database is locked. Please close any database viewers/plugins in VS Code and try again.");
                _logger.LogError("Database path: {DbPath}", context.GetDatabasePath());
                throw new InvalidOperationException(
                    "Database is locked by another process (likely VS Code SQLite plugin). " +
                    "Please close the database connection in VS Code and retry.", ex);
            }

            var cachedData = await context.HotfixOccamDisputes
                .Select(d => d.DataJson)
                .ToListAsync(cancellationToken);

            if (cachedData.Any())
            {
                _logger.LogInformation("Loading {Count} dispute records from SQLite cache: {DbPath}",
                    cachedData.Count, $"{Name}.db");
                return cachedData
                    .Select(json => JsonSerializer.Deserialize<OccamDispute>(json)!)
                    .Where(d => d != null)
                    .ToList();
            }

            // Fetch fresh data from OCCAM
            _logger.LogInformation("Fetching fresh dispute data from OCCAM database");
            var response = await _occamDisputeRepository.GetListAsync(disputesRequest, cancellationToken);

            if (response?.Rows == null)
            {
                _logger.LogWarning("No data received from OCCAM repository");
                return new List<OccamDispute>();
            }

            var disputeData = response.Rows;

            // Cache the data
            await CacheDisputeDataAsync(context, disputeData, cancellationToken);

            // Dispute dispute = await _oracleDataApi.GetDisputeAsync(options.DisputeId, options.Assign, cancellationToken);

            return disputeData;
        }

        private async Task CacheDisputeDataAsync(HotfixSqliteContext context, List<OccamDispute> disputeData, CancellationToken cancellationToken)
        {
            // Clear old cache entries
            var oldEntries = await context.HotfixOccamDisputes.ToListAsync(cancellationToken);
            context.HotfixOccamDisputes.RemoveRange(oldEntries);

            // Add new cache entries
            var cacheEntries = disputeData.Select(d => new Data.HotfixOccamDispute
            {
                TicketNumber = d.ticket_number_txt,
                CachedAt = DateTime.UtcNow,
                DataJson = JsonSerializer.Serialize(d) // Store the entire dispute as JSON
            });

            await context.HotfixOccamDisputes.AddRangeAsync(cacheEntries, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cached {Count} dispute records in SQLite database: {DbPath}", 
                disputeData.Count, $"{Name}.db");
        }

        /// <summary>
        /// Loads RSI ticket data from cache or fetches from RSI service and caches the result
        /// </summary>
        /// <param name="ticketNumber">The ticket number to search for</param>
        /// <param name="timeOfViolation">Time of the violation</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The cached or freshly fetched ticket data as JSON string</returns>
        private async Task<string?> LoadOrFetchRSITicketDataAsync(string ticketNumber, TimeOnly timeOfViolation, CancellationToken cancellationToken)
        {
            using var context = new HotfixSqliteContext(Name, Env);

            try
            {
                // Test if database is accessible for writing
                await context.Database.OpenConnectionAsync(cancellationToken);
                await context.Database.CloseConnectionAsync();
                
                // Ensure database and tables are created
                await context.EnsureDatabaseCreatedAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("database is locked"))
            {
                _logger.LogError("SQLite database is locked. Please close any database viewers/plugins in VS Code and try again.");
                _logger.LogError("Database path: {DbPath}", context.GetDatabasePath());
                throw new InvalidOperationException(
                    "Database is locked by another process (likely VS Code SQLite plugin). " +
                    "Please close the database connection in VS Code and retry.", ex);
            }

            // Check if ticket data exists in cache
            var cachedTicket = await context.HotfixRSITicketSearches
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

            if (cachedTicket != null)
            {
                _logger.LogInformation("Loading RSI ticket data for {TicketNumber} from SQLite cache: {DbPath}",
                    ticketNumber, $"{Name}.db");
                return cachedTicket.DataJson;
            }

            // Fetch fresh data from RSI
            _logger.LogInformation("Fetching fresh RSI ticket data for {TicketNumber} from RSI service", ticketNumber);
            
            try
            {
                var rsiTicketData = await _ticketSearchService.SearchAsync(ticketNumber, timeOfViolation, cancellationToken);
                
                // Serialize the ticket data to JSON for caching
                var jsonData = rsiTicketData != null ? JsonSerializer.Serialize(rsiTicketData) : null;

                // Cache the data
                await CacheRSITicketDataAsync(context, ticketNumber, jsonData, cancellationToken);

                return jsonData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch RSI ticket data for {TicketNumber}", ticketNumber);
                
                // Cache null result to avoid repeated failed requests
                await CacheRSITicketDataAsync(context, ticketNumber, null, cancellationToken);
                return null;
            }
        }

        /// <summary>
        /// Caches RSI ticket search data in SQLite database
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="ticketNumber">The ticket number</param>
        /// <param name="jsonData">The ticket data as JSON string</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task CacheRSITicketDataAsync(HotfixSqliteContext context, string ticketNumber, string? jsonData, CancellationToken cancellationToken)
        {
            // Check if entry already exists
            var existingEntry = await context.HotfixRSITicketSearches
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

            if (existingEntry != null)
            {
                // Update existing entry
                existingEntry.DataJson = jsonData;
                existingEntry.CachedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new cache entry
                var cacheEntry = new Data.HotfixRSITicketSearch
                {
                    TicketNumber = ticketNumber,
                    DataJson = jsonData,
                    CachedAt = DateTime.UtcNow
                };

                await context.HotfixRSITicketSearches.AddAsync(cacheEntry, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cached RSI ticket data for {TicketNumber} in SQLite database: {DbPath}", 
                ticketNumber, $"{Name}.db");
        }

        /// <summary>
        /// Initializes the database and validates that all required tables exist
        /// </summary>
        private async Task InitializeAndValidateDatabaseAsync(CancellationToken cancellationToken)
        {
            using var context = new HotfixSqliteContext(Name, Env);
            
            try
            {
                // Ensure database is created with proper schema
                await context.EnsureDatabaseCreatedAsync();
                
                // Test that both tables exist by performing simple queries
                var disputeCount = await context.HotfixOccamDisputes.CountAsync(cancellationToken);
                var ticketSearchCount = await context.HotfixRSITicketSearches.CountAsync(cancellationToken);
                
                _logger.LogInformation("Database initialized successfully. OccamDisputes: {DisputeCount}, RSITicketSearches: {TicketSearchCount}", 
                    disputeCount, ticketSearchCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize database. Attempting to recreate database schema.");
            }
        }

        // Implementation of IHotfix interface
        public async Task<dynamic> ExecuteAsync(HotfixExecutionContext context)
        {
            _logger.LogInformation("Starting execution of hotfix: {HotfixName} with DryRun={DryRun}, Environment={Environment}",
                Name, context.DryRun, context.Environment);

            try
            {
                // Initialize and validate database schema first
                await InitializeAndValidateDatabaseAsync(context.CancellationToken);
                
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
                
                // foreach (var ticketNumber in ticketsToProcess) 
                // {
                //     // TODO: Add your RSI database comparison logic here

                // }
                if (!ticketsToProcess.Any() || ticketsToProcess[6] == null)
                {
                    _logger.LogInformation("No tickets found to process in OCCAM database for the given date range.");
                    return new { ticketsToProcess = new List<string>() };
                }

                var ticketNumber = ticketsToProcess[6]; // Example: Get the 7th ticket number for processing

                if (ticketNumber?.ticket_number_txt == null)
                {
                    _logger.LogInformation("Ticket number is null, skipping RSI data fetch.");
                    return new { ticketsToProcess };
                }

                // Example: Get RSI ticket data with caching for one ticket
                var rsiTicketDataJson = await LoadOrFetchRSITicketDataAsync(
                    "EH02000155", TimeOnly.Parse("09:12"), context.CancellationToken);
                
                // Deserialize if needed for processing
                object? rsiTicketData = null;
                if (!string.IsNullOrEmpty(rsiTicketDataJson))
                {
                    rsiTicketData = JsonSerializer.Deserialize<object>(rsiTicketDataJson);
                }
                _logger.LogInformation("Fetched RSI ticket data for {TicketNumber}: {Data}", 
                    ticketsToProcess[6], rsiTicketDataJson ?? "No data found");

                // Step 3; Get Violation ticket with Counts from OCCAM database

                // Step 3: For each ticket check if the data is missing or mismatching data in RSI database 


                // Step 4: If data is missing, Update ticket in OCCAM with RSI Counts
                if (!context.DryRun && ticketsToProcess.Any())
                {
                    _logger.LogInformation("Would update {Count} tickets in OCCAM", ticketsToProcess.Count);
                    // TODO: Add your OCCAM update logic here
                }

                return new { ticketsToProcess, rsiTicketData };            
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
    }
}
