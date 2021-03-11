using DisputeApi.Web.Features.TcoDispute.Models;
using DisputeApi.Web.Features.TcoDispute.Service;
using DisputeApi.Web.Features.TicketService.DBContexts;
using DisputeApi.Web.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Features.TcoDispute.Services
{
    public class DisputeServiceTest
    {
        private IDisputeService _service;
        private readonly Mock<ILogger<DisputeService>> _loggerMock = LoggerServiceMock.LoggerMock<DisputeService>();

        [SetUp]
        public void SetUp()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TicketContext>();
            optionsBuilder.UseInMemoryDatabase("DisputeApi");
            _service = new DisputeService(_loggerMock.Object, new TicketContext(optionsBuilder.Options));
        }

        [Test]
        public async Task get_disputes()
        {
            var result = await _service.GetDisputes();
            Assert.IsInstanceOf<IQueryable<Dispute>>(result);
            _loggerMock.VerifyLog(LogLevel.Information, "Returning list of mock disputes", Times.Once());
        }

        [Test]
        public async Task create_and_get_dispute()
        {
            var dispute = new Dispute
            {
                Id = 3,
                TicketId = 3,
                EmailAddress = "jones_234@email.com",
                LawyerPresent = true,
                InterpreterRequired = true,
                InterpreterLanguage = "Spanish",
                CallWitness = false,
                CertifyCorrect = true,
                StatusCode = "SUBM"
            };
            
            var result = await _service.CreateDispute(dispute);
            Assert.IsInstanceOf<Dispute>(result);
            _loggerMock.VerifyLog(LogLevel.Information, "Creating mock dispute", Times.Once());

            result = await _service.GetDispute(3);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, dispute.Id);
            _loggerMock.VerifyLog(LogLevel.Information, "Returning a specific mock dispute", Times.Once());
        }
    }
}
