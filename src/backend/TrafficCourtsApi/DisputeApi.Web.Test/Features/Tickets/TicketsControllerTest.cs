using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using DisputeApi.Web.Features.Tickets;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Tickets
{
    [ExcludeFromCodeCoverage]
    public class TicketsControllerTest
    {
        private Mock<ILogger<TicketsController>> _loggerMock;
        private Mock<ITicketsService> _ticketsServiceMock;
        private Mock<IMediator> _mediatorMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<TicketsController>();
            _ticketsServiceMock = new Mock<ITicketsService>();
            _mediatorMock = new Mock<IMediator>();
        }

        [Test]
        public void throw_ArgumentNullException_if_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new TicketsController(null, _ticketsServiceMock.Object, _mediatorMock.Object));
            Assert.Throws<ArgumentNullException>(() => new TicketsController(_loggerMock.Object, null, _mediatorMock.Object));
            Assert.Throws<ArgumentNullException>(() => new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, null));
        }

        [Theory]
        [AutoData]
        public async Task get_tickets(Ticket ticket)
        {
            IEnumerable<Ticket> data = new List<Ticket> { ticket };

            _ticketsServiceMock
                .Setup(x => x.GetTickets())
                .Returns(Task.FromResult(data));

            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);

            var result = (OkObjectResult)await sut.GetTickets();
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

            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);

            var result = (OkObjectResult)await sut.SaveTicket(ticket);
            Assert.IsInstanceOf<Ticket>(result.Value);
            Assert.IsNotNull(result.Value);

            _ticketsServiceMock.Verify(x => x.SaveTicket(ticket), Times.Once);
        }
    }
}
