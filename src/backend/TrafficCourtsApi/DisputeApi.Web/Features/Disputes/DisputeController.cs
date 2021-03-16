using DisputeApi.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Disputes
{
    [OpenApiTag("Dispute API")]
    [Route("api/[controller]")]
    [ApiController]
    public class DisputeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;

        public DisputeController(ILogger<DisputeController> logger, IDisputeService disputeService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDispute([FromBody] Dispute dispute)
        {
            var result = await _disputeService.CreateAsync(dispute);
            return Ok(result);
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Dispute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDisputes()
        {
            var disputes = await _disputeService.GetAllAsync();

            return Ok(disputes);
        }

        [HttpGet("{disputeId}")]
        [Produces("application/json")]
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
