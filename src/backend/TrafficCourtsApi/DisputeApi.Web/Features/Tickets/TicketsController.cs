using System;
using System.Threading.Tasks;
using DisputeApi.Web.Features.Tickets.Queries;
using DisputeApi.Web.Models;
using Gov.TicketSearch;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace DisputeApi.Web.Features.Tickets
{
    [Route("api/[controller]")]
    [ApiController]
    [OpenApiTag("Dispute API")]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;
        private readonly IMediator _mediator;

        public TicketsController(ILogger<TicketsController> logger, ITicketsService ticketsService, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketsService ?? throw new ArgumentNullException(nameof(ticketsService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        //[HttpPost]
        //[Route("saveticket")]
        //[Produces("application/json")]
        //[ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
        //public async Task<IActionResult> SaveTicket([FromBody] Ticket ticket)
        //{
        //    return Ok(await _ticketsService.SaveTicket(ticket));
        //}

        //[HttpGet]
        //[Route("getTickets")]
        //[Produces("application/json")]
        //[ProducesResponseType(typeof(IQueryable<Ticket>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetTickets()
        //{
        //    return Ok(await _ticketsService.GetTickets());
        //}


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiBadRequestResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTicket([FromQuery]TicketSearchQuery query)
        {
            try
            {
                _logger.LogInformation("get ticket search query.");
                var response = await _mediator.Send(query);
                return response == null ? NoContent() : Ok(ApiResponse.Result(response));
            }
            catch (TicketSearchException exception) when (exception.StatusCode == 204)
            {
                return NoContent();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "GetTicket failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            }
        }
    }
}
