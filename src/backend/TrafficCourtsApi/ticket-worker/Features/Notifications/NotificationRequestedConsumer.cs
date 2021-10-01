using Gov.TicketWorker.Features.Emails;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;

namespace Gov.TicketWorker.Features.Notifications
{
    public class NotificationRequestedConsumer : IConsumer<NotificationContract>
    {
        private readonly ILogger<NotificationRequestedConsumer> _logger;
        private readonly IEmailSender _emailSender;

        public NotificationRequestedConsumer(ILogger<NotificationRequestedConsumer> logger, IEmailSender emailSender)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Consume(ConsumeContext<NotificationContract> context)
        {
            try
            {
                _logger.LogInformation("get dispute notification");
                NotificationContract n = context.Message;
                _logger.LogDebug("receive requested notification {n}", JsonSerializer.Serialize(n));
                TicketDisputeContract disputeContract = n.TicketDisputeContract;
                _emailSender.SendUsingTemplate(disputeContract.Disputant.EmailAddress, "Ticket request submitted successfully", disputeContract);
            }
            catch(Exception ex)
            {
                _logger.LogError("NotificationRequestedConsumer", ex);
            }
            return Task.CompletedTask;
        }
    }
}   
