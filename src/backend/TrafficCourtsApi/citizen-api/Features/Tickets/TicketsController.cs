using System;
using System.Threading.Tasks;
using Gov.CitizenApi.Features.Tickets.Commands;
using Gov.CitizenApi.Features.Tickets.Queries;
using Gov.CitizenApi.Models;
using Gov.TicketSearch;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace Gov.CitizenApi.Features.Tickets
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [OpenApiTag("Ticket API")]
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

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Ticket([FromQuery]TicketSearchQuery query)
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

        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ShellTicket([FromBody]CreateShellTicketCommand createShellTicket)
        {
            try
            {
                _logger.LogInformation("get create shell ticket request.");
                var response = await _mediator.Send(createShellTicket);
                if(response.Id == 0)
                {
                    ModelState.AddModelError("TicketNumber", "This ticket already exists.");
                    return BadRequest(ApiResponse.BadRequest(ModelState));
                }
                return RedirectToAction("Ticket", new { ticketNumber = createShellTicket.ViolationTicketNumber, time = createShellTicket.ViolationTime });
                //return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "create shell ticket failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            }
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Pay([FromQuery] TicketPaymentCommand ticketPayment)
        {
            try
            {
                _logger.LogInformation("get create ticket payment request.");
                var response = await _mediator.Send(ticketPayment);
                return Ok(ApiResponse.Result(response));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Create ticket payment request failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            }
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType( StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Pay([FromQuery] TicketPaymentConfirmCommand ticketPayConfirm)
        {
            try
            {
                _logger.LogInformation("get ticket payment confirmation");
                var response = await _mediator.Send(ticketPayConfirm);
                return RedirectToAction("Ticket", new { ticketNumber = response.TicketNumber, time = response.Time });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Update ticket payment failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResultResponse<TicketDispute>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImageUpload([FromForm] ShellTicketImageCommand shellTicketImage)
        {
            try
            {
                _logger.LogInformation("get shell ticket image");
                var response = await _mediator.Send(shellTicketImage);
                return Ok();
                //return RedirectToAction("Ticket", new { ticketNumber = response.TicketNumber, time = response.Time });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "save shell ticket image failed");
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse.Message(e.Message));
            }
            
        }
    }
}
