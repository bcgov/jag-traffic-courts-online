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
                TicketNumber = 11235,
                Name = "Jane Doe",
                DateOfIssue = "11-12-2003",
                TimeOfIssue = "12:24",
                DriversLicence = "L2323G8"
            };
            var result = await _service.SaveTicket(ticket);
            Assert.IsInstanceOf<Ticket>(result);
            _loggerMock.VerifyLog(LogLevel.Information, "Saving mock ticket", Times.Once());
        }
    }
}
