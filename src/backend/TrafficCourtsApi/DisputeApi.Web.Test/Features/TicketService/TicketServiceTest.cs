using DisputeApi.Web.Features.TicketService;
using DisputeApi.Web.Infrastructure;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Features.TicketService
{
    [ExcludeFromCodeCoverage]
    public class TicketServiceTest
    {
        private ITicketsService _service;
        private readonly Mock<ILogger<TicketsService>> _loggerMock = LoggerServiceMock.LoggerMock<TicketsService>();

        [SetUp]
        public void SetUp()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            optionsBuilder.UseInMemoryDatabase("DisputeApi");
            _service = new TicketsService(_loggerMock.Object, new ViolationContext(optionsBuilder.Options));
        }

        [Test]
        public async Task get_tickets()
        {
            var result = await _service.GetTickets();
            Assert.IsInstanceOf<IEnumerable<Ticket>>(result);
            //_loggerMock.VerifyLog(LogLevel.Information, "Returning list of mock tickets", Times.Once());
        }

        [Test]
        public async Task save_ticket()
        {
            var ticket = new Ticket
            {
                Id = 3,
                UserId = "User125",
                ViolationTicketNumber = "AX87877777",
                ViolationDate = "11-11-2002 12:23",
                SurName = "Smith",
                GivenNames = "Tim"
            };
            var result = await _service.SaveTicket(ticket);
            Assert.IsInstanceOf<Ticket>(result);
            //_loggerMock.VerifyLog(LogLevel.Information, "Saving mock ticket", Times.Once());
        }
    }
}
