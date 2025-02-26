using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Models.Disputes;
using TrafficCourts.Staff.Service.Services;
using static System.Collections.Specialized.BitVector32;

namespace TrafficCourts.Staff.Service.Controllers.Occam;

[Route("api/[controller]/[action]")]
public class DisputeController : StaffControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDisputeService _disputeService;
    private readonly IPrintDigitalCaseFileService _printService;
    private readonly ILogger<Controllers.DisputeController> _logger;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="disputeService"></param>
    /// <param name="printService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public DisputeController(IMediator mediator, IDisputeService disputeService, IPrintDigitalCaseFileService printService, ILogger<Controllers.DisputeController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
#if DEBUG
    [AllowAnonymous]
#endif
    [HttpGet]
    [Route("/api/v2/occam/dispute/disputes")]
    [Produces("application/json")]
    //[ProducesResponseType(typeof(IList<OccamDispute>), StatusCodes.Status200OK)]
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
#if DEBUG
    [AllowAnonymous]
#endif
    [HttpGet]
    [Route("/api/v2/occam/dispute/disputes2")]
    [Produces("application/json")]
    //[ProducesResponseType(typeof(IList<OccamDispute>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PagedDisputeListItemCollection), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputes2Async(GetAllDisputesParameters parameters, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Occam Disputes");

        parameters ??= GetAllDisputesParameters.Default;
        try
        {
            Features.Occam.Disputes.Request request = new() { Parameters = parameters };
            Features.Occam.Disputes.Response response = await _mediator.Send(request, cancellationToken);

            return Ok(response.Items);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Disputes from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
