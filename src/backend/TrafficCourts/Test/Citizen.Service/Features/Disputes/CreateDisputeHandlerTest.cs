using AutoFixture;
using AutoMapper;
using HashidsNet;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Features.Disputes;
using TrafficCourts.Citizen.Service.Models.Disputes;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Features.Disputes
{
    public class CreateDisputeHandlerTest
    {
        private readonly Mock<ILogger<Create.Handler>> _loggerMock = new Mock<ILogger<Create.Handler>>();

        private readonly AutoFixture.Fixture _fixture;

        public CreateDisputeHandlerTest()
        {
            _fixture = new AutoFixture.Fixture();
            _fixture.Customizations.Add(new DateOnlySpecimenBuilder());
        }

        [Fact]
        public void constructor_throws_ArgumentNullException_when_passed_null()
        {
            var bus = Mock.Of<IBus>();
            var redisCacheService = Mock.Of<IRedisCacheService>();
            var objectManagementService = Mock.Of<IObjectManagementService>();
            var memoryStreamManager = Mock.Of<IMemoryStreamManager>();
            var mapper = Mock.Of<IMapper>();
            var clock = Mock.Of<TimeProvider>();
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
        public async Task TestHandlePublishMessageAndReturnsResponse()
        {
            var mockBus = new Mock<IBus>();
            var mockRedisCacheService = new Mock<IRedisCacheService>();
            var objectManagementService = new Mock<IObjectManagementService>();
            var memoryStreamManager = new Mock<IMemoryStreamManager>();
            var mockHashids = new Mock<IHashids>();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TrafficCourts.Citizen.Service.Mappings.NoticeOfDisputeToMessageContractMappingProfile());
            }).CreateMapper();

            var now = DateTimeOffset.UtcNow;

            FakeTimeProvider clock = new FakeTimeProvider(now)
            {
                AutoAdvanceAmount = TimeSpan.FromSeconds(1)
            };

            mockBus.Setup(bus => 
                bus.Publish(
                    It.IsAny<SubmitNoticeOfDispute>(), 
                    It.IsAny<CancellationToken>()));

            var disputeHandler = new Create.Handler(
                mockBus.Object,
                mockRedisCacheService.Object, 
                objectManagementService.Object,
                memoryStreamManager.Object, 
                mapper, 
                clock, mockHashids.Object, 
                _loggerMock.Object);

            NoticeOfDispute dispute = _fixture.Create<NoticeOfDispute>();
            dispute.TicketId = $"{Guid.NewGuid()}-l"; // Simulate a looked up ticket

            TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket violationTicket = _fixture.Create<TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket>();
            mockRedisCacheService
                .Setup(service => service.GetRecordAsync<TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket>(It.Is<string>(key => key == dispute.TicketId)))
                .ReturnsAsync(violationTicket);

            var request = new Create.Request(dispute);

            // Act
            Create.Response response = await disputeHandler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Null(response.Exception);

            // Verify that the Publish method was called with the expected parameters
            mockBus.Verify(bus => bus.Publish(
                It.Is<SubmitNoticeOfDispute>(message => Is(message, now.DateTime.Truncate(), dispute, violationTicket)),
                It.IsAny<CancellationToken>()),
                Times.Once); // Ensure it was called exactly once
        }

        private static bool Is(SubmitNoticeOfDispute actual, DateTime now, NoticeOfDispute dispute, TrafficCourts.Citizen.Service.Models.Tickets.ViolationTicket violationTicket)
        {
            static bool Is(DateTime? actual, DateTime? expected)
            {
                return actual.HasValue && actual.Value.Kind == DateTimeKind.Unspecified && actual.Value == expected;
            }

            return actual != null &&
                // verify the dispute information
                Is(actual.DisputantBirthdate, dispute.DisputantBirthdate) &&
                Is(actual.IssuedTs, dispute.IssuedTs) &&
                Is(actual.SubmittedTs, now) &&
                // verify the ticket information
                actual.ViolationTicket is not null &&
                Is(actual.ViolationTicket.IssuedTs, violationTicket.IssuedTs);
        }
    }
}
