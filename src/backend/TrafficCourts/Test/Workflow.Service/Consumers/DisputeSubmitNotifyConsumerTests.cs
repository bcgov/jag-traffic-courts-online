using AutoMapper;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Consumers;
using Moq;
using Xunit;

namespace TrafficCourts.Test.Workflow.Service.Consumers
{
    public class DisputeSubmitNotifyConsumerTests
    {
        private readonly Mock<ILogger<DisputeSubmitNotifyConsumer>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;

        public DisputeSubmitNotifyConsumerTests()
        {
            _mockLogger = new Mock<ILogger<DisputeSubmitNotifyConsumer>>();
            _mockMapper = new Mock<IMapper>();
        }

        private DisputeSubmitNotifyConsumer CreateConsumer()
        {
            return new DisputeSubmitNotifyConsumer(_mockLogger.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task DisputeSubmitNotifyConsumerAsync_WithCorrectParams_ShouldReturnTaskComplete()
        {
            var consumer = CreateConsumer();
            var harness = new InMemoryTestHarness();

            var emailConsumer = harness.Consumer(() => consumer);
 
            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<SubmitNoticeOfDispute>(new
                {
                    TicketNumber = "abc1234",
                    EmailAddress = "test@test.com"
                }, c => c.RequestId = NewId.NextGuid());

                Assert.True(emailConsumer.Consumed.Select<SubmitNoticeOfDispute>().Any());
            }
            finally
            {
                await harness.Stop();
            }

        }

    }
}
