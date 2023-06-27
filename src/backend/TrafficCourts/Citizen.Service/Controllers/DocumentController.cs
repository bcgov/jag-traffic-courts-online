using HashidsNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using TrafficCourts.Citizen.Service.Models.OAuth;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Common;
using TrafficCourts.Common.Errors;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Citizen.Service.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DisputesController> _logger;
    private readonly IHashids _hashids;
    private readonly ICitizenDocumentService _documentService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="hashids"></param>
    /// <param name="documentService"></param>
    /// <exception cref="ArgumentNullException"> <paramref name="logger"/> is null.</exception>
    public DocumentController(ILogger<DisputesController> logger, IHashids hashids, ICitizenDocumentService documentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
        _documentService = documentService ?? throw new ArgumentNullException(nameof(documentService));
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
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
}
