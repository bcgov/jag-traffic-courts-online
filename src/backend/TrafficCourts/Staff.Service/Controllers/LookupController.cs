using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using MediatR;

namespace TrafficCourts.Staff.Service.Controllers;

[Route("api/[controller]/[action]")]
public class LookupController : StaffControllerBase
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

    /// <summary> 
    /// Returns a list of agencies.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">OK</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Agency>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AgenciesAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Agencies");

        AgencyLookup.Request request = new();
        AgencyLookup.Response response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Agencies);
    }

    /// <summary> 
    /// Returns a list of provinces.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">OK</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Province>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ProvinceAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Provinces");

        ProvinceLookup.Request request = new();
        ProvinceLookup.Response response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Provinces);
    }

    /// <summary> 
    /// Returns a list of countries.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <response code="200">OK</response>
    /// <response code="401">Request lacks valid authentication credentials.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Country>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CountryAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Countries");

        CountryLookup.Request request = new();
        CountryLookup.Response response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Countries);
    }
}
