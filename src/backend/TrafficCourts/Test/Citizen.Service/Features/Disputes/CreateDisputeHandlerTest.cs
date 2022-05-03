using Microsoft.Extensions.Logging;
using Moq;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Models.Dispute;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Messaging.MessageContracts;
using Xunit;
using Xunit.Abstractions;

namespace TrafficCourts.Test.Citizen.Service.Features.Disputes
{
    public class CreateDisputeHandlerTest
    {
        private readonly Mock<ILogger<Create.CreateDisputeHandler>> _loggerMock = new Mock<ILogger<Create.CreateDisputeHandler>>();

        [Fact]
        public void constructor_throws_ArgumentNullException_when_passed_null()
        {
            var mockDisputeRequestClient = new Mock<IRequestClient<SubmitDispute>>();
            var mockEmailRequestClient = new Mock<IRequestClient<SendEmail>>();
            var mockRedisCacheService = new Mock<IRedisCacheService>();

            Assert.Throws<ArgumentNullException>("submitDisputeRequestClient", () => new Create.CreateDisputeHandler(_loggerMock.Object, null, mockEmailRequestClient.Object, mockRedisCacheService.Object));
            Assert.Throws<ArgumentNullException>("sendEmailRequestClient", () => new Create.CreateDisputeHandler(_loggerMock.Object, mockDisputeRequestClient.Object, null, mockRedisCacheService.Object));
            Assert.Throws<ArgumentNullException>("redisCacheService", () => new Create.CreateDisputeHandler(_loggerMock.Object, mockDisputeRequestClient.Object, mockEmailRequestClient.Object, null));
            Assert.Throws<ArgumentNullException>("logger", () => new Create.CreateDisputeHandler(null, mockDisputeRequestClient.Object, mockEmailRequestClient.Object, mockRedisCacheService.Object));
        }

        [Fact]
        public async void TestHandleReturnsResponse()
        {
            var mockTicketDispute = new Mock<TicketDispute>();
            Guid mockGuid = Guid.NewGuid();

            var mockDisputeRequestClient = new Mock<IRequestClient<SubmitDispute>>();
            mockDisputeRequestClient.Setup(x => x.GetResponse<DisputeSubmitted>(It.IsAny<Object>(), default, default)
            .Result.Message).Returns(new DisputeSubmitted
            {
                DisputeId = mockGuid,
            });
            var mockEmailRequestClient = new Mock<IRequestClient<SendEmail>>();
            var mockRedisCacheService = new Mock<IRedisCacheService>();
            var disputeHandler = new Create.CreateDisputeHandler(_loggerMock.Object, mockDisputeRequestClient.Object, mockEmailRequestClient.Object, mockRedisCacheService.Object);

            var request = new Create.Request(new TicketDispute());

            // Act
            Create.Response response = await disputeHandler.Handle(request, CancellationToken.None);


            Assert.Equal(mockGuid, response.Id);
        }


    }
}
