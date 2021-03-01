using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TicketService.Models;
using DisputeApi.Web.Features.TicketService.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace DisputeApi.Web.Features.TicketService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;
        public TicketsController(
            ILogger<TicketsController> logger, ITicketsService ticketsService)
        {
            _logger = logger;
            _ticketsService = ticketsService;
        }

        [HttpPost]
        [Route("saveticket")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> SaveTicket([FromBody] Ticket ticket)
        {
            return Ok(await _ticketsService.SaveTicket(ticket));
        }

        [HttpGet]
        [Route("getTickets")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Ticket>), StatusCodes.Status200OK)]
        [OpenApiTag("Dispute API")]
        public async Task<IActionResult> GetTickets()
        {
            return Ok(await _ticketsService.GetTickets());
        }
    }
}
