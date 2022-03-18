using MassTransit;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Consumers
{
    /// <summary>
    ///     Consumer for SubmitDispute message.
    /// </summary>
    public class SubmitDisputeConsumer : IConsumer<SubmitDispute>
    {
        private readonly ILogger<SubmitDisputeConsumer> _logger;

        public SubmitDisputeConsumer(ILogger<SubmitDisputeConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmitDispute> context)
        {
            _logger.LogInformation($@"SubmitDisputeConsumer is consuming dispute id {context.Message.Id}
                                      for ticket number : {context.Message.TicketNumber}");
        }
    }
}
