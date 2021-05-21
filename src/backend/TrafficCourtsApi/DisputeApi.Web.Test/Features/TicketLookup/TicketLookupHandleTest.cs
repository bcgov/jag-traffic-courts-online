using AutoFixture.Xunit2;
using DisputeApi.Web.Features.TicketLookup;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static DisputeApi.Web.Features.TicketLookup.TicketLookup;

namespace DisputeApi.Web.Test.Features.TicketLookup
{
    [ExcludeFromCodeCoverage]
    public class TicketLookupHandleTest
    {
        [Theory]
        [AutoMockAutoData]
        public async Task TicketDisputeHandler_handle_will_call_retrieveTicket(Query query, TicketDispute ticketDispute,
            [Frozen] Mock<ITicketDisputeService> ticketDisputeServiceMock, TicketDisputeHandler sut)
        {
            ticketDisputeServiceMock
                .Setup(m => m.RetrieveTicketDisputeAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(ticketDispute));
            query.TicketNumber = "E400876";
            query.Time = "18:49";

            var result = await sut.Handle(query, CancellationToken.None);
            ticketDisputeServiceMock.Verify(
                x => x.RetrieveTicketDisputeAsync(query.TicketNumber, query.Time, CancellationToken.None), Times.Once);
        }
    }
}