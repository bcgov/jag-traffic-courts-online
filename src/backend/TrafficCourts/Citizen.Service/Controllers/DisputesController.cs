using AutoMapper;
using HashidsNet;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TrafficCourts.Citizen.Service.Features.CurrentUserInfo;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Citizen.Service.Models.OAuth;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Common;
using TrafficCourts.Common.Features.EmailVerificationToken;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Messaging.Models;
using DisputantContactInformation = TrafficCourts.Citizen.Service.Models.Disputes.DisputantContactInformation;
using Dispute = TrafficCourts.Citizen.Service.Models.Disputes.Dispute;
using DisputeUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputeUpdateRequest;

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
    /// <param name="mapper"></param>
    /// <param name="documentService"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="mediator"/> or <paramref name="logger"/> is null.</exception>
    public DisputesController(IBus bus, IMediator mediator, ILogger<DisputesController> logger, IHashids hashids, IDisputeEmailVerificationTokenEncoder tokenEncoder, IMapper mapper, ICitizenDocumentService documentService)
    {
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
        _tokenEncoder = tokenEncoder ?? throw new ArgumentNullException(nameof(tokenEncoder));
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

        // Part of the 'send' here is to add the scanned in image of a ticket(if there is one) to COMS and remove from REDIS cache
        // In the call to the COMS service to CreateObjectsAsync, when reading the response as a List of TrafficCourts.Coms.Client.Anonymous
        // which extends TrafficCourts.Coms.Client.DBObject, it attempts to deserialize the JSON response and fails since
        // BucketId returns as null from the coms service but deserialization is trying to stuff it into System.Guid datatype that does not allow null
        // To fix, update BucketId datatype from System.Guid to System.Guid? to make it optional in DBObject in Models.g.cs of Traffic.Coms.Client
        // Seems to be explicitly a fault of the code generator that generates Models.g.cs and perhaps not the openapi spec from COMS team
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
        catch (RequestTimeoutException exception)
        {
            _logger.LogError(exception, "Request timeout verifying email, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error verifying email, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Search for a Dispute.
    /// </summary>
    /// <param name="ticketNumber">The violation ticket number. Must start with two upper case letters and end with eight digits.</param>
    /// <param name="time">The time the violation ticket number was issued. Must be formatted a valid 24-hour clock, HH:MM.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The dispute was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="404">The dispute was not found.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully.</response>
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

            var searchResponse = response.Message;
            if (searchResponse.IsNotFound)
            {
                _logger.LogDebug("Dispute not found, returning not found");
                return NotFound();
            }

            if (searchResponse.IsError)
            {
                // TODO: check if logging allows us to correlate this log message with the problem in the message consumer.
                _logger.LogError("Search returned an error, returning internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            // dispute is found and not an error, process the response data
            if (searchResponse.NoticeOfDisputeGuid is not null)
            {
                var token = _hashids.EncodeHex(searchResponse.NoticeOfDisputeGuid.Value.ToString("n"));
                // TODO: we are not detecting if any of these enumeration values could not be parsed
                _ = Enum.TryParse(searchResponse.DisputeStatus, out DisputeStatus disputeStatus);
                _ = Enum.TryParse(searchResponse.JJDisputeStatus, out JJDisputeStatus jjDisputeStatus);
                _ = Enum.TryParse(searchResponse.HearingType, out JJDisputeHearingType hearingType);

                SearchDisputeResult searchResult = new()
                {
                    NoticeOfDisputeGuid = token,
                    DisputeStatus = disputeStatus,
                    JJDisputeStatus = jjDisputeStatus,
                    HearingType = hearingType
                };

                return Ok(searchResult);
            }

            // TODO: should we return internal server error or not found? This is invalid situation if the data is correct.
            _logger.LogError("No NoticeOfDisputeGuid on search response, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (RequestTimeoutException exception)
        {
            _logger.LogError(exception, "Request timeout, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error searching for dispute, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Get a Dispute with authentication.
    /// </summary>
    /// <param name="guidHash">A hash of the noticeOfDisputeGuid.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute was found.</response>
    /// <response code="400">The uuid doesn't appear to be a valid UUID.</response>
    /// <response code="404">The dispute was not found.</response>
    /// <response code="500">There was a internal server error.</response>
    [Authorize]
    [HttpGet("/api/disputes/{guidHash}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisputeAsync(string guidHash, CancellationToken cancellationToken)
    {
        try
        {
            if (!_hashids.TryDecodeGuid(guidHash, out Guid noticeOfDisputeGuid))
            {
                // TODO: add instrumentation to monitor invalid values being posted
                return BadRequest("Invalid guidHash");
            }

            var userInfoResponse = await _mediator.Send(GetCurrentUserInfoRequest.Default, cancellationToken);
            UserInfo? user = userInfoResponse.UserInfo;

            // since we need to compare names and we couldn't get the current user, should we just return now?

            // can throw
            var check = await CheckDisputeStatus(noticeOfDisputeGuid, cancellationToken);
            if (check is not null)
            {
                return check;
            }

            GetDisputeRequest message = new()
            {
                NoticeOfDisputeGuid = noticeOfDisputeGuid
            };

            // can throw
            var response = await _bus.Request<GetDisputeRequest, SubmitNoticeOfDispute>(message, cancellationToken);
            if (string.IsNullOrEmpty(response?.Message?.TicketNumber))
            {
                _logger.LogInformation("Cound not get dispute or the ticket number is missing, returning not found");
                return NotFound("Dispute not found");
            }

            // Compare Contact Names to BC Services Card
            if (!CompareNames(response.Message, user))
            {
                _logger.LogDebug("User and dispute names do not match, returning bad request");
                return BadRequest("Contact names do not match.");
            }

            var result = _mapper.Map<NoticeOfDispute>(response.Message);

            DocumentProperties properties = new() { NoticeOfDisputeId = noticeOfDisputeGuid };

            // can throw
            result.FileData = await _documentService.FindFilesAsync(properties, cancellationToken);

            return Ok(result);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error getting dispute, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Submits an update request for a Dispute with authentication.
    /// </summary>
    /// <param name="guidHash">A hash of the noticeOfDisputeGuid.</param>
    /// <param name="dispute">The requested fields to update.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute is updated.</response>
    /// <response code="400">The uuid doesn't appear to be a valid UUID.</response>
    /// <response code="404">The dispute was not found.</response>
    /// <response code="500">There was a internal server error.</response>
    [Authorize]
    [HttpPut("/api/disputes/{guidHash}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDisputeAsync(string guidHash, [FromBody] Dispute dispute, CancellationToken cancellationToken)
    {
        try
        {
            if (!_hashids.TryDecodeGuid(guidHash, out Guid noticeOfDisputeGuid))
            {
                // TODO: add instrumentation to monitor invalid values being posted
                return BadRequest("Invalid guidHash");
            }

            // can throw
            var userInfoResponse = await _mediator.Send(GetCurrentUserInfoRequest.Default, cancellationToken);
            UserInfo? user = userInfoResponse.UserInfo;

            // since we need to compare names and we couldn't get the current user, should we just return now?

            // can throw
            var check = await CheckDisputeStatus(noticeOfDisputeGuid, cancellationToken);
            if (check is not null)
            {
                return check;
            }

            GetDisputeRequest message = new GetDisputeRequest
            {
                NoticeOfDisputeGuid = noticeOfDisputeGuid
            };

            // can throw
            var response = await _bus.Request<GetDisputeRequest, SubmitNoticeOfDispute>(message, cancellationToken);
            if (response is null || response.Message is null || String.IsNullOrEmpty(response.Message.TicketNumber))
            {
                return NotFound("Dispute not found");
            }

            // Compare Contact Names to BC Services Card
            if (!CompareNames(response.Message, user)) return BadRequest("Contact names do not match.");

            // Submit request to Workflow Service for processing.
            DisputeUpdateRequest request = _mapper.Map<DisputeUpdateRequest>(dispute);
            request.NoticeOfDisputeGuid = noticeOfDisputeGuid;

            if (dispute.FileData is not null)
            {
                var uploadPendingFiles = dispute.FileData.Where(i => !String.IsNullOrEmpty(i.PendingFileStream));
                request.UploadedDocuments = new List<UploadDocumentRequest>();
                foreach (FileMetadata fileMetadata in uploadPendingFiles)
                {
                    DocumentProperties properties = new() { NoticeOfDisputeId = noticeOfDisputeGuid, DocumentType = fileMetadata.DocumentType };

                    // can throw
                    Guid id = await _documentService.SaveFileAsync(fileMetadata.PendingFileStream, fileMetadata.FileName, properties, cancellationToken);

                    UploadDocumentRequest uploadDocumentRequest = new() { DocumentId= id, DocumentType = fileMetadata.DocumentType };

                    request.UploadedDocuments?.Add(uploadDocumentRequest);
                }

                var deletePendingFiles = dispute.FileData.Where(i => i.DeleteRequested == true && i.FileId is not null);
                foreach (FileMetadata fileMetadata in deletePendingFiles)
                {
                    // can throw
                    await _documentService.DeleteFileAsync(fileMetadata.FileId.Value, cancellationToken);
                }
            }

            // can throw
            await _bus.PublishWithLog(_logger, request, cancellationToken);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error updating dispute, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
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
                // TODO: add instrumentation to monitor invalid values being posted
                return BadRequest("Invalid guidHash");
            }

            var check = await CheckDisputeStatus(noticeOfDisputeGuid, cancellationToken);
            if (check is not null)
            {
                return check;
            }

            // Submit request to Workflow Service for processing.
            DisputeUpdateRequest message = _mapper.Map<DisputeUpdateRequest>(_mapper.Map<DisputeUpdateContactRequest>(disputantContactInformation));
            message.NoticeOfDisputeGuid = noticeOfDisputeGuid;
            await _bus.PublishWithLog(_logger, message, cancellationToken);

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error updating dispute contact information, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Searches for the dispute by <paramref name="noticeOfDisputeGuid"/>.
    /// </summary>
    /// <param name="noticeOfDisputeGuid"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<IActionResult?> CheckDisputeStatus(Guid noticeOfDisputeGuid, CancellationToken cancellationToken)
    {
        // Verify Dispute exists and whose value is one of [NEW, VALIDATED, or PROCESSING] and there is no corresponding JJDispute
        SearchDisputeRequest searchRequest = new() { NoticeOfDisputeGuid = noticeOfDisputeGuid };
        Response<SearchDisputeResponse> response = await _bus.Request<SearchDisputeRequest, SearchDisputeResponse>(searchRequest, cancellationToken);

        var searchResponse = response.Message;

        if (searchResponse.IsNotFound)
        {
            _logger.LogDebug("Search returned not found, returning not found");
            return NotFound();
        }

        if (searchResponse.NoticeOfDisputeGuid is null)
        {
            _logger.LogDebug("Search returned null NoticeOfDisputeGuid, returning not found");
            return NotFound();
        }

        if (searchResponse.IsError)
        {
            _logger.LogDebug("Search returned error, returning internal server error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (!IsValidDisputeStatus(searchResponse.DisputeStatus))
        {
            _logger.LogDebug("Dispute status is not valid, returning bad request");

            var status = searchResponse.DisputeStatus ?? "missing";
            return BadRequest($"Dispute has a status of {status}. Expecting one of NEW, VALIDATED, or PROCESSING.");
        }
        else if (searchResponse.JJDisputeStatus is not null)
        {
            _logger.LogDebug("JJDispute status is not valid, returning bad request");
            return BadRequest($"JJDispute has status of {searchResponse.JJDisputeStatus}. Must be blank.");
        }

        _logger.LogDebug("Dispute status is ok, returning null");
        return null;
    }

    private static bool IsValidDisputeStatus(string? value)
    {
        if (value is null) return false;

        return IsDisputeStatus(value, DisputeStatus.NEW)
            || IsDisputeStatus(value, DisputeStatus.VALIDATED)
            || IsDisputeStatus(value, DisputeStatus.PROCESSING);
    }


    private static bool IsDisputeStatus(string value, DisputeStatus status)
    {
        return status.ToString() == value;
    }

    private bool CompareNames(SubmitNoticeOfDispute message, UserInfo? user)
    {
#if DEBUG 
#warning Contact Name Comparisons with BC Services Cards have been disabled 
       return true;
#endif
        bool result = true;

        // if contact type is individual then match with disputant name otherwise match with contact names
        if (message.ContactTypeCd == DisputeContactTypeCd.INDIVIDUAL)
        {
            var givenNames = message.DisputantGivenName1
                + (message.DisputantGivenName2 != null ? (" " + message.DisputantGivenName2) : "")
                + (message.DisputantGivenName3 != null ? (" " + message.DisputantGivenName3) : "");
            if (!message.DisputantSurname.Equals(user?.Surname, StringComparison.OrdinalIgnoreCase)
                || !(message.DisputantGivenName1.Equals(user?.GivenName, StringComparison.OrdinalIgnoreCase) || givenNames.Equals(user?.GivenNames, StringComparison.OrdinalIgnoreCase)))
            {
                result = false;
            }
        }
        else if (message.ContactTypeCd == DisputeContactTypeCd.LAWYER || message.ContactTypeCd == DisputeContactTypeCd.OTHER)
        {
            var givenNames = message.ContactGiven1Nm
                + (message.ContactGiven2Nm != null ? (" " + message.ContactGiven2Nm) : "")
                + (message.ContactGiven3Nm != null ? (" " + message.ContactGiven3Nm) : "");
            if (!message.ContactSurnameNm.Equals(user?.Surname, StringComparison.OrdinalIgnoreCase)
                || !(message.ContactGiven1Nm.Equals(user?.GivenName, StringComparison.OrdinalIgnoreCase) || givenNames.Equals(user?.GivenNames, StringComparison.OrdinalIgnoreCase)))
            {
                result = false;
            }
        }
        else
        {
            result = false;
        }

        return result;
    }
}
