using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Disputes;

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
    }
}
