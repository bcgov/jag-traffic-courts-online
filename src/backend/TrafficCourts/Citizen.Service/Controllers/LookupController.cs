using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Citizen.Service.Models.Deprecated;

namespace TrafficCourts.Citizen.Service.Controllers;

[Obsolete]
[Route("api/[controller]/[action]")]
public class LookupController : ControllerBase
{
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
}

