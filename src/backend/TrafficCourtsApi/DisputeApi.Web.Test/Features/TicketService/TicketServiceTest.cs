using Microsoft.EntityFrameworkCore;
using DisputeApi.Web.Features.TicketService.Models;
using DisputeApi.Web.Features.TicketService.Service;
using DisputeApi.Web.Features.TicketService.DBContexts;
using DisputeApi.Web.Test.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Features.TicketService.Services
{
    public class TicketServiceTest
    {
        private ITicketsService _service;
        private readonly Mock<ILogger<TicketsService>> _loggerMock = LoggerServiceMock.LoggerMock<TicketsService>();

        [SetUp]
        public void SetUp()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TicketContext>();
            optionsBuilder.UseInMemoryDatabase("DisputeApi");
            _service = new TicketsService(_loggerMock.Object, new TicketContext(optionsBuilder.Options));
        }

        [Test]
        public async Task get_tickets()
        {
            var result = await _service.GetTickets();
            Assert.IsInstanceOf<IQueryable<Ticket>>(result);
            _loggerMock.VerifyLog(LogLevel.Information, "Returning list of mock tickets", Times.Once());
        }

        [Test]
        public async Task save_ticket()
        {
            var ticket = new Ticket
            {
                Id = 2,
                UserId = "User125",
                ViolationTicketNumber = "AX87877777",
                ViolationDate = "11-11-2002 12:23",
                SurName = "Smith",
                GivenNames = "Tim"
            };
            var result = await _service.SaveTicket(ticket);
            Assert.IsInstanceOf<Ticket>(result);
            _loggerMock.VerifyLog(LogLevel.Information, "Saving mock ticket", Times.Once());
        }
    }
}
