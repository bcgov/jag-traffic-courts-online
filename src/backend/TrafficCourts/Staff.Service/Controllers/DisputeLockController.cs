using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using TrafficCourts.Common.Errors;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

public class DisputeLockController : StaffControllerBase<DisputeLockController>
{
    private readonly IDisputeLockService _disputeLockService;
    
    /// <summary>
    /// Defafult constructor for DisputeLockController
    /// </summary>
    /// <param name="disputeLockService"></param>
    /// <param name="logger"></param>
    public DisputeLockController(IDisputeLockService disputeLockService, ILogger<DisputeLockController> logger) : base(logger)
    {
        _disputeLockService = disputeLockService;
    }

    /// <summary>
    /// Acquires a lock for a JJ Dispute.
    /// </summary>
    /// <param name="ticketNumber"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(Lock), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<Lock>> GetLock(string ticketNumber, string username)
    {
        try
        {
            var result = _disputeLockService.GetLock(ticketNumber, username);
            return Task.FromResult<ActionResult<Lock>>(Ok(result));
        }
        catch (LockIsInUseException e)
        {
            _logger.LogInformation(e, "JJ Dispute has already been locked by another user");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.Conflict;
            problemDetails.Title = e.Source + ": Error Locking JJ Dispute";
            problemDetails.Instance = HttpContext?.Request?.Path;
            string? innerExceptionMessage = e.InnerException?.Message;
            string? lockedBy = e.Username;
            problemDetails.Extensions.Add("lockedBy", lockedBy ?? string.Empty);
            if (innerExceptionMessage is not null)
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message, innerExceptionMessage });
            }
            else
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message });
            }
            var result = new ObjectResult(problemDetails);
            return Task.FromResult<ActionResult<Lock>>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get lock for ticket: {ticketNumber}", ticketNumber);
            var result = new HttpError(StatusCodes.Status500InternalServerError, ex.Message);
            return Task.FromResult<ActionResult<Lock>>(result);
        }
    }

    /// <summary>
    /// Refreshes the expiry time of a lock.
    /// </summary>
    /// <param name="lockId"></param>
    /// <returns></returns>
    [HttpPut("{lockId}")]
    [ProducesResponseType(typeof(DateTimeOffset), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<DateTimeOffset>> RefreshLock(string lockId)
    {
        try
        {
            var result = _disputeLockService.RefreshLock(lockId, GetUserName(User));
            if (result is null) return Task.FromResult<ActionResult<DateTimeOffset>>(NotFound());
            return Task.FromResult<ActionResult<DateTimeOffset>>(Ok(result));
        }
        catch (LockIsInUseException e)
        {
            _logger.LogInformation(e, "Failed to refresh lock {lockId}", lockId);
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.Conflict;
            problemDetails.Title = e.Source + ": Error Refreshing Lock";
            problemDetails.Instance = HttpContext?.Request?.Path;
            string? innerExceptionMessage = e.InnerException?.Message;
            string? lockedBy = e.Username;
            problemDetails.Extensions.Add("lockedBy", lockedBy ?? string.Empty);
            if (innerExceptionMessage is not null)
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message, innerExceptionMessage });
            }
            else
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message });
            }
            var result = new ObjectResult(problemDetails);
            return Task.FromResult<ActionResult<DateTimeOffset>>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh lock {lockId}", lockId);
            var result = new HttpError(StatusCodes.Status500InternalServerError, ex.Message);
            return Task.FromResult<ActionResult<DateTimeOffset>>(result);
        }
    }

    /// <summary>
    /// Releases a lock.
    /// </summary>
    /// <param name="lockId"></param>
    /// <returns></returns>
    [HttpDelete("{lockId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult> ReleaseLock(string lockId)
    {
        try
        {
            _disputeLockService.ReleaseLock(lockId);
            return Task.FromResult<ActionResult>(Ok());
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Failed to release lock {lockId}", lockId);
            var result = new HttpError(StatusCodes.Status500InternalServerError, ex.Message);
            return Task.FromResult<ActionResult>(result);
        }
    }

    private static string GetUserName(ClaimsPrincipal user)
    {
        return user?.Identity?.Name ?? string.Empty;
    }
}
