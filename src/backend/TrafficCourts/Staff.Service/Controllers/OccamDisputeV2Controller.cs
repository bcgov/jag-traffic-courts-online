using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Models.Disputes;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

//[Route("api/v2/occamdispute/[action]")]
public class OccamDisputeV2Controller : StaffControllerBase
{
    private readonly IDisputeService _disputeService;
    private readonly IPrintDigitalCaseFileService _printService;
    private readonly ILogger<DisputeController> _logger;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="disputeService"></param>
    /// <param name="printService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public OccamDisputeV2Controller(IDisputeService disputeService, IPrintDigitalCaseFileService printService, ILogger<DisputeController> logger)
    {
        _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
        _printService = printService ?? throw new ArgumentNullException(nameof(printService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns all Disputes from the Oracle Data API with given parameters.
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The Disputes were found.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of Dispute records</returns>
    [HttpGet("disputes")]
    [ProducesResponseType(typeof(PagedDisputeListItemCollection), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputesAsync(GetAllDisputesParameters parameters, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all Disputes from oracle-data-api");
        parameters ??= GetAllDisputesParameters.Default;
        try
        {
            PagedDisputeListItemCollection disputes = await _disputeService.GetAllDisputesAsync(parameters, cancellationToken);
            return Ok(disputes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Disputes from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
