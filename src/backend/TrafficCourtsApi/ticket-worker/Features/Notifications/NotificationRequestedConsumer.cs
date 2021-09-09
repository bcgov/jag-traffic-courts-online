﻿using Gov.TicketWorker.Features.Emails;
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
            _emailSender = emailSender;
            _logger = logger;
        }

        public NotificationRequestedConsumer(ILogger<NotificationRequestedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<NotificationContract> context)
        {
            try
            {
                NotificationContract n = context.Message;

                TicketDisputeContract disputeContract = n.TicketDisputeContract;
                _emailSender.SendUsingTemplate(disputeContract.Disputant.EmailAddress, "Ticket request submitted successfully", disputeContract);
                _logger.LogInformation("receive requested notification {n}", JsonSerializer.Serialize(n));
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductChangedConsumerError", ex);
            }

            return Task.CompletedTask;
        }
    }
}
