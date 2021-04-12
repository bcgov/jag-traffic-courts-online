using DisputeApi.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DisputeApi.Web.Features.Disputes
{
    [OpenApiTag("Dispute API")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class DisputesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;

        public DisputesController(ILogger<DisputesController> logger, IDisputeService disputeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDispute([FromBody] Dispute dispute)
        {
            var result = await _disputeService.CreateAsync(dispute);
            if (result == null)
            {
                ModelState.AddModelError("DisputeOffenceNumber", "the dispute already exists for this offence.");
                return BadRequest(ApiResponse.BadRequest(ModelState));
            }

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IQueryable<Dispute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDisputes()
        {
            var disputes = await _disputeService.GetAllAsync();

            return Ok(disputes);
        }

        [HttpGet("{disputeId}")]
        [ProducesResponseType(typeof(IQueryable<Dispute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDispute(int disputeId)
        {
            var dispute = await _disputeService.GetAsync(disputeId);

            if (dispute != null)
            {
                return Ok(dispute);
            }

            return NotFound();
        }
    }
}
