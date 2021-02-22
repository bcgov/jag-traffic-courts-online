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
        private TicketsController _controller;
        private readonly Mock<ILogger<TicketsController>> _loggerMock = LoggerServiceMock.LoggerMock<TicketsController>();
        private Mock<ITicketsService> _ticketsServiceMock = new Mock<ITicketsService>();

        [SetUp]
        public void SetUp()
        {
            _controller = new TicketsController(_loggerMock.Object, _ticketsServiceMock.Object);
        }

        [Test]
        public async Task get_tickets()
        {
            var ticket = new Ticket
            {
                Id = 1,
                UserId = "User123",
                ViolationTicketNumber = "LM87878888",
                CourtLocation = "Nanaimo",
                ViolationDate = "11-10-2002 12:23",
                SurName = "Smith",
                GivenNames = "Will",
                Mailing = "Mailing",
                Postal = "V1W0A1",
                City = "Victoria",
                Province = "BC",
                Licence = "R2323G7",
                ProvLicense = "R34343G64",
                HomePhone = "2434552233",
                WorkPhone = "3343353344",
                Birthdate = "11-12-2002",
                LawyerPresent = false,
                InterpreterRequired = true,
                InterpreterLanguage = "French",
                CallWitness = false
            };
            _ticketsServiceMock.Setup(x => x.GetTickets()).Returns(
              Task.FromResult(new List<Ticket> { ticket }.AsQueryable()));
            var result = (OkObjectResult)await _controller.GetTickets();
            Assert.IsInstanceOf<IQueryable<Ticket>>(result.Value);
            Assert.IsNotNull(result);
            Assert.AreEqual(((IQueryable<Ticket>)result.Value).Count(), 1);
            _ticketsServiceMock.Verify(x => x.GetTickets(), Times.Once);

        }

        [Test]
        public async Task save_ticket()
        {
            var ticket = new Ticket
            {
                Id = 2,
                UserId = "User14",
                ViolationTicketNumber = "BC87878888",
                ViolationDate = "11-11-2002 12:23",
                SurName = "Smith",
                GivenNames = "Tim",
            };

            _ticketsServiceMock.Setup(x => x.SaveTicket(ticket)).Returns(Task.FromResult(
               ticket));

            var result = (OkObjectResult)await _controller.SaveTicket(ticket);
            Assert.IsInstanceOf<Ticket>(result.Value);
            Assert.IsNotNull(result.Value);
            _ticketsServiceMock.Verify(x => x.SaveTicket(ticket), Times.Once);

        }
    }
}
