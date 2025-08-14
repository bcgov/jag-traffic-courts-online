using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Features.Occam.Disputes;
using TrafficCourts.Staff.Service.Features.Occam.DisputesWithUpdateRequests;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Services;

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
    [ProducesResponseType(typeof(PagedOccamDisputeListItemCollection), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputesAsyncV2(OccamDisputeListingParameters parameters, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Data from ORDS - OCCAM Disputes");

        if (!ValidateTimeZone(parameters?.TimeZone, out TimeZoneInfo? timeZoneInfo, out IActionResult? validationResult))
        {
            return validationResult; // Return BadRequest if validation fails
        }

        parameters ??= OccamDisputeListingParameters.Default;

        try
        {
            Features.Occam.Disputes.Request request = new() { Parameters = parameters };
            Features.Occam.Disputes.Response response = await _mediator.Send(request, cancellationToken);

            return Ok(response.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Data from ORDS - OCCAM Disputes");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
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
#if DEBUG
    [AllowAnonymous]
#endif
    [HttpGet]
    [Route("/api/v2/occam/dispute/disputesWithUpdateRequests")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(PagedOccamDisputeWithUpdateRequestListItemCollection), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputesWithPendingUpdateRequestsAsyncV2(OccamDisputeWithUpdateRequestsListingParameters parameters, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Data from ORDS - OCCAM Disputes with pending Update Requests");

        try
        {
            Features.Occam.DisputesWithUpdateRequests.Request request = new() { Parameters = parameters };
            Features.Occam.DisputesWithUpdateRequests.Response response = await _mediator.Send(request, cancellationToken);

            return Ok(response.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Data from ORDS - OCCAM Disputes with pending Update Requests");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
