using Refit;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TrafficCourts.Ticket.Search.Service.Features.Search.RoadSafetyTicketSearch
{
    public interface IRoadSafetyTicketSearchApi
    {
        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/")]
        Task<RawTicketSearchResponse> GetTicket(GetTicketParams ticketParams, CancellationToken cancellationToken);

        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/{invoiceNumber}")]
        Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken);
    }

    public class RoadSafetyTicketSearchService : ITicketSearchService
    {
        private readonly IRoadSafetyTicketSearchApi _api;
        private readonly ILogger<RoadSafetyTicketSearchService> _logger;

        public RoadSafetyTicketSearchService(IRoadSafetyTicketSearchApi api, ILogger<RoadSafetyTicketSearchService> logger)
        {
            _api = api ?? throw new ArgumentNullException(nameof(api));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<IEnumerable<Invoice>> SearchTicketAsync(string ticketNumber, string time, CancellationToken cancellationToken)
        {
            var response = await _api.GetTicket(GetParameters(ticketNumber, time), cancellationToken);
            if (response.Items == null && !string.IsNullOrEmpty(response.Error))
            {
                // expected: "Traffic violation ticket not found. Traffic violation ticket numbers that begin with 'E' or 'S' are available to pay online. Please allow 48 hours for your ticket to appear. If you wish to pay now and your ticket is not available, please use one of the other methods of payments listed on the ticket"
                if (response.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogDebug("Ticket not found, Road Safety service returned error message {Error}", response.Error);
                }
                else
                {
                    // not the error message we were expecting, log out at information level
                    _logger.LogInformation("Road Safety service returned error message {Error}", response.Error);
                }
                
                return Enumerable.Empty<Invoice>();
            }

            if (response != null && response.Items != null)
            {
                IEnumerable<Invoice> invoices = await GetInvoicesAsync(response.Items);
                return invoices;
            }

            return Enumerable.Empty<Invoice>();
        }

        private async Task<IEnumerable<Invoice>> GetInvoicesAsync(IEnumerable<Item> items)
        {
            if (items == null)
            {
                return Enumerable.Empty<Invoice>();
            }

            var invoices = new ConcurrentBag<Invoice>();

            await Parallel.ForEachAsync(items, async (item, cancellationToken) =>
            {
                if (item?.SelectedInvoice?.Reference is not null)
                {
                    string reference = item.SelectedInvoice.Reference;
                    var lastSlash = reference.LastIndexOf('/');
                    if (lastSlash != -1)
                    {
                        string invoiceNumber = reference[(lastSlash + 1)..];

                        var invoice = await _api.GetInvoice(invoiceNumber, cancellationToken);
                        invoices.Add(invoice);
                    }
                }
            });

            return invoices.OrderBy(_ => _.InvoiceNumber);
        }

        private static GetTicketParams GetParameters(string ticketNumber, string time)
        {
            return new GetTicketParams { TicketNumber = ticketNumber, Prn = "10006", IssuedTime = time.Replace(":", "") };
        }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class GetTicketParams
    {
        [AliasAs("in")]
        public string TicketNumber { get; set; } = null!;

        [AliasAs("prn")]
        public string Prn { get; set; } = null!;

        [AliasAs("time")]
        public string IssuedTime { get; set; } = null!;
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class RawTicketSearchResponse
    {
        [JsonPropertyName("items")]
        public List<Item>? Items { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Invoice
    {
        public static readonly Invoice Empty = new();

        [JsonPropertyName("invoice_number")]
        public string? InvoiceNumber { get; set; }

        [JsonPropertyName("pbc_ref_number")]
        public string? PbcRefNumber { get; set; }

        [JsonPropertyName("party_number")]
        public string? PartyNumber { get; set; }

        [JsonPropertyName("party_name")]
        public string? PartyName { get; set; }

        [JsonPropertyName("account_number")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("site_number")]
        public string? SiteNumber { get; set; }

        [JsonPropertyName("cust_trx_type")]
        public string? InvoiceType { get; set; }

        [JsonPropertyName("term_due_date")]
        public string? ViolationDateTime { get; set; }

        [JsonPropertyName("total")]
        public decimal TicketedAmount { get; set; }

        [JsonPropertyName("amount_due")]
        public decimal AmountDue { get; set; }

        [JsonPropertyName("attribute1")]
        public string? OffenceDescription { get; set; }

        [JsonPropertyName("attribute2")]
        public string? VehicleDescription { get; set; }

        [JsonPropertyName("attribute3")]
        public string? ViolationDate { get; set; }

        [JsonPropertyName("attribute4")]
        public string? DiscountAmount { get; set; }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Item
    {
        [JsonPropertyName("selected_invoice")]
        public SelectedInvoice? SelectedInvoice { get; set; }

        [JsonPropertyName("open_invoices_for_site")]
        public object? OpenInvoicesForSite { get; set; }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class SelectedInvoice
    {
        [JsonPropertyName("$ref")]
        public string? Reference { get; set; }

        [JsonPropertyName("invoice")]
        public Invoice? Invoice { get; set; }
    }
}
