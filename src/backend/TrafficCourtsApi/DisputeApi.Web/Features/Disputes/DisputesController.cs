using DisputeApi.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Messaging.Configuration;
using MassTransit;
using TrafficCourts.Common.Contract;
using Microsoft.Extensions.Options;
using MediatR;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Features.Disputes.Queries;

namespace DisputeApi.Web.Features.Disputes
{
    [OpenApiTag("Dispute API")]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class DisputesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IDisputeService _disputeService;

        public DisputesController(ILogger<DisputesController> logger, IDisputeService disputeService,
            ISendEndpointProvider sendEndpointProvider, IOptions<RabbitMQConfiguration> rabbitMqOptions, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDispute([FromBody]CreateDisputeCommand createDisputeCommand)
        {

            var response = await _mediator.Send(createDisputeCommand);
            if (response.Id == 0)
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
            var disputes = await _mediator.Send(new GetAllDisputesQuery());

            return Ok(disputes);
        }

        [HttpGet("{disputeId}")]
        [ProducesResponseType(typeof(IQueryable<Dispute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDispute(int disputeId)
        {
            var dispute = await _mediator.Send(new GetDisputeQuery { DisputeId=disputeId});
            if (dispute != null)
            {
                return Ok(dispute);
            }

            return NotFound();
        }


    }
}
