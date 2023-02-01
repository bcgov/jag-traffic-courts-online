using AutoMapper;
using HashidsNet;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Citizen.Service.Models.OAuth;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Common;
using TrafficCourts.Common.Errors;
using TrafficCourts.Common.Features.EmailVerificationToken;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Messaging.Models;
using DisputantContactInformation = TrafficCourts.Citizen.Service.Models.Disputes.DisputantContactInformation;
using DisputantUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputantUpdateRequest;
using Dispute = TrafficCourts.Citizen.Service.Models.Disputes.Dispute;

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
    private readonly IOAuthUserService _oAuthUserService;
    private readonly IMapper _mapper;
    private readonly ICitizenDocumentService _documentService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bus"></param>
    /// <param name="mediator"></param>
    /// <param name="logger"></param>
    /// <param name="hashids"></param>
    /// <param name="tokenEncoder"></param>
    /// <param name="oAuthUserService"></param>
    /// <param name="mapper"></param>
    /// <param name="comsService"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="mediator"/> or <paramref name="logger"/> is null.</exception>
    public DisputesController(IBus bus, IMediator mediator, ILogger<DisputesController> logger, IHashids hashids, IDisputeEmailVerificationTokenEncoder tokenEncoder, IOAuthUserService oAuthUserService, IMapper mapper, ICitizenDocumentService documentService)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
        _tokenEncoder = tokenEncoder ?? throw new ArgumentNullException(nameof(tokenEncoder));
        _oAuthUserService = oAuthUserService ?? throw new ArgumentNullException(nameof(oAuthUserService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
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
    public async Task<IActionResult> CreateAsync([FromBody] Models.Disputes.NoticeOfDispute dispute, CancellationToken cancellationToken)
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
    /// <param name="guidHash"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// <response code="202">Resend email acknowledged.</response>
    /// <response code="400">The uuid doesn't appear to be a valid UUID.</response>
    /// <response code="500">There was a internal server error when triggering an email to resend.</response>
    /// </returns>
    [HttpPut("/api/disputes/email/{guidHash}/resend")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ResendEmailAsync(string guidHash, CancellationToken cancellationToken)
    {
        if (!_hashids.TryDecodeGuid(guidHash, out Guid noticeOfDisputeGuid))
        {
            return BadRequest("Invalid guidHash");
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
    [ProducesResponseType(typeof(SearchDisputeResult), StatusCodes.Status200OK)]
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

            if (response.Message.NoticeOfDisputeGuid is not null)
            {
                var token = _hashids.EncodeHex(response.Message.NoticeOfDisputeGuid.Value.ToString("n"));
                _ = Enum.TryParse(response.Message.DisputeStatus, out DisputeStatus disputeStatus);
                _ = Enum.TryParse(response.Message.JJDisputeStatus, out JJDisputeStatus jjDisputeStatus);
                _ = Enum.TryParse(response.Message.HearingType, out JJDisputeHearingType hearingType);

                SearchDisputeResult searchResult = new()
                {
                    NoticeOfDisputeGuid = token,
                    DisputeStatus = disputeStatus,
                    JJDisputeStatus = jjDisputeStatus,
                    HearingType = hearingType
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


    [Authorize]
    [HttpGet("/api/disputes/{guidHash}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisputeAsync(string guidHash, CancellationToken cancellationToken)
    {
        try
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (String.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid access_token");
            }

            if (!_hashids.TryDecodeGuid(guidHash, out Guid noticeOfDisputeGuid))
            {
                return BadRequest("Invalid guidHash");
            }

            var check = await CheckDisputeStatus(noticeOfDisputeGuid, cancellationToken);
            if (!String.IsNullOrEmpty(check))
            {
                return BadRequest(check);
            }

            var user = await _oAuthUserService.GetUserInfoAsync<UserInfo>(token, cancellationToken);
            var message = new GetDisputeRequest();
            message.NoticeOfDisputeGuid = noticeOfDisputeGuid;
            var response = await _bus.Request<GetDisputeRequest, SubmitNoticeOfDispute>(message, cancellationToken);
            if (response is null || response.Message is null || String.IsNullOrEmpty(response.Message.TicketNumber))
            {
                return NotFound("Dispute not found");
            }

            var givenNames = response.Message.ContactGiven1Nm + " " + response.Message.ContactGiven2Nm + " " + response.Message.ContactGiven3Nm;
            if (response.Message.ContactSurnameNm != user?.Surname
                || !(response.Message.ContactGiven1Nm == user?.GivenName || givenNames.TrimEnd() == user?.GivenNames))
            {
                return BadRequest("Disputant not match");
            }

            var result = _mapper.Map<NoticeOfDispute>(response.Message);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown Error");
            throw;
        }
    }

    /// <summary>
    /// Downloads a document for the given unique file ID if the virus scan staus is clean.
    /// </summary>
    /// <param name="fileId">Unique identifier for a specific document.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The document is successfully downloaded.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires jjdispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the file to be downloaded successfully.</response>
    /// <returns>The document</returns>
    [Authorize]
    [HttpGet("/api/disputes/downloadDocument")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DownloadDocumentAsync(Guid fileId, CancellationToken cancellationToken)
    {
        try
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (String.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid access_token");
            }

            Coms.Client.File file = await _documentService.GetFileAsync(fileId, cancellationToken);

            var stream = file.Data;
            // Reset position to the beginning of the stream
            stream.Position = 0;

            return File(stream, file.ContentType ?? "application/octet-stream", file.FileName ?? "download");
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogError(e, "User is unauthorized. Invalid Access Token");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.Unauthorized;
            problemDetails.Title = e.Source + ": Exception Authorizing User";
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.ObjectManagementServiceException e)
        {
            _logger.LogError(e, "Could not download the document because of ObjectManagementServiceException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Title = e.Source + ": Error getting file from COMS";
            problemDetails.Instance = HttpContext?.Request?.Path;
            string? innerExceptionMessage = e.InnerException?.Message;
            if (innerExceptionMessage is not null)
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message, innerExceptionMessage });
            }
            else
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message });
            }

            return new ObjectResult(problemDetails);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving the document");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [Authorize]
    [HttpPut("/api/disputes/{guidHash}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDisputeAsync(string guidHash, [FromBody] Dispute dispute, CancellationToken cancellationToken)
    {
        try
        {
            var token = HttpContext.Request.Headers.Authorization.FirstOrDefault();
            if (String.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Invalid access_token");
            }

            if (!_hashids.TryDecodeGuid(guidHash, out Guid noticeOfDisputeGuid))
            {
                return BadRequest("Invalid guidHash");
            }

            var check = await CheckDisputeStatus(noticeOfDisputeGuid, cancellationToken);
            if (!String.IsNullOrEmpty(check))
            {
                return BadRequest(check);
            }

            var user = await _oAuthUserService.GetUserInfoAsync<UserInfo>(token, cancellationToken);
            var message = new GetDisputeRequest();
            message.NoticeOfDisputeGuid = noticeOfDisputeGuid;
            var response = await _bus.Request<GetDisputeRequest, SubmitNoticeOfDispute>(message, cancellationToken);
            if (response is null || response.Message is null || String.IsNullOrEmpty(response.Message.TicketNumber))
            {
                return NotFound("Dispute not found");
            }

            var givenNames = response.Message.ContactGiven1Nm + " " + response.Message.ContactGiven2Nm + " " + response.Message.ContactGiven3Nm;
            if (response.Message.ContactSurnameNm != user?.Surname
                || !(response.Message.ContactGiven1Nm == user?.GivenName || givenNames.TrimEnd() == user?.GivenNames))
            {
                return BadRequest("Contact name does not match");
            }

            // Submit request to Workflow Service for processing.
            DisputantUpdateRequest request = _mapper.Map<DisputantUpdateRequest>(dispute);
            request.NoticeOfDisputeGuid = noticeOfDisputeGuid;
            await _bus.PublishWithLog(_logger, request, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown Error");
            throw;
        }
    }

    /// <summary>
    /// Submits an update request for a Disputant's contact information.
    /// </summary>
    /// <param name="guidHash">A hash of the noticeOfDisputeGuid.</param>
    /// <param name="disputantContactInformation">The requested fields to update.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute is updated.</response>
    [HttpPut("/api/dispute/{guidHash}/contact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDisputeContactInfoAsync([Required] string guidHash, [FromBody] DisputantContactInformation disputantContactInformation, CancellationToken cancellationToken)
    {
        try
        {
            if (!_hashids.TryDecodeGuid(guidHash, out Guid noticeOfDisputeGuid))
            {
                return BadRequest("Invalid guidHash");
            }

            var check = await CheckDisputeStatus(noticeOfDisputeGuid, cancellationToken);
            if (!String.IsNullOrEmpty(check))
            {
                return BadRequest(check);
            }

            // Submit request to Workflow Service for processing.
            DisputantUpdateRequest message = _mapper.Map<DisputantUpdateRequest>(_mapper.Map<DisputantUpdateContactRequest>(disputantContactInformation));
            message.NoticeOfDisputeGuid = noticeOfDisputeGuid;
            await _bus.PublishWithLog(_logger, message, cancellationToken);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unknown Error");
            throw;
        }
    }

    private async Task<string> CheckDisputeStatus(Guid noticeOfDisputeGuid, CancellationToken cancellationToken)
    {
        // Verify Dispute exists and whose status is one of [NEW, VALIDATED, or PROCESSING] and there is no corresponding JJDispute
        string[] validStatuses = { DisputeStatus.NEW.ToString(), DisputeStatus.VALIDATED.ToString(), DisputeStatus.PROCESSING.ToString() };
        SearchDisputeRequest searchRequest = new() { NoticeOfDisputeGuid = noticeOfDisputeGuid };
        Response<SearchDisputeResponse> response = await _bus.Request<SearchDisputeRequest, SearchDisputeResponse>(searchRequest, cancellationToken);
        if (response.Message?.NoticeOfDisputeGuid is null)
        {
            return "Dispute not found";
        }
        else if (response.Message?.DisputeStatus is not null && !validStatuses.Contains(response.Message.DisputeStatus))
        {
            return $"Dispute has a status of {response.Message.DisputeStatus}. Expecting one of NEW, VALIDATED, or PROCESSING.";
        }
        else if (response.Message?.JJDisputeStatus is not null)
        {
            return $"JJDispute has status of {response.Message?.JJDisputeStatus}. Must be blank.";
        }
        else
        {
            return "";
        }
    }
}
