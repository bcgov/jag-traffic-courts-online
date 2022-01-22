using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Models;
using TrafficCourts.Citizen.Service.Models.Search;

namespace TrafficCourts.Citizen.Service.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(IMediator mediator, ILogger<TicketsController> logger)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(logger);

            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// This API is depricated. Use /api/tickets/search instead.
        /// </summary>
        /// <param name="ticketNumber">The violation ticket number. Must start with two upper case letters and end with eight digits.</param>
        /// <param name="time">The time the violation ticket number was issued. Must be formatted a valid 24-hour clock, HH:MM.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [Obsolete($"Use {nameof(SearchAsync)}")]
        [HttpGet("ticket")]
        [ProducesResponseType(typeof(Models.Deprecated.TicketDispute), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> TicketAsync(
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TicketNumberRegex, ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")] string ticketNumber,
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TimeRegex, ErrorMessage = "time must be properly formatted 24 hour clock")] string time,
            CancellationToken cancellationToken)
        {
            Search.Request request = new(ticketNumber, time);
            Search.Response response = await _mediator.Send(request, cancellationToken);

            if (response == Search.Response.Empty)
            {
                return NotFound();
            }

            var result = response.Result.Match<IActionResult>(
                ticket => { return Ok(ticket.CreateDeprecated()); },
                exception => { return StatusCode(StatusCodes.Status500InternalServerError); });

            return result;
        }

        /// <summary>
        /// Searches for a violation ticket that exists on file.
        /// </summary>
        /// <param name="ticketNumber">The violation ticket number. Must start with two upper case letters and end with eight digits.</param>
        /// <param name="time">The time the violation ticket number was issued. Must be formatted a valid 24-hour clock, HH:MM.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <response code="200">The violation ticket was found.</response>
        /// <response code="400">The request was not well formed. Check the parameters.</response>
        /// <response code="404">The violation ticket was not found.</response>
        /// <response code="500">There was a server error that prevented the search from completing successfully.</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(TicketSearchResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SearchAsync(
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TicketNumberRegex, ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")] string ticketNumber,
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TimeRegex, ErrorMessage = "time must be properly formatted 24 hour clock")] string time,
            CancellationToken cancellationToken)
        {
            Search.Request request = new(ticketNumber, time);
            Search.Response response = await _mediator.Send(request, cancellationToken);

            if (response == Search.Response.Empty)
            {
                return NotFound();
            }

            var result = response.Result.Match<IActionResult>(
                ticket => { return Ok(ticket); },
                exception => { return StatusCode(StatusCodes.Status500InternalServerError); });

            return result;
        }

        [HttpPost("analyse")]
        [ProducesResponseType(typeof(Search.Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(Search.Response), (int)HttpStatusCode.BadRequest)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> AnalyseAsync([Required] IFormFile image, CancellationToken cancellationToken)
        {
            AnalyseHandler.AnalyseRequest request = new AnalyseHandler.AnalyseRequest(image);
            AnalyseHandler.AnalyseResponse response = await _mediator.Send(request, cancellationToken);
            if (response.OcrViolationTicket is null)
            {
                // Return BadRequest 
                // - if the image is not an image of a TrafficViolation
                // - if the TicketNumber could not be extracted
                return BadRequest();
            }
            return Ok(response.OcrViolationTicket);
        }
    }
}
