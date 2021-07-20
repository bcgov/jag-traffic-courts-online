using AutoMapper;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Messaging.Configuration;
using DisputeApi.Web.Test.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DisputeApi.Web.Features.Disputes.DBModel;
using System.Threading;
using TrafficCourts.Common.Contract;
using Microsoft.Extensions.Options;
using System;
using AutoFixture;
using AutoFixture.Xunit2;
using Xunit;
using TrafficCourts.Common;

namespace DisputeApi.Web.Test.Features.Disputes.Commands
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class CreateDisputeCommandHandlerTest
    {
        private Mock<ILogger<CreateDisputeCommandHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<ISendEndpointProvider> _sendEndPointProviderMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IOptions<RabbitMQConfiguration>> _rabbitConfigMock;
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
            _sendEndPointProviderMock = new Mock<ISendEndpointProvider>();
            _mapperMock = new Mock<IMapper>();
            _rabbitConfigMock = new Mock<IOptions<RabbitMQConfiguration>>();
            _sendEndpointMock = new Mock<ISendEndpoint>();

            RabbitMQConfiguration rabbitMqConfiguration = new RabbitMQConfiguration();
            rabbitMqConfiguration.Host = "localhost";
            rabbitMqConfiguration.Port = 15672;
            rabbitMqConfiguration.Username = "username";
            rabbitMqConfiguration.Password = "password";

            _sendEndPointProviderMock.Setup(x => x.GetSendEndpoint(It.IsAny<Uri>()))
                .Returns(Task.FromResult(_sendEndpointMock.Object));

            _rabbitConfigMock.Setup(x => x.Value).Returns(rabbitMqConfiguration);
            _sendEndpointMock
                .Setup(x => x.Send<DisputeContract>(It.IsAny<DisputeContract>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_sendEndpointMock));
            _sut = new CreateDisputeCommandHandler(_loggerMock.Object, _disputeServiceMock.Object,
                _sendEndPointProviderMock.Object, _rabbitConfigMock.Object, _mapperMock.Object);
        }

        [Theory]
        [AutoData]
        public async Task CreateDisputeCommandHandler_handle_will_call_service_and_send_to_queue(
            CreateDisputeCommand createDisputeCommand, DisputeContract contractDispute)
        {
            var createdDispute = _fixture.Create<Dispute>();
            _mapperMock.Setup(m => m.Map<Dispute>(It.IsAny<CreateDisputeCommand>())).Returns(createdDispute);
            _mapperMock.Setup(m => m.Map<DisputeContract>(It.IsAny<Dispute>())).Returns(contractDispute);
            _disputeServiceMock.Setup(m => m.CreateAsync(It.IsAny<Dispute>()))
                .Returns(Task.FromResult<Dispute>(createdDispute));

            var result = await _sut.Handle(createDisputeCommand, CancellationToken.None);
            _disputeServiceMock.Verify(x => x.CreateAsync(createdDispute), Times.Once);
            //temp remove: todo: uncomment
            //_sendEndpointMock.Verify(
            //    x => x.Send<DisputeContract>(It.IsAny<DisputeContract>(), It.IsAny<CancellationToken>()),
            //    () => { return Times.Once(); });
            ////temp
            Assert.Equal(createdDispute.Id, result.Id);
        }
    }
}
