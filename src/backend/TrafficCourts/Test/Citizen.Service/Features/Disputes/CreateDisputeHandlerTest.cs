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
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Messaging.MessageContracts;
using Xunit;
using Xunit.Abstractions;
using AutoMapper;
using MediatR;
using NodaTime;
using NodaTime.Testing;

namespace TrafficCourts.Test.Citizen.Service.Features.Disputes
{
    public class CreateDisputeHandlerTest
    {
        private readonly Mock<ILogger<Create.Handler>> _loggerMock = new Mock<ILogger<Create.Handler>>();

        [Fact]
        public void constructor_throws_ArgumentNullException_when_passed_null()
        {
            var mockBus = new Mock<IBus>();
            var mockRedisCacheService = new Mock<IRedisCacheService>();
            var mockFilePersistenceService = new Mock<IFilePersistenceService>();
            var mockAutoMapper = new Mock<IMapper>();
            var mockClock = new Mock<IClock>();

            Assert.Throws<ArgumentNullException>("bus", () => new Create.Handler(null!, mockRedisCacheService.Object, mockFilePersistenceService.Object, mockAutoMapper.Object, mockClock.Object, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("redisCacheService", () => new Create.Handler(mockBus.Object, null!, mockFilePersistenceService.Object, mockAutoMapper.Object, mockClock.Object, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("filePersistenceService", () => new Create.Handler(mockBus.Object, mockRedisCacheService.Object, null!, mockAutoMapper.Object, mockClock.Object, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("logger", () => new Create.Handler(mockBus.Object, mockRedisCacheService.Object, mockFilePersistenceService.Object, mockAutoMapper.Object, mockClock.Object, null!));
            Assert.Throws<ArgumentNullException>("clock", () => new Create.Handler(mockBus.Object, mockRedisCacheService.Object, mockFilePersistenceService.Object, mockAutoMapper.Object, null!, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("mapper", () => new Create.Handler(mockBus.Object, mockRedisCacheService.Object, mockFilePersistenceService.Object, null!, mockClock.Object, _loggerMock.Object));
        }

        [Fact]
        public async void TestHandlePublishMessageAndReturnsResponse()
        {
            var mockRedisCacheService = new Mock<IRedisCacheService>();
            var mockFilePersistenceService = new Mock<IFilePersistenceService>();
            var mockAutoMapper = new Mock<IMapper>();
            FakeClock clock = new FakeClock(Instant.FromDateTimeUtc(DateTime.UtcNow));

            mockAutoMapper.Setup(_ => _.Map<SubmitNoticeOfDispute>(It.IsAny<NoticeOfDispute>())).Returns(new SubmitNoticeOfDispute());

            var mockDisputeBus = new Mock<IBus>();
            mockDisputeBus.Setup(x => x.Publish(It.IsAny<SubmitNoticeOfDispute>(), It.IsAny<CancellationToken>()));

            var disputeHandler = new Create.Handler(mockDisputeBus.Object, mockRedisCacheService.Object, mockFilePersistenceService.Object, mockAutoMapper.Object, clock, _loggerMock.Object);

            NoticeOfDispute dispute = new NoticeOfDispute();
            string host = "localhost";
            var request = new Create.Request(dispute, host);

            // Act
            Create.Response response = await disputeHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsType<Create.Response>(response);
        }


    }
}
