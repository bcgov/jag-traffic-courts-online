using Gov.TicketSearch.Models;
using Gov.TicketSearch.Services;
using Gov.TicketSearch.Services.RsiServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
            _ticketsService = ticketsService ?? throw new ArgumentNullException(nameof(ticketsService));
        }

        // GET: api/<TicketsController>
        [HttpGet]
        [ProducesResponseType(typeof(TicketSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery]TicketSearchRequest searchRequest)
        {
            _logger.LogInformation("Get ticket search request");
            try
            {
                RawTicketSearchResponse response = await _ticketsService.SearchTicketAsync(searchRequest.TicketNumber, searchRequest.Time, new CancellationToken());
                _logger.LogInformation("Get Raw ticket search response successfully");
                var searchResponse = BuildTicketSearchResponse(response);
                _logger.LogInformation("Build ticket search response successfully");
                if (searchResponse == null) return NoContent();
                return Ok(searchResponse);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Search Ticket failed.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { ReasonCode = "error", Message = ex.Message });
            }
        }

        private TicketSearchResponse BuildTicketSearchResponse(RawTicketSearchResponse rawResponse)
        {
            //string rawResponseJsonStr = JS
            //_logger.LogDebug("Raw response = ")
            if (rawResponse?.Items == null || rawResponse.Items.Count <= 0) return null;
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
