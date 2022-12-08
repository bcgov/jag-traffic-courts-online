using HashidsNet;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Common;
using TrafficCourts.Common.Features.EmailVerificationToken;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Citizen.Service.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DisputesController : ControllerBase
{
    private readonly IBus _bus;
    private readonly IMediator _mediator;
    private readonly ILogger<DisputesController> _logger;
    private readonly IHashids _hashids;
    private readonly IDisputeEmailVerificationTokenEncoder _tokenEncoder;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bus"></param>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    /// <param name="hashids"></param>
    /// <param name="tokenEncoder"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="mediator"/> or <paramref name="logger"/> is null.</exception>
    public DisputesController(IBus bus, IMediator mediator, ILogger<DisputesController> logger, IHashids hashids, IDisputeEmailVerificationTokenEncoder tokenEncoder)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
        _tokenEncoder = tokenEncoder ?? throw new ArgumentNullException(nameof(tokenEncoder));
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    /// <param name="uuidHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// <response code="202">Resend email acknowledged.</response>
    /// <response code="400">The uuid doesn't appear to be a valid UUID.</response>
    /// <response code="500">There was a internal server error when triggering an email to resend.</response>
    /// </returns>
    [HttpPut("/api/disputes/email/{uuidHash}/resend")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ResendEmailAsync(string uuidHash, CancellationToken cancellationToken)
    {
        var hex = _hashids.DecodeHex(uuidHash);
        if (hex == String.Empty || hex.Length != 32 || !Guid.TryParse(hex, out Guid noticeOfDisputeGuid))
        {
            return BadRequest("Invalid dispute id");
        }

        var message = new ResendEmailVerificationEmail { NoticeOfDisputeGuid = noticeOfDisputeGuid };
        await _bus.PublishWithLog(_logger, message, cancellationToken);

        return Accepted();
    }

    /// <summary>
    /// Verifies email address based on token sent to a Disputant.
    /// </summary>
    /// <param name="token">The email verification token.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("/api/disputes/email/verify")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyEmailAsync([Required] string token, CancellationToken cancellationToken)
    {
        if (!_tokenEncoder.TryDecode(token, out DisputeEmailVerificationToken? verificationToken))
        {
            _logger.LogDebug("Invalid verification token");
            return BadRequest("Invalid verification token");
        }

        using var scope = _logger.BeginScope(verificationToken, _ => _.NoticeOfDisputeGuid);

        CheckEmailVerificationTokenRequest emailVerificationReceived = new CheckEmailVerificationTokenRequest
        {
            NoticeOfDisputeGuid = verificationToken.NoticeOfDisputeGuid,
            Token = verificationToken.Token
        };

        try
        {
            var response = await _bus.Request<CheckEmailVerificationTokenRequest, CheckEmailVerificationTokenResponse>(emailVerificationReceived, cancellationToken);

            IActionResult result;
            switch (response.Message.Status)
            {
                case CheckEmailVerificationTokenStatus.NotFound:
                    _logger.LogDebug("No active email verification found");
                    result = NotFound("No active email verification found");
                    break;

                case CheckEmailVerificationTokenStatus.Invalid:
                    _logger.LogDebug("Token not valid");
                    result = BadRequest("Token not valid");
                    break;
                case CheckEmailVerificationTokenStatus.Valid:
                    _logger.LogDebug("Token valid");
                    result = Ok();
                    break;
                default:
                    _logger.LogError("Unknown CheckEmailVerificationTokenStatus {CheckEmailVerificationTokenStatus}", response.Message.Status);
                    result = StatusCode(StatusCodes.Status500InternalServerError);
                    break;
            }

            return result;
        }
        catch (RequestTimeoutException ex)
        {
            _logger.LogError(ex, "Request Timed out");
            throw;
        }
    }

    [HttpGet("/api/disputes/search")]
    [ProducesResponseType(typeof(SearchDisputeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchDisputeAsync(
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TicketNumberRegex, ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")] string ticketNumber,
            [FromQuery]
            [Required]
            [RegularExpression(Search.Request.TimeRegex, ErrorMessage = "time must be properly formatted 24 hour clock")] string time,
            CancellationToken cancellationToken)
    {

        try
        {
            var message = new SearchDisputeRequest { TicketNumber = ticketNumber, IssuedTime = time };
            var response = await _bus.Request<SearchDisputeRequest, SearchDisputeResponse>(message, cancellationToken);
            IActionResult result;

            if (!string.IsNullOrEmpty(response.Message.DisputeId))
            {
                _logger.LogDebug("Dispute found");
                var disputeId = _hashids.EncodeHex(response.Message.DisputeId);
                _ = Enum.TryParse(response.Message.DisputeStatus, out DisputeStatus disputeStatus);
                _ = Enum.TryParse(response.Message.JJDisputeStatus, out JJDisputeStatus jjDisputeStatus);
                SearchDisputeResult searchResult = new()
                {
                    DisputeId = disputeId,
                    DisputeStatus = disputeStatus,
                    JJDisputeStatus = jjDisputeStatus
                };

                result = Ok(searchResult);
            }
            else if (response.Message.IsError)
            {
                _logger.LogError("Unknown search dispute error");
                result = StatusCode(StatusCodes.Status500InternalServerError);
            }
            else
            {
                _logger.LogDebug("Dispute not found");
                result = NotFound("Dispute not found");
            }

            return result;
        }
        catch (RequestTimeoutException ex)
        {
            _logger.LogError(ex, "Request Timed out");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown Error");
            throw;
        }
    }
}
