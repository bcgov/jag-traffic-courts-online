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
using TrafficCourts.Common.Contract;
using Microsoft.Extensions.Options;
using System;
using AutoFixture;

namespace DisputeApi.Web.Test.Features.Disputes.Commands
{
    [ExcludeFromCodeCoverage]
    public class CreateOffenceDisputeCommandHandlerTest
    {
        private Mock<ILogger<CreateOffenceDisputeCommandHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<ISendEndpointProvider> _sendEndPointProviderMock;
        private Mock<IMapper> _mapperMock;
        private Mock<IOptions<RabbitMQConfiguration>> _rabbitConfigMock;
        private Mock<ISendEndpoint> _sendEndpointMock;
        private CreateOffenceDisputeCommandHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<CreateOffenceDisputeCommandHandler>();
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
            _sut = new CreateOffenceDisputeCommandHandler(_loggerMock.Object, _disputeServiceMock.Object,
                _sendEndPointProviderMock.Object, _rabbitConfigMock.Object, _mapperMock.Object);
        }

        [Test, AllowCirculationAutoData]
        public async Task CreateOffenceDisputeCommandHandler_handle_will_call_service_and_send_to_queue(
            CreateOffenceDisputeCommand createOffenceDisputeCommand,
            Dispute updatedDispute, 
            Dispute existingDispute,
            DisputeContract contractDispute,
            OffenceDisputeDetail offenceDisputeDetail)
        {
            _mapperMock.Setup(m => m.Map<DisputeContract>(It.IsAny<Dispute>())).Returns(contractDispute);
            _mapperMock.Setup(m => m.Map<OffenceDisputeDetail>(It.IsAny<Web.Models.OffenceDisputeDetail>())).Returns(offenceDisputeDetail);
            _disputeServiceMock.Setup(m => m.FindTicketDisputeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<Dispute>(existingDispute));
            _disputeServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Dispute>()))
                .Returns(Task.FromResult<Dispute>(updatedDispute));

            var result = await _sut.Handle(createOffenceDisputeCommand, CancellationToken.None);
            _disputeServiceMock.Verify(x => x.UpdateAsync(It.IsAny<Dispute>()), Times.Once);
            _sendEndpointMock.Verify(
                x => x.Send<DisputeContract>(It.IsAny<DisputeContract>(), It.IsAny<CancellationToken>()),
                () => { return Times.Once(); });
            Assert.AreEqual(updatedDispute.Id, result.Id);
        }
    }
}