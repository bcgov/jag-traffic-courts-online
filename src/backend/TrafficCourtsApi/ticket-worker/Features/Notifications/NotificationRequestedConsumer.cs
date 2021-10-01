using Gov.TicketWorker.Features.Emails;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
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

        public async Task Consume(ConsumeContext<NotificationContract> context)
        {
            _logger.LogInformation("get dispute notification");
            NotificationContract n = context.Message;
            (bool IsValid, string InvalidReason) validation = IsValid(n);
            _logger.LogDebug("receive dispute notification {n}", n);
            if ( validation.IsValid )
            {
                await _emailSender.SendUsingTemplateAsync(n.TicketDisputeContract.Disputant.EmailAddress, "Ticket request submitted successfully", n.TicketDisputeContract);
            }
            else
            {
                _logger.LogDebug("receive invalid dispute notification {invalidReason}", validation.InvalidReason);
                await context.Publish(new InvalidContract<NotificationContract> { Contract = n, Reason =  validation.InvalidReason});
            }
        }

        private static (bool IsValid, string InvalidReason) IsValid(NotificationContract notificationContract)
        {
            if (notificationContract.TicketDisputeContract == null)
            {
                return (false, "TicketDisputeContract could not be null");
            }
            if (notificationContract.TicketDisputeContract.Disputant == null)
            {
                return (false, "Disputant could not be null");
            }
            if (string.IsNullOrWhiteSpace(notificationContract.TicketDisputeContract.Disputant.EmailAddress))
            {
                return (false, "Email address could not be empty");
            }
            if( !Validation.IsValidEmail(notificationContract.TicketDisputeContract.Disputant.EmailAddress))
            {
                return (false, "Email address is invalid");
            }
            return (true, string.Empty);
        }
    }
}   
