using System;
using DisputeApi.Web.Infrastructure;
using DisputeApi.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using DisputeApi.Web.Features.Disputes.DBModel;
using System.Linq;
using System.Collections.ObjectModel;
using OffenceDisputeDetail = DisputeApi.Web.Features.Disputes.DBModel.OffenceDisputeDetail;

namespace DisputeApi.Web.Features.Disputes
{
    public interface IDisputeService
    {
        /// <summary>
        /// create dispute in db. 
        /// </summary>
        /// <param name="dispute">the dispute to create</param>
        /// <returns>the dispute created, with Id value. If the dispute existed, return id=0 </returns>
        Task<Dispute> CreateAsync(Dispute dispute);
        Task<Dispute> UpdateAsync(Dispute dispute);
        Task<IEnumerable<Dispute>> GetAllAsync();
        Task<Dispute> GetAsync(int disputeId);
        Task<Dispute> FindTicketDisputeAsync(string ticketNumber);
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
            var existedDispute = await FindTicketDisputeAsync(dispute.ViolationTicketNumber);
            if (existedDispute == null)
            {
                _logger.LogDebug("Creating dispute");
                var createdDispute = await _context.Disputes.AddAsync(dispute);
                await _context.SaveChangesAsync();
                return createdDispute.Entity;

            }
            _logger.LogError("found the dispute for the same ticketNumber={ticketNumber}", dispute.ViolationTicketNumber);
            return new Dispute { Id = 0 };
        }

        public async Task<Dispute> UpdateAsync(Dispute dispute)
        {
            try
            {
                _logger.LogDebug("Update dispute");
                var updatedDispute = _context.Disputes.Update(dispute);
                await _context.SaveChangesAsync();
                return updatedDispute.Entity;
            }
            catch (Exception e)
            {
                string str = e.Message;
                return null;
            }
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

        public async Task<Dispute> FindTicketDisputeAsync(string ticketNumber)
        {
            _logger.LogDebug("Find dispute for ticketNumber {ticketNumber}", ticketNumber);

            var dispute = await _context.Disputes.FirstOrDefaultAsync(_ => _.ViolationTicketNumber == ticketNumber);
            if (dispute != null)
            {
                dispute.OffenceDisputeDetails = new Collection<DBModel.OffenceDisputeDetail>(
                    _context.OffenceDisputeDetails.Where(m => m.DisputeId == dispute.Id).ToList()
                    );
            }
            return dispute;
        }
    }
}
