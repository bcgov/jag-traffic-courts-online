using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Citizen.Service.Models.Deprecated;

namespace TrafficCourts.Citizen.Service.Controllers;

[Obsolete]
[Route("api/[controller]/[action]")]
public class AddressAutocompleteController : ControllerBase
{
    // GET: api/AddressAutocomplete/find
    /// <summary>
    /// Gets autocomplete results
    /// Find addresses matching the search term.
    /// </summary>
    /// <param name="searchTerm"></param>
    /// <param name="lastId"></param>
    [HttpGet(Name = nameof(Find))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<AddressAutocompleteFindResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Find([FromQuery] string searchTerm, [FromQuery] string lastId = null)
    {
        //if (searchTerm == null)
        //{
        //    return BadRequest();
        //}
        //var result = await _addressAutocompleteClient.Find(searchTerm, lastId);
        //return Ok(result);

        return NoContent();
    }

    // GET: api/AddressAutocomplete/retrieve
    /// <summary>
    /// Gets autocomplete retrieve result
    /// Returns the full address details based on the Id.
    /// </summary>
    /// <param name="id"></param>
    [HttpGet(Name = nameof(Retrieve))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<AddressAutocompleteRetrieveResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Retrieve([FromQuery] string id)
    {
        //if (id == null)
        //{
        //    return BadRequest();
        //}
        //var result = await _addressAutocompleteClient.Retrieve(id);
        //return Ok(result);

        return NoContent();
    }

}