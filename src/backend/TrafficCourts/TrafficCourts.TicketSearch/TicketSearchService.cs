using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using TrafficCourts.Domain.Models;
using TrafficCourts.TicketSearch.Common;

namespace TrafficCourts.TicketSearch;

public partial class TicketSearchService : ITicketSearchService
{
    private const string _mva = "MVA";
    private const string _mvar = "MVAR";
    private const string _mvr = "MVR";
    private const string _formNumber = "^MV6000E\\s*\\(\\s*040924\\s*\\)$";
    private readonly ITicketInvoiceSearchService _invoiceSearchService;
    private readonly ILogger<TicketSearchService> _logger;

    public TicketSearchService(ITicketInvoiceSearchService invoiceSearchService, ILogger<TicketSearchService> logger)
    {
        _invoiceSearchService = invoiceSearchService ?? throw new ArgumentNullException(nameof(invoiceSearchService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    public async Task<Ticket?> SearchAsync(string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ticketNumber);

        using (Activity activity = new("ticket search"))
        {
            activity.Start();

            var response = await SearchAsync(activity, ticketNumber, issuedTime, cancellationToken);
            return response;
        }
    }

    private async Task<Ticket?> SearchAsync(Activity activity, string ticketNumber, TimeOnly issuedTime, CancellationToken cancellationToken)
    {
        var time = $"{issuedTime.Hour:d2}:{issuedTime.Minute:d2}";

        using var scope = _logger.BeginScope(new Dictionary<string, string> { { "TicketNumber", ticketNumber }, { "Time", time } });

        try
        {
            _logger.LogDebug("Searching for violation ticket");

            // call the ticket service
            IEnumerable<Invoice>? response = await _invoiceSearchService.SearchAsync(ticketNumber, issuedTime, cancellationToken);

            var invoices = response.ToList();

            // TCVP-2563 New requirement, only filter if ticketNumber starts with "E"
            if (ticketNumber.StartsWith('E')) {
                var count = invoices.Count;

                // TCVP-2651 Filter results to only MVA/MVAR violation tickets 
                invoices = invoices.Where(_ => _.Act == _mva || _.Act == _mvr || _.Act == _mvar).ToList();
                if (invoices.Count != count) {
                    _logger.LogDebug("Dropped non MVA/MVAR violation ticket(s) from the RSI search results");
                    activity?.AddTag("unsupported.act.dropped.counts", count - invoices.Count);
                    count = invoices.Count;
                }

                // TCVP-2563 Filter results to only those whose Form Number is "MV6000E (040924)" or "MV6000E(040924)".
                invoices = invoices.Where(_ => _.FormNumber is not null && Regex.IsMatch(_.FormNumber, _formNumber, RegexOptions.IgnoreCase)).ToList();
                if (invoices.Count != count) {
                    _logger.LogDebug("Dropped invalid Form Number violation ticket(s) from the RSI search results");
                    activity?.AddTag("unsupported.form.number.dropped.counts", count - invoices.Count);
                }
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
        catch (Exception exception)
        {
            _logger.LogInformation(exception, "Error finding violation ticket");
            activity?.SetStatus(ActivityStatusCode.Error, exception?.Message);
            throw new TicketSearchErrorException("Error finding violation ticket", exception);
        }

    }

    private Ticket AssembleViolationTicket(string ticketNumber, TimeOnly issuedTime, List<Invoice> invoices)
    {
        Debug.Assert(invoices.Count != 0);

        if (string.IsNullOrEmpty(invoices[0].ViolationDateTime))
        {
            _logger.LogInformation("Violation date and time is empty");
        }

        Ticket ticket = new(ticketNumber);

        if (DateTime.TryParseExact(invoices[0].ViolationDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime violationDateTime))
        {
            ticket.Issued = DateTime.SpecifyKind(violationDateTime, DateTimeKind.Unspecified);            
        }
        ticket.Surname = invoices[0].PartySurname ?? string.Empty;
        ticket.FirstGivenName = invoices[0].PartyFirstGivenName ?? string.Empty;
        ticket.SecondGivenName = invoices[0].PartySecondGivenName ?? string.Empty;

        foreach (var invoice in invoices)
        {
            Count count = new();

            if (!string.IsNullOrEmpty(invoice.InvoiceNumber))
            {
                count.Number = (int)char.GetNumericValue(invoice.InvoiceNumber[^1]);
            }


            count.TicketedAmount = invoice.TicketedAmount;
            count.AmountDue = invoice.AmountDue;
            count.DiscountAmount = GetDiscountAmount(invoice);

            if (string.IsNullOrEmpty(count.Act) && !string.IsNullOrEmpty(invoice.Act)) count.Act = invoice.Act;
            count.FormNumber = invoice.FormNumber;
        
            // 
            if (string.IsNullOrEmpty(invoice.Section) || !LegalSection.TryParse(invoice.Section, out LegalSection? legalSection))
            {
                legalSection = new(); // force all spaces
            }

            count.Section = legalSection.Section;
            count.Subsection = legalSection.Subsection;
            count.Paragraph = legalSection.Paragraph;
            count.Subparagraph = legalSection.Subparagraph;

            if (!string.IsNullOrEmpty(invoice.OffenceDescription))
            {
                count.Description = invoice.OffenceDescription;
            }

            ticket.Counts.Add(count);
        }

        return ticket;
    }

    private decimal? GetDiscountAmount(Invoice invoice)
    {
        if (!string.IsNullOrEmpty(invoice.DiscountAmount))
        {
            if (invoice.DiscountAmount != "n/a")
            {
                if (decimal.TryParse(invoice.DiscountAmount, out decimal discountAmount))
                {
                    return discountAmount;
                }
                else
                {
                    _logger.LogInformation("Invoice discount amount is not a valid decimal value, the value was {InvalidDiscountAmount}", invoice.DiscountAmount);
                }
            }
        }
        else
        {
            _logger.LogInformation("Invoice discount amount is empty");
        }

        return null;
    }
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
