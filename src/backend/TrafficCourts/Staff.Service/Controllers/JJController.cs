using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

// implement role authorization by using TCOControllerBase class as in csrs project
public class JJController : JJControllerBase<JJController>
{
    private readonly IJJDisputeService _JJDisputeService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="JJDisputeService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public JJController(IJJDisputeService JJDisputeService, ILogger<JJController> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(JJDisputeService);
        _JJDisputeService = JJDisputeService;
    }

    /// <summary>
    /// Returns all JJ Disputes from the Oracle Data API 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="jjAssignedTo">If specified, will retrieve the records which are assigned to the specified jj staff</param>
    /// <response code="200">The JJ disputes were found.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires jj-dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of JJ dispute records</returns>
    [HttpGet("Disputes")]
    [ProducesResponseType(typeof(IList<JJDispute>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GetJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all JJ Disputes from oracle-data-api");

        try
        {
            ICollection<JJDispute> JJDisputes = await _JJDisputeService.GetAllJJDisputesAsync(jjAssignedTo, cancellationToken);
            return Ok(JJDisputes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving JJ disputes from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Returns a single JJ Dispute with the given identifier from the Oracle Data API.
    /// </summary>
    /// <param name="JJDisputeId">Unique identifier for a specific JJ dispute record.</param>
    /// <param name="assignVTC">boolean to indicate need to assign VTC.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A single JJ dispute record</returns>
    /// <response code="200">The JJ dispute was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires jj-dispute:read permission.</response>
    /// <response code="409">The JJDispute has already been assigned to a user. JJDispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    [HttpGet("{JJDisputeId}")]
    [ProducesResponseType(typeof(JJDispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GetJJDisputeAsync(string JJDisputeId, bool assignVTC, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving JJ Dispute from oracle-data-api");

        try
        {
            JJDispute JJDispute = await _JJDisputeService.GetJJDisputeAsync(JJDisputeId, assignVTC, cancellationToken);
            return Ok(JJDispute);
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
                Title = "JJ Dispute Already Assigned",
                Detail = "The selected JJ dispute record is already assigned",
                Status = e.StatusCode,
                Instance = HttpContext?.Request?.Path,
            };
            pd.Extensions.Add("errors", e.Message);

            return new ObjectResult(pd);
        }
        catch (ApiException e)
        {
            _logger.LogError(e, "Error retrieving JJ dispute from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving JJ dispute from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates a single JJ Dispute through the Oracle Data Interface API based on unique violation ticket number and the jj dispute data being passed in the body.
    /// </summary>
    /// <param name="ticketNumber">Unique identifier for a specific JJ Dispute record.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="jjDispute"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Admin resolution is submitted. The JJ Dispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="403">Forbidden, requires jj-dispute:update permission.</response>
    /// <response code="404">The JJ Dispute to update was not found.</response>
    /// <response code="405">An invalid JJ Dispute status is provided. Update failed.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}")]
    [ProducesResponseType(typeof(JJDispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> SubmitAdminResolutionAsync(string ticketNumber, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJ Dispute in oracle-data-api");

        try
        {
            await _JJDisputeService.SubmitAdminResolutionAsync(ticketNumber, checkVTC, jjDispute, cancellationToken);
            return Ok(jjDispute);
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
            _logger.LogError(e, "API Exception: error submitting JJ Dispute Admin Resolution to oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "General Exception: server error submitting JJ Dispute Admin Resolution");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

}
