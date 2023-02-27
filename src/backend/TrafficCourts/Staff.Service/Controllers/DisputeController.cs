using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Services;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Common.Errors;

namespace TrafficCourts.Staff.Service.Controllers;

public class DisputeController : StaffControllerBase<DisputeController>
{
    private readonly IDisputeService _disputeService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="disputeService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public DisputeController(IDisputeService disputeService, ILogger<DisputeController> logger) : base(logger)
    {
        _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
    }

    /// <summary>
    /// Returns all Disputes from the Oracle Data API with optional exclusion parameter to exclude disputes having specified status from the result.
    /// </summary>
    /// <param name="excludeStatus"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The Disputes were found.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of Dispute records</returns>
    [HttpGet("disputes")]
    [ProducesResponseType(typeof(IList<Dispute>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputesAsync(ExcludeStatus? excludeStatus, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all Disputes from oracle-data-api");

        try
        {
            ICollection<Dispute> disputes = await _disputeService.GetAllDisputesAsync(excludeStatus, cancellationToken);
            return Ok(disputes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Disputes from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Returns a single Dispute with the given identifier from the Oracle Data API.
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A single Dispute record</returns>
    /// <response code="200">The Dispute was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:read permission.</response>
    /// <response code="404">The dispute was not found.</response>
    /// <response code="409">The Dispute has already been assigned to a user. Dispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    [HttpGet("{disputeId}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Dispute from oracle-data-api");

        try
        {
            Dispute dispute = await _disputeService.GetDisputeAsync(disputeId, cancellationToken);
            return Ok(dispute);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status409Conflict)
        {
            ProblemDetails pd = new ProblemDetails
            {
                Title = "Ticket Dispute Already Assigned",
                Detail = "The selected ticket dispute record is already assigned",
                Status = e.StatusCode,
                Instance = HttpContext?.Request?.Path,
            };
            pd.Extensions.Add("errors", e.Message);

            return new ObjectResult(pd);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error retrieving Dispute from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Dispute from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates a single Dispute through the Oracle Data Interface API based on unique dispute id and the dispute data being passed in the body.
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record.</param>
    /// <param name="dispute"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:update permission.</response>
    /// <response code="404">The Dispute to update was not found.</response>
    /// <response code="409">The Dispute has already been assigned to a user. Dispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Update)]
    public async Task<IActionResult> UpdateDisputeAsync(long disputeId, Dispute dispute, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute in oracle-data-api");

        try
        {
            await _disputeService.UpdateDisputeAsync(disputeId, dispute, cancellationToken);
            return Ok(dispute);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error retrieving Dispute from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating Dispute in oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates the status of a particular Dispute record to REJECTED.
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record to cancel.</param>
    /// <param name="rejectedReason">The reason or note (max 256 characters) the Dispute was cancelled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:reject permission.</response>
    /// <response code="404">Dispute record not found. Update failed.</response>
    /// <response code="405">A Dispute status can only be set to REJECTED iff status is NEW, CANCELLED, VALIDATED or REJECTED and the rejected reason must be &lt;= 256 characters. Update failed.</response>
    /// <response code="409">The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Reject)]
    public async Task<IActionResult> RejectDisputeAsync(
        long disputeId, 
        [FromForm] 
        [Required]
        [StringLength(256, ErrorMessage = "Rejected reason cannot exceed 256 characters.")] string rejectedReason, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute status");

        try
        {
            await _disputeService.RejectDisputeAsync(disputeId, rejectedReason, User, cancellationToken);
            return Ok();
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error updating Dispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating Dispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates the status of a particular Dispute record to VALIDATED.
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record to validate.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:validate permission.</response>
    /// <response code="404">Dispute record not found. Update failed.</response>
    /// <response code="405">A Dispute status can only be set to VALIDATED iff status is NEW. Update failed.</response>
    /// <response code="409">The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}/validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Validate)]
    public async Task<IActionResult> ValidateDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute status to {Status}", "VALIDATED");

        try
        {
            await _disputeService.ValidateDisputeAsync(disputeId, User, cancellationToken);
            return Ok();
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error updating Dispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating Dispute status");            
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates the status of a particular Dispute record to CANCELLED.
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record to cancel.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:cancel permission.</response>
    /// <response code="404">Dispute record not found. Update failed.</response>
    /// <response code="405">A Dispute status can only be set to CANCELLED iff status is NEW, VALIDATED, REJECTED or PROCESSING.Update failed.</response>
    /// <response code="409">The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Cancel)]
    public async Task<IActionResult> CancelDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute status to {Status}", "CANCELLED");

        try
        {
            await _disputeService.CancelDisputeAsync(disputeId, User, cancellationToken);
            return Ok();
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error updating Dispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating Dispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// An endpoint for resending an email to a Disputant.
    /// </summary>
    /// <param name="disputeId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// <response code="200">OK.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:submit permission.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    /// </returns>
    [HttpPut("{disputeId}/resendEmailVerify")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Update)]
    public async Task<IActionResult> ResendEmailAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Resending Email Verification");

        try
        {
            string email = await _disputeService.ResendEmailVerificationAsync(disputeId, cancellationToken);
            return Ok(email);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error resending email verification");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (BadHttpRequestException e)
        {
            _logger.LogError(e, "Error resending email verification. " + e.Message);
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error resending email verification");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Submits a Dispute record, setting it's status to PROCESSING
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record to submit.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:submit permission.</response>
    /// <response code="404">Dispute record not found. Update failed.</response>
    /// <response code="405">A Dispute can only be submitted if the status is NEW or is already set to PROCESSING. Update failed.</response>
    /// <response code="409">The Dispute has already been assigned to a different user. Dispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}/submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Submit)]
    public async Task<IActionResult> SubmitDisputeAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute status to {Status}", "PROCESSING");

        try
        {
            await _disputeService.SubmitDisputeAsync(disputeId, User, cancellationToken);
            return Ok();
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e) when (e.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error updating Dispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating Dispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Approves a DisputeUpdateRequest record, setting it's status to ACCEPTED.
    /// </summary>
    /// <param name="updateStatusId">Unique identifier for a specific DisputeUpdateRequest record to accept.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("updateRequest/{updateStatusId}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Update)]
    public async Task<IActionResult> AcceptDisputeUpdateRequestAsync(long updateStatusId, CancellationToken cancellationToken)
    {
        await _disputeService.AcceptDisputeUpdateRequestAsync(updateStatusId, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Rejects a DisputeUpdateRequest record, setting it's status to REJECTED.
    /// </summary>
    /// <param name="updateStatusId">Unique identifier for a specific DisputeUpdateRequest record to reject.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("updateRequest/{updateStatusId}/reject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Update)]
    public async Task<IActionResult> RejectDisputeUpdateRequestAsync(long updateStatusId, CancellationToken cancellationToken)
    {
        await _disputeService.RejectDisputeUpdateRequestAsync(updateStatusId, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Returns all Disputes that have pending update requests from the Oracle Data API
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The Disputes were found.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of Dispute records</returns>
    [HttpGet("disputesWithUpdateRequests")]
    [ProducesResponseType(typeof(IList<DisputeWithUpdates>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputesWithPendingUpdateRequestsAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all Disputes from oracle-data-api with pending update requests");

        try
        {
            ICollection<DisputeWithUpdates> disputes = await _disputeService.GetAllDisputesWithPendingUpdateRequestsAsync(cancellationToken);
            return Ok(disputes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Disputes with pending update requests from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }


    /// <summary>
    /// Returns all update requests for a specific dispute 
    /// </summary>
    /// <param name="disputeId">Dispute Id</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The Update requests were found.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of Dispute update request records</returns>
    [HttpGet("{disputeId}/disputeUpdateRequests")]
    [ProducesResponseType(typeof(IList<DisputeUpdateRequest>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputeUpdateRequestsAsync(long disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all Dispute update requests from oracle-data-api for a dispute id");

        try
        {
            ICollection<DisputeUpdateRequest> disputeUpdateRequests = await _disputeService.GetDisputeUpdateRequestsAsync(disputeId, cancellationToken);
            return Ok(disputeUpdateRequests);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Dispute update requests for a dispute from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
