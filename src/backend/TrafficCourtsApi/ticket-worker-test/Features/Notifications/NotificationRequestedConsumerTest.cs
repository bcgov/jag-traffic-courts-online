using AutoFixture;
using AutoFixture.Xunit2;
using Gov.TicketWorker.Features.Emails;
using Gov.TicketWorker.Features.Notifications;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;
using Xunit;

namespace Gov.TicketWorker.Test.Features.Notifications
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class NotificationRequestedConsumerTest
    {
        private Mock<ILogger<NotificationRequestedConsumer>> _loggerMock;
        private Fixture _fixture;
        private NotificationRequestedConsumer _sut;
        private Mock<IEmailSender> _emailSender;

        public NotificationRequestedConsumerTest()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<NotificationRequestedConsumer>>();
            _emailSender = new Mock<IEmailSender>();
            _sut = new NotificationRequestedConsumer(_loggerMock.Object, _emailSender.Object);
        }

        [Fact]
        public async Task Consume_NotificationContract_Successfully()
        {
            var harness = new InMemoryTestHarness();
            var consumerHarness = harness.Consumer(()=>_sut);
            await harness.Start();
            try 
            {
                await harness.InputQueueSendEndpoint.Send(_fixture.Create<NotificationContract>());
                
                Assert.True(await harness.Consumed.Any<NotificationContract>());
                Assert.True(await consumerHarness.Consumed.Any<NotificationContract>());
                _emailSender.Verify(x => x.SendUsingTemplateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TicketDisputeContract>()),
                 Times.Once);
            }
            finally 
            {
                await harness.Stop();
            }
        }
    }
}
