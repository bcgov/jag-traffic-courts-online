using AutoFixture.NUnit3;
using AutoMapper;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Features.Disputes.Commands;
using DisputeApi.Web.Messaging.Configuration;
using DisputeApi.Web.Test.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DisputeApi.Web.Features.Disputes.DBModel;
using System.Threading;
using ContractDispute = TrafficCourts.Common.Contract.Dispute;
using Microsoft.Extensions.Options;
using System;

namespace DisputeApi.Web.Test.Features.Disputes.Commands
{
    [ExcludeFromCodeCoverage]
    public class CreateDisputeCommandHandlerTest
    {
        private Mock<ILogger<CreateDisputeCommandHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<ISendEndpointProvider> _sendEndPointProviderMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IOptions<RabbitMQConfiguration>> _rabbitConfigMock;
        private Mock<ISendEndpoint> _sendEndpointMock;
        private CreateDisputeCommandHandler _sut;

        [SetUp]
        public void SetUp()
        {
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
                .Setup(x => x.Send<ContractDispute>(It.IsAny<ContractDispute>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_sendEndpointMock));
            _sut = new CreateDisputeCommandHandler(_loggerMock.Object, _disputeServiceMock.Object,
                _sendEndPointProviderMock.Object, _rabbitConfigMock.Object, _mapperMock.Object);
        }

        [Test, AutoData]
        public async Task CreateDisputeCommandHandler_handle_will_call_service_and_send_to_queue(
            CreateDisputeCommand createDisputeCommand, Dispute createdDispute, ContractDispute contractDispute)
        {
            _mapperMock.Setup(m => m.Map<Dispute>(It.IsAny<CreateDisputeCommand>())).Returns(createdDispute);
            _mapperMock.Setup(m => m.Map<ContractDispute>(It.IsAny<Dispute>())).Returns(contractDispute);
            _disputeServiceMock.Setup(m => m.CreateAsync(It.IsAny<Dispute>()))
                .Returns(Task.FromResult<Dispute>(createdDispute));

            var result = await _sut.Handle(createDisputeCommand, CancellationToken.None);
            _disputeServiceMock.Verify(x => x.CreateAsync(createdDispute), Times.Once);
            _sendEndpointMock.Verify(
                x => x.Send<ContractDispute>(It.IsAny<ContractDispute>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });
            Assert.AreEqual(createdDispute.Id, result.Id);
        }
    }
}