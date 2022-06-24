using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
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
    /// <param name="jjGroupAssignedTo">If specified, will retrieve the records which are assigned to the specified jj group</param>
    /// <param name="jjAssignedTo">If specified, will retrieve the records which are assigned to the specified jj staff</param>
    /// <response code="200">The JJ disputes were found.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of JJ dispute records</returns>
    [HttpGet("Disputes")]
    [ProducesResponseType(typeof(IList<JJDispute>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetJJDisputesAsync(string? jjGroupAssignedTo, string? jjAssignedTo, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all JJ Disputes from oracle-data-api");

        try
        {
            ICollection<JJDispute> JJDisputes = await _JJDisputeService.GetAllJJDisputesAsync(jjGroupAssignedTo, jjAssignedTo, cancellationToken);
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
    /// <param name="cancellationToken"></param>
    /// <returns>A single JJ dispute record</returns>
    /// <response code="200">The JJ dispute was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, wrong user roles.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    [HttpGet("{JJDisputeId}")]
    [ProducesResponseType(typeof(JJDispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetJJDisputeAsync(string JJDisputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving JJ Dispute from oracle-data-api");

        try
        {
            JJDispute JJDispute = await _JJDisputeService.GetJJDisputeAsync(JJDisputeId, cancellationToken);
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

}
