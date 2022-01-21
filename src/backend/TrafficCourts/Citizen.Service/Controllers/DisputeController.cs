using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TrafficCourts.Citizen.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisputeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DisputeController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="mediator"/> or <paramref name="logger"/> is null.</exception>
        public DisputeController(IMediator mediator, ILogger<DisputeController> logger)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            ArgumentNullException.ThrowIfNull(logger);

            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAsync(CancellationToken cancellationToken)
        {
            return Ok(string.Empty);
        }
    }
}
