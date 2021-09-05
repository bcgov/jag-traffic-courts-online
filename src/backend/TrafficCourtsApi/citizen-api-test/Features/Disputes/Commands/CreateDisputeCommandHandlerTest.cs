using AutoMapper;
using Gov.CitizenApi.Features.Disputes;
using Gov.CitizenApi.Features.Disputes.Commands;
using Gov.CitizenApi.Test.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Gov.CitizenApi.Features.Disputes.DBModel;
using System.Threading;
using TrafficCourts.Common.Contract;
using System;
using AutoFixture;
using AutoFixture.Xunit2;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Disputes.Commands
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class CreateDisputeCommandHandlerTest
    {
        private Mock<ILogger<CreateDisputeCommandHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IBus> _busMock;
        private Mock<ISendEndpoint> _sendEndpointMock;
        private CreateDisputeCommandHandler _sut;
        private Fixture _fixture;

        public CreateDisputeCommandHandlerTest()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _loggerMock = LoggerServiceMock.LoggerMock<CreateDisputeCommandHandler>();
            _disputeServiceMock = new Mock<IDisputeService>();
            _sendEndpointMock = new Mock<ISendEndpoint>();
            _mapperMock = new Mock<IMapper>();
            _busMock = new Mock<IBus>();
            _sut = new CreateDisputeCommandHandler(_loggerMock.Object, _disputeServiceMock.Object, _mapperMock.Object, _busMock.Object);
        }

        [Theory]
        [AutoData]
        public async Task CreateDisputeCommandHandler_handle_will_call_service_and_send_to_queue(
            CreateDisputeCommand createDisputeCommand, DisputeContract contractDispute)
        {
            var createdDispute = _fixture.Create<Dispute>();
            var ticketDisputeContract = _fixture.Create<TicketDisputeContract>();
            var disputantContract = _fixture.Create<TrafficCourts.Common.Contract.Disputant>();
            var additionalContract = _fixture.Create<TrafficCourts.Common.Contract.Additional>();
            var offenceContract = _fixture.Create<TrafficCourts.Common.Contract.Offence>();
            _mapperMock.Setup(m => m.Map<Dispute>(It.IsAny<CreateDisputeCommand>())).Returns(createdDispute);
            _mapperMock.Setup(m => m.Map<DisputeContract>(It.IsAny<Dispute>())).Returns(contractDispute);

            _mapperMock.Setup(m => m.Map<TicketDisputeContract>(It.IsAny<CreateDisputeCommand>())).Returns(ticketDisputeContract);
            _mapperMock.Setup(m => m.Map<TrafficCourts.Common.Contract.Disputant>(It.IsAny<Disputant>())).Returns(disputantContract);
            _mapperMock.Setup(m => m.Map<TrafficCourts.Common.Contract.Additional>(It.IsAny<Additional>())).Returns(additionalContract);
            _mapperMock.Setup(m => m.Map<TrafficCourts.Common.Contract.Offence>(It.IsAny<Offence>())).Returns(offenceContract);

            _disputeServiceMock.Setup(m => m.CreateAsync(It.IsAny<Dispute>()))
                .Returns(Task.FromResult<Dispute>(createdDispute));
            _busMock.Setup(x => x.GetSendEndpoint(It.IsAny<Uri>())).Returns(Task.FromResult(_sendEndpointMock.Object));
            _sendEndpointMock
                .Setup(x => x.Send<DisputeContract>(It.IsAny<DisputeContract>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_sendEndpointMock));
            EndpointConvention.Map<DisputeContract>(new Uri($"amqp://localhost:5672/{(typeof(DisputeContract)).GetQueueName()}"));
            EndpointConvention.Map<NotificationContract>(new Uri($"amqp://localhost:5672/{(typeof(NotificationContract)).GetQueueName()}"));

            var result = await _sut.Handle(createDisputeCommand, CancellationToken.None);
            _disputeServiceMock.Verify(x => x.CreateAsync(createdDispute), Times.Once);

            _sendEndpointMock.Verify(
                x => x.Send<DisputeContract>(It.IsAny<DisputeContract>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });

            _sendEndpointMock.Verify(
                x => x.Send<NotificationContract>(It.IsAny<NotificationContract>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });

            Assert.Equal(createdDispute.Id, result.Id);

        }
    }
}
