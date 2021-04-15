using DisputeApi.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TrafficCourts.Common.Contract;

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
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public DisputesController(ILogger<DisputesController> logger, IDisputeService disputeService,
            ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDispute([FromBody] Dispute dispute)
        {
            dispute.Status = DisputeStatus.Submitted;
            var result = await _disputeService.CreateAsync(dispute);

            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://localhost:5672/DisputeWorker.Dispute"));

            await sendEndpoint.Send<IDispute>(new Dispute(){OffenceNumber = 1, ViolationTicketNumber = "violationTicket"});

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
