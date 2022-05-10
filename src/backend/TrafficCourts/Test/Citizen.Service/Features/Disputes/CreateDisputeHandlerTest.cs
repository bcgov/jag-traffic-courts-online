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
using AutoMapper;

namespace TrafficCourts.Test.Citizen.Service.Features.Disputes
{
    public class CreateDisputeHandlerTest
    {
        private readonly Mock<ILogger<Create.CreateDisputeHandler>> _loggerMock = new Mock<ILogger<Create.CreateDisputeHandler>>();

        [Fact]
        public void constructor_throws_ArgumentNullException_when_passed_null()
        {
            var mockDisputeRequestClient = new Mock<IRequestClient<SubmitNoticeOfDispute>>();
            var mockEmailRequestClient = new Mock<IRequestClient<SendEmail>>();
            var mockRedisCacheService = new Mock<IRedisCacheService>();
            var mockAutoMapper = new Mock<IMapper>();

            Assert.Throws<ArgumentNullException>("submitDisputeRequestClient", () => new Create.CreateDisputeHandler(_loggerMock.Object, null, mockEmailRequestClient.Object, mockRedisCacheService.Object, mockAutoMapper.Object));
            Assert.Throws<ArgumentNullException>("sendEmailRequestClient", () => new Create.CreateDisputeHandler(_loggerMock.Object, mockDisputeRequestClient.Object, null, mockRedisCacheService.Object, mockAutoMapper.Object));
            Assert.Throws<ArgumentNullException>("redisCacheService", () => new Create.CreateDisputeHandler(_loggerMock.Object, mockDisputeRequestClient.Object, mockEmailRequestClient.Object, null, mockAutoMapper.Object));
            Assert.Throws<ArgumentNullException>("logger", () => new Create.CreateDisputeHandler(null, mockDisputeRequestClient.Object, mockEmailRequestClient.Object, mockRedisCacheService.Object, mockAutoMapper.Object));
            Assert.Throws<ArgumentNullException>("mapper", () => new Create.CreateDisputeHandler(_loggerMock.Object, mockDisputeRequestClient.Object, mockEmailRequestClient.Object, mockRedisCacheService.Object, null));
        }

        [Fact]
        public async void TestHandleReturnsResponse()
        {
            var mockTicketDispute = new Mock<NoticeOfDispute>();
            var mockViolationTicket = new Mock<TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket>();
            Guid mockGuid = Guid.NewGuid();

            
            var mockEmailRequestClient = new Mock<IRequestClient<SendEmail>>();
            var mockRedisCacheService = new Mock<IRedisCacheService>();
            var mockAutoMapper = new Mock<IMapper>();
            mockAutoMapper.Setup(_ => _.Map<SubmitNoticeOfDispute>(It.IsAny<NoticeOfDispute>())).Returns(new SubmitNoticeOfDispute());

            var mockDisputeRequestClient = new Mock<IRequestClient<SubmitNoticeOfDispute>>();
            mockDisputeRequestClient.Setup(x => x.GetResponse<DisputeSubmitted>(It.IsAny<SubmitNoticeOfDispute>(), default, default)
            .Result.Message).Returns(new DisputeSubmitted
            {
                DisputeId = mockGuid,
            });

            var disputeHandler = new Create.CreateDisputeHandler(_loggerMock.Object, mockDisputeRequestClient.Object, mockEmailRequestClient.Object, mockRedisCacheService.Object, mockAutoMapper.Object);

            NoticeOfDispute dispute = new NoticeOfDispute();
            var request = new Create.Request(dispute);

            // Act
            Create.Response response = await disputeHandler.Handle(request, CancellationToken.None);


            Assert.Equal(mockGuid, response.Id);
        }


    }
}
