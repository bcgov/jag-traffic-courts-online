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
    /// Returns a single Dispute with the given identifier from the Oracle Data Interface API.
    /// </summary>
    /// <param name="disputeId">Unique identifier for a specific Dispute record.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/dispute/{disputeId}")]
    [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDisputeAsync(int disputeId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Dispute from oraface-api");

        Dispute dispute = await _oracleDataApi.GetDisputeAsync(disputeId, cancellationToken);

        return Ok(dispute);
    }

}
