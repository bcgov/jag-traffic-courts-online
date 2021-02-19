using DisputeApi.Web.Features.TicketService.Models;
using DisputeApi.Web.Features.TicketService;
using DisputeApi.Web.Test.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Features.Tickets.Services
{
    public class TicketServiceTest
    {
        private TicketService _sut;
        private readonly Mock<ILogger<TicketService>> _loggerMock = LoggerServiceMock.LoggerMock<TicketService>();

        // [SetUp]
        // public void SetUp()
        // {
        //     _sut = new TicketService(_loggerMock.Object);
        // }

        // [Test]
        // public async Task return_list_of_tickets()
        // {
        //     var result = await _sut.ListTickets();
        //     Assert.IsInstanceOf<IQueryable<Ticket>>(result);
        //     _loggerMock.VerifyLog(LogLevel.Information, "Returning list of mock tickets", Times.Once());
        // }
    }
}
