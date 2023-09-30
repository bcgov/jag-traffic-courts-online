using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Common.Authorization;
using TrafficCourts.Common.Errors;
using TrafficCourts.Staff.Service.Authentication;

namespace TrafficCourts.Staff.Service.Controllers;

public class DocumentGenerationController : StaffControllerBase<DocumentGenerationController>
{
    private readonly IDocumentGenerationService _documentGenerationService;

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="documentGenerationService"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"><paramref name="logger"/> is null.</exception>
    public DocumentGenerationController(IDocumentGenerationService documentGenerationService, ILogger<DocumentGenerationController> logger) : base(logger)
    {
        _documentGenerationService = documentGenerationService ?? throw new ArgumentNullException(nameof(documentGenerationService));
    }

    /// <summary>
    /// Returns generated document
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Generated Document.</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="500">There was a server error that prevented the search from completing successfully or no data found.</response>
    /// <returns>A generated document</returns>
    [AllowAnonymous]
    [HttpPost("Generate")]
    [ProducesResponseType(typeof(RenderedReport), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [KeycloakAuthorize(Resources.JJDispute, Scopes.Read)]
    public async Task<IActionResult> GenerateAsync([Required] IFormFile template, [FromHeader][Required] string data, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Document Generation started");

        try
        {
            using (var templateStream = new MemoryStream())
            {
                await template.CopyToAsync(templateStream);
                templateStream.Seek(0, SeekOrigin.Begin);
                ConvertTo convertTo = ConvertTo.Pdf;
                TemplateType templateType = TemplateType.Word;
                string reportName = "test";
                dynamic dynamicData = !string.IsNullOrWhiteSpace(data) ? JsonSerializer.Deserialize<dynamic>(data) : null;
                RenderedReport result = await _documentGenerationService.UploadTemplateAndRenderReportAsync<dynamic>(templateStream, templateType, convertTo, reportName, dynamicData, cancellationToken);
                return Ok(result);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error generating document");
            return new HttpError(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
