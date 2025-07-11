using AutoMapper;
using TrafficCourts.TicketSearch;
using TrafficCourts.OrdsDataService.Generated.OCCAM.Client.V1;
using TrafficCourts.OrdsDataService.Occam;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Hotfix.DataMigration.Data;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace TrafficCourts.Hotfix.DataMigration.Hotfixes
{
    public class Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix : IHotfix
    {
        private readonly ITicketSearchService _ticketSearchService;
        private readonly ILogger<Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix> _logger;
        private readonly IMapper _mapper;
        private readonly IOccamDisputeRepository _occamDisputeRepository;
        private readonly HttpClient _httpClient;
        private readonly IOCCAMORDSDataServiceClientV1 _oCCAMORDSDataServiceClientV1;

        // Configuration properties for violation data API
        private readonly string _violationApiBaseUrl = "https://wsgw.dev.jag.gov.bc.ca/";
        private readonly string _violationApiUsername = "occam_dev";
        private readonly string _violationApiPassword = "4kuY15ma9!";

        private readonly Newtonsoft.Json.JsonSerializerSettings _jsonSettings = new() 
        { 
            MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        };

        public Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix(
            ITicketSearchService ticketSearchService,
            IOccamDisputeRepository occamDisputeRepository,
            IOCCAMORDSDataServiceClientV1 oCCAMORDSDataServiceClientV1,
            ILogger<Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix> logger,
            IMapper mapper,
            HttpClient httpClient)
        {
            _ticketSearchService = ticketSearchService;
            _occamDisputeRepository = occamDisputeRepository;
            _oCCAMORDSDataServiceClientV1 = oCCAMORDSDataServiceClientV1;
            _logger = logger;
            _mapper = mapper;
            _httpClient = httpClient;

            // TODO: These should be injected via configuration or environment variables
            _violationApiBaseUrl = "https://wsgw.dev.jag.gov.bc.ca";
            _violationApiUsername = "occam_dev";
            _violationApiPassword = "4kuY15ma9!";
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
                .Select(d => d.BeforeHotfixDataJson)
                .ToListAsync(cancellationToken);

            if (cachedData.Any())
            {
                _logger.LogInformation("Loading {Count} dispute records from SQLite cache: {DbPath}",
                    cachedData.Count, $"{Name}.db");
                return cachedData
                    .Where(json => !string.IsNullOrEmpty(json))
                    .Select(json => 
                    {
                        try
                        {
                            return JsonSerializer.Deserialize<OccamDispute>(json!);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to deserialize cached dispute data");
                            return null;
                        }
                    })
                    .Where(d => d != null)
                    .ToList()!;
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
                DisputeId = d.dispute_id,
                CachedAt = DateTime.UtcNow,
                BeforeHotfixDataJson = JsonSerializer.Serialize(d) // Store the entire dispute as JSON
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
        private async Task<Ticket?> LoadOrFetchRSITicketDataAsync(string ticketNumber, TimeOnly timeOfViolation, CancellationToken cancellationToken)
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
                return cachedTicket.BeforeHotfixDataJson != null
                    ? JsonSerializer.Deserialize<Ticket>(cachedTicket.BeforeHotfixDataJson)
                    : null;
            }

            // Fetch fresh data from RSI
            _logger.LogInformation("Fetching fresh RSI ticket data for {TicketNumber} from RSI service", ticketNumber);
            
            try
            {
                Ticket? rsiTicketData = await _ticketSearchService.SearchAsync(ticketNumber, timeOfViolation, cancellationToken);
                
                // Serialize the ticket data to JSON for caching
                var jsonData = rsiTicketData != null ? JsonSerializer.Serialize(rsiTicketData) : null;

                // Cache the data
                await CacheRSITicketDataAsync(context, ticketNumber, rsiTicketData, cancellationToken);

                return rsiTicketData;
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
        private async Task CacheRSITicketDataAsync(HotfixSqliteContext context, string ticketNumber, Ticket? jsonData, CancellationToken cancellationToken)
        {
            // Check if entry already exists
            var existingEntry = await context.HotfixRSITicketSearches
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

            if (existingEntry != null)
            {
                // Update existing entry
                existingEntry.BeforeHotfixDataJson = jsonData != null ? JsonSerializer.Serialize(jsonData) : null;
                existingEntry.CachedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new cache entry
                var cacheEntry = new Data.HotfixRSITicketSearch
                {
                    TicketNumber = ticketNumber,
                    BeforeHotfixDataJson = jsonData != null ? JsonSerializer.Serialize(jsonData) : null,
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

        /// <summary>
        /// Gets violation data from the external API using HTTP client with basic authentication
        /// </summary>
        /// <param name="ticketNumber">The ticket number to get violation data for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The violation data as JSON string</returns>
        private async Task<ViolationTicket?> GetViolationDataAsync(string ticketNumber, CancellationToken cancellationToken)
        {
            try
            {
                // Configure basic authentication
                // var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_violationApiUsername}:{_violationApiPassword}"));
                // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

                // Set base URL if not already set
                _httpClient.BaseAddress = new Uri(_violationApiBaseUrl);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_violationApiUsername}:{_violationApiPassword}")));

                // Build the API endpoint URL
                var endpoint = "/occam/ords/devj/occamords/occam/v1/violationTicket?disputeId=4327&noticeOfDisputeGuid=&violationTicketId=";

                _logger.LogInformation("Fetching violation data for ticket {TicketNumber} from {Endpoint}",
                    ticketNumber, _httpClient.ToString());

                // Make the HTTP request
                // var response = await _httpClient.GetAsync(endpoint, cancellationToken);

                var response = await _oCCAMORDSDataServiceClientV1.ViolationTicketGetAsync(null, 4327, cancellationToken);
                if (response == null)
                {
                    _logger.LogWarning("No response received for ticket {TicketNumber}", ticketNumber);
                    return null;
                }
                else
                {
                    _logger.LogInformation("Received response for ticket {TicketNumber}",
                        ticketNumber);
                    return response;
                }
                
                // if (response)
                // {
                //     var content = await response.Content.ReadAsStringAsync(cancellationToken);
                //     _logger.LogInformation("Successfully retrieved violation data for ticket {TicketNumber}", ticketNumber);
                //     return response.;
                // }
                // else
                // {
                //     _logger.LogWarning("Failed to retrieve violation data for ticket {TicketNumber}. Status: {StatusCode}, Reason: {ReasonPhrase}",
                //         ticketNumber, response.StatusCode, response.ReasonPhrase);
                //     return null;
                // }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching violation data for ticket {TicketNumber}", ticketNumber);
                return null;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout while fetching violation data for ticket {TicketNumber}", ticketNumber);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching violation data for ticket {TicketNumber}", ticketNumber);
                return null;
            }
        }

        /// <summary>
        /// Gets violation data with caching support
        /// </summary>
        /// <param name="ticketNumber">The ticket number to get violation data for</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The cached or freshly fetched violation data as JSON string</returns>
        private async Task<ViolationTicket?> LoadOrFetchViolationTicketDataAsync(string ticketNumber, CancellationToken cancellationToken)
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

            // Check if violation data exists in cache
            var cachedViolation = await context.HotfixViolationTickets
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

            if (cachedViolation != null && cachedViolation.BeforeHotfixDataJson != null)
            {
                _logger.LogInformation("Loading violation data for {TicketNumber} from SQLite cache: {Dispute}",
                    ticketNumber, cachedViolation.BeforeHotfixDataJson);
                return cachedViolation.BeforeHotfixDataJson != null
                    ? Newtonsoft.Json.JsonConvert.DeserializeObject<ViolationTicket>(cachedViolation.BeforeHotfixDataJson, _jsonSettings)
                    : null;
            }

            // Fetch fresh data from violation API
            _logger.LogInformation("Fetching fresh violation data for {TicketNumber} from violation API", ticketNumber);
            
            var violationData = await GetViolationDataAsync(ticketNumber, cancellationToken);

            // Cache the data (using a prefixed key to distinguish from RSI data)
            await CacheViolationDataAsync(context, ticketNumber, violationData, cancellationToken);

            return violationData;
        }

        /// <summary>
        /// Caches violation data in SQLite database
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="ticketNumber">The ticket number</param>
        /// <param name="jsonData">The violation data as JSON string</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task CacheViolationDataAsync(HotfixSqliteContext context, string ticketNumber, ViolationTicket? jsonData, CancellationToken cancellationToken)
        {
            // Check if entry already exists
            var existingEntry = await context.HotfixViolationTickets
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

            if (existingEntry != null)
            {
                // Update existing entry
                existingEntry.BeforeHotfixDataJson = jsonData != null ? JsonSerializer.Serialize(jsonData) : null;
                existingEntry.CachedAt = DateTime.UtcNow;
            }
            else
            {
                // Add new cache entry
                var cacheEntry = new Data.HotfixViolationTicket
                {
                    TicketNumber = ticketNumber,
                    BeforeHotfixDataJson = jsonData != null ? JsonSerializer.Serialize(jsonData) : null,
                    CachedAt = DateTime.UtcNow
                };

                await context.HotfixViolationTickets.AddAsync(cacheEntry, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cached violation data for {TicketNumber} in SQLite database: {DbPath}", 
                ticketNumber, $"{Name}.db");
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
                var rsiTicket = await LoadOrFetchRSITicketDataAsync(
                    "EH02000155", TimeOnly.Parse("09:12"), context.CancellationToken);
                
                _logger.LogInformation("Fetched RSI ticket data for {TicketNumber}: {Data}", 
                    ticketsToProcess[6], "rsiTicket" ?? "No data found");

                // Example: Get violation data with caching for the same ticket
                var violationTicket = await LoadOrFetchViolationTicketDataAsync("EH02000155", context.CancellationToken);

                _logger.LogInformation("Fetched violation data for ticket EH02000155: {Data}",
                    violationTicket?.Dispute?.DisputeId ?? "No data found");

                // Step 3; Get Violation ticket with Counts from OCCAM database

                // Step 3: For each ticket check if the data is missing or mismatching data in RSI database 


                // Step 4: If data is missing, Update ticket in OCCAM with RSI Counts
                if (!context.DryRun && ticketsToProcess.Any())
                {
                    _logger.LogInformation("Would update {Count} tickets in OCCAM", ticketsToProcess.Count);
                    // TODO: Add your OCCAM update logic here
                }

                return new { ticketsToProcess, rsiTicket, violationTicket };            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing hotfix: {HotfixName}", Name);
                throw;
            }
        }

        /// <summary>
        /// Merges missing count information from an RSI ticket into a violation ticket.
        /// It iterates through the counts of the violation ticket and updates any null properties
        /// with data from the corresponding count in the RSI ticket.
        /// </summary>
        /// <param name="violationTicket">The violation ticket data object, which will be modified.</param>
        /// <param name="rsiTicket">The RSI ticket data object used as the source of truth.</param>
        /// <returns>The updated violation ticket data object.</returns>
        public ViolationTicket? MergeMissingCountData(ViolationTicket? violationTicket, Ticket? rsiTicket)
        {
            if (violationTicket?.ViolationTicketCounts is null || rsiTicket?.Counts is null)
            {
                _logger.LogWarning("Violation ticket or RSI ticket data is null or contains no counts to merge.");
                return violationTicket;
            }

            var rsiCountsByNumber = rsiTicket.Counts.ToDictionary(c => c.Number);

            foreach (var violationCount in violationTicket.ViolationTicketCounts)
            {
                if (!int.TryParse(violationCount.CountNo, out var countNumber))
                {
                    continue; // Skip if count number is not a valid integer
                }

                if (rsiCountsByNumber.TryGetValue(countNumber, out var rsiCount))
                {
                    // Copy data from rsiCount to violationCount only if the target property is null
                    violationCount.DescriptionTxt ??= rsiCount.Description;
                    violationCount.ActOrRegulationNameCd ??= rsiCount.Act;
                    violationCount.IsActYn ??= rsiCount.IsAct ? "Y" : "N";
                    violationCount.IsRegulationYn ??= rsiCount.IsRegulation ? "Y" : "N";
                    violationCount.StatSectionTxt ??= rsiCount.Section;
                    violationCount.StatSubSectionTxt ??= rsiCount.Subsection;
                    violationCount.StatParagraphTxt ??= rsiCount.Paragraph;
                    violationCount.StatSubParagraphTxt ??= rsiCount.Subparagraph;
                    // violationCount.TicketedAmt ??= rsiCount.TicketedAmount;
                }
            }

            return violationTicket;
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
