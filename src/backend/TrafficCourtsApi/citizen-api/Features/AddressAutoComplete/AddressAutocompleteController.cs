using System.Collections.Generic;
using System.Threading.Tasks;
using Gov.CitizenApi.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Gov.CitizenApi.Features.AddressAutoComplete.AddressAutocompleteClient;

namespace Gov.CitizenApi.Features.AddressAutoComplete
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressAutocompleteController : ControllerBase
    {
        private readonly IAddressAutocompleteClient _addressAutocompleteClient;

        public AddressAutocompleteController(IAddressAutocompleteClient addressAutocompleteClient)
        {
            _addressAutocompleteClient = addressAutocompleteClient;
        }

        // GET: api/AddressAutocomplete/find
        /// <summary>
        /// Gets autocomplete results
        /// Find addresses matching the search term.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="lastId"></param>
        [HttpGet("find", Name = nameof(Find))]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResultResponse<IEnumerable<AddressAutocompleteFindResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Find([FromQuery] string searchTerm, [FromQuery] string lastId = null)
        {
            if (searchTerm == null)
            {
                return BadRequest();
            }
            var result = await _addressAutocompleteClient.Find(searchTerm, lastId);
            return Ok(result);
        }

        // GET: api/AddressAutocomplete/retrieve
        /// <summary>
        /// Gets autocomplete retrieve result
        /// Returns the full address details based on the Id.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("retrieve", Name = nameof(Retrieve))]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResultResponse<IEnumerable<AddressAutocompleteRetrieveResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult> Retrieve([FromQuery] string id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var result = await _addressAutocompleteClient.Retrieve(id);
            return Ok(result);
        }
    }
}
