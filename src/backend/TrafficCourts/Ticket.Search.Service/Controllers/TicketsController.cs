using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Ticket.Search.Service.Features.Search;

namespace TrafficCourts.Ticket.Search.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [ProducesResponseType(typeof(TicketSearch.Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Get([FromQuery] TicketSearch.Request searchRequest)
        {
            var response = await _mediator.Send(searchRequest);

            if (response == TicketSearch.Response.NotFound)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }
}
