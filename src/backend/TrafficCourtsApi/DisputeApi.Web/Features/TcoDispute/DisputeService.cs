using DisputeApi.Web.Features.TcoDispute.Models;
using DisputeApi.Web.Features.TicketService.DBContexts;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.TcoDispute.Service
{
    public interface IDisputeService
    {
        Task<Dispute> CreateDispute(Dispute dispute);
        Task<IQueryable<Dispute>> GetDisputes();
        Task<Dispute> GetDispute(int disputeId);
    }

    public class DisputeService : IDisputeService
    {
        private readonly ILogger<DisputeService> _logger;

        private readonly TicketContext _context;

        public DisputeService(ILogger<DisputeService> logger, TicketContext context)
        {
            _logger = logger;
            _context = context;
            if (!_context.Disputes.Any())
            {
                _context.Disputes.Add(new Dispute
                {
                    Id = 2,
                    TicketId = 2,
                    EmailAddress = "jones_234@email.com",
                    LawyerPresent = true,
                    InterpreterRequired = true,
                    InterpreterLanguage = "Spanish",
                    CallWitness = false,
                    CertifyCorrect = true,
                    StatusCode = "SUBM"
                });
                _context.SaveChanges();
            }
        }

        public async Task<Dispute> CreateDispute(Dispute dispute)
        {
            _logger.LogInformation("Creating mock dispute");
            _context.Disputes.Add(dispute);
            _context.SaveChanges();
            return await Task.FromResult(dispute);
        }

        public async Task<IQueryable<Dispute>> GetDisputes()
        {
            _logger.LogInformation("Returning list of mock disputes");
            return await Task.FromResult(_context.Disputes);
        }

        public async Task<Dispute> GetDispute(int disputeId)
        {
            _logger.LogInformation("Returning a specific mock dispute");
            Dispute dispute = _context.Disputes.SingleOrDefault(s => s.Id == disputeId);
            return await Task.FromResult(dispute);
        }
    }
}
