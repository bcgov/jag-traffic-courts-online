using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Citizen.Service.Controllers;

[Route("api/[controller]/[action]")]
public class LookupController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<LookupController> _logger;

    public LookupController(IMediator mediator, ILogger<LookupController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary> 
    /// Returns a list of Violation Ticket Statutes filtered by given section text (if provided).
    /// </summary>
    /// <param name="section">Motor vehicle Section text to query by, ie "13(1)(a)" returns "Motor Vehicle or Trailer without Licence" contravention, or blank for no filter.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Statute>), StatusCodes.Status200OK)]
    public async Task<IActionResult> StatutesAsync(string? section, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving a Statutes");

        var request = new Features.Lookups.Statutes.Request(section);
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Items);
    }

    /// <summary> 
    /// Returns a list of Languages.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Language>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LanguagesAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Languages");

        var request = new Features.Lookups.Languages.Request();
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response.Items);
    }
}

