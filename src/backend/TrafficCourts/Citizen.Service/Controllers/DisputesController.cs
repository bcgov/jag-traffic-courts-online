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
        private readonly IMediator _mediator;
        private readonly ILogger<DisputesController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="mediator"/> or <paramref name="logger"/> is null.</exception>
        public DisputesController(IMediator mediator, ILogger<DisputesController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// An endpoint for creating and saving dispute ticket data
        /// </summary>
        /// <param name="dispute"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// <response code="400">The request was not well formed. Check the parameters.</response>
        /// <response code="500">There was a internal server error that prevented creation of the dispute.</response>
        /// </returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] Models.Dispute.NoticeOfDispute dispute, CancellationToken cancellationToken)
        {
            // Validation for Fine Reduction Reason field
            if (dispute.DisputedCounts.Select(_ => _.RequestReduction).Any() == true && string.IsNullOrEmpty(dispute.FineReductionReason))
            {
                string exception = string.Concat(nameof(dispute.FineReductionReason), " cannot be null since RequestReduction set to true for one of the counts");
                _logger.LogInformation(exception);
                return StatusCode(StatusCodes.Status400BadRequest, exception);
            }

            // Validation for Time To Pay Reason field
            if (dispute.DisputedCounts.Select(_ => _.RequestTimeToPay).Any() == true && string.IsNullOrEmpty(dispute.TimeToPayReason))
            {
                string exception = string.Concat(nameof(dispute.TimeToPayReason), " cannot be null since RequestTimeToPay set to true for one of the counts");
                _logger.LogInformation(exception);
                return StatusCode(StatusCodes.Status400BadRequest, exception);
            }

            // Validation for the Disputed Counts
            if (dispute.DisputedCounts.Count < 1 || dispute.DisputedCounts.Count > 3)
            {
                string exception = string.Concat(nameof(dispute.DisputedCounts), " must be minimum 1 and maximum 3 counts");
                _logger.LogInformation(exception);
                return StatusCode(StatusCodes.Status400BadRequest, exception);
            }

            Create.Request request = new Create.Request(dispute);
            Create.Response response = await _mediator.Send(request, cancellationToken);

            if (response.Exception is not null)
            {
                _logger.LogInformation(response.Exception, "Failed to create Notice of Dispute");
                return StatusCode(StatusCodes.Status500InternalServerError, response.Exception);
            }

            return Ok(response);
        }
    }
}
