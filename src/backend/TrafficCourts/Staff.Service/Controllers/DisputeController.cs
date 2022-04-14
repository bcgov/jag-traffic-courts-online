using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Controllers;

[ApiController]
[Route("api")]
public class DisputeController : ControllerBase
{
    private readonly IOracleDataApi_v1_0Client _oracleDataApi;
    private readonly ILogger<DisputeController> _logger;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="oracleDataApi"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public DisputeController(IOracleDataApi_v1_0Client oracleDataApi, ILogger<DisputeController> logger)
    {
        ArgumentNullException.ThrowIfNull(oracleDataApi);
        ArgumentNullException.ThrowIfNull(logger);
        _oracleDataApi = oracleDataApi;
        _logger = logger;
    }

    /// <summary>
    /// Returns all Disputes from the Oracle Data Interface API.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/disputes")]
    [ProducesResponseType(typeof(ICollection<Dispute>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisputesAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all Disputes from oracle-data-api");

        try
        {
            ICollection<Dispute> disputes = await _oracleDataApi.GetAllDisputesAsync(cancellationToken);
            return Ok(disputes);
        }
        catch (Exception e)
        {
            _logger.LogError("Error retrieving Disputes from oracle-data-api:", e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Returns a single Dispute with the given identifier from the Oracle Data Interface API.
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The Dispute was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="404">The Dispute was not found.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully.</response>
    [HttpGet("/dispute/{disputeId}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDisputeAsync(int disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Dispute from oracle-data-api");

        try
        {
            Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, cancellationToken);
            return Ok(dispute);
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status400BadRequest)
        {
            return BadRequest();
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e) when (e.StatusCode == StatusCodes.Status404NotFound)
        {
            return NotFound();
        }
        catch (TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0.ApiException e)
        {
            _logger.LogError("Error retrieving Dispute from oracle-data-api:", e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        catch (Exception e)
        {
            _logger.LogError("Error retrieving Dispute from oracle-data-api:", e);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

}
