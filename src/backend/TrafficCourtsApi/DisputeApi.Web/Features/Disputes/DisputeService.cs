﻿using System;
using DisputeApi.Web.Infrastructure;
using DisputeApi.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using DisputeApi.Web.Features.Disputes.DBModel;

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
        Task<IEnumerable<Dispute>> GetAllAsync();
        Task<Dispute> GetAsync(int disputeId);
        Task<Dispute> FindDisputeAsync(string ticketNumber, int offenceNumber);
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

            var existedDispute = await FindDisputeAsync(dispute.ViolationTicketNumber, dispute.OffenceNumber);
            if (existedDispute == null)
            {
                _logger.LogDebug("Creating dispute");
                var createdDispute = await _context.Disputes.AddAsync(dispute);
                await _context.SaveChangesAsync();
                return createdDispute.Entity;

            }
            _logger.LogError("found the dispute for the same offence, ticketNumber={ticketNumber}, offenceNumer={offenceNumber}", dispute.ViolationTicketNumber, dispute.OffenceNumber);
            return new Dispute { Id=0};

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

        public async Task<Dispute> FindDisputeAsync(string ticketNumber, int offenceNumber)
        {
            _logger.LogDebug("Find dispute for ticketNumber {ticketNumber}, offenceNumber {offenceNumber}",ticketNumber,offenceNumber);

            var dispute = await _context.Disputes.FirstOrDefaultAsync(_ => _.ViolationTicketNumber == ticketNumber && _.OffenceNumber==offenceNumber);

            return dispute;
        }
    }
}
