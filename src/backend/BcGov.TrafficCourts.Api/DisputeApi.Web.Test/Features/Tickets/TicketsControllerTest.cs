using DisputeApi.Web.Features;
using DisputeApi.Web.Features.Tickets.Models;
using DisputeApi.Web.Features.Tickets.Services;
using DisputeApi.Web.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Features.Tickets
{
    public class TicketsControllerTest
    {
        private TicketsController _sut;
        private readonly Mock<ILogger<TicketsController>> _loggerMock = LoggerServiceMock.LoggerMock<TicketsController>();
        private Mock<ITicketService> _ticketServiceMock = new Mock<ITicketService>();

        [SetUp]
        public void SetUp()
        {
            _ticketServiceMock.Setup(x => x.ListTickets()).Returns(
                Task.FromResult(new List<Ticket> { new Ticket { Id = "TicketId", Description = "Ticket Description" } }.AsQueryable()));


            _sut = new TicketsController(_loggerMock.Object, _ticketServiceMock.Object);
        }

        [Test]
        public async Task return_list_of_tickets()
        {
            var result = (OkObjectResult)await _sut.GetListOfTickets();
            Assert.IsInstanceOf<IQueryable<Ticket>>(result.Value);
            Assert.IsNotNull(result);
            Assert.AreEqual(((IQueryable<Ticket>)result.Value).Count(), 1);
            _ticketServiceMock.Verify(x => x.ListTickets(), Times.Once);
            
        }
    }
}
