using MassTransit;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using TrafficCourts.Citizen.Service.Caching;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;
using TrafficCourts.Common;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Logging;
using TrafficCourts.Messaging.MessageContracts;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

public partial class TicketSearchService : ITicketSearchService
{
    private const string MVA = "MVA";
    private const string MVAR = "MVAR";
    private readonly IBus _bus;
    private readonly ITicketInvoiceSearchService _invoiceSearchService;
    private readonly ILogger<TicketSearchService> _logger;
    private static readonly DateTime _validVT2TicketEffectiveDate = new(2024, 4, 9);
    private readonly IFusionCache _cache;


    public TicketSearchService(IBus bus, ITicketInvoiceSearchService invoiceSearchService, IFusionCacheProvider cacheProvider, ILogger<TicketSearchService> logger)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _invoiceSearchService = invoiceSearchService ?? throw new ArgumentNullException(nameof(invoiceSearchService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _cache = cacheProvider.GetCache(Cache.TicketSearch.Name);
    }

    public async Task<Models.Tickets.ViolationTicket?> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ticketNumber);

        using var activity = Diagnostics.Source.StartActivity("Ticket Search");

        var time = $"{issuedTime.Hour:d2}:{issuedTime.Minute:d2}";

        using var scope = _logger.BeginScope(new Dictionary<string, string> { { "TicketNumber", ticketNumber }, { "Time", time } });

