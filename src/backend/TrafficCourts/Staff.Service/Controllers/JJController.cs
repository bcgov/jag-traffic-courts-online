using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

public class JJController : StaffControllerBase<JJController>
{
    private readonly IJJDisputeService _jjDisputeService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="jjDisputeService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public JJController(IJJDisputeService jjDisputeService, ILogger<JJController> logger) : base(logger)
    {
        _jjDisputeService = jjDisputeService ?? throw new ArgumentNullException(nameof(JJDisputeService));
    }

    /// <summary>
    /// Returns all JJ Disputes from the Oracle Data API 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="jjAssignedTo">If specified, will retrieve the records which are assigned to the specified jj staff</param>
    /// <response code="200">The JJ disputes were found.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jj-dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A collection of JJ dispute records</returns>
    [HttpGet("Disputes")]
    [ProducesResponseType(typeof(IList<JJDispute>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GetJJDisputesAsync(string? jjAssignedTo, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all JJ Disputes from oracle-data-api");

        try
        {
            ICollection<JJDispute> JJDisputes = await _jjDisputeService.GetAllJJDisputesAsync(jjAssignedTo, cancellationToken);
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
    /// <param name="jjDisputeId">Unique identifier for a specific JJ dispute record.</param>
    /// <param name="ticketNumber">Ticket number for a specific JJ dispute record.</param>
    /// <param name="assignVTC">boolean to indicate need to assign VTC.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A single JJ dispute record</returns>
    /// <response code="200">The JJ dispute was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jj-dispute:read permission.</response>
    /// <response code="409">The JJDispute has already been assigned to a user. JJDispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    [HttpGet("{jjDisputeId}")]
    [ProducesResponseType(typeof(JJDispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GetJJDisputeAsync(long jjDisputeId, string ticketNumber, bool assignVTC, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving JJ Dispute {JJDisputeId} from oracle-data-api", jjDisputeId);

        try
        {
            JJDispute JJDispute = await _jjDisputeService.GetJJDisputeAsync(jjDisputeId, ticketNumber, assignVTC, cancellationToken);
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
        catch (Coms.Client.ObjectManagementServiceException e)
        {
            _logger.LogError(e, "Could not return document IDs because of ObjectManagementServiceException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Title = e.Source + ": Error Invoking COMS";
            problemDetails.Instance = HttpContext?.Request?.Path;
            string? innerExceptionMessage = e.InnerException?.Message;
            if (innerExceptionMessage is not null)
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message, innerExceptionMessage });
            }
            else
            {
                problemDetails.Extensions.Add("errors", new string[] { e.Message });
            }

            return new ObjectResult(problemDetails);
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
    /// Returns a single Justin Document for a given ticket number and docment type.
    /// </summary>
    /// <param name="ticketNumber">Ticket number for a specific JJ dispute record.</param>
    /// <param name="documentType">indicates document type.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A single Ticket Image Data record</returns>
    /// <response code="200">The document was found.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jj-dispute:read permission.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    [HttpGet("ticketimage/{ticketNumber}/{documentType}")]
    [ProducesResponseType(typeof(TicketImageDataJustinDocument), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GetJustinDocument(string ticketNumber, DocumentType documentType, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Justin Documnet {ticketNumber} from oracle-data-api", ticketNumber);

        try
        {
            TicketImageDataJustinDocument justinDocument = await _jjDisputeService.GetJustinDocumentAsync(ticketNumber, documentType, cancellationToken);

            // base 64 decoding (comes from Oracle as base 64 encoded string)
            var decodedFileData = Convert.FromBase64String(justinDocument.FileData);

            MemoryStream stream = new MemoryStream( decodedFileData );
            stream.Position = 0;

            var fileName = (justinDocument.ParticipantName ?? "Disputant") + "_" + (justinDocument.ReportType.ToString() ?? "justinDoc") + "." + justinDocument.ReportFormat ?? "pdf";

            return File(stream, "application/pdf", fileName);
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
            _logger.LogError(e, "Error retrieving Justin Document");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error retrieving Justin Document");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates a single JJ Dispute through the Oracle Data Interface API based on unique violation ticket number and the jj dispute data being passed in the body.
    /// </summary>
    /// <param name="jjDisputeId">Unique identifier for a specific JJ Dispute record.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="jjDispute"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">Admin resolution is submitted. The JJ Dispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jj-dispute:update permission.</response>
    /// <response code="404">The JJ Dispute to update was not found.</response>
    /// <response code="405">An invalid JJ Dispute status is provided. Update failed.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}")]
    [ProducesResponseType(typeof(JJDispute), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> SubmitAdminResolutionAsync(long jjDisputeId, bool checkVTC, JJDispute jjDispute, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJ Dispute in oracle-data-api");

        try
        {
            var updatedJJDispute = await _jjDisputeService.SubmitAdminResolutionAsync(jjDisputeId, checkVTC, jjDispute, User, cancellationToken);
            return Ok(updatedJJDispute);
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

    /// <summary>
    /// Updates each JJ Dispute based on the passed in IDs (ticket number) to assign them to a specific JJ or unassign them if JJ not specified.
    /// </summary>
    /// <param name="ticketNumbers">List of Unique identifiers for JJ Dispute records to be assigend/unassigned.</param>
    /// <param name="username">IDIR username of the JJ that JJ Dispute(s) will be assigned to, if specified. Otherwise JJ Disputes will be unassigned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">JJ Disputes are assigned/unassigned to/from a JJ successfully. The JJ Disputes are updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jj-dispute:assign permission.</response>
    /// <response code="404">The JJ Dispute(s) to update was not found.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("Assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Assign)]
    public async Task<IActionResult> AssignJJDisputesToJJ([BindRequired, FromQuery(Name = "ticketNumbers")] List<string> ticketNumbers, string? username, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJ Dispute(s) in oracle-data-api for assigning/unassigning them to/from a JJ");

        try
        {
            await _jjDisputeService.AssignJJDisputesToJJ(ticketNumbers, username, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "API Exception: error assigning/unassigning JJ Dispute to/from a JJ through oracle-data-api");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "General Exception: server error assigning/unassigning JJ Dispute to/from a JJ");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates the status of a particular JJDispute record to REVIEW as well as adds an optional remark that explaining why the status was set to REVIEW.
    /// </summary>
    /// <param name="ticketNumber">Unique identifier for a specific JJ Dispute record.</param>
    /// <param name="remark">The remark or note (max 256 characters) the JJDispute was set to REVIEW.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The JJDispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jjdispute:review permission.</response>
    /// <response code="404">JJDispute record not found. Update failed.</response>
    /// <response code="405">A JJDispute status can only be set to REVIEW iff status is NEW or VALIDATED and the remark must be less than or equal to 256 characters. Update failed.</response>
    /// <response code="409">The JJDispute has already been assigned to a different user. JJDispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}/review")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Review)]
    public async Task<IActionResult> ReviewJJDisputeAsync(
        string ticketNumber,
        [FromForm]
        [StringLength(256, ErrorMessage = "Remark note cannot exceed 256 characters.")] string remark,
        bool checkVTC,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJDispute status to REVIEW");

        try
        {
            await _jjDisputeService.ReviewJJDisputeAsync(ticketNumber, remark, checkVTC, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates the status of a particular JJDispute record to REQUIRE_COURT_HEARING, hearing type to COURT_APPEARANCE as well as adds an optional remark that explaining why the status was set.
    /// </summary>
    /// <param name="ticketNumber">Unique identifier for a specific JJ Dispute record.</param>
    /// <param name="remark">The remark or note (max 256 characters) the JJDispute was set to REVIEW.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The JJDispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jjdispute:require_court_hearing permission.</response>
    /// <response code="404">JJDispute record not found. Update failed.</response>
    /// <response code="405">A JJDispute status can only be set to REQUIRE_COURT_HEARING iff status is one of the following: NEW, IN_PROGRESS, REVIEW, REQUIRE_COURT_HEARING and the remark must be less than or equal to 256 characters. Update failed.</response>
    /// <response code="409">The JJDispute has already been assigned to a different user. JJDispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}/requirecourthearing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.RequireCourtHearing)]
    public async Task<IActionResult> RequireCourtHearingJJDisputeAsync(
        string ticketNumber,
        [FromForm]
        [StringLength(256, ErrorMessage = "Remark note cannot exceed 256 characters.")] string remark,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJDispute status to REQUIRE_COURT_HEARING");

        try
        {
            await _jjDisputeService.RequireCourtHearingJJDisputeAsync(ticketNumber, remark, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates the status of a particular JJDispute record to ACCEPTED.
    /// </summary>
    /// <param name="ticketNumber">Ticket number for a specific JJ Dispute record.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The JJDispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jjdispute:accept permission.</response>
    /// <response code="404">JJDispute record not found. Update failed.</response>
    /// <response code="405">A JJDispute status can only be set to ACCEPTED iff status is CONFIRMED. Update failed.</response>
    /// <response code="409">The JJDispute has already been assigned to a different user. JJDispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}/accept")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Accept)]
    public async Task<IActionResult> AcceptJJDisputeAsync(
        string ticketNumber,
        bool checkVTC,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJDispute status to ACCEPTED");

        try
        {
            await _jjDisputeService.AcceptJJDisputeAsync(ticketNumber, checkVTC, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (PartIdNotFoundException e)
        {
            return new HttpError(StatusCodes.Status400BadRequest, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates the status of a particular JJDispute record to CONCLUDED.
    /// </summary>
    /// <param name="ticketNumber">Ticket number for a specific JJ Dispute record.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The JJDispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jjdispute:accept permission.</response>
    /// <response code="404">JJDispute record not found. Update failed.</response>
    /// <response code="409">The JJDispute has already been assigned to a different user. JJDispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}/conclude")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> ConcludeJJDisputeAsync(
        string ticketNumber,
        bool checkVTC,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJDispute status to CONCLUDED");

        try
        {
            await _jjDisputeService.ConcludeJJDisputeAsync(ticketNumber, checkVTC, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (PartIdNotFoundException e)
        {
            return new HttpError(StatusCodes.Status400BadRequest, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    /// <summary>
    /// Updates the status of a particular JJDispute record to CANCELLED.
    /// </summary>
    /// <param name="ticketNumber">Ticket number for a specific JJ Dispute record.</param>
    /// <param name="checkVTC">boolean to indicate need to check VTC assigned.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The JJDispute is updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jjdispute:accept permission.</response>
    /// <response code="404">JJDispute record not found. Update failed.</response>
    /// <response code="409">The JJDispute has already been assigned to a different user. JJDispute cannot be modified until assigned time expires.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Accept)]
    public async Task<IActionResult> CancelJJDisputeAsync(
        string ticketNumber,
        bool checkVTC,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating the JJDispute status to CANCELLED");

        try
        {
            await _jjDisputeService.CancelJJDisputeAsync(ticketNumber, checkVTC, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (PartIdNotFoundException e)
        {
            return new HttpError(StatusCodes.Status400BadRequest, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    /// <summary>
    /// Updates court appearance record as well as the status of a particular JJDispute record to REQUIRE_COURT_HEARING, hearing type to COURT_APPEARANCE.
    /// </summary>
    /// <param name="ticketNumber">Ticket number for a specific JJ Dispute record.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The court appearance and JJDispute status are updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jjdispute:update permission.</response>
    /// <response code="404">JJDispute record not found. Update failed.</response>
    /// <response code="405">A JJDispute status can only be set to REQUIRE_COURT_HEARING iff status is one of the following: NEW, IN_PROGRESS, REVIEW, REQUIRE_COURT_HEARING. Update failed.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}/updatecourtappearance/requirecourthearing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> UpdateCourtAppearanceAndRequireCourtHearingJJDisputeAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating court appearance and the JJDispute status to REQUIRE_COURT_HEARING");

        try
        {
            // TODO: Call Oracle API to update court appearance when TCVP-1999 is completed as per TCVP-1978

            await _jjDisputeService.RequireCourtHearingJJDisputeAsync(ticketNumber, null, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Updates court appearance record as well as the status of a particular JJDispute record to CONFIRMED.
    /// </summary>
    /// <param name="ticketNumber">Ticket number for a specific JJ Dispute record.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">The court appearance and JJDispute status are updated.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden, requires jjdispute:update permission.</response>
    /// <response code="404">JJDispute record not found. Update failed.</response>
    /// <response code="405">A JJDispute status can only be set to CONFIRMED iff status is one of the following: REVIEW, NEW, HEARING_SCHEDULED, IN_PROGRESS, CONFIRMED. Update failed.</response>
    /// <response code="500">There was a server error that prevented the update from completing successfully.</response>
    [HttpPut("{ticketNumber}/updatecourtappearance/confirm")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> UpdateCourtAppearanceAndConfirmJJDisputeAsync(string ticketNumber, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating court appearance and the JJDispute status to REQUIRE_COURT_HEARING");

        try
        {
            // TODO: Call Oracle API to update court appearance when TCVP-1999 is completed as per TCVP-1978

            await _jjDisputeService.ConfirmJJDisputeAsync(ticketNumber, User, cancellationToken);
            return Ok();
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
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating JJDispute status");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

}
