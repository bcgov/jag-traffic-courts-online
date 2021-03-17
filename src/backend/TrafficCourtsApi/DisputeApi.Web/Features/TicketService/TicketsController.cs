using DisputeApi.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.TicketService
{
    [Route("api/[controller]")]
    [ApiController]
    [OpenApiTag("Dispute API")]
    public class TicketsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;
        public TicketsController(ILogger<TicketsController> logger, ITicketsService ticketsService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketsService ?? throw new ArgumentNullException(nameof(ticketsService));
        }

        [HttpPost]
        [Route("saveticket")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Ticket), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveTicket([FromBody] Ticket ticket)
        {
            return Ok(await _ticketsService.SaveTicket(ticket));
        }

        [HttpGet]
        [Route("getTickets")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<Ticket>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTickets()
        {
            return Ok(await _ticketsService.GetTickets());
        }
    }
}
