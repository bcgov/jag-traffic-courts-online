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
    [Route("api/[controller]")]
    [ApiController]
    [OpenApiTag("Lookup API")]
    public class LookupController : Controller
    {
        private ILogger<LookupController> _logger;
        private ILookupsService _lookupsService;
        public LookupController(ILogger<LookupController> logger, ILookupsService lookupsService)
        {
            _logger = logger;
            _lookupsService = lookupsService;
        }


        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResultResponse<Lookups>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessageResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            _logger.LogDebug("Get all Look Up Tables now.");
            return Ok(_lookupsService.GetAllLookUps());
        }
    }
}