        try
        {
            _logger.LogDebug("Searching for violation ticket");

            // call the ticket service
            IEnumerable<Invoice> response = await SearchInvoicesAsync(ticketNumber, issuedTime, cancellationToken);

            // TCVP-2651 Filter results to only MVA/MVAR violation tickets
            var invoices = response.Where(_ => _.Act == MVA || _.Act == MVAR).ToList();

            if (invoices.Count != 0)
            {
                _logger.LogDebug("Found violation ticket with {Count} counts", invoices.Count);

                var reply = AssembleViolationTicket(ticketNumber, issuedTime, invoices);

                _logger.LogDebug("Search complete");

                return reply;
            }

            _logger.LogDebug("Violation ticket not found");
            activity?.AddTag("status", "Not Found");

            return null;
        }
        catch (InvalidTicketVersionException)
        {
            throw;
        }
        catch (Exception exception)
        {
            _logger.LogInformation(exception, "Error finding violation ticket");
            activity?.SetStatus(ActivityStatusCode.Error, exception?.Message);
            throw new TicketSearchErrorException("Error finding violation ticket", exception);
        }
    }

    private Models.Tickets.ViolationTicket AssembleViolationTicket(string ticketNumber, TimeOnly issuedTime, List<Invoice> invoices)
    {
        Debug.Assert(invoices.Count != 0);

        if (string.IsNullOrEmpty(invoices[0].ViolationDateTime))
        {
            _logger.LogInformation("Violation date and time is empty");
        }

        Models.Tickets.ViolationTicket ticket = new()
        {
            TicketNumber = ticketNumber
        };

        if (DateTime.TryParseExact(invoices[0].ViolationDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime violationDateTime))
        {
            var issuedTs = DateTime.SpecifyKind(violationDateTime, DateTimeKind.Unspecified);
            
            // TCVP-2560 all ISC tickets that always start with the letter 'S' and have a violation date before April 9, 2024 (VT1) are ineligible for TCO
            if (ticketNumber.StartsWith("S", StringComparison.OrdinalIgnoreCase) && issuedTs < _validVT2TicketEffectiveDate)
            {
                _logger.LogInformation("Ticket found is not a valid VT2 type");
                throw new InvalidTicketVersionException(issuedTs);
            }
            ticket.IssuedTs = issuedTs;
        }

        foreach (var invoice in invoices)
        {
            Models.Tickets.ViolationTicketCount count = new Models.Tickets.ViolationTicketCount();
            if (!string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                count.CountNo = (short)char.GetNumericValue(invoice.InvoiceNumber[^1]);
            }

            count.Description = invoice.OffenceDescription;
            count.TicketedAmount = invoice.TicketedAmount;

            count.ActOrRegulationNameCode = invoice.Act;

            if (string.IsNullOrEmpty(invoice.Section) || !LegalSection.TryParse(invoice.Section, out LegalSection? legalSection))
            {
                legalSection = new(); // force all spaces
            }
            
            count.Section = legalSection.Section;
            count.Subsection = legalSection.Subsection;
            count.Paragraph = legalSection.Paragraph;
            count.Subparagraph = legalSection.Subparagrah;

            count.IsAct = invoice.Act == MVA ? ViolationTicketCountIsAct.Y : ViolationTicketCountIsAct.N;
            count.IsRegulation = invoice.Act == MVAR ? ViolationTicketCountIsRegulation.Y : ViolationTicketCountIsRegulation.N;

            ticket.Counts.Add(count);
        }

        return ticket;
    }

    public async Task<bool> IsDisputeSubmittedBefore(string ticketNumber, CancellationToken cancellationToken)
    {
        // Check if a dispute has been submitted before for the given ticket number by verfying any Dispute exists associated to the provided ticket number
        SearchDisputeRequest searchRequest = new() { TicketNumber = ticketNumber, ExcludeStatus = ExcludeStatus2.REJECTED };

        try
        {
            Response<SearchDisputeResponse> response = await _bus.Request<SearchDisputeRequest, SearchDisputeResponse>(searchRequest, cancellationToken);
            
            var searchResponse = response.Message;

            if (searchResponse is null)
            {
                var exception = new DisputeSearchFailedException(ticketNumber);
                _logger.LogError("Search response is null, throwing DisputeSearchFailedException {ErrorId}", exception.ErrorId);
                throw exception;
            }

            if (!searchResponse.IsNotFound)
            {
                _logger.LogDebug("Found a dispute for the given ticket number: {ticketNumber}", ticketNumber);
                return true;
            }

            if (searchResponse.IsError)
            {
                var exception = new DisputeSearchFailedException(ticketNumber);
                _logger.LogError("Search returned error, throwing DisputeSearchFailedException {ErrorId}", exception.ErrorId);
                throw exception;
            }

            _logger.LogDebug("Dispute not submitted before, returning false");
            return false;
        }
        catch (RequestTimeoutException ex)
        {
            var exception = new DisputeSearchFailedException(ticketNumber, ex);
            LogRequestTimeout(ex, ticketNumber, exception.ErrorId);
            throw exception;
        }
        catch (Exception ex)
        {
            var exception = new DisputeSearchFailedException(ticketNumber, ex);
            _logger.LogError(ex, "An error occurred while checking if dispute has already been submitted {ErrorId}", exception.ErrorId);
            throw exception;
        }
    }

    [LoggerMessage(Level = LogLevel.Error, EventName = "RequestTimeout", Message = "An timeout error occurred while checking if dispute has already been submitted")]
    private partial void LogRequestTimeout(
        RequestTimeoutException exception,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTicketNumber), OmitReferenceName = true)]
        string ticketNumber,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordErrorId), OmitReferenceName = true)]
        Guid errorId);

    private async Task<IEnumerable<Invoice>> SearchInvoicesAsync(string ticketNumber, TimeOnly timeOnly, CancellationToken cancellationToken)
    {
        var key = Cache.TicketSearch.Key(ticketNumber, timeOnly);

        var value = await _cache.GetOrSetAsync<List<Invoice>>(
            key,
            SearchAsync,
            options: null,
            cancellationToken);

        return value;

        // determine the cache duration based on the result
        // see: https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/AdaptiveCaching.md
        TimeSpan GetCacheDuration(IList<Invoice> result)
        {
            // wasn't found, cache for 5 minutes, otherwise 1 day - this helps avoid DDoS
            return result.Count == 0
                ? TimeSpan.FromMinutes(5)
                : TimeSpan.FromDays(1);
        }

        // search and set the cache duration based result set
        async Task<List<Invoice>> SearchAsync(FusionCacheFactoryExecutionContext<List<Invoice>> context, CancellationToken cancellationToken)
        {
            IList<Invoice> result = await _invoiceSearchService.SearchAsync(ticketNumber, timeOnly, cancellationToken);
            context.Options.Duration = GetCacheDuration(result);
            return result.ToList();
        }
    }
}
