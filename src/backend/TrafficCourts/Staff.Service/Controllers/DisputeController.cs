﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

// implement role authorization by using TCOControllerBase class as in csrs project
public class DisputeController : TCOControllerBase<DisputeController>
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
        ArgumentNullException.ThrowIfNull(disputeService);
        _disputeService = disputeService;
    }

    /// <summary>
    /// Returns all Disputes from the Oracle Data API.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The Disputes were found.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of Dispute records</returns>
    [HttpGet("disputes")]
    [ProducesResponseType(typeof(IList<Dispute>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisputesAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all Disputes from oracle-data-api");

        try
        {
            ICollection<Dispute> disputes = await _disputeService.GetAllDisputesAsync(cancellationToken);
            return Ok(disputes);
        }
        catch (Exception e)
        {
            _logger.LogError("Error retrieving Disputes from oracle-data-api:", e);
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
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="409">The Dispute has already been assigned to a user. Dispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    [HttpGet("{disputeId}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisputeAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Dispute from oracle-data-api");

        try
        {
            Dispute dispute = await _disputeService.GetDisputeAsync(disputeId, cancellationToken);
            return Ok(dispute);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status409Conflict)
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
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e)
        {
            _logger.LogError("Error retrieving Dispute from oracle-data-api:", e);
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error retrieving Dispute from oracle-data-api:", e);
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
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="404">The Dispute to update was not found.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDisputeAsync(Guid disputeId, Dispute dispute, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute in oracle-data-api");

        try
        {
            await _disputeService.UpdateDisputeAsync(disputeId, dispute, cancellationToken);
            return Ok(dispute);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e)
        {
            _logger.LogError("Error retrieving Dispute from oracle-data-api:", e);
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error updating Dispute in oracle-data-api:", e);
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
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="404">Dispute record not found. Update failed.</response>
    /// <response code="405">A Dispute status can only be set to REJECTED iff status is NEW, CANCELLED, or REJECTED and the rejected reason must be &lt;= 256 characters. Update failed.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}/reject")]
    public async Task<IActionResult> RejectDisputeAsync(
        Guid disputeId, 
        [FromForm] 
        [Required]
        [StringLength(256, ErrorMessage = "Rejected reason cannot exceed 256 characters.")] string rejectedReason, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute status");

        try
        {
            await _disputeService.RejectDisputeAsync(disputeId, rejectedReason, cancellationToken);
            return Ok();
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e)
        {
            _logger.LogError("Error updating Dispute status:", e);
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error updating Dispute status:", e);
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
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="404">Dispute record not found. Update failed.</response>
    /// <response code="405">A Dispute status can only be set to CANCELLED iff status is REJECTED or PROCESSING.Update failed.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelDisputeAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute status");

        try
        {
            await _disputeService.CancelDisputeAsync(disputeId, cancellationToken);
            return Ok();
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e)
        {
            _logger.LogError("Error updating Dispute status:", e);
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error updating Dispute status:", e);
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
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="404">Dispute record not found. Update failed.</response>
    /// <response code="405">A Dispute can only be submitted if the status is NEW or is already set to PROCESSING. Update failed.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{disputeId}/submit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitDisputeAsync(Guid disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the Dispute status");

        try
        {
            await _disputeService.SubmitDisputeAsync(disputeId, cancellationToken);
            return Ok();
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status405MethodNotAllowed)
        {
            return new HttpError(e.StatusCode, e.Message);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e)
        {
            _logger.LogError("Error updating Dispute status:", e);
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Error updating Dispute status:", e);
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

}
