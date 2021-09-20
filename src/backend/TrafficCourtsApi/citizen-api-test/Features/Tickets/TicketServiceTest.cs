using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Gov.CitizenApi.Features.Lookups;
using Gov.CitizenApi.Features.Tickets;
using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.CitizenApi.Infrastructure;
using Gov.CitizenApi.Models;
using Gov.CitizenApi.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Tickets
{
    [ExcludeFromCodeCoverage]
    public class TicketServiceTest
    {
        private readonly ITicketsService _service;
        private readonly Mock<ILogger<TicketsService>> _loggerMock;
        private readonly Mock<ILookupsService> _lookupsMock;

        private static ViolationContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            // use a random name because the in-memory database is shared anywhere the same name is used.
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            return new ViolationContext(optionsBuilder.Options);
        }

        public TicketServiceTest()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<TicketsService>();
            _lookupsMock = new Mock<ILookupsService>();
            _service = new TicketsService(_loggerMock.Object, CreateContext());
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void throw_ArgumentNullException_if_passed_null()
#pragma warning restore IDE1006 // Naming Styles
        {
            Assert.Throws<ArgumentNullException>(() => new TicketsService(null, CreateContext()));
            Assert.Throws<ArgumentNullException>(() => new TicketsService(_loggerMock.Object, null));
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public async Task get_tickets()
#pragma warning restore IDE1006 // Naming Styles
        {
            var result = await _service.GetTickets();
            Assert.IsAssignableFrom<IEnumerable<Ticket>>(result);
            //_loggerMock.VerifyLog(LogLevel.Information, "Returning list of mock tickets", Times.Once());
        }

        [Theory]
        [AllowCirculationAutoData]
        public async Task CreateTicket(Ticket ticket)
        {
            _lookupsMock.Setup(m => m.GetCountStatute(It.IsAny<decimal> ())).Returns(new Statute {Name="test", Code=123 });
            var result = await _service.CreateTicketAsync(ticket);
            Assert.IsAssignableFrom<Ticket>(result);
        }
    }
}
