using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrafficCourts.Citizen.Service.Features.CurrentUserInfo;
using TrafficCourts.Citizen.Service.Models.OAuth;

namespace TrafficCourts.Citizen.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Return userinfo for given access token
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">The UserInfo was found.</response>
        /// <response code="404">The access token doesn't appear to be a valid access token.</response>
        /// <response code="500">There was a internal server error.</response>
        [Authorize]
        [HttpGet("whoami")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> WhoAmI(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(GetCurrentUserInfoRequest.Default, cancellationToken);

            if (response == GetCurrentUserInfoResponse.Empty)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
