using Microsoft.AspNetCore.Mvc;
using System.Net;
using Statute = TrafficCourts.Staff.Service.Models.Statute;
using TrafficCourts.Staff.Service.Services;

namespace TrafficCourts.Staff.Service.Controllers;

public class LookupController : TCOControllerBase<LookupController>
{
    private readonly ILookupService _lookupService;
    
    public LookupController(ILookupService lookupService, ILogger<LookupController> logger) : base(logger)
    {
        _lookupService = lookupService ?? throw new ArgumentNullException(nameof(lookupService));
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

