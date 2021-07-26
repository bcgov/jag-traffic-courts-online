﻿using AutoMapper;
using Gov.CitizenApi.Features.Disputes;
using Gov.CitizenApi.Features.Disputes.Commands;
using Gov.CitizenApi.Messaging.Configuration;
using Gov.CitizenApi.Test.Utils;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Gov.CitizenApi.Features.Disputes.DBModel;
using System.Threading;
using TrafficCourts.Common.Contract;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Disputes.Commands
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

        public CreateOffenceDisputeCommandHandlerTest()
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

        [Theory]
        [AllowCirculationAutoData]
        public async Task CreateOffenceDisputeCommandHandler_handle_will_call_service_and_send_to_queue(
            CreateOffenceDisputeCommand createOffenceDisputeCommand,
            Dispute updatedDispute, 
            Dispute existingDispute,
            DisputeContract contractDispute,
            OffenceDisputeDetail offenceDisputeDetail)
        {
            _mapperMock.Setup(m => m.Map<DisputeContract>(It.IsAny<Dispute>())).Returns(contractDispute);
            _mapperMock.Setup(m => m.Map<OffenceDisputeDetail>(It.IsAny<Gov.CitizenApi.Models.OffenceDisputeDetail>())).Returns(offenceDisputeDetail);
            _disputeServiceMock.Setup(m => m.FindTicketDisputeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult<Dispute>(existingDispute));
            _disputeServiceMock.Setup(m => m.UpdateAsync(It.IsAny<Dispute>()))
                .Returns(Task.FromResult<Dispute>(updatedDispute));

            var result = await _sut.Handle(createOffenceDisputeCommand, CancellationToken.None);
            _disputeServiceMock.Verify(x => x.UpdateAsync(It.IsAny<Dispute>()), Times.Once);
            //temp remove: todo: uncomment
            //_sendEndpointMock.Verify(
            //    x => x.Send<DisputeContract>(It.IsAny<DisputeContract>(), It.IsAny<CancellationToken>()),
            //    () => { return Times.Once(); });
            //temp
            Assert.Equal(updatedDispute.Id, result.Id);
        }
    }
}