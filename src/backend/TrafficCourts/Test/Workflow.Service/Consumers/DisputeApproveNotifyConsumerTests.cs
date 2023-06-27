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
    public class DisputeApproveNotifyConsumerTests
    {
        private readonly Mock<ILogger<DisputeApprovedNotifyConsumer>> _mockLogger;

        public DisputeApproveNotifyConsumerTests()
        {
            _mockLogger = new Mock<ILogger<DisputeApprovedNotifyConsumer>>();
        }

        private DisputeApprovedNotifyConsumer CreateConsumer()
        {
            return new DisputeApprovedNotifyConsumer(_mockLogger.Object);
        }

        [Fact(Skip = "Failing in github actions but not locally")]
        public async Task DisputeApproveNotifyConsumerAsync_WithCorrectParams_ShouldReturnTaskComplete()
        {
            var consumer = CreateConsumer();
            var harness = new InMemoryTestHarness();

            var emailConsumer = harness.Consumer(() => consumer);
 
            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<DisputeApproved>(new
                {
                    TicketFileNumber = "abc1234",
                    Email = "test@test.com"
                }, c => c.RequestId = NewId.NextGuid());

                Assert.True(emailConsumer.Consumed.Select<DisputeApproved>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
