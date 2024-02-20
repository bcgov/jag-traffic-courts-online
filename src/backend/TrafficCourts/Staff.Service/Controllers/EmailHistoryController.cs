using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

// implement role authorization by using TCOControllerBase class as in csrs project
public class EmailHistoryController : StaffControllerBase
{
    private readonly IEmailHistoryService _emailHistoryService;
    private readonly ILogger<EmailHistoryController> _logger;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="emailHistoryService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public EmailHistoryController(IEmailHistoryService emailHistoryService, ILogger<EmailHistoryController> logger)
    {
        _emailHistoryService = emailHistoryService ?? throw new ArgumentNullException(nameof(emailHistoryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns all File History Records from the Oracle Data API related to a specific ticket number.
    /// </summary>
    /// <param name="ticketNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The File history records were found.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of File History records</returns>
    [HttpGet("EmailHistory")]
    [ProducesResponseType(typeof(IList<EmailHistory>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GetEmailHistoryRecordsAsync(String ticketNumber, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all email history records from oracle-data-api for a specified ticket");

        try
        {
            ICollection<EmailHistory> fileHistories = await _emailHistoryService.GetEmailHistoryForTicketAsync(ticketNumber, cancellationToken);
            return Ok(fileHistories);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving email history records from oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
