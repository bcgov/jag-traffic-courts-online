using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TicketService.Models;
using DisputeApi.Web.Features.TicketService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace DisputeApi.Web.Features
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITicketService _ticketService;
        public TicketsController(
            ILogger<TicketsController> logger, ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;
        }

        [HttpPost]
        [Route("savetickets")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Ticket>), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> SaveTickets([FromBody] Ticket ticket)
        {
            return Ok(await _ticketService.SaveTicket(ticket));
        }

        [HttpGet]
        [Route("getTickets")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Ticket>), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> GetTickets()
        {
            return Ok(await _ticketService.GetTickets());
        }
    }
}
