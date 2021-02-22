
using DisputeApi.Web.Features.TicketService.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TicketService.DBContexts;

namespace DisputeApi.Web.Features.TicketService.Service
{

    public interface ITicketsService
    {
        Task<Ticket> SaveTicket(Ticket ticket);
        Task<IQueryable<Ticket>> GetTickets();
    }

    public class TicketsService : ITicketsService
    {
        private readonly ILogger<TicketsService> _logger;

        private readonly TicketContext _context;

        public TicketsService(ILogger<TicketsService> logger, TicketContext context)
        {
            _logger = logger;
            _context = context;
            if (!_context.Tickets.Any())
            {
                _context.Tickets.Add(new Ticket
                {
                    Id = 0001,
                    UserId = "nouser",
                    ViolationTicketNumber = "AH135699",
                    CourtLocation = "Saanich",
                    ViolationDate = "09-09-2009 12:23",
                    SurName = "Hope",
                    GivenNames = "Bob",
                    Mailing = "PO 8908",
                    Postal = "V0X0B0",
                    City = "Saanich",
                    Province = "BC",
                    Licence = "L2523G7",
                    ProvLicense = "L34543G64",
                    HomePhone = "2437772233",
                    WorkPhone = "3349993344",
                    Birthdate = "06-06-2006",
                    LawyerPresent = false,
                    InterpreterRequired = false,
                    CallWitness = true
                });
                _context.SaveChanges();
            }
        }
        public async Task<Ticket> SaveTicket(Ticket ticket)
        {
            _logger.LogInformation("Saving mock ticket");
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return await Task.FromResult(ticket);
        }

        public async Task<IQueryable<Ticket>> GetTickets()
        {
            _logger.LogInformation("Returning list of mock tickets");
            return await Task.FromResult(_context.Tickets);
        }
    }
}
