using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Citizen.Service.Services;
using Statute = TrafficCourts.Citizen.Service.Models.Tickets.Statute;

namespace TrafficCourts.Citizen.Service.Controllers;

[Route("api/[controller]/[action]")]
public class LookupController : ControllerBase
{
    private readonly ILookupService _lookupService;
    private readonly ILogger<LookupController> _logger;

    public LookupController(ILookupService lookupService, ILogger<LookupController> logger)
    {
        _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary> 
    /// Returns a list of Violation Ticket Statutes filtered by given section text (if provided).
    /// </summary>
    /// <param name="section">Motor vehicle act Section text to query by, ie "13(1)(a)" returns "Motor Vehicle or Trailer without Licence" contravention, or blank for no filter.</param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IList<Statute>), (int)HttpStatusCode.OK)]
    public IActionResult Statutes(string? section)
    {
        _logger.LogDebug("Retrieving a Statutes");

        List<Statute> statutes = _lookupService.GetStatutes(section).ToList();

        return Ok(statutes);
    }
}

