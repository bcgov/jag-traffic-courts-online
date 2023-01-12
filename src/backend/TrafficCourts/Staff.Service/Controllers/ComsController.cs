using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Models;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

public class ComsController : StaffControllerBase<ComsController>
{
    private readonly IComsService _comsService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="comsService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public ComsController(IComsService comsService, ILogger<ComsController> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(comsService);
        _comsService = comsService;
    }

    /// <summary>
    /// Uploads and saves the provided document file in the common object management service along with metadata.
    /// </summary>
    /// <param name="fileUploadRequest">The file to save in the common object management service and the metadata of the uploaded file to be saved including the document type</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The document is successfully uploaded and saved.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires jjdispute:update permission.</response>
    /// <response code="500">There was a server error that prevented the file upload from completing successfully.</response>
    /// <returns>The ID of the uploaded file</returns>
    [HttpPost("UploadDocument")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> UploadDocumentAsync([FromForm] FileUploadRequest fileUploadRequest, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Uploading the document to the object storage");

        try
        {
            Guid id = await _comsService.SaveFileAsync(fileUploadRequest.File, fileUploadRequest.Metadata, cancellationToken);
            return Ok(id);
        }
        catch (Coms.Client.MetadataInvalidKeyException e)
        {
            _logger.LogError(e, "Coms.Client.MetadataInvalidKeyException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Invalid Metadata Key";
            problemDetails.Detail = "Invalid Key: " + e.Key;
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.MetadataTooLongException e)
        {
            _logger.LogError(e, "Coms.Client.MetadataTooLongException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Metadata Too Long";
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TagKeyEmptyException e)
        {
            _logger.LogError(e, "Coms.Client.TagKeyEmptyException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Tag Key Empty";
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TagKeyTooLongException e)
        {
            _logger.LogError(e, "Coms.Client.TagKeyTooLongException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Tag Key Too Long";
            problemDetails.Detail = "Invalid Key: " + e.Key;
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TagValueTooLongException e)
        {
            _logger.LogError(e, "Coms.Client.TagValueTooLongException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Tag Value Too Long";
            problemDetails.Detail = "Tag value: " + e.Value + " is too long for Key: " + e.Key;
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TooManyTagsException e)
        {
            _logger.LogError(e, "Coms.Client.TooManyTagsException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Too Many Tags";
            problemDetails.Detail = "Tag count: " + e.TagCount;
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.ObjectManagementServiceException e)
        {
            _logger.LogError(e, "Coms.Client.ObjectManagementServiceException");
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
        catch (Exception e)
        {
            _logger.LogError(e, "Error uploading the document");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
