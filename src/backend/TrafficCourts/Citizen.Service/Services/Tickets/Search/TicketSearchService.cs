using System.Diagnostics;
using System.Globalization;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search;

public class TicketSearchService : ITicketSearchService
{
    private readonly ITicketInvoiceSearchService _invoiceSearchService;
    private readonly ILogger<TicketSearchService> _logger;

    public TicketSearchService(ITicketInvoiceSearchService invoiceSearchService, ILogger<TicketSearchService> logger)
    {
        _invoiceSearchService = invoiceSearchService ?? throw new ArgumentNullException(nameof(invoiceSearchService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ViolationTicket?> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
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
            var invoices = response.ToList();

            if (invoices.Count != 0)
            {
                _logger.LogDebug("Found violation ticket with {Count} counts", invoices.Count);

                var reply = AssemblyViolationTicket(ticketNumber, issuedTime, invoices);

                _logger.LogDebug("Search complete");

                return reply;
            }

            _logger.LogDebug("Violation ticket not found");
            activity?.AddTag("status", "Not Found");

            return null;
        }
        catch (Exception exception)
        {
            _logger.LogInformation(exception, "Error finding violation ticket");
            activity?.SetStatus(ActivityStatusCode.Error, exception?.Message);
            throw new TicketSearchErrorException("Error finding violation ticket", exception);
        }
    }

    private ViolationTicket AssemblyViolationTicket(string ticketNumber, TimeOnly issuedTime, List<Invoice> invoices)
    {
        Debug.Assert(invoices.Count != 0);

        if (string.IsNullOrEmpty(invoices[0].ViolationDateTime))
        {
            _logger.LogInformation("Violation date and time is empty");
        }

        ViolationTicket ticket = new ViolationTicket();
        ticket.TicketNumber = ticketNumber;

        if (DateTime.TryParseExact(invoices[0].ViolationDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime violationDateTime))
        {
            ticket.IssuedDate = DateTime.SpecifyKind(violationDateTime, DateTimeKind.Unspecified);
        }

        foreach (var invoice in invoices)
        {
            ViolationTicketCount count = new ViolationTicketCount();
            if (!string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                count.Count = (short)char.GetNumericValue(invoice.InvoiceNumber[^1]);
            }

            count.AmountDue = invoice.AmountDue;
            count.Description = invoice.OffenceDescription;
            count.TicketedAmount = invoice.TicketedAmount;

            ticket.Counts.Add(count);
        }

        return ticket;
    }
}
