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
        Task<DisputeViewModel> CreateAsync(DisputeViewModel dispute);
        Task<IEnumerable<DisputeViewModel>> GetAllAsync();
        Task<DisputeViewModel> GetAsync(int disputeId);
        Task<DisputeViewModel> FindDispute(string ticketNumber, int offenceNumber);
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

        public async Task<DisputeViewModel> CreateAsync(DisputeViewModel dispute)
        {

            var existedDispute = await FindDispute(dispute.ViolationTicketNumber, dispute.OffenceNumber);
            if (existedDispute == null)
            {
                _logger.LogInformation("Creating dispute");
                await _context.Disputes.AddAsync(dispute);
                await _context.SaveChangesAsync();
                return dispute;

            }
            _logger.LogError("found the dispute for the same offence, ticketNumber={ticketNumber}, offenceNumer={offenceNumber}", dispute.ViolationTicketNumber, dispute.OffenceNumber);
            return null;

        }

        public async Task<IEnumerable<DisputeViewModel>> GetAllAsync()
        {
            _logger.LogDebug("Getting all disputes");

            var disputes = await _context.Disputes.ToListAsync();

            return disputes;
        }

        public async Task<DisputeViewModel> GetAsync(int disputeId)
        {
            _logger.LogDebug("Get dispute");

            DisputeViewModel dispute = await _context.Disputes.SingleOrDefaultAsync(_ => _.Id == disputeId);

            return dispute;
        }

        public async Task<DisputeViewModel> FindDispute(string ticketNumber, int offenceNumber)
        {
            _logger.LogDebug("Find dispute for ticketNumber {ticketNumber}, offenceNumber {offenceNumber}",ticketNumber,offenceNumber);

            var dispute = await _context.Disputes.FirstOrDefaultAsync(_ => _.ViolationTicketNumber == ticketNumber && _.OffenceNumber==offenceNumber);

            return dispute;
        }
    }
}
