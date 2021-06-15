using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DisputeApi.Web.Features;
using DisputeApi.Web.Features.Tickets;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static DisputeApi.Web.Features.TicketLookup.TicketLookup;

namespace DisputeApi.Web.Test.Features.Tickets
{
    [ExcludeFromCodeCoverage]
    public class TicketsControllerTest
    {
        private Mock<ILogger<TicketsController>> _loggerMock;
        private Mock<ITicketsService> _ticketsServiceMock;
        private Mock<IMediator> _mediatorMock;

        public TicketsControllerTest()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<TicketsController>();
            _ticketsServiceMock = new Mock<ITicketsService>();
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void throw_ArgumentNullException_if_passed_null()
#pragma warning restore IDE1006 // Naming Styles
        {
            Assert.Throws<ArgumentNullException>(() => new TicketsController(null, _ticketsServiceMock.Object, _mediatorMock.Object));
            Assert.Throws<ArgumentNullException>(() => new TicketsController(_loggerMock.Object, null, _mediatorMock.Object));
            Assert.Throws<ArgumentNullException>(() => new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, null));
        }

        [Theory]
        [AutoData]
#pragma warning disable IDE1006 // Naming Styles
        public async Task get_tickets(Ticket ticket)
#pragma warning restore IDE1006 // Naming Styles
        {
            IEnumerable<Ticket> data = new List<Ticket> { ticket };

            _ticketsServiceMock
                .Setup(x => x.GetTickets())
                .Returns(Task.FromResult(data));

            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);

            var result = (OkObjectResult)await sut.GetTickets();
            Assert.IsAssignableFrom<IEnumerable<Ticket>>(result.Value);
            Assert.NotNull(result);
            var actual = result.Value as IEnumerable<Ticket>;

            Assert.Single(actual);

            _ticketsServiceMock.Verify(x => x.GetTickets(), Times.Once);
        }

        [Theory]
        [AutoData]
#pragma warning disable IDE1006 // Naming Styles
        public async Task save_ticket(Ticket ticket)
#pragma warning restore IDE1006 // Naming Styles
        {
            _ticketsServiceMock
                .Setup(x => x.SaveTicket(ticket))
                .Returns(Task.FromResult(ticket));

            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);

            var result = (OkObjectResult)await sut.SaveTicket(ticket);
            Assert.IsAssignableFrom<Ticket>(result.Value);
            Assert.NotNull(result.Value);

            _ticketsServiceMock.Verify(x => x.SaveTicket(ticket), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_response_with_OK(Query query, TicketDispute response)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);
            _mediatorMock.Setup(m => m.Send(It.IsAny<Query>(), CancellationToken.None)).Returns(Task.FromResult(response));
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = (OkObjectResult)await sut.GetTicket(query);
            Assert.IsAssignableFrom<ApiResultResponse<TicketDispute>>(result.Value);
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_null_with_NoContent(Query query)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);
            _mediatorMock.Setup(m => m.Send(It.IsAny<Query>(), CancellationToken.None)).Returns(Task.FromResult<TicketDispute>(null));
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = (NoContentResult)await sut.GetTicket(query);
            Assert.Equal(204, result.StatusCode);
        }
    }
}
