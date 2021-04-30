using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Features.Disputes.Queries;
using DisputeApi.Web.Features.Disputes.DBModel;

namespace DisputeApi.Web.Features.Disputes
{
    [OpenApiTag("Dispute API")]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [ApiController]
    public class DisputesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        public DisputesController(ILogger<DisputesController> logger,IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTicketDispute([FromBody]CreateDisputeCommand createDisputeCommand)
        {
            var response = await _mediator.Send(createDisputeCommand);
            if (response.Id == 0)
            {
                ModelState.AddModelError("DisputeOffenceNumber", "the dispute already exists for this offence.");
                return BadRequest(ApiResponse.BadRequest(ModelState));
            }
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiBadRequestResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOffenceDispute([FromBody] CreateDisputeCommand createDisputeCommand)
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

            return disputes == null ? NoContent() : Ok(ApiResponse.Result(disputes));
        }

        [HttpGet("{disputeId}")]
        [ProducesResponseType(typeof(IQueryable<Dispute>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDispute(int disputeId)
        {
            var dispute = await _mediator.Send(new GetDisputeQuery { DisputeId=disputeId});
            return dispute == null ? NoContent() : Ok(ApiResponse.Result(dispute));
        }


    }
}
