using DisputeApi.Web.Features.TcoDispute.Models;
using DisputeApi.Web.Features.TcoDispute.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.TcoDispute.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisputeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;
        public DisputeController(
            ILogger<DisputeController> logger, IDisputeService disputeService)
        {
            _logger = logger;
            _disputeService = disputeService;
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Dispute), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> CreateDispute([FromBody] Dispute dispute)
        {
            return Ok(await _disputeService.CreateDispute(dispute));
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Dispute>), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> GetDisputes()
        {
            return Ok(await _disputeService.GetDisputes());
        }

        [HttpGet("{disputeId}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Dispute>), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> GetDispute(int disputeId)
        {
            return Ok(await _disputeService.GetDispute(disputeId));
        }
    }
}
