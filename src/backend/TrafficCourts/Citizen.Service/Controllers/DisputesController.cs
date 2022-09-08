using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Citizen.Service.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DisputesController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IMediator _mediator;
    private readonly ILogger<DisputesController> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bus"></param>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="mediator"/> or <paramref name="logger"/> is null.</exception>
    public DisputesController(IBus bus, IMediator mediator, ILogger<DisputesController> logger)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
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
        Create.Request request = new Create.Request(dispute);
        Create.Response response = await _mediator.Send(request, cancellationToken);

        if (response.Exception is not null)
        {
            _logger.LogInformation(response.Exception, "Failed to create Notice of Dispute");
            return StatusCode(StatusCodes.Status500InternalServerError, response.Exception);
        }

        return Ok(response);
    }

    /// <summary>
    /// An endpoint for resending an email to a Disputant.
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// <response code="202">Resend email acknowledged.</response>
    /// <response code="400">The uuid doesn't appear to be a valid UUID.</response>
    /// <response code="500">There was a internal server error when triggering an email to resend.</response>
    /// </returns>
    [HttpPut("/api/disputes/email/{uuid}/resend")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ResendEmailAsync(Guid uuid, CancellationToken cancellationToken)
    {
        EmailSendValidation emailSendValidation = new(uuid);
        await _bus.Publish(emailSendValidation, cancellationToken);
        return Accepted();
    }

    /// <summary>
    /// An endpoint for validating an email sent to a Disputant.
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// <response code="202">Validate email acknowledged.</response>
    /// <response code="400">The uuid doesn't appear to be a valid UUID.</response>
    /// <response code="500">There was a internal server error when triggering a received email message.</response>
    /// </returns>
    [HttpPut("/api/disputes/email/{uuid}/validate")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ValidateEmailAsync(Guid uuid, CancellationToken cancellationToken)
    {
        EmailReceivedValidation emailReceiveValidation = new(uuid);
        await _bus.Publish(emailReceiveValidation, cancellationToken);
        return Accepted();
    }
}
