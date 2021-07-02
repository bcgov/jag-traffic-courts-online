using Gov.TicketSearch.Models;
using Gov.TicketSearch.Services;
using Gov.TicketSearch.Services.RsiServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;

        public TicketsController(ILogger<TicketsController> logger, ITicketsService ticketsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketsService;
        }

        // GET: api/<TicketsController>
        [HttpGet]
        [Produces("application/json")]
        public async Task<TicketSearchResponse> Get([FromQuery]TicketSearchRequest searchRequest)
        {
            _logger.LogInformation("Get ticket search request");            
            RawTicketSearchResponse response = await _ticketsService.SearchTicketAsync(searchRequest.TicketNumber, searchRequest.Time, new CancellationToken());
            return BuildTicketSearchResponse(response);
        }

        private TicketSearchResponse BuildTicketSearchResponse(RawTicketSearchResponse rawResponse)
        {
            if (rawResponse.Items == null || rawResponse.Items.Count <= 0) return null;
            TicketSearchResponse searchResponse = new TicketSearchResponse();
            Invoice firstInvoice = rawResponse.Items.First().SelectedInvoice.Invoice;
            searchResponse.ViolationTicketNumber =
                        firstInvoice.InvoiceNumber.Remove(firstInvoice.InvoiceNumber.Length - 1);
            searchResponse.ViolationDate = DateTime.Parse(firstInvoice.ViolationDateTime).ToString("yyyy-MM-dd");
            searchResponse.ViolationTime = DateTime.Parse(firstInvoice.ViolationDateTime).ToString("HH:mm");
            searchResponse.Offences = rawResponse.Items.Select((_, i) => new Offence
            {
                OffenceNumber = (int)Char.GetNumericValue(_.SelectedInvoice.Invoice.InvoiceNumber.Last()),
                AmountDue = _.SelectedInvoice.Invoice.AmountDue,
                OffenceDescription = _.SelectedInvoice.Invoice.OffenceDescription,
                InvoiceType = _.SelectedInvoice.Invoice.InvoiceType,
                VehicleDescription = _.SelectedInvoice.Invoice.VehicleDescription,
                ViolationDateTime = _.SelectedInvoice.Invoice.ViolationDateTime,
                TicketedAmount = _.SelectedInvoice.Invoice.TicketedAmount,
                DiscountAmount = (_.SelectedInvoice.Invoice.DiscountAmount == Keys.Nothing)
                    ? 0
                    : Convert.ToDecimal(_.SelectedInvoice.Invoice.DiscountAmount),
                DiscountDueDate = (_.SelectedInvoice.Invoice.DiscountAmount == Keys.Nothing)
                    ? null
                    : DateTime.Parse(firstInvoice.ViolationDateTime).AddDays(Keys.TicketDiscountValidDays).ToString("yyyy-MM-dd")
            }).ToList();
            return searchResponse;
        }
    }
}
