using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Gov.CitizenApi.Features.Lookups
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [OpenApiTag("Lookup API")]
    public class LookupController : Controller
    {
        private ILogger<LookupController> _logger;
        private LookupsService _lookupsService;
        public LookupController(ILogger<LookupController> logger, LookupsService lookupsService)
        {
            _logger = logger;
            _lookupsService = lookupsService;
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<IEnumerable<Status>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult CountCodes()
        {
            _logger.LogDebug("Get count codes info now.");
            return Ok(_lookupsService.GetStatutes());
        }
    }
}
