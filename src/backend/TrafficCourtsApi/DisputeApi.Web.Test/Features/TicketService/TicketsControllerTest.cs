using DisputeApi.Web.Features.TicketService;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;

namespace DisputeApi.Web.Test.Features.TicketService
{
    [ExcludeFromCodeCoverage]
    public class TicketsControllerTest
    {
        private TicketsController _controller;
        private readonly Mock<ILogger<TicketsController>> _loggerMock = LoggerServiceMock.LoggerMock<TicketsController>();
        private Mock<ITicketsService> _ticketsServiceMock;

        [SetUp]
        public void SetUp()
        {
            _ticketsServiceMock = new Mock<ITicketsService>();

            _controller = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object);
        }

        [Theory]
        [AutoData]
        public async Task get_tickets(Ticket ticket)
        {
            IEnumerable<Ticket> data = new List<Ticket> { ticket };

            _ticketsServiceMock
                .Setup(x => x.GetTickets())
                .Returns(Task.FromResult(data));

            var result = (OkObjectResult)await _controller.GetTickets();
            Assert.IsInstanceOf<IEnumerable<Ticket>>(result.Value);
            Assert.IsNotNull(result);
            Assert.AreEqual(((IEnumerable<Ticket>)result.Value).Count(), 1);

            _ticketsServiceMock.Verify(x => x.GetTickets(), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task save_ticket(Ticket ticket)
        {
            _ticketsServiceMock
                .Setup(x => x.SaveTicket(ticket))
                .Returns(Task.FromResult(ticket));

            var result = (OkObjectResult)await _controller.SaveTicket(ticket);
            Assert.IsInstanceOf<Ticket>(result.Value);
            Assert.IsNotNull(result.Value);

            _ticketsServiceMock.Verify(x => x.SaveTicket(ticket), Times.Once);
        }
    }
}
