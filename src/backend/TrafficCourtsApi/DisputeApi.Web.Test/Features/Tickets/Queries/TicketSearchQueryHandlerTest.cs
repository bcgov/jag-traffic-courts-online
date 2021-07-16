using AutoFixture.Xunit2;
using DisputeApi.Web.Features.Tickets.Queries;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DisputeApi.Web.Test.Features.Tickets.Queries
{
    public class TicketSearchQueryHandlerTest
    {
        [Theory]
        [AutoMockAutoData]
        public async Task TicketDisputeHandler_handle_will_call_retrieveTicket(TicketSearchQuery query, TicketDispute ticketDispute,
            TicketSearchQueryHandler sut)
        {
            //todo: add unit test here.
            //ticketDisputeServiceMock
            //    .Setup(m => m.RetrieveTicketDisputeAsync(It.IsAny<string>(), It.IsAny<string>(),
            //        It.IsAny<CancellationToken>()))
            //    .Returns(Task.FromResult(ticketDispute));
            query.TicketNumber = "E400876";
            query.Time = "18:49";

            var result = await sut.Handle(query, CancellationToken.None);
            //ticketDisputeServiceMock.Verify(
            //    x => x.RetrieveTicketDisputeAsync(query.TicketNumber, query.Time, CancellationToken.None), Times.Once);
        }
    }
}
