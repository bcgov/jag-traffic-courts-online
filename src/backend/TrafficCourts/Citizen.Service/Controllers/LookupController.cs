using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Citizen.Service.Models.Deprecated;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using Statute = TrafficCourts.Citizen.Service.Models.Tickets.Statute;

namespace TrafficCourts.Citizen.Service.Controllers;

[Route("api/[controller]/[action]")]
public class LookupController : ControllerBase
{
    private readonly ILogger<LookupController> _logger;
    private readonly ILookupService _lookupService;

    public LookupController(ILookupService lookupService, ILogger<LookupController> logger)
    {
        ArgumentNullException.ThrowIfNull(lookupService);
        ArgumentNullException.ThrowIfNull(logger);
        _lookupService = lookupService;
        _logger = logger;
    }

    [Obsolete]
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResultResponse<LookupsAll>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get()
    {
        //_logger.LogDebug("Getting lookup tables");
        //LookupsAll lookUpsAll = await _lookupsService.GetAllLookUpsAsync();
        //if (lookUpsAll == null)
        //    return NoContent();
        //return Ok(new ApiResultResponse<LookupsAll>(lookUpsAll));

        var response = new ApiResultResponse<LookupsAll>(new LookupsAll());

        return Ok(response);
    }

    /// <summary> 
    /// Returns a list of Violation Ticket Statutes filtered by given section text (if provided).
    /// </summary>
    /// <param name="section">Motor vehicle act Section text to query by, ie "13(1)(a)" returns "Motor Vehicle or Trailer without Licence" contravention, or blank for no filter.</param>
    /// <returns></returns>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<Statute>), 200)]
    public IActionResult Statutes(string? section)
    {
        _logger.LogDebug("Retrieving a Statutes");
        List<Statute> statutes = _lookupService.GetStatutes(section).ToList<Statute>();

        return Ok(statutes);
    }
}

