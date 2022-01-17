using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Tickets;

namespace TrafficCourts.Citizen.Service.Controllers
{

    [ApiController]
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

        [HttpGet("search")]
        [ProducesResponseType(typeof(Search.Response), (int)HttpStatusCode.OK)]
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
            Search.Request request = new Search.Request(ticketNumber, time);
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
        [DisableRequestSizeLimit]
        public Task<Analyse.AnalyseResponse> AnalyseSync([Required] IFormFile image, CancellationToken cancellationToken)
        {
            Analyse.AnalyseRequest request = new Analyse.AnalyseRequest(image);
            return _mediator.Send(request, cancellationToken);
        }
    }
}
