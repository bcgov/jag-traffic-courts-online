using DisputeApi.Web.Features;
using DisputeApi.Web.Features.TicketService.Models;
using DisputeApi.Web.Features.TicketService;
using DisputeApi.Web.Features.TicketService.Controller;
using DisputeApi.Web.Features.TicketService.Service;
using DisputeApi.Web.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Features.TicketService
{
    public class TicketsControllerTest
    {
        private TicketsController _sut;
        private readonly Mock<ILogger<TicketsController>> _loggerMock = LoggerServiceMock.LoggerMock<TicketsController>();
        private Mock<ITicketsService> _ticketsServiceMock = new Mock<ITicketsService>();

        [SetUp]
        public void SetUp()
        {
            _ticketsServiceMock.Setup(x => x.GetTickets()).Returns(
                Task.FromResult(new List<Ticket> {  new Ticket
                {
                    TicketNumber = 11234,
                    Name = "John Doe",
                    DateOfIssue = "11-12-2002",
                    TimeOfIssue = "12:23",
                    DriversLicence = "L2323G7"
                } }.AsQueryable()));


            _sut = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object);
        }

        [Test]
        public async Task return_list_of_tickets()
        {
            var result = (OkObjectResult)await _sut.GetTickets();
            Assert.IsInstanceOf<IQueryable<Ticket>>(result.Value);
            Assert.IsNotNull(result);
            Assert.AreEqual(((IQueryable<Ticket>)result.Value).Count(), 1);
            _ticketsServiceMock.Verify(x => x.GetTickets(), Times.Once);

        }
    }
}
