using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Models.Deprecated;

namespace TrafficCourts.Citizen.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DisputesController : ControllerBase
    {
#pragma warning disable IDE0052 // Remove unread private members - will be used in the future
        private readonly IMediator _mediator;
        private readonly ILogger<DisputesController> _logger;
#pragma warning restore IDE0052 // Remove unread private members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="mediator"/> or <paramref name="logger"/> is null.</exception>
        public DisputesController(IMediator mediator, ILogger<DisputesController> logger)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(logger);

            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// An endpoint for creating and saving dispute ticket data
        /// </summary>
        /// <param name="dispute"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync([FromBody] TrafficCourts.Citizen.Service.Models.Dispute.TicketDispute dispute, CancellationToken cancellationToken)
        {
            Create.Request request = new Create.Request(dispute);
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }


        [Obsolete]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TicketDispute([FromBody] CreateDisputeCommand createDisputeCommand)
        {
            //var response = await _mediator.Send(createDisputeCommand);
            //if (response.Id == 0)
            //{
            //    ModelState.AddModelError("TicketNumber", "the dispute already exists for this ticket.");
            //    return BadRequest(ApiResponse.BadRequest(ModelState));
            //}
            //return RedirectToAction("Ticket", "Tickets", new { ticketNumber = createDisputeCommand.ViolationTicketNumber, time = createDisputeCommand.ViolationTime });
            return NoContent();
        }

        [Obsolete]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OffenceDispute([FromBody] CreateOffenceDisputeCommand createOffenceDisputeCommand)
        {
            //var response = await _mediator.Send(createOffenceDisputeCommand);
            //if (response.Id == 0)
            //{
            //    ModelState.AddModelError("DisputeOffenceNumber", "Cannot find the offence.");
            //    return BadRequest(ApiResponse.BadRequest(ModelState));
            //}
            //return Ok();
            return NoContent();
        }

        [Obsolete]
        [HttpGet]
        [ProducesResponseType(typeof(IList<Dispute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDisputes()
        {
            //var disputes = await _mediator.Send(new GetAllDisputesQuery());

            //return disputes == null ? NoContent() : Ok(ApiResponse.Result(disputes));
            return NoContent();
        }

        [Obsolete]
        [HttpGet("{disputeId}")]
        [ProducesResponseType(typeof(IList<Dispute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDispute(int disputeId)
        {
            // Note: the ProducesResponseType if list of dispute would be wrong... but keeping it to maintain backward compatibility with generated front end code 

            //var dispute = await _mediator.Send(new GetDisputeQuery { DisputeId = disputeId });
            //return dispute == null ? NoContent() : Ok(ApiResponse.Result(dispute));
            return NoContent();
        }

    }
}
