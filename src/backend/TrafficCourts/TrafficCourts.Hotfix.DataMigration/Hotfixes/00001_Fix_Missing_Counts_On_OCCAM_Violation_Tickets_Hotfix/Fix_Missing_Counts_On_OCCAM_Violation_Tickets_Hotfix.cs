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
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Collections;

namespace TrafficCourts.Hotfix.DataMigration.Hotfixes
{
    public class Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix : IHotfix
    {
        private readonly ITicketSearchService _ticketSearchService;
        private readonly ILogger<Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix> _logger;
        private readonly IOccamDisputeRepository _occamDisputeRepository;
        private readonly HttpClient _httpClient;
        private readonly IOCCAMORDSDataServiceClientV1 _occamORDSDataServiceClientV1;
        private readonly IConfiguration _configuration;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,

        };

        public Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix(
            ITicketSearchService ticketSearchService,
            IOccamDisputeRepository occamDisputeRepository,
            IOCCAMORDSDataServiceClientV1 occamORDSDataServiceClientV1,
            ILogger<Fix_Missing_Counts_On_OCCAM_Violation_Tickets_Hotfix> logger,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _ticketSearchService = ticketSearchService;
            _occamDisputeRepository = occamDisputeRepository;
            _occamORDSDataServiceClientV1 = occamORDSDataServiceClientV1;
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
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


            // Fetch fresh data from OCCAM
            _logger.LogInformation("Fetching fresh dispute data from OCCAM database");
            var response = await _occamDisputeRepository.GetListAsync(disputesRequest, cancellationToken);

            if (response?.Rows == null)
            {
                _logger.LogWarning("No data received from OCCAM repository");
                return new List<OccamDispute>();
            }

            var disputeData = response.Rows.Select(d =>
            {
                // Redact PII
                if (d.disputant_given_1_nm != null) d.disputant_given_1_nm = "<Redacted>";
                if (d.disputant_given_2_nm != null) d.disputant_given_2_nm = "<Redacted>";
                if (d.disputant_given_3_nm != null) d.disputant_given_3_nm = "<Redacted>";
                if (d.disputant_surname_nm != null) d.disputant_surname_nm = "<Redacted>";
                if (d.email_address_txt != null) d.email_address_txt = "<Redacted>";
                
                return d;
            }).ToList();

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
        private async Task<Ticket?> LoadOrFetchRSITicketDataAsync(HotfixExecutionContext requestContext, string ticketNumber, TimeOnly timeOfViolation, CancellationToken cancellationToken)
        {
            await Task.Delay(1500, cancellationToken); // Wait for 500 milliseconds

            using var context = new HotfixSqliteContext(Name, Env);

            try
            {
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

            if (!requestContext.SkipCache)
            {
                // Check if ticket data exists in cache
                var cachedTicket = await context.HotfixRSITicketSearches
                    .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber && t.BeforeHotfixDataJson != null, cancellationToken);

                if (cachedTicket != null)
                {
                    _logger.LogInformation("Loading RSI ticket data for {TicketNumber} from SQLite cache: {DbPath}",
                        ticketNumber, $"{Name}.db");
                    return cachedTicket.BeforeHotfixDataJson != null
                        ? JsonSerializer.Deserialize<Ticket>(cachedTicket.BeforeHotfixDataJson)
                        : null;
                }
            }

            // Fetch fresh data from RSI
            _logger.LogInformation("Fetching fresh RSI ticket data for {TicketNumber} from RSI service", ticketNumber);

            try
            {
                Ticket? rsiTicketData = await _ticketSearchService.SearchAsync(ticketNumber, timeOfViolation, cancellationToken);

                if (rsiTicketData?.Surname != null) rsiTicketData.Surname = "<Redacted>";
                if (rsiTicketData?.FirstGivenName != null) rsiTicketData.FirstGivenName = "<Redacted>";
                if (rsiTicketData?.SecondGivenName != null) rsiTicketData.SecondGivenName = "<Redacted>";
                
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
        private async Task<ViolationTicket?> GetViolationDataAsync(long dispute_id, string ticketNumber, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching violation data for ticket {TicketNumber}: {DisputeId} from OCCAM API", ticketNumber, dispute_id);


                var violationTicket = await _occamORDSDataServiceClientV1.ViolationTicketGetAsync(null, dispute_id, cancellationToken);
                if (violationTicket == null)
                {
                    _logger.LogWarning("No response received for ticket {TicketNumber}", ticketNumber);
                    return null;
                }
                else
                {
                    _logger.LogInformation("Received response for ticket {TicketNumber}", ticketNumber);

                    // Redact PII fields in the response
                    if (violationTicket.AddressCityTxt != null) violationTicket.AddressCityTxt = "<Redacted>"; 
                    if (violationTicket.AddressCountryTxt != null) violationTicket.AddressCountryTxt = "<Redacted>";
                    if (violationTicket.AddressPostalCodeTxt != null) violationTicket.AddressPostalCodeTxt = "<Redacted>";
                    if (violationTicket.AddressTxt != null) violationTicket.AddressTxt = "<Redacted>";
                    if (violationTicket.CourtLocationTxt != null) violationTicket.CourtLocationTxt = "<Redacted>";
                    if (violationTicket.DisputantBirthDt != null) violationTicket.DisputantBirthDt = "<Redacted>";
                    if (violationTicket.DisputantDrvLicNumberTxt != null) violationTicket.DisputantDrvLicNumberTxt = "<Redacted>";
                    if (violationTicket.DisputantGivenNamesTxt != null) violationTicket.DisputantGivenNamesTxt = "<Redacted>";
                    if (violationTicket.DisputantOrganizationNmTxt != null) violationTicket.DisputantOrganizationNmTxt = "<Redacted>";
                    if (violationTicket.DisputantSurnameTxt != null) violationTicket.DisputantSurnameTxt = "<Redacted>";
                    if (violationTicket.DrvLicExpiryYearNo != null) violationTicket.DrvLicExpiryYearNo = "<Redacted>";
                    if (violationTicket.DrvLicIssuedCountryTxt != null) violationTicket.DrvLicIssuedCountryTxt = "<Redacted>";
                    if (violationTicket.DrvLicIssuedProvinceTxt != null) violationTicket.DrvLicIssuedProvinceTxt = "<Redacted>";
                    if (violationTicket.DrvLicIssuedYearNo != null) violationTicket.DrvLicIssuedYearNo = "<Redacted>";
                    if (violationTicket.Dispute?.AddressCityCtryId != null) violationTicket.Dispute.AddressCityCtryId = "<Redacted>";
                    if (violationTicket.Dispute?.AddressCitySeqNo != null) violationTicket.Dispute.AddressCitySeqNo = "<Redacted>";
                    if (violationTicket.Dispute?.AddressCtryId != null) violationTicket.Dispute.AddressCtryId = "<Redacted>";
                    if (violationTicket.Dispute?.AddressIntlCityTxt != null) violationTicket.Dispute.AddressIntlCityTxt = "<Redacted>";
                    if (violationTicket.Dispute?.AddressIntlProvTxt != null) violationTicket.Dispute.AddressIntlProvTxt = "<Redacted>";
                    if (violationTicket.Dispute?.AddressLine1Txt != null) violationTicket.Dispute.AddressLine1Txt = "<Redacted>";
                    if (violationTicket.Dispute?.AddressLine2Txt != null) violationTicket.Dispute.AddressLine2Txt = "<Redacted>";
                    if (violationTicket.Dispute?.AddressLine3Txt != null) violationTicket.Dispute.AddressLine3Txt = "<Redacted>";
                    if (violationTicket.Dispute?.AddressProvCtryId != null) violationTicket.Dispute.AddressProvCtryId = "<Redacted>";
                    if (violationTicket.Dispute?.AddressProvSeqNo != null) violationTicket.Dispute.AddressProvSeqNo = "<Redacted>";
                    if (violationTicket.Dispute?.ContactGiven1Nm != null) violationTicket.Dispute.ContactGiven1Nm = "<Redacted>";
                    if (violationTicket.Dispute?.ContactGiven2Nm != null) violationTicket.Dispute.ContactGiven2Nm = "<Redacted>";
                    if (violationTicket.Dispute?.ContactGiven3Nm != null) violationTicket.Dispute.ContactGiven3Nm = "<Redacted>";
                    if (violationTicket.Dispute?.ContactLawFirmNm != null) violationTicket.Dispute.ContactLawFirmNm = "<Redacted>";
                    if (violationTicket.Dispute?.ContactSurnameNm != null) violationTicket.Dispute.ContactSurnameNm = "<Redacted>";
                    if (violationTicket.Dispute?.DisputantBirthDt != null) violationTicket.Dispute.DisputantBirthDt = "<Redacted>";
                    if (violationTicket.Dispute?.DisputantDrvLicNumberTxt != null) violationTicket.Dispute.DisputantDrvLicNumberTxt = "<Redacted>";
                    if (violationTicket.Dispute?.DisputantGiven1Nm != null) violationTicket.Dispute.DisputantGiven1Nm = "<Redacted>";
                    if (violationTicket.Dispute?.DisputantGiven2Nm != null) violationTicket.Dispute.DisputantGiven2Nm = "<Redacted>";
                    if (violationTicket.Dispute?.DisputantGiven3Nm != null) violationTicket.Dispute.DisputantGiven3Nm = "<Redacted>";
                    if (violationTicket.Dispute?.DisputantOrganizationNm != null) violationTicket.Dispute.DisputantOrganizationNm = "<Redacted>";
                    if (violationTicket.Dispute?.DisputantSurnameNm != null) violationTicket.Dispute.DisputantSurnameNm = "<Redacted>";
                    if (violationTicket.Dispute?.DrvLicIssuedCtryId != null) violationTicket.Dispute.DrvLicIssuedCtryId = "<Redacted>";
                    if (violationTicket.Dispute?.DrvLicIssuedIntlProvTxt != null) violationTicket.Dispute.DrvLicIssuedIntlProvTxt = "<Redacted>";
                    if (violationTicket.Dispute?.DrvLicIssuedProvSeqNo != null) violationTicket.Dispute.DrvLicIssuedProvSeqNo = "<Redacted>";
                    if (violationTicket.Dispute?.EmailAddressTxt != null) violationTicket.Dispute.EmailAddressTxt = "<Redacted>";
                    if (violationTicket.Dispute?.HomePhoneNumberTxt != null) violationTicket.Dispute.HomePhoneNumberTxt = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrCityCtryId != null) violationTicket.Dispute.LawFirmAddrCityCtryId = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrCitySeqNo != null) violationTicket.Dispute.LawFirmAddrCitySeqNo = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrCtryId != null) violationTicket.Dispute.LawFirmAddrCtryId = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrIntlCityTxt != null) violationTicket.Dispute.LawFirmAddrIntlCityTxt = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrIntlProvTxt != null) violationTicket.Dispute.LawFirmAddrIntlProvTxt = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrLine1Txt != null) violationTicket.Dispute.LawFirmAddrLine1Txt = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrLine2Txt != null) violationTicket.Dispute.LawFirmAddrLine2Txt = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrLine3Txt != null) violationTicket.Dispute.LawFirmAddrLine3Txt = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrPostalCodeTxt != null) violationTicket.Dispute.LawFirmAddrPostalCodeTxt = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrProvCtryId != null) violationTicket.Dispute.LawFirmAddrProvCtryId = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmAddrProvSeqNo != null) violationTicket.Dispute.LawFirmAddrProvSeqNo = "<Redacted>";
                    if (violationTicket.Dispute?.LawFirmNm != null) violationTicket.Dispute.LawFirmNm = "<Redacted>";
                    if (violationTicket.Dispute?.LawyerEmailAddressTxt != null) violationTicket.Dispute.LawyerEmailAddressTxt = "<Redacted>";
                    if (violationTicket.Dispute?.LawyerGiven1Nm != null) violationTicket.Dispute.LawyerGiven1Nm = "<Redacted>";
                    if (violationTicket.Dispute?.LawyerGiven2Nm != null) violationTicket.Dispute.LawyerGiven2Nm = "<Redacted>";
                    if (violationTicket.Dispute?.LawyerGiven3Nm != null) violationTicket.Dispute.LawyerGiven3Nm = "<Redacted>";
                    if (violationTicket.Dispute?.LawyerPhoneNumberTxt != null) violationTicket.Dispute.LawyerPhoneNumberTxt = "<Redacted>";
                    if (violationTicket.Dispute?.LawyerSurnameNm != null) violationTicket.Dispute.LawyerSurnameNm = "<Redacted>";
                    if (violationTicket.Dispute?.PostalCodeTxt != null) violationTicket.Dispute.PostalCodeTxt = "<Redacted>";

                    // Clean the response to remove additionalProperties
                    var jsonString = JsonSerializer.Serialize(violationTicket, _jsonOptions);
                    var cleanedResponse = JsonSerializer.Deserialize<ViolationTicket>(jsonString, _jsonOptions);

                    return cleanedResponse;
                }
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
        private async Task<ViolationTicket?> LoadOrFetchViolationTicketDataAsync(HotfixExecutionContext requestContext, long dispute_id, string ticketNumber, CancellationToken cancellationToken)
        {
            using var context = new HotfixSqliteContext(Name, Env);

            try
            {
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

            if (!requestContext.SkipCache)
            {
                // Check if violation data exists in cache
                var cachedViolation = await context.HotfixViolationTickets
                    .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

                if (cachedViolation != null && cachedViolation.BeforeHotfixDataJson != null)
                {
                    _logger.LogInformation("Loading violation data for {TicketNumber} from SQLite cache",
                        ticketNumber);
                    return cachedViolation.BeforeHotfixDataJson != null
                        ? Newtonsoft.Json.JsonConvert.DeserializeObject<ViolationTicket>(cachedViolation.BeforeHotfixDataJson, new Newtonsoft.Json.JsonSerializerSettings
                        {
                            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                            DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore,
                            ContractResolver = new TolerantContractResolver()
                        })
                        : null;
                }
            }

            // Fetch fresh data from violation API
                _logger.LogInformation("Fetching fresh violation data for {TicketNumber} from violation API", ticketNumber);

            var violationData = await GetViolationDataAsync(dispute_id, ticketNumber, cancellationToken);

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

        /// <summary>
        /// Updates a violation ticket in OCCAM database and caches the update data
        /// </summary>
        /// <param name="correctedViolationTicket">The merged violation ticket with updated count data</param>
        /// <param name="originalViolationTicket">The original violation ticket before updates</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated violation ticket response from OCCAM API</returns>
        private async Task<ViolationTicket?> UpdateViolationTicketAsync(ViolationTicket correctedViolationTicket, ViolationTicket originalViolationTicket, CancellationToken cancellationToken)
        {
            using var context = new HotfixSqliteContext(Name, Env);

            try
            {
                // Ensure database is accessible
                await context.EnsureDatabaseCreatedAsync();

                _logger.LogInformation("Updating violation ticket {TicketNumber} in OCCAM database", correctedViolationTicket.TicketNumberTxt);

                _logger.LogInformation("Corrected ViolationTicket JSON: {Json}", JsonSerializer.Serialize(correctedViolationTicket, _jsonOptions));
                // Call OCCAM API to update the violation ticket
                var response = await _occamORDSDataServiceClientV1.UpdateViolationTicketAsync(correctedViolationTicket, cancellationToken);

                if (response.Status == "1")
                {
                    _logger.LogInformation("Successfully updated violation ticket {TicketNumber} in OCCAM", correctedViolationTicket.TicketNumberTxt);
                    _logger.LogDebug("OCCAM update response: {Response}", JsonSerializer.Serialize(response, _jsonOptions));

                    // Cache the update data in SQLite
                    await CacheViolationTicketUpdateAsync(context, correctedViolationTicket.TicketNumberTxt!,
                        originalViolationTicket, correctedViolationTicket, cancellationToken);

                    return correctedViolationTicket;
                }
                else
                {
                    _logger.LogError("Failed to update violation ticket {TicketNumber} in OCCAM. No response received",
                        correctedViolationTicket.TicketNumberTxt);
                    _logger.LogError("Failed: OCCAM update response: {Response}", JsonSerializer.Serialize(response, _jsonOptions));

                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating violation ticket {TicketNumber} in OCCAM", correctedViolationTicket.TicketNumberTxt);
                return null;
            }
        }

        /// <summary>
        /// Caches violation ticket update data in SQLite database
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="ticketNumber">The ticket number</param>
        /// <param name="beforeUpdateData">The violation ticket data before update</param>
        /// <param name="correctedViolationTicketUpdateData">The violation ticket data after update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task CacheViolationTicketUpdateAsync(HotfixSqliteContext context, string ticketNumber,
            ViolationTicket beforeUpdateData, ViolationTicket correctedViolationTicketUpdateData, CancellationToken cancellationToken)
        {
            // Find existing violation ticket entry
            var existingEntry = await context.HotfixViolationTickets
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

            if (existingEntry != null)
            {
                // Update existing entry with hotfix update data
                existingEntry.HotfixUpdateDataJson = JsonSerializer.Serialize(correctedViolationTicketUpdateData);
                existingEntry.IsHotfixApplied = true;
                existingEntry.CachedAt = DateTime.UtcNow;

                // Ensure we have the before data
                if (string.IsNullOrEmpty(existingEntry.BeforeHotfixDataJson))
                {
                    existingEntry.BeforeHotfixDataJson = JsonSerializer.Serialize(beforeUpdateData);
                }
            }
            else
            {
                // Create new entry
                var cacheEntry = new Data.HotfixViolationTicket
                {
                    TicketNumber = ticketNumber,
                    BeforeHotfixDataJson = JsonSerializer.Serialize(beforeUpdateData),
                    HotfixUpdateDataJson = JsonSerializer.Serialize(correctedViolationTicketUpdateData),
                    IsHotfixApplied = true,
                    CachedAt = DateTime.UtcNow
                };

                await context.HotfixViolationTickets.AddAsync(cacheEntry, cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cached violation ticket update data for {TicketNumber} in SQLite database: {DbPath}",
                ticketNumber, $"{Name}.db");
        }

        /// <summary>
        /// Performs data integrity check after updating violation ticket
        /// Fetches fresh data from OCCAM and compares with before/after update data
        /// </summary>
        /// <param name="ticketNumber">The ticket number to check</param>
        /// <param name="beforeUpdateData">The violation ticket data before update</param>
        /// <param name="expectedUpdateData">The expected violation ticket data after update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>ValidationResult with detailed validation information</returns>
        private async Task<ValidationResult> PerformDataIntegrityCheckAsync(long dispute_id, string ticketNumber, ViolationTicket beforeUpdateData,
            ViolationTicket expectedUpdateData, CancellationToken cancellationToken)
        {
            using var context = new HotfixSqliteContext(Name, Env);

            try
            {
                await context.EnsureDatabaseCreatedAsync();

                _logger.LogInformation("Performing data integrity check for ticket {TicketNumber}", ticketNumber);

                // Fetch fresh data from OCCAM to verify the update
                var freshViolationData = await GetViolationDataAsync(dispute_id, ticketNumber, cancellationToken);

                if (freshViolationData == null)
                {
                    var failureReason = "Failed to fetch fresh violation data for integrity check";
                    _logger.LogError("{FailureReason}. Ticket: {TicketNumber}", failureReason, ticketNumber);

                    var failureResult = new ValidationResult
                    {
                        IsValid = false,
                        FailureReason = failureReason
                    };

                    await UpdateIntegrityCheckResultAsync(context, ticketNumber, null, false, failureReason, cancellationToken);
                    return failureResult;
                }

                // Compare the fresh data with expected data
                var validationResult = ValidateCountUpdates(beforeUpdateData, expectedUpdateData, freshViolationData);

                // Update the cache with integrity check results
                await UpdateIntegrityCheckResultAsync(context, ticketNumber, freshViolationData, validationResult.IsValid, validationResult.FailureReason, cancellationToken);

                if (validationResult.IsValid)
                {
                    _logger.LogInformation("Data integrity check PASSED for ticket {TicketNumber}", ticketNumber);
                }
                else
                {
                    _logger.LogWarning("Data integrity check FAILED for ticket {TicketNumber}. Reason: {FailureReason}", ticketNumber, validationResult.FailureReason);

                    // Log all validation errors in detail
                    if (validationResult.ValidationErrors.Any())
                    {
                        _logger.LogError("Data integrity validation errors for ticket {TicketNumber}:", ticketNumber);
                        for (int i = 0; i < validationResult.ValidationErrors.Count; i++)
                        {
                            _logger.LogError("  Error {Index}: {ValidationError}", i + 1, validationResult.ValidationErrors[i]);
                        }
                    }
                }

                return validationResult;
            }
            catch (Exception ex)
            {
                var failureReason = $"Error performing data integrity check: {ex.Message}";
                _logger.LogError(ex, "Error performing data integrity check for ticket {TicketNumber}", ticketNumber);

                var exceptionResult = new ValidationResult
                {
                    IsValid = false,
                    FailureReason = failureReason
                };

                await UpdateIntegrityCheckResultAsync(context, ticketNumber, null, false, failureReason, cancellationToken);
                return exceptionResult;
            }
        }

        /// <summary>
        /// Validates that only the expected count fields were updated and NO other properties changed
        /// Performs comprehensive comparison of all ViolationTicket properties
        /// </summary>
        /// <param name="beforeData">Data before update</param>
        /// <param name="expectedData">Expected data after update</param>
        /// <param name="actualData">Actual data fetched from database</param>
        /// <returns>ValidationResult with detailed failure information</returns>
        private ValidationResult ValidateCountUpdates(ViolationTicket beforeData, ViolationTicket expectedData, ViolationTicket actualData)
        {
            var result = new ValidationResult { IsValid = true };

            _logger.LogInformation("Starting comprehensive data integrity validation");

            // Step 1: Validate all top-level ViolationTicket properties remain unchanged
            var propertyValidationResult = ValidateViolationTicketPropertiesUnchanged(beforeData, actualData);
            if (!propertyValidationResult.IsValid)
            {
                result.IsValid = false;
                result.ValidationErrors.AddRange(propertyValidationResult.ValidationErrors);
                _logger.LogError("Top-level ViolationTicket properties validation failed");
            }

            // Step 2: Validate ViolationTicketCounts collection
            var collectionValidationResult = ValidateViolationTicketCountsCollection(beforeData, expectedData, actualData);
            if (!collectionValidationResult.IsValid)
            {
                result.IsValid = false;
                result.ValidationErrors.AddRange(collectionValidationResult.ValidationErrors);
                _logger.LogError("ViolationTicketCounts collection validation failed");
            }

            // Step 3: Validate individual count field updates
            var countValidationResult = ValidateIndividualCountUpdates(beforeData, expectedData, actualData);
            if (!countValidationResult.IsValid)
            {
                result.IsValid = false;
                result.ValidationErrors.AddRange(countValidationResult.ValidationErrors);
                _logger.LogError("Individual count field validation failed");
            }

            if (result.IsValid)
            {
                _logger.LogInformation("All data integrity validations passed");
                result.FailureReason = null;
            }
            else
            {
                result.FailureReason = string.Join("; ", result.ValidationErrors);
                _logger.LogError("Data integrity validation failed: {FailureReason}", result.FailureReason);
            }

            return result;
        }

        /// <summary>
        /// Validates that all top-level ViolationTicket properties remain unchanged
        /// </summary>
        private ValidationResult ValidateViolationTicketPropertiesUnchanged(ViolationTicket beforeData, ViolationTicket actualData)
        {
            var result = new ValidationResult { IsValid = true };

            // Compare all primitive properties of ViolationTicket (excluding ViolationTicketCounts)
            var propertiesToValidate = new Dictionary<string, (object? before, object? actual)>
            {
                { "ViolationTicketId", (beforeData.ViolationTicketId, actualData.ViolationTicketId) },
                { "TicketNumberTxt", (beforeData.TicketNumberTxt, actualData.TicketNumberTxt) },
                { "AddressTxt", (beforeData.AddressTxt, actualData.AddressTxt) },
                { "AddressCityTxt", (beforeData.AddressCityTxt, actualData.AddressCityTxt) },
                { "AddressCountryTxt", (beforeData.AddressCountryTxt, actualData.AddressCountryTxt) },
                { "AddressPostalCodeTxt", (beforeData.AddressPostalCodeTxt, actualData.AddressPostalCodeTxt) },
                { "AddressProvinceTxt", (beforeData.AddressProvinceTxt, actualData.AddressProvinceTxt) },
                { "CourtLocationTxt", (beforeData.CourtLocationTxt, actualData.CourtLocationTxt) },
                { "DetachmentLocationTxt", (beforeData.DetachmentLocationTxt, actualData.DetachmentLocationTxt) },
                { "DisputantBirthDt", (beforeData.DisputantBirthDt, actualData.DisputantBirthDt) },
                { "DisputantClientNumberTxt", (beforeData.DisputantClientNumberTxt, actualData.DisputantClientNumberTxt) },
                { "DisputantDrvLicNumberTxt", (beforeData.DisputantDrvLicNumberTxt, actualData.DisputantDrvLicNumberTxt) },
                { "DisputantGivenNamesTxt", (beforeData.DisputantGivenNamesTxt, actualData.DisputantGivenNamesTxt) },
                { "DisputantOrganizationNmTxt", (beforeData.DisputantOrganizationNmTxt, actualData.DisputantOrganizationNmTxt) },
                { "DisputantSurnameTxt", (beforeData.DisputantSurnameTxt, actualData.DisputantSurnameTxt) },
                { "DrvLicExpiryYearNo", (beforeData.DrvLicExpiryYearNo, actualData.DrvLicExpiryYearNo) },
                { "DrvLicIssuedCountryTxt", (beforeData.DrvLicIssuedCountryTxt, actualData.DrvLicIssuedCountryTxt) },
                { "DrvLicIssuedProvinceTxt", (beforeData.DrvLicIssuedProvinceTxt, actualData.DrvLicIssuedProvinceTxt) },
                { "DrvLicIssuedYearNo", (beforeData.DrvLicIssuedYearNo, actualData.DrvLicIssuedYearNo) },
                { "IsChangeOfAddressYn", (beforeData.IsChangeOfAddressYn, actualData.IsChangeOfAddressYn) },
                { "IsDriverYn", (beforeData.IsDriverYn, actualData.IsDriverYn) },
                { "IsOwnerYn", (beforeData.IsOwnerYn, actualData.IsOwnerYn) },
                { "IsYoungPersonYn", (beforeData.IsYoungPersonYn, actualData.IsYoungPersonYn) },
                { "IssuedAtOrNearCityTxt", (beforeData.IssuedAtOrNearCityTxt, actualData.IssuedAtOrNearCityTxt) },
                { "IssuedDt", (beforeData.IssuedDt, actualData.IssuedDt) },
                { "IssuedOnRoadOrHighwayTxt", (beforeData.IssuedOnRoadOrHighwayTxt, actualData.IssuedOnRoadOrHighwayTxt) },
                { "OfficerPinTxt", (beforeData.OfficerPinTxt, actualData.OfficerPinTxt) },
                // Audit fields from AuditBase
                { "EntDtm", (beforeData.EntDtm, actualData.EntDtm) },
                { "EntUserId", (beforeData.EntUserId, actualData.EntUserId) },
                // Note: UpdDtm and UpdUserId may change during updates, so we don't validate those
            };

            foreach (var (propertyName, (before, actual)) in propertiesToValidate)
            {
                if (!AreValuesEqual(before, actual))
                {
                    result.IsValid = false;
                    result.ValidationErrors.Add($"Property '{propertyName}' changed from '{before}' to '{actual}'");
                }
            }

            // Validate nested Dispute object if it exists
            var disputeValidationErrors = new List<string>();
            if (!ValidateDisputeObjectUnchanged(beforeData.Dispute, actualData.Dispute, disputeValidationErrors))
            {
                result.IsValid = false;
                result.ValidationErrors.AddRange(disputeValidationErrors);
            }

            if (!result.IsValid)
            {
                _logger.LogError("ViolationTicket property validation failed. Violations: {Violations}",
                    string.Join("; ", result.ValidationErrors));
            }

            return result;
        }

        /// <summary>
        /// Validates that the Dispute nested object remains unchanged
        /// </summary>
        private bool ValidateDisputeObjectUnchanged(object? beforeDispute, object? actualDispute, List<string> violations)
        {
            // If both are null, that's fine
            if (beforeDispute == null && actualDispute == null)
                return true;

            // If one is null and the other isn't, that's a change
            if (beforeDispute == null || actualDispute == null)
            {
                violations.Add($"Dispute object nullability changed: before={beforeDispute != null}, actual={actualDispute != null}");
                return false;
            }

            // Use JSON serialization to compare complex objects
            var beforeJson = JsonSerializer.Serialize(beforeDispute, _jsonOptions);
            var actualJson = JsonSerializer.Serialize(actualDispute, _jsonOptions);

            if (beforeJson != actualJson)
            {
                violations.Add("Dispute object content has changed");
                _logger.LogDebug("Dispute object before: {Before}", beforeJson);
                _logger.LogDebug("Dispute object actual: {Actual}", actualJson);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates the ViolationTicketCounts collection structure and count
        /// </summary>
        private ValidationResult ValidateViolationTicketCountsCollection(ViolationTicket beforeData, ViolationTicket expectedData, ViolationTicket actualData)
        {
            var result = new ValidationResult { IsValid = true };

            if (beforeData.ViolationTicketCounts == null || expectedData.ViolationTicketCounts == null || actualData.ViolationTicketCounts == null)
            {
                result.IsValid = false;
                result.ValidationErrors.Add("One or more violation ticket counts collections are null during validation");
                _logger.LogWarning("One or more violation ticket counts collections are null during validation");
                return result;
            }

            // Ensure the count of items hasn't changed
            if (beforeData.ViolationTicketCounts.Count != actualData.ViolationTicketCounts.Count)
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"ViolationTicketCounts collection count changed: before={beforeData.ViolationTicketCounts.Count}, actual={actualData.ViolationTicketCounts.Count}");
                _logger.LogError("ViolationTicketCounts collection count changed: before={Before}, actual={Actual}",
                    beforeData.ViolationTicketCounts.Count, actualData.ViolationTicketCounts.Count);
                return result;
            }

            // Ensure all count numbers are still present
            var beforeCountNos = beforeData.ViolationTicketCounts.Select(c => c.CountNo).OrderBy(x => x).ToList();
            var actualCountNos = actualData.ViolationTicketCounts.Select(c => c.CountNo).OrderBy(x => x).ToList();

            if (!beforeCountNos.SequenceEqual(actualCountNos))
            {
                result.IsValid = false;
                result.ValidationErrors.Add($"ViolationTicketCounts CountNo values changed: before=[{string.Join(",", beforeCountNos)}], actual=[{string.Join(",", actualCountNos)}]");
                _logger.LogError("ViolationTicketCounts CountNo values changed: before=[{Before}], actual=[{Actual}]",
                    string.Join(",", beforeCountNos), string.Join(",", actualCountNos));
                return result;
            }

            return result;
        }

        /// <summary>
        /// Validates individual count field updates
        /// </summary>
        private ValidationResult ValidateIndividualCountUpdates(ViolationTicket beforeData, ViolationTicket expectedData, ViolationTicket actualData)
        {
            var result = new ValidationResult { IsValid = true };

            // Group counts by count number for easier comparison
            var beforeCounts = beforeData.ViolationTicketCounts!.ToDictionary(c => c.CountNo);
            var expectedCounts = expectedData.ViolationTicketCounts!.ToDictionary(c => c.CountNo);
            var actualCounts = actualData.ViolationTicketCounts!.ToDictionary(c => c.CountNo);

            foreach (var countNo in beforeCounts.Keys)
            {
                if (!expectedCounts.ContainsKey(countNo) || !actualCounts.ContainsKey(countNo))
                {
                    result.ValidationErrors.Add($"Count number {countNo} missing in expected or actual data");
                    _logger.LogWarning("Count number {CountNo} missing in expected or actual data", countNo);
                    continue;
                }

                var beforeCount = beforeCounts[countNo];
                var expectedCount = expectedCounts[countNo];
                var actualCount = actualCounts[countNo];

                // Check that only null fields were updated and all other fields remain unchanged
                var countValidationResult = ValidateCountFieldUpdate(beforeCount, expectedCount, actualCount);
                if (!countValidationResult.IsValid)
                {
                    result.IsValid = false;
                    result.ValidationErrors.AddRange(countValidationResult.ValidationErrors);
                    _logger.LogWarning("Count {CountNo} failed validation - unexpected field changes detected", countNo);
                }
            }

            return result;
        }

        /// <summary>
        /// Compares two values for equality, handling nulls properly
        /// </summary>
        private bool AreValuesEqual(object? value1, object? value2)
        {
            // Handle null cases
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;

            // For DateTime comparisons, handle potential precision differences
            if (value1 is DateTime dt1 && value2 is DateTime dt2)
            {
                // Compare with millisecond precision to avoid database precision issues
                return Math.Abs((dt1 - dt2).TotalMilliseconds) < 1;
            }

            // For other types, use Equals
            return value1.Equals(value2);
        }

        /// <summary>
        /// Validates that only null fields were updated in a specific count
        /// </summary>
        /// <param name="beforeCount">Count before update</param>
        /// <param name="expectedCount">Expected count after update</param>
        /// <param name="actualCount">Actual count from database</param>
        /// <returns>ValidationResult with detailed failure information</returns>
        private ValidationResult ValidateCountFieldUpdate(ViolationTicketCount beforeCount, ViolationTicketCount expectedCount, ViolationTicketCount actualCount)
        {
            var result = new ValidationResult { IsValid = true };

            // List of fields that should be validated
            var fieldsToValidate = new Dictionary<string, (object? before, object? expected, object? actual)>
            {
                { "DescriptionTxt", (beforeCount.DescriptionTxt, expectedCount.DescriptionTxt, actualCount.DescriptionTxt) },
                { "ActOrRegulationNameCd", (beforeCount.ActOrRegulationNameCd, expectedCount.ActOrRegulationNameCd, actualCount.ActOrRegulationNameCd) },
                { "IsActYn", (beforeCount.IsActYn, expectedCount.IsActYn, actualCount.IsActYn) },
                { "IsRegulationYn", (beforeCount.IsRegulationYn, expectedCount.IsRegulationYn, actualCount.IsRegulationYn) },
                { "StatSectionTxt", (beforeCount.StatSectionTxt, expectedCount.StatSectionTxt, actualCount.StatSectionTxt) },
                { "StatSubSectionTxt", (beforeCount.StatSubSectionTxt, expectedCount.StatSubSectionTxt, actualCount.StatSubSectionTxt) },
                { "StatParagraphTxt", (beforeCount.StatParagraphTxt, expectedCount.StatParagraphTxt, actualCount.StatParagraphTxt) },
                { "StatSubParagraphTxt", (beforeCount.StatSubParagraphTxt, expectedCount.StatSubParagraphTxt, actualCount.StatSubParagraphTxt) },
                { "ticketedAmt", (beforeCount.TicketedAmt, expectedCount.TicketedAmt, actualCount.TicketedAmt) }
            };

            foreach (var (fieldName, (before, expected, actual)) in fieldsToValidate)
            {
                // If field was null before and expected to be updated
                if (before == null && expected != null)
                {
                    // Check if actual matches expected
                    if (!object.Equals(actual, expected))
                    {
                        result.IsValid = false;
                        result.ValidationErrors.Add($"Field {fieldName} validation failed. Expected: {expected}, Actual: {actual}");
                        // _logger.LogWarning("Field {FieldName} validation failed. Expected: {Expected}, Actual: {Actual}", 
                        // fieldName, expected, actual);
                    }
                }
                // If field was not null before, it should not have changed
                else if (before != null)
                {
                    if (!object.Equals(actual, before))
                    {
                        result.IsValid = false;
                        result.ValidationErrors.Add($"Field {fieldName} was modified when it shouldn't have been. Before: {before}, Actual: {actual}");
                        // _logger.LogWarning("Field {FieldName} was modified when it shouldn't have been. Before: {Before}, Actual: {Actual}", 
                        // fieldName, before, actual);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the integrity check result in the database cache
        /// </summary>
        /// <param name="context">Database context</param>
        /// <param name="ticketNumber">Ticket number</param>
        /// <param name="freshData">Fresh data from database</param>
        /// <param name="integrityCheckPassed">Whether integrity check passed</param>
        /// <param name="failureReason">Reason for failure if integrity check failed</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task UpdateIntegrityCheckResultAsync(HotfixSqliteContext context, string ticketNumber,
            ViolationTicket? freshData, bool integrityCheckPassed, string? failureReason, CancellationToken cancellationToken)
        {
            var existingEntry = await context.HotfixViolationTickets
                .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, cancellationToken);

            if (existingEntry != null)
            {
                if (freshData != null)
                {
                    existingEntry.AfterHotfixDataJson = JsonSerializer.Serialize(freshData);
                }
                existingEntry.IsIntegrityCheckPassed = integrityCheckPassed;
                existingEntry.IntegrityCheckFailureReason = failureReason;
                existingEntry.CachedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated integrity check result for ticket {TicketNumber}: {Result}",
                    ticketNumber, integrityCheckPassed ? "PASSED" : "FAILED");

                if (!integrityCheckPassed && !string.IsNullOrEmpty(failureReason))
                {
                    _logger.LogWarning("Integrity check failure reason for ticket {TicketNumber}: {FailureReason}",
                        ticketNumber, failureReason);
                }
            }
            else
            {
                _logger.LogWarning("No existing cache entry found for ticket {TicketNumber} during integrity check update", ticketNumber);
            }
        }

        // Implementation of IHotfix interface
        public async Task<dynamic> ExecuteAsync(HotfixExecutionContext context)
        {
            _logger.LogInformation("Starting execution of hotfix");

            try
            {
                // Initialize and validate database schema first
                await InitializeAndValidateDatabaseAsync(context.CancellationToken);

                var timeZone = "America/Vancouver";
                TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

                // Cast to the specific request type we expect
                var disputesRequest = new Dictionary<string, string>();

                // "2025-06-25T10:12:00" 
                DateTime startDate = DateTime.ParseExact("2025-06-25T10:12:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                startDate = TimeZoneInfo.ConvertTimeToUtc(startDate, tz);
                disputesRequest.Add("submitted_dt_ge", startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                // "2025-07-09T10:30:00"
                DateTime endDate = DateTime.ParseExact("2025-07-09T10:30:00", "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                endDate = TimeZoneInfo.ConvertTimeToUtc(endDate, tz);
                disputesRequest.Add("submitted_dt_lt", endDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                _logger.LogInformation("Fetching disputes from OCCAM database for date range: {StartDate} to {EndDate}",
                    startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"), endDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                // Add pagination parameters to the disputesRequest dictionary
                if (context.PageSize != null && context.PageNumber != null)
                {
                    int offset = ((int)context.PageNumber - 1) * (int)context.PageSize;
                    _logger.LogInformation("Pagination: pageSize={PageSize}, pageNumber={PageNumber}, offset={Offset}", context.PageSize, context.PageNumber, offset);

                    disputesRequest.Add("fetch_rows", context.PageSize.ToString());
                    disputesRequest.Add("offset_rows", offset.ToString());

                    _logger.LogInformation("fetch_rows: {FetchRows}, offset_rows: {OffsetRows}", disputesRequest["fetch_rows"], disputesRequest["offset_rows"]);
                }
                else
                {
                    _logger.LogInformation("No pagination parameters provided, fetching all disputes.");
                    disputesRequest.Add("fetch_rows", "25"); // Default fetch size if not provided
                }

                var ticketType = "E";
                // tickets starting with E 
                disputesRequest.Add("ticket_number_txt_like", "E%");
                // tickets starting with S
                // disputesRequest.Add("ticket_number_txt_like", "S%");

                // Step 1: Get disputes with caching from OCCAM database for the given date range
                var disputesToProcess = await LoadOrFetchDisputeDataAsync(disputesRequest, context.CancellationToken);

                // Step 2: Process the tickets
                _logger.LogInformation("Step 2: Get ViolationTicket from RSI");

                if (!disputesToProcess.Any())
                {
                    _logger.LogInformation("No tickets found to process in OCCAM database for the given date range.");
                    return new { isComplete = false, ticketsToProcess = new List<string>(), results = new List<object>() };
                }

                var results = new List<dynamic>();
                var errors = new List<string>();
                var allSqlStatements = new List<string>(); // Collect all SQL statements from all tickets

                foreach (var dispute in disputesToProcess)
                {
                    if (dispute?.ticket_number_txt == null)
                    {
                        _logger.LogInformation("Ticket number is null, skipping RSI data fetch for dispute {DisputeId}.", dispute?.dispute_id);
                        results.Add(new
                        {
                            isComplete = false,
                            disputeId = dispute?.dispute_id,
                            ticketNumber = dispute?.ticket_number_txt,
                            error = "Ticket number is null",
                            CorrectionResult = new CountCorrectionResult
                            {
                                HasUpdates = false,
                            }
                        });
                        continue; // Continue to next dispute instead of returning
                    }

                    try
                    {
                        // Step 3: Get Violation ticket with Counts from OCCAM database
                        var violationTicket = await LoadOrFetchViolationTicketDataAsync(context, dispute.dispute_id, dispute.ticket_number_txt, context.CancellationToken);

                        _logger.LogInformation("Fetched violation data for ticket {TicketNumber}: {Data}",
                            dispute.ticket_number_txt, violationTicket?.Dispute?.DisputeId ?? "No data found");

                        if (violationTicket == null)
                        {
                            errors.Add(dispute.ticket_number_txt + ": Occam violation ticket data not found");
                            _logger.LogInformation("Occam violation ticket data not found for ticket {TicketNumber}. Skipping update.", dispute.ticket_number_txt);
                            results.Add(new
                            {
                                isComplete = false,
                                disputeId = dispute.dispute_id,
                                ticketNumber = dispute.ticket_number_txt,
                                rsiTicket = (Ticket?)null,
                                violationTicket = (ViolationTicket?)null,
                                error = "Occam violation ticket data not found",
                                CorrectionResult = new CountCorrectionResult
                                {
                                    HasUpdates = false,
                                }
                            });
                            continue;
                        }

                        // Get RSI ticket data with caching
                        TimeOnly? issuedTime = violationTicket.IssuedDt != null
                            ? TimeOnly.FromDateTime(violationTicket.IssuedDt.DateTime)
                            : null;

                        _logger.LogInformation("Issued time for ticket {TicketNumber}: {IssuedTime}",
                            dispute.ticket_number_txt, issuedTime?.ToString() ?? "null");

                        if (issuedTime == null)
                        {
                            _logger.LogInformation("Issued time is null for ticket {TicketNumber}, skipping RSI data fetch.", dispute.ticket_number_txt);
                            results.Add(new
                            {
                                isComplete = false,
                                disputeId = dispute.dispute_id,
                                ticketNumber = dispute.ticket_number_txt,
                                rsiTicket = (Ticket?)null,
                                violationTicket,
                                error = "Issued time is null",
                                CorrectionResult = new CountCorrectionResult
                                {
                                    HasUpdates = false,
                                }
                            });
                            continue;
                        }

                        var rsiTicket = await LoadOrFetchRSITicketDataAsync(context,
                            dispute.ticket_number_txt, (TimeOnly)issuedTime, context.CancellationToken);

                        _logger.LogInformation("Fetched RSI ticket data for {TicketNumber}: {Data}",
                            dispute.ticket_number_txt, rsiTicket != null ? "Found" : "No data found");

                        if (rsiTicket == null)
                        {
                            errors.Add(dispute.ticket_number_txt + ": RSI ticket data not found");
                            _logger.LogInformation("RSI ticket data not found for ticket {TicketNumber}. Skipping update.", dispute.ticket_number_txt);
                            results.Add(new
                            {
                                isComplete = false,
                                disputeId = dispute.dispute_id,
                                ticketNumber = dispute.ticket_number_txt,
                                rsiTicket = (Ticket?)null,
                                violationTicket,
                                error = "RSI ticket data not found",
                                CorrectionResult = new CountCorrectionResult
                                {
                                    HasUpdates = false,
                                }
                            });
                            continue;
                        }

                        _logger.LogInformation("Merging missing count data for ticket {TicketNumber}", dispute.ticket_number_txt);

                        // Merge missing count data from RSI ticket into violation ticket
                        var correctionResult = CorrectMissingCountDataWithSQLGeneration(violationTicket, rsiTicket);

                        if (correctionResult.HasErrors)
                        {
                            errors.Add(violationTicket.TicketNumberTxt);
                        }
                        // Collect SQL statements from this ticket
                        if (correctionResult.GeneratedSQLStatements.Any())
                        {
                            allSqlStatements.AddRange(correctionResult.GeneratedSQLStatements);
                        }

                        if (correctionResult.CorrectedViolationTicket == null)
                        {
                            _logger.LogInformation("No counts to merge for ticket {TicketNumber}", dispute.ticket_number_txt);
                            results.Add(new
                            {
                                isComplete = false,
                                disputeId = dispute.dispute_id,
                                ticketNumber = dispute.ticket_number_txt,
                                rsiTicket,
                                error = "No counts to merge",
                                CorrectionResult = correctionResult
                            });
                            continue;
                        }

                        // Step 4: Update ticket in OCCAM if not dry run and has updates
                        if (false && !context.DryRun && correctionResult.HasUpdates)
                        {
                            _logger.LogInformation("Updating ticket {TicketNumber} in OCCAM", dispute.ticket_number_txt);
                            // Update logic here...
                        }

                        // Add successful result
                        results.Add(new
                        {
                            isComplete = true,
                            disputeId = dispute.dispute_id,
                            ticketNumber = dispute.ticket_number_txt,
                            rsiTicket,
                            CorrectionResult = correctionResult,
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing dispute {DisputeId} with ticket {TicketNumber}",
                            dispute.dispute_id, dispute.ticket_number_txt);

                        results.Add(new
                        {
                            isComplete = false,
                            disputeId = dispute.dispute_id,
                            ticketNumber = dispute.ticket_number_txt,
                            error = ex.Message,
                            CorrectionResult = new CountCorrectionResult
                            {
                                HasUpdates = false,
                            }
                        });
                    }
                }

                // Write all collected SQL statements to a single file
                string? masterSqlFilePath = null;
                if (allSqlStatements.Any())
                {
                    var pagePrefix = context.PageNumber != null ? $"Page_{context.PageNumber}_" : "";
                    var masterFileName = $"{ticketType}_{pagePrefix}Master_Update_Statements_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                    masterSqlFilePath = await WriteSQLToFileAsync(allSqlStatements, masterFileName);
                    _logger.LogInformation("Generated master SQL file: {FilePath} with {StatementCount} total UPDATE statements from {TicketCount} tickets",
                        masterSqlFilePath, allSqlStatements.Count, results.Count);
                }

                // Return all results after processing all disputes
                var response = new
                {
                    pageNumber = context.PageNumber,
                    pageSize = context.PageSize,
                    errors,
                    isComplete = true,
                    hotFixUpdatedTicketsCounts = results.Count(r => r?.CorrectionResult?.HasUpdates == true),
                    totalProcessed = results.Count,
                    newCount = disputesToProcess.Count(d => d?.dispute_status_type_cd == "NEW"),
                    processingCount = disputesToProcess.Count(d => d?.dispute_status_type_cd == "PROC"),
                    validCount = disputesToProcess.Count(d => d?.dispute_status_type_cd == "VALD"),
                    rejectedCount = disputesToProcess.Count(d => d?.dispute_status_type_cd == "REJ"),
                    cancelledCount = disputesToProcess.Count(d => d?.dispute_status_type_cd == "CANC"),
                    concludedCount = disputesToProcess.Count(d => d?.dispute_status_type_cd == "CONC"),
                    totalSqlStatements = allSqlStatements.Count,
                    masterSqlFilePath,
                    // hasUpdatesCount = results.Select(r => r.correctionResult).Count(cr => cr.hasUpdates),
                    disputesToProcess,
                    results,
                };

                // Write results to a file in .local, Results folder for each env
                string? resultsFilePath = null;
                if (results.Any())
                {
                    var pagePrefix = context.PageNumber != null ? $"Page_{context.PageNumber}_" : "";
                    var resultsFileName = $"{ticketType}_{pagePrefix}Results_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                    var resultsOutputPath = Path.Combine(Environment.CurrentDirectory, ".local", "Results", Env);
                    Directory.CreateDirectory(resultsOutputPath);
                    resultsFilePath = Path.Combine(resultsOutputPath, resultsFileName);

                    var resultsJson = JsonSerializer.Serialize(response, _jsonOptions);
                    await File.WriteAllTextAsync(resultsFilePath, resultsJson);

                    _logger.LogInformation("Wrote results to file: {FilePath}", resultsFilePath);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing hotfix: {HotfixName}", Name);
                throw;
            }
        }

        /// <summary>
        /// Result object returned by validation methods
        /// </summary>
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string? FailureReason { get; set; }
            public List<string> ValidationErrors { get; set; } = new List<string>();
        }

        /// <summary>
        /// Result class for the count correction operation
        /// </summary>
        public class CountCorrectionResult
        {
            /// <summary>
            /// The original violation ticket before any corrections
            /// </summary>
            public ViolationTicket? OriginalViolationTicket { get; set; }

            /// <summary>
            /// The corrected violation ticket after applying updates
            /// </summary>
            public ViolationTicket? CorrectedViolationTicket { get; set; }

            /// <summary>
            /// Indicates whether any updates were made to the violation ticket
            /// </summary>
            public bool HasUpdates { get; set; }

            /// <summary>
            /// The number of fields that were updated
            /// </summary>
            public int UpdatedFieldsCount { get; set; }

            /// <summary>
            /// List of field names that were updated
            /// </summary>
            public List<string> UpdatedFields { get; set; } = new List<string>();

            /// <summary>
            /// Generated SQL UPDATE statements for the corrections made
            /// </summary>
            public List<string> GeneratedSQLStatements { get; set; } = new List<string>();

            /// <summary>
            /// File path where the generated SQL statements were saved
            /// </summary>
            public string? GeneratedSQLFilePath { get; set; }
            public bool HasErrors { get; set; } = false;
            public RSICountErrors RsiCountErrors { get; set; } = new RSICountErrors();
            public ViolationTicketCountErrors ViolationTicketCountErrors { get; set; } = new ViolationTicketCountErrors();
        }

        public class RSICountErrors
        {
            public bool HasNoCounts { get; set; } = false;
            public bool HasMissingCounts { get; set; } = false;
        }

        public class ViolationTicketCountErrors
        {
            public bool ViolationTicketCountNumberIsNotInt { get; set; } = false;
            public bool CountDisputedButDoesNotExistInRSI { get; set; } = false;
        }

        /// <summary>
        /// Generates SQL UPDATE statements for fields that need to be updated
        /// </summary>
        /// <param name="violationTicketCountId">The violation ticket count ID</param>
        /// <param name="originalCount">The original count data</param>
        /// <param name="updatedCount">The updated count data with RSI values</param>
        /// <returns>SQL UPDATE statement with actual values</returns>
        private string GenerateUpdateSQL(long violationTicketCountId, ViolationTicketCount originalCount, ViolationTicketCount updatedCount)
        {
            var setClause = new List<string>();
            var fieldsToUpdate = new Dictionary<string, (object? original, object? updated)>
            {
                { "description_txt", (originalCount.DescriptionTxt, updatedCount.DescriptionTxt) },
                { "act_or_regulation_name_cd", (originalCount.ActOrRegulationNameCd, updatedCount.ActOrRegulationNameCd) }, // Lookup 
                { "is_act_yn", (originalCount.IsActYn, updatedCount.IsActYn) },
                { "is_regulation_yn", (originalCount.IsRegulationYn, updatedCount.IsRegulationYn) },
                { "stat_section_txt", (originalCount.StatSectionTxt, updatedCount.StatSectionTxt) }, // Lookup 
                { "stat_sub_section_txt", (originalCount.StatSubSectionTxt, updatedCount.StatSubSectionTxt) }, // Lookup
                { "stat_paragraph_txt", (originalCount.StatParagraphTxt, updatedCount.StatParagraphTxt) }, // Lookup
                { "stat_sub_paragraph_txt", (originalCount.StatSubParagraphTxt, updatedCount.StatSubParagraphTxt) }, // Lookup
                { "ticketed_amt", (originalCount.TicketedAmt, updatedCount.TicketedAmt) }
            };

            // Only include fields that were actually updated (originally null, now has value)
            foreach (var (columnName, (original, updated)) in fieldsToUpdate)
            {
                if (original == null && updated != null)
                {
                    setClause.Add($"{columnName} = {FormatSQLValue(updated)}");
                }
            }

            if (!setClause.Any())
            {
                return string.Empty; // No updates needed
            }

            var sql = $@"-- Update violation_ticket_count_id: {violationTicketCountId}: CountNo:{originalCount.CountNo}
UPDATE occam_violation_ticket_counts 
SET {string.Join(",\n    ", setClause)}
WHERE violation_ticket_count_id = {violationTicketCountId};
";

            return sql;
        }

        /// <summary>
        /// Formats a C# value for SQL insertion, handling nulls and proper quoting
        /// </summary>
        /// <param name="value">The value to format</param>
        /// <returns>Formatted SQL value</returns>
        private string FormatSQLValue(object? value)
        {
            if (value == null)
                return "NULL";

            return value switch
            {
                string str => $"'{str.Replace("'", "''")}'", // Escape single quotes
                DateTime dt => $"TO_DATE('{dt:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH24:MI:SS')",
                decimal dec => dec.ToString("F2"),
                float f => f.ToString("F2"),
                double d => d.ToString("F2"),
                bool b => b ? "1" : "0",
                _ => value.ToString()
            };
        }

        /// <summary>
        /// Writes SQL statements to a single master file for database execution
        /// </summary>
        /// <param name="sqlStatements">List of SQL statements to write</param>
        /// <param name="fileName">Optional custom file name</param>
        /// <returns>The full path to the generated SQL file</returns>
        private async Task<string> WriteSQLToFileAsync(List<string> sqlStatements, string? fileName = null)
        {
            if (!sqlStatements.Any())
            {
                _logger.LogInformation("No SQL statements to write to file");
                return string.Empty;
            }

            // Generate file name with timestamp if not provided
            fileName ??= $"Generated_Update_Statements_{DateTime.Now:yyyyMMdd_HHmmss}.sql";

            // Use the same directory as the hotfix or a specific output directory
            var outputPath = Path.Combine(Environment.CurrentDirectory, ".local", "GeneratedSQL", Env);
            Directory.CreateDirectory(outputPath);

            var fullPath = Path.Combine(outputPath, fileName);

            var sqlContent = new StringBuilder();
            sqlContent.AppendLine("-- SQL UPDATE statements for OCCAM violation ticket counts");
            sqlContent.AppendLine("-- This file contains all SQL statements for multiple tickets");
            sqlContent.AppendLine($"-- Generated at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sqlContent.AppendLine($"-- Hotfix: {Name} v{FixVersion}");
            sqlContent.AppendLine();

            foreach (var sql in sqlStatements)
            {
                sqlContent.AppendLine(sql);
            }

            sqlContent.AppendLine();
            sqlContent.AppendLine($"-- Total statements: {sqlStatements.Count}");

            await File.WriteAllTextAsync(fullPath, sqlContent.ToString());

            _logger.LogInformation("Generated SQL file: {FilePath} with {StatementCount} UPDATE statements",
                fullPath, sqlStatements.Count);

            return fullPath;
        }

        /// <summary>
        /// Enhanced version of CorrectMissingCountData that also generates SQL statements
        /// </summary>
        /// <param name="violationTicket">The violation ticket data object</param>
        /// <param name="rsiTicket">The RSI ticket data object</param>
        /// <returns>CountCorrectionResult with SQL statements included</returns>
        public CountCorrectionResult CorrectMissingCountDataWithSQLGeneration(ViolationTicket? violationTicket, Ticket? rsiTicket)
        {
            var result = new CountCorrectionResult();
            var sqlStatements = new List<string>();

            if (violationTicket?.ViolationTicketCounts is null || rsiTicket?.Counts is null)
            {
                _logger.LogWarning("Violation ticket or RSI ticket data is null or contains no counts to merge.");
                result.OriginalViolationTicket = violationTicket;
                result.CorrectedViolationTicket = violationTicket;
                result.HasUpdates = false;
                result.HasErrors = true;
                result.GeneratedSQLStatements = sqlStatements;
                return result;
            }

            // Create a deep copy of the original violation ticket before making changes
            var originalJson = JsonSerializer.Serialize(violationTicket, _jsonOptions);
            result.OriginalViolationTicket = JsonSerializer.Deserialize<ViolationTicket>(originalJson, _jsonOptions);

            // Create a copy to work with (don't modify the original)
            var correctedViolationTicket = JsonSerializer.Deserialize<ViolationTicket>(originalJson, _jsonOptions);

            var rsiCountsByNumber = rsiTicket.Counts.ToDictionary(c => c.Number);
            bool hasUpdates = false;
            int updatedFieldsCount = 0;
            var updatedFields = new List<string>();

            // RSI Ticket counts are the source of truth
            // Flag if RSI Ticket doesnt have at least one count
            if (!rsiCountsByNumber.Any())
            {
                result.HasErrors = true;
                result.RsiCountErrors.HasNoCounts = true;
                result.HasUpdates = false;
                return result;
            }                     

            // Flag if RSI Ticket is missing previous counts
            var rsiCountNumbers = rsiCountsByNumber.Keys.OrderBy(n => n).ToList();
            for (int i = 0; i < rsiCountNumbers.Count; i++)
            {
                if (rsiCountNumbers[i] != i + 1)
                {
                    result.HasErrors = true;
                    result.RsiCountErrors.HasMissingCounts = true;
                    result.HasUpdates = false;
                    return result;
                }
            }

            // Add a header comment for this ticket's SQL statements
            if (rsiCountsByNumber.Any() || violationTicket.ViolationTicketCounts.Any())
            {
                sqlStatements.Add($"-- ========================================");
                sqlStatements.Add($"-- Ticket: {correctedViolationTicket.TicketNumberTxt}");
                sqlStatements.Add($"-- Dispute ID: {correctedViolationTicket.Dispute?.DisputeId}");
                sqlStatements.Add($"-- ========================================");
                sqlStatements.Add("");
            }

   
            foreach (var violationCount in correctedViolationTicket.ViolationTicketCounts)
            {
                if (!int.TryParse(violationCount.CountNo, out var countNumber))
                {
                    result.HasErrors = true;
                    result.ViolationTicketCountErrors.ViolationTicketCountNumberIsNotInt = true;
                    continue; // Skip if count number is not a valid integer
                }

                // If a count exists in the violation ticket but not in the RSI ticket, it should be deleted.
                if (!rsiCountsByNumber.ContainsKey(countNumber))
                {
                    // Check if the violation count is associated with a dispute count.
                    bool hasDisputeCount = violationCount.DisputeCount != null;

                    if (!hasDisputeCount)
                    {
                        // Generate SQL statement to remove count from violation ticket
                        sqlStatements.Add($"-- Deleting count {countNumber} as it does not exist in RSI ticket and is not disputed.");
                        sqlStatements.Add($"DELETE FROM occam_violation_ticket_counts WHERE violation_ticket_count_id = {violationCount.ViolationTicketCountId};");
                        hasUpdates = true;
                    }
                    else
                    {
                        result.HasErrors = true;
                        result.ViolationTicketCountErrors.CountDisputedButDoesNotExistInRSI = true;
                    }
                    continue; // Continue to the next violation count
                }

                if (rsiCountsByNumber.TryGetValue(countNumber, out var rsiCount))
                {
                    // Store the original count state before making changes
                    var originalCountJson = JsonSerializer.Serialize(violationCount, _jsonOptions);
                    var originalCount = JsonSerializer.Deserialize<ViolationTicketCount>(originalCountJson, _jsonOptions);

                    // Copy data from rsiCount to violationCount only if the target property is null
                    if (violationCount.DescriptionTxt == null && rsiCount.Description != null && rsiCount.Description != "")
                    {
                        violationCount.DescriptionTxt = rsiCount.Description;
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: DescriptionTxt");
                    }

                    if (violationCount.ActOrRegulationNameCd == null && rsiCount.Act != null && rsiCount.Act != "")
                    {
                        violationCount.ActOrRegulationNameCd = rsiCount.Act;
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: ActOrRegulationNameCd");
                    }

                    if (violationCount.IsActYn == null)
                    {
                        violationCount.IsActYn = rsiCount.IsAct ? "Y" : "N";
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: IsActYn");
                    }

                    if (violationCount.IsRegulationYn == null)
                    {
                        violationCount.IsRegulationYn = rsiCount.IsRegulation ? "Y" : "N";
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: IsRegulationYn");
                    }

                    if (violationCount.StatSectionTxt == null && rsiCount.Section != null && rsiCount.Section != "")
                    {
                        violationCount.StatSectionTxt = rsiCount.Section;
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: StatSectionTxt");
                    }

                    if (violationCount.StatSubSectionTxt == null && rsiCount.Subsection != null && rsiCount.Subsection != "")
                    {
                        violationCount.StatSubSectionTxt = rsiCount.Subsection;
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: StatSubSectionTxt");
                    }

                    if (violationCount.StatParagraphTxt == null && rsiCount.Paragraph != null && rsiCount.Paragraph != "")
                    {
                        violationCount.StatParagraphTxt = rsiCount.Paragraph;
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: StatParagraphTxt");
                    }

                    if (violationCount.StatSubParagraphTxt == null && rsiCount.Subparagraph != null && rsiCount.Subparagraph != "")
                    {
                        violationCount.StatSubParagraphTxt = rsiCount.Subparagraph;
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: StatSubParagraphTxt");
                    }

                    // Update TicketedAmt if it is null and RSI has a value
                    if (violationCount.TicketedAmt == null)
                    {
                        violationCount.TicketedAmt = rsiCount.TicketedAmount.ToString();
                        hasUpdates = true;
                        updatedFieldsCount++;
                        updatedFields.Add($"Count {countNumber}: TicketedAmt");
                    }

                    // Generate SQL for this count if there were updates
                    if (long.TryParse(violationCount.ViolationTicketCountId, out var countId))
                    {
                        var sql = GenerateUpdateSQL(countId, originalCount, violationCount);
                        if (!string.IsNullOrEmpty(sql))
                        {
                            sqlStatements.Add(sql);
                        }
                    }
                }
            }

            result.CorrectedViolationTicket = correctedViolationTicket;
            result.HasUpdates = hasUpdates;
            result.UpdatedFieldsCount = updatedFieldsCount;
            result.UpdatedFields = updatedFields;
            result.GeneratedSQLStatements = sqlStatements;

            // Note: Individual ticket SQL files are no longer generated
            // All SQL statements are collected and written to a single master file
            result.GeneratedSQLFilePath = null;

            // Add a separator after this ticket's SQL statements
            if (sqlStatements.Any())
            {
                sqlStatements.Add("");
                sqlStatements.Add("-- End of statements for this ticket");
                sqlStatements.Add("");
            }

            if (hasUpdates)
            {
                _logger.LogInformation("Updated {UpdatedFieldsCount} fields for ticket {TicketNumber}. Fields: {UpdatedFields}",
                    updatedFieldsCount, correctedViolationTicket.TicketNumberTxt, string.Join(", ", updatedFields));
                _logger.LogInformation("Generated {SQLCount} SQL statements for ticket {TicketNumber}",
                    sqlStatements.Count, correctedViolationTicket.TicketNumberTxt);
            }
            else
            {
                _logger.LogInformation("No updates needed for ticket {TicketNumber}", correctedViolationTicket.TicketNumberTxt);
            }

            return result;
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
