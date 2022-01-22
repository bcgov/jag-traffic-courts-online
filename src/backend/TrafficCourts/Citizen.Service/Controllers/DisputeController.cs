using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TrafficCourts.Citizen.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisputeController : ControllerBase
    {
#pragma warning disable IDE0052 // Remove unread private members - will be used in the future
        private readonly IMediator _mediator;
        private readonly ILogger<DisputeController> _logger;
#pragma warning restore IDE0052 // Remove unread private members

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
        public Task<IActionResult> CreateAsync(CancellationToken cancellationToken)
        {
            IActionResult result = Ok(string.Empty);
            return Task.FromResult(result);
        }
    }
}
