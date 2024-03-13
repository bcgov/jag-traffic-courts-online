using MassTransit;
using System.Diagnostics;
using System.Globalization;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;
using TrafficCourts.Common;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

public class TicketSearchService : ITicketSearchService
{
    private const string _mva = "MVA";
    private const string _mvar = "MVAR";
    private const string _mvr = "MVR";
    private readonly IBus _bus;
    private readonly ITicketInvoiceSearchService _invoiceSearchService;
    private readonly ILogger<TicketSearchService> _logger;
    private static readonly DateTime _validVT2TicketEffectiveDate = new(2024, 4, 9);
    

    public TicketSearchService(IBus bus, ITicketInvoiceSearchService invoiceSearchService, ILogger<TicketSearchService> logger)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _invoiceSearchService = invoiceSearchService ?? throw new ArgumentNullException(nameof(invoiceSearchService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            IEnumerable<Invoice>? response = await _invoiceSearchService.SearchAsync(ticketNumber, issuedTime, cancellationToken);

            // TCVP-2651 Filter results to only MVA/MVAR violation tickets
            var invoices = response.Where(_ => _.Act == _mva || _.Act == _mvr || _.Act == _mvar).ToList();
            
            var droppedInvoices = response.Where(_ => _.Act != _mva && _.Act != _mvr && _.Act != _mvar).ToList();
            if (droppedInvoices.Count != 0) {
                _logger.LogDebug("Dropped {Count} non MVA/MVAR violation ticket(s) from the RSI search results", droppedInvoices.Count);
            }

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

            count.IsAct = invoice.Act == _mva ? ViolationTicketCountIsAct.Y : ViolationTicketCountIsAct.N;
            count.IsRegulation = invoice.Act == _mvar ? ViolationTicketCountIsRegulation.Y : ViolationTicketCountIsRegulation.N;

            ticket.Counts.Add(count);
        }

        return ticket;
    }

    public async Task<bool> IsDisputeSubmittedBefore(string ticketNumber, CancellationToken cancellationToken)
    {
        try
        {
            // Check if a dispute has been submitted before for the given ticket number by verfying any Dispute exists associated to the provided ticket number
            SearchDisputeRequest searchRequest = new() { TicketNumber = ticketNumber, ExcludeStatus = ExcludeStatus2.REJECTED };
            Response<SearchDisputeResponse> response = await _bus.Request<SearchDisputeRequest, SearchDisputeResponse>(searchRequest, cancellationToken);
            
            var searchResponse = response.Message;

            if (searchResponse == null)
            {
                _logger.LogError("Search response is null, throwing DisputeSearchFailedException");
                throw new DisputeSearchFailedException(ticketNumber);
            }

            if (!searchResponse.IsNotFound)
            {
                _logger.LogDebug("Found a dispute for the given ticket number: {ticketNumber}, returning bad request", ticketNumber);
                return true;
            }

            if (searchResponse.IsError)
            {
                _logger.LogError("Search returned error, throwing DisputeSearchFailedException");
                throw new DisputeSearchFailedException(ticketNumber);
            }

            _logger.LogDebug("Dispute not submitted before, returning false");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking if dispute submitted before");
            throw;
        }
    }
}

[Serializable]
public class InvalidTicketVersionException : Exception
{
    public InvalidTicketVersionException(DateTime violationDate) : base($"Invalid ticket found with violation date: {violationDate}. The version of the Ticket is not VT2 since its violation date is before April 9, 2024")
    {
        ViolationDate = violationDate;
    }

    public DateTime ViolationDate { get; init; }
}

[Serializable]
public class DisputeSearchFailedException : Exception
{
    public DisputeSearchFailedException(string ticketNumber) : base($"Dispute search failed. Dispute search response returned an error for: {ticketNumber}.")
    {
        TicketNumber = ticketNumber;
    }

    public string TicketNumber { get; set; }
}

