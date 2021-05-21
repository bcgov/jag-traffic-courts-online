using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DisputeApi.Web.Features.Tickets;
using DisputeApi.Web.Infrastructure;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DisputeApi.Web.Test.Features.Tickets
{
    [ExcludeFromCodeCoverage]
    public class TicketServiceTest
    {
        private ITicketsService _service;
        private Mock<ILogger<TicketsService>> _loggerMock;

        private ViolationContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            optionsBuilder.UseInMemoryDatabase("DisputeApi");

            return new ViolationContext(optionsBuilder.Options);
        }

        public TicketServiceTest()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<TicketsService>();
            _service = new TicketsService(_loggerMock.Object, CreateContext());
        }

        [Fact]
        public void throw_ArgumentNullException_if_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new TicketsService(null, CreateContext()));
            Assert.Throws<ArgumentNullException>(() => new TicketsService(_loggerMock.Object, null));
        }

        [Fact]
        public async Task get_tickets()
        {
            var result = await _service.GetTickets();
            Assert.IsAssignableFrom<IEnumerable<Ticket>>(result);
            //_loggerMock.VerifyLog(LogLevel.Information, "Returning list of mock tickets", Times.Once());
        }

        [Theory]
        [AutoData]
        public async Task save_ticket(Ticket ticket)
        {
            var result = await _service.SaveTicket(ticket);
            Assert.IsAssignableFrom<Ticket>(result);
            //_loggerMock.VerifyLog(LogLevel.Information, "Saving mock ticket", Times.Once());
        }
    }
}
