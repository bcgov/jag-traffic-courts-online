using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Gov.CitizenApi.Features.Lookups;
using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.CitizenApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gov.CitizenApi.Features.Tickets
{

    public interface ITicketsService
    {
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        Task<IEnumerable<Ticket>> GetTickets();
        Task<Ticket> FindTicketAsync(string ticketNumber, string ticketTime=null);
        Task<Payment> CreatePaymentAsync(Payment ticket);
        Task<Payment> FindPaymentAsync(Guid guid);
        Task<Payment> UpdatePaymentAsync(Payment ticket);
        List<Payment> FindTicketPayments(string ticketNumber, string ticketTime = null);
    }

    public class TicketsService : ITicketsService
    {
        private readonly ILogger<TicketsService> _logger;

        private readonly ViolationContext _context;

        public TicketsService(ILogger<TicketsService> logger, ViolationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            var existedTicket = await FindTicketAsync(ticket.ViolationTicketNumber);
            if (existedTicket == null)
            {
                _logger.LogDebug("Creating ticket");
                var createdTicket = await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();
                return createdTicket.Entity;

            }
            _logger.LogError("found the ticket for the same ticketNumber={ticketNumber}", ticket.ViolationTicketNumber);
            return new Ticket { Id = 0 };
        }

        public async Task<IEnumerable<Ticket>> GetTickets()
        {
            _logger.LogDebug("Returning all tickets");
            var tickets = await _context.Tickets.ToListAsync();

            return tickets;
        }

        public async Task<Ticket> FindTicketAsync(string ticketNumber, string ticketTime=null)
        {
            _logger.LogDebug("Find ticket for ticketNumber {ticketNumber}", ticketNumber);

            var ticket = await _context.Tickets.FirstOrDefaultAsync(_ => _.ViolationTicketNumber == ticketNumber && (ticketTime==null || _.ViolationTime==ticketTime));
            if (ticket != null)
            {
                ticket.Offences = new Collection<DBModel.Offence>(
                    _context.Offences.Where(m => m.TicketId == ticket.Id).ToList()
                    );
            }
            return ticket;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {         
            _logger.LogDebug("Creating payment in db");
            var createdPayment = await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return createdPayment.Entity;
        }

        public async Task<Payment> FindPaymentAsync(Guid guid)
        {
            _logger.LogDebug("Creating payment in db");
            var foundPayment = await _context.Payments.FirstOrDefaultAsync(p=>p.Guid==guid);
            return foundPayment;
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            _logger.LogDebug("Updating payment in db");
            var existingPayment = await FindPaymentAsync(payment.Guid);
            existingPayment.PaidAmount = payment.PaidAmount;
            existingPayment.PaymentStatus = payment.PaymentStatus;
            existingPayment.TransactionId = payment.TransactionId;
            existingPayment.CompletedDateTime = payment.CompletedDateTime;
            existingPayment.ConfirmationNumber = payment.ConfirmationNumber;
            var updatedPayment = _context.Update(existingPayment);
            await _context.SaveChangesAsync();
            return updatedPayment.Entity;
        }

        public List<Payment> FindTicketPayments(string ticketNumber, string ticketTime = null)
        {
            return _context.Payments.Where(m=>m.ViolationTicketNumber==ticketNumber).ToList();
        }
    }
}
