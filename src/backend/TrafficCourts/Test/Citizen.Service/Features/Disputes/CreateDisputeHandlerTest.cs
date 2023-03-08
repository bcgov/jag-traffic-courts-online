using Microsoft.Extensions.Logging;
using Moq;
using MassTransit;
using System;
using System.Threading;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Messaging.MessageContracts;
using Xunit;
using AutoMapper;
using NodaTime;
using NodaTime.Testing;
using HashidsNet;
using TrafficCourts.Coms.Client;

namespace TrafficCourts.Test.Citizen.Service.Features.Disputes
{
    public class CreateDisputeHandlerTest
    {
        private readonly Mock<ILogger<Create.Handler>> _loggerMock = new Mock<ILogger<Create.Handler>>();

        [Fact]
        public void constructor_throws_ArgumentNullException_when_passed_null()
        {
            var bus = Mock.Of<IBus>();
            var redisCacheService = Mock.Of<IRedisCacheService>();
            var objectManagementService = Mock.Of<IObjectManagementService>();
            var memoryStreamManager = Mock.Of<IMemoryStreamManager>();
            var mapper = Mock.Of<IMapper>();
            var clock = Mock.Of<IClock>();
            var hashids = Mock.Of<IHashids>();

            Assert.Throws<ArgumentNullException>("bus", () => new Create.Handler(null!, redisCacheService, objectManagementService, memoryStreamManager, mapper, clock, hashids, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("redisCacheService", () => new Create.Handler(bus, null!, objectManagementService, memoryStreamManager, mapper, clock, hashids, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("objectManagementService", () => new Create.Handler(bus, redisCacheService, null!, memoryStreamManager, mapper, clock, hashids, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("memoryStreamManager", () => new Create.Handler(bus, redisCacheService, objectManagementService, null!, mapper, clock, hashids, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("mapper", () => new Create.Handler(bus, redisCacheService, objectManagementService, memoryStreamManager, null!, clock, hashids, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("clock", () => new Create.Handler(bus, redisCacheService, objectManagementService, memoryStreamManager, mapper, null!, hashids, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("hashids", () => new Create.Handler(bus, redisCacheService, objectManagementService, memoryStreamManager, mapper, clock, null!, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>("logger", () => new Create.Handler(bus, redisCacheService, objectManagementService, memoryStreamManager, mapper, clock, hashids, null!));
        }

        [Fact]
        public async void TestHandlePublishMessageAndReturnsResponse()
        {
            var mockBus = new Mock<IBus>();
            var mockRedisCacheService = new Mock<IRedisCacheService>();
            var objectManagementService = new Mock<IObjectManagementService>();
            var memoryStreamManager = new Mock<IMemoryStreamManager>();
            var mockAutoMapper = new Mock<IMapper>();
            var mockHashids = new Mock<IHashids>();
            FakeClock clock = new FakeClock(Instant.FromDateTimeUtc(DateTime.UtcNow), Duration.FromSeconds(1));

            mockAutoMapper.Setup(_ => _.Map<SubmitNoticeOfDispute>(It.IsAny<NoticeOfDispute>())).Returns(new SubmitNoticeOfDispute());


            mockBus.Setup(x => x.Publish(It.IsAny<SubmitNoticeOfDispute>(), It.IsAny<CancellationToken>()));

            var disputeHandler = new Create.Handler(mockBus.Object, mockRedisCacheService.Object, objectManagementService.Object, memoryStreamManager.Object, mockAutoMapper.Object, clock, mockHashids.Object, _loggerMock.Object);

            NoticeOfDispute dispute = new NoticeOfDispute();
            var request = new Create.Request(dispute);

            // Act
            Create.Response response = await disputeHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.IsType<Create.Response>(response);
        }
    }
}
