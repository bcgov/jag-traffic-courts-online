using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using MediatR;

namespace TrafficCourts.Staff.Service.Controllers;

public class LookupController : TCOControllerBase<LookupController>
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
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Statute>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> StatutesAsync(string? section, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving a Statutes");

        StatuteLookup.Request request = new StatuteLookup.Request(section);
        StatuteLookup.Response response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Statutes);
    }
}

