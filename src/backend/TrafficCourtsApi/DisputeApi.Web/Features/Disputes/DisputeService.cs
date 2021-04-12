using System;
using DisputeApi.Web.Infrastructure;
using DisputeApi.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Disputes
{
    public interface IDisputeService
    {
        Task<Dispute> CreateAsync(Dispute dispute);
        Task<IEnumerable<Dispute>> GetAllAsync();
        Task<Dispute> GetAsync(int disputeId);
        Task<Dispute> FindDispute(string ticketNumber, int offenceNumber);
    }

    public class DisputeService : IDisputeService
    {
        private readonly ILogger<DisputeService> _logger;

        private readonly ViolationContext _context;

        public DisputeService(ILogger<DisputeService> logger, ViolationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Dispute> CreateAsync(Dispute dispute)
        {
            _logger.LogDebug("Creating dispute");

            await _context.Disputes.AddAsync(dispute);
            await _context.SaveChangesAsync();

            return dispute;
        }

        public async Task<IEnumerable<Dispute>> GetAllAsync()
        {
            _logger.LogDebug("Getting all disputes");

            var disputes = await _context.Disputes.ToListAsync();

            return disputes;
        }

        public async Task<Dispute> GetAsync(int disputeId)
        {
            _logger.LogDebug("Get dispute");

            Dispute dispute = await _context.Disputes.SingleOrDefaultAsync(_ => _.Id == disputeId);

            return dispute;
        }

        public async Task<Dispute> FindDispute(string ticketNumber, int offenceNumber)
        {
            _logger.LogDebug("Find dispute for ticketNumber {ticketNumber}, offenceNumber {offenceNumber}",ticketNumber,offenceNumber);

            var dispute = await _context.Disputes.FirstOrDefaultAsync(_ => _.ViolationTicketNumber == ticketNumber && _.OffenceNumber==offenceNumber);

            return dispute;
        }
    }
}
