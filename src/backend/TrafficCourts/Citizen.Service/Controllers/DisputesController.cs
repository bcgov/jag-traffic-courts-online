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
        /// <returns>
        /// <response code="400">The request was not well formed. Check the parameters.</response>
        /// <response code="500">There was a server error that prevented the search from completing successfully.</response>
        /// </returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] Models.Dispute.NoticeOfDispute dispute, CancellationToken cancellationToken)
        {
            Create.Request request = new Create.Request(dispute);
            Create.Response response = await _mediator.Send(request, cancellationToken);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Exception);
            }

            return Ok(response);
        }
    }
}
