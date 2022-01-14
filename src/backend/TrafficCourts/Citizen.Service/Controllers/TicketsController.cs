using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TrafficCourts.Citizen.Service.Features.Tickets;

namespace TrafficCourts.Citizen.Service.Controllers
{
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TicketsController(IMediator mediator)
        {
            ArgumentNullException.ThrowIfNull(mediator);
            _mediator = mediator;
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(Search.Response), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public Task<Search.Response> Search(Search.Request request)
        {
            return _mediator.Send(request);
        }

        [HttpPost("analyse")]
        [DisableRequestSizeLimit]
        public Task<Analyse.AnalyseResponse> AnalyseSync([Required] IFormFile image, CancellationToken cancellationToken)
        {
            Analyse.AnalyseRequest request = new Analyse.AnalyseRequest(image);
            return _mediator.Send(request, cancellationToken);
        }
    }
}
