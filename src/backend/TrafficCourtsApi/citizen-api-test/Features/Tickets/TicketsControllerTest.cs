using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Gov.CitizenApi.Features;
using Gov.CitizenApi.Features.Tickets;
using Gov.CitizenApi.Features.Tickets.Commands;
using Gov.CitizenApi.Features.Tickets.Queries;
using Gov.CitizenApi.Models;
using Gov.CitizenApi.Test.Utils;
using Gov.TicketSearch;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TrafficCourts.Common;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Tickets
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
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
        public void Contructor_throw_ArgumentNullException_if_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new TicketsController(null, _ticketsServiceMock.Object, _mediatorMock.Object));
            Assert.Throws<ArgumentNullException>(() => new TicketsController(_loggerMock.Object, null, _mediatorMock.Object));
            Assert.Throws<ArgumentNullException>(() => new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, null));
        }
       
        [Theory]
        [AutoData]
        public async Task CreateShellTicket(CreateShellTicketCommand ticket, CreateShellTicketResponse response)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateShellTicketCommand>(), CancellationToken.None)).Returns(Task.FromResult(response));
            ActionResult result = (ActionResult)await sut.ShellTicket(ticket);
            Assert.True(result is RedirectToActionResult);
            Assert.True(((RedirectToActionResult)result).ActionName == "Ticket");

        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_response_with_OK(TicketSearchQuery query, TicketDispute response)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);
            _mediatorMock.Setup(m => m.Send(It.IsAny<TicketSearchQuery>(), CancellationToken.None)).Returns(Task.FromResult(response));
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = (OkObjectResult)await sut.Ticket(query);
            Assert.IsAssignableFrom<ApiResultResponse<TicketDispute>>(result.Value);
            Assert.NotNull(result.Value);
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_null_with_NoContent(TicketSearchQuery query)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);
            _mediatorMock.Setup(m => m.Send(It.IsAny<TicketSearchQuery>(), CancellationToken.None)).Returns(Task.FromResult<TicketDispute>(null));
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = (NoContentResult)await sut.Ticket(query);
            Assert.Equal(204, result.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_null_with_TicketSearchException_noContent(TicketSearchQuery query)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);
            _mediatorMock.Setup(m => m.Send(It.IsAny<TicketSearchQuery>(), CancellationToken.None)).Throws(
                new TicketSearchException("message", 204, null, null, null)
                );
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = (NoContentResult)await sut.Ticket(query);
            Assert.Equal(204, result.StatusCode);
        }

        [Theory]
        [AutoData]
        public async Task GetTicket_return_InternalServerError_when_server_throws_exception(TicketSearchQuery query)
        {
            TicketsController sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object, _mediatorMock.Object);
            _mediatorMock.Setup(m => m.Send(It.IsAny<TicketSearchQuery>(), CancellationToken.None)).Throws(
                new TicketSearchException("message", 500, null, null, null)
                );
            query.TicketNumber = "EZ02000460";
            query.Time = "09:21";
            var result = (ObjectResult)await sut.Ticket(query);
            Assert.Equal(500, result.StatusCode);
        }
    }
}
