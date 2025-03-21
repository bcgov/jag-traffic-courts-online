using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Features.Occam.Disputes;
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
//#if DEBUG
//    [AllowAnonymous]
//#endif
//    [HttpGet]
//    [Route("/api/v2/occam/dispute/disputes")]
//    [Produces("application/json")]
//    //[ProducesResponseType(typeof(IList<OccamDispute>), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(PagedDisputeListItemCollection), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//    [ProducesResponseType(StatusCodes.Status403Forbidden)]
//    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
//    public async Task<IActionResult> GetDisputesAsync(OccamDisputeListingParameters parameters, CancellationToken cancellationToken)
//    {
//        _logger.LogDebug("Retrieving all Disputes from oracle-data-api");
//        parameters ??= OccamDisputeListingParameters.Default;
//        try
//        {
//            PagedDisputeListItemCollection disputes = await _disputeService.GetAllDisputesAsync(parameters, cancellationToken);
//            return Ok(disputes);
//        }
//        catch (Exception e)
//        {
//            _logger.LogError(e, "Error retrieving Disputes from oracle-data-api");
//            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
//        }
//    }

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
    [ProducesResponseType(typeof(PagedOccamDisputeListItemCollection), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.Dispute, Scopes.Read)]
    public async Task<IActionResult> GetDisputes2Async(OccamDisputeListingParameters parameters, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Occam Disputes");

        parameters ??= OccamDisputeListingParameters.Default;
        try
        {
            Features.Occam.Disputes.Request request = new() { Parameters = parameters };
            Features.Occam.Disputes.Response response = await _mediator.Send(request, cancellationToken);

            return Ok(response.Data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Disputes from ORDS");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}



    //public async Task<IActionResult> SearchDisputesAsync(
    //    bool? appearances,
    //    bool? notice_of_hearing_yn,
    //    bool? multiple_officers_yn,
    //    bool? electronic_ticket_yn,
    //    string? time_zone,
    //    string? submitted_from,
    //    string? submitted_thru,
    //    string? ticket_number,
    //    string? surname,
    //    string? surname_or_org_name,
    //    string? jj_assigned_to,
    //    string? jj_decision_dt_from,
    //    string? jj_decision_dt_thru,
    //    string? dispute_status_codes,
    //    string? appearance_courthouse_ids,
    //    string? appearance_dt_from,
    //    string? appearance_dt_thru,
    //    string? to_be_heard_at_courthouse_ids,
    //    string? hearing_type_cd,
    //    string? sort_by,
    //    int? page_number,
    //    int? page_size,
    //    CancellationToken cancellationToken)
    //{
    //    Request request = new Request
    //    {
    //        appearances = appearances,
    //        notice_of_hearing_yn = notice_of_hearing_yn,
    //        multiple_officers_yn = multiple_officers_yn,
    //        electronic_ticket_yn = electronic_ticket_yn,
    //        time_zone = time_zone,
    //        submitted_from = submitted_from,
    //        submitted_thru = submitted_thru,
    //        ticket_number = ticket_number,
    //        surname = surname,
    //        surname_or_org_name = surname_or_org_name,
    //        jj_assigned_to = jj_assigned_to,
    //        jj_decision_dt_from = jj_decision_dt_from,
    //        jj_decision_dt_thru = jj_decision_dt_thru,
    //        dispute_status_codes = dispute_status_codes,
    //        appearance_courthouse_ids = appearance_courthouse_ids,
    //        appearance_dt_from = appearance_dt_from,
    //        appearance_dt_thru = appearance_dt_thru,
    //        to_be_heard_at_courthouse_ids = to_be_heard_at_courthouse_ids,
    //        hearing_type_cd = hearing_type_cd,
    //        sort_by = sort_by,
    //        page_number = page_number,
    //        page_size = page_size
    //    };

    //    try
    //    {
    //        var response = await _mediator.Send(request, cancellationToken);

    //        if (response.Data is not null)
    //        {
    //            return Ok(response.Data);
    //        }

    //        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error fetching disputes", errorId = response.ErrorId });
    //    }
    //    catch (Exception exception)
    //    {
    //        _logger.LogError(exception, "Error searching JJ disputes");
    //        return StatusCode(StatusCodes.Status500InternalServerError, new { message = exception.Message });
    //    }
