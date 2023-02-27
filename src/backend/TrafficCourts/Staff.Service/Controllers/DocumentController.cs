using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Common.Models;
using TrafficCourts.Staff.Service.Authentication;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

public class DocumentController : StaffControllerBase<DocumentController>
{
    private readonly IStaffDocumentService _documentService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="documentService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public DocumentController(IStaffDocumentService documentService, ILogger<DocumentController> logger) : base(logger)
    {
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
    }

    /// <summary>
    /// Creates a new file the document management service along with metadata.
    /// </summary>
    /// <param name="file">The file to save in the common object management service and the metadata of the uploaded file to be saved including the document type</param>
    /// <param name="disputeId">The TCO dispute id to associate document with.</param>
    /// <param name="cancellationToken"></param>
    /// <param name="documentType">The document type to associate with this file.</param>
    /// <response code="200">The document is successfully uploaded and saved.</response>
    /// <response code="400">The request was not well formed. The file and ticket number are required</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires jjdispute:update permission.</response>
    /// <response code="500">There was a server error that prevented the file upload from completing successfully.</response>
    /// <returns>The ID of the uploaded file</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> CreateAsync(
        [Required]
        IFormFile file,
        [FromHeader]
        [Required]
        [Range(1, int.MaxValue)]
        long disputeId,
        [FromHeader]
        [Required]
        string documentType,
        CancellationToken cancellationToken)
    {
        // note: the range check on disputeId to prevent passing too large numbers in javascript, the max javascript number 
        _logger.LogDebug("Uploading the document to the object storage");

        try
        {
            DocumentProperties properties = new() { TcoDisputeId = disputeId, DocumentType = documentType };
            Guid id = await _documentService.SaveFileAsync(file, properties, User, cancellationToken);
            return Ok(id);
        }
        catch (Coms.Client.MetadataInvalidKeyException e)
        {
            var key = e.Key ?? "is null";
            _logger.LogError(e, "Could not upload a document because a metadata {Key} was invalid", key);
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Invalid Metadata Key";
            problemDetails.Detail = "Invalid Key: " + key;
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.MetadataTooLongException e)
        {
            _logger.LogError(e, "Could not upload a document because metadata was too long");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Metadata Too Long";
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TagKeyEmptyException e)
        {
            _logger.LogError(e, "Could not upload a document because tag key was empty");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Tag Key Empty";
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TagKeyTooLongException e)
        {
            var key = e.Key ?? "is null";
            _logger.LogError(e, "Could not upload a document because a tag {Key} was too long", key);
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Tag Key Too Long";
            problemDetails.Detail = "Invalid Key: " + key;
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TagValueTooLongException e)
        {
            var key = e.Key ?? "is null";
            var value = e.Value ?? "is null";
            _logger.LogError(e, "Could not upload a document because tag {Value} was too long", value);
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Title = e.Source + ": Exception Invoking COMS - Tag Value Too Long";
            problemDetails.Detail = "Tag value: " + value + " is too long for Key: " + key;
            problemDetails.Instance = HttpContext?.Request?.Path;
            problemDetails.Extensions.Add("errors", e.Message);

            return new ObjectResult(problemDetails);
        }
        catch (Coms.Client.TooManyTagsException e)
        {
            _logger.LogError(e, "Could not upload a document because there were too many tags. TagCount: {Count}", e.TagCount);
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
            _logger.LogError(e, "Could not upload a document because of ObjectManagementServiceException");
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

    /// <summary>
    /// Downloads a document for the given unique file ID if the virus scan staus is clean.
    /// </summary>
    /// <param name="fileId">Unique identifier for a specific document.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The document is successfully downloaded.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires jjdispute:read permission.</response>
    /// <response code="404">The file was not found.</response>
    /// <response code="500">There was a server error that prevented the file to be downloaded successfully.</response>
    /// <returns>The document</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GetAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Downloading the document from the object storage");

        try
        {
            Coms.Client.File file = await _documentService.GetFileAsync(fileId, cancellationToken);

            var stream = file.Data;
            // Reset position to the beginning of the stream
            stream.Position = 0;

            return File(stream, file.ContentType ?? "application/octet-stream", file.FileName ?? "download");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Coms.Client.ObjectManagementServiceException e)
        {
            _logger.LogError(e, "Could not download the document because of ObjectManagementServiceException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Title = e.Source + ": Error getting file from COMS";
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
            _logger.LogError(e, "Error retrieving the document");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// Removes the specified document for the given unique file ID.
    /// </summary>
    /// <param name="fileId">Unique identifier for a specific document.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">The document is successfully removed.</response>
    /// <response code="400">The request was not well formed. Check the parameters.</response>
    /// <response code="401">Unauthenticated.</response>
    /// <response code="403">Forbidden, requires jjdispute:delete permission.</response>
    /// <response code="500">There was a server error that prevented the file to be removed successfully.</response>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Update)]
    public async Task<IActionResult> DeleteAsync(Guid fileId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting the document from the object storage");

        try
        {
            await _documentService.DeleteFileAsync(fileId, User, cancellationToken);

            return Ok();
        }
        catch (Coms.Client.ObjectManagementServiceException e)
        {
            _logger.LogError(e, "Could not remove the document because of ObjectManagementServiceException");
            ProblemDetails problemDetails = new();
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Title = e.Source + ": Error removing file from COMS";
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
            _logger.LogError(e, "Error removing the document");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
