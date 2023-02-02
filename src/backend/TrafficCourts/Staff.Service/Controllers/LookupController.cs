using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using MediatR;

namespace TrafficCourts.Staff.Service.Controllers;

[Route("api/[controller]/[action]")]
public class LookupController : StaffControllerBase<LookupController>
{
    private readonly IMediator _mediator;
    
    public LookupController(IMediator mediator, ILogger<LookupController> logger) : base(logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary> 
    /// Returns a list of Violation Ticket Statutes filtered by given section text (if provided).
    /// </summary>
    /// <param name="section">Motor vehicle act Section text to query by, ie "13(1)(a)" returns "Motor Vehicle or Trailer without Licence" contravention, or blank for no filter.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">OK</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Statute>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> StatutesAsync(string? section, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Statutes");

        StatuteLookup.Request request = new(section);
        StatuteLookup.Response response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Statutes);
    }

    /// <summary> 
    /// Returns a list of Languages.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">OK</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Language>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LanguagesAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Languages");

        LanguageLookup.Request request = new();
        LanguageLookup.Response response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Languages);
    }
}
