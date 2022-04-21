using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Models;
using TrafficCourts.Workflow.Service.Features.Mail;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using Moq;
using Xunit;

namespace TrafficCourts.Test.Workflow.Service.Features.Mail
{
    public class SendEmailConsumerTests
    {
        private readonly Mock<ILogger<SendEmailConsumer>> _mockLogger;
        private readonly Mock<ILogger<EmailSenderService>> _mockSenderLogger;
        private readonly Mock<ISmtpClient> _mockSmtpClient;
        private readonly Mock<ISmtpClientFactory> _mockSmtpClientFactory;
        private readonly Mock<IEmailSenderService> _mockEmailSenderService;

        public SendEmailConsumerTests()
        {

            _mockLogger = new Mock<ILogger<SendEmailConsumer>>();
            _mockSenderLogger = new Mock<ILogger<EmailSenderService>>();
            _mockSmtpClientFactory = new Mock<ISmtpClientFactory>();
            _mockSmtpClient = new Mock<ISmtpClient>();
            _mockEmailSenderService = new Mock<IEmailSenderService>();
        }

        private EmailSenderService CreateService()
        {
            var configValues = new EmailConfiguration
            {
                Sender = "default@test.com",
                AllowList = new string[]{ "@test.com" }
            };

            IOptions<EmailConfiguration> options = Options.Create<EmailConfiguration>(configValues);

            return new EmailSenderService(
                _mockSenderLogger.Object,
                options,
                _mockSmtpClientFactory.Object);
        }

        private SendEmailConsumer CreateConsumer(EmailSenderService senderService)
        {
            return new SendEmailConsumer(
                _mockLogger.Object,
                senderService);
        }

        // Tests:
        // happy path with all values
        // null message
        // missing mandatory fields
        // bad e-mails

        [Fact]
        public async Task EmailConsumerAsync_WithCorrectParams_ShouldReturnTaskComplete()
        {
            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null));

            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();
            var consumer = CreateConsumer(service);
            var harness = new InMemoryTestHarness();

            var emailConsumer = harness.Consumer(() => consumer);
 
            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<SendEmail>(new
                {
                    From = "mail@test.com",
                    To = new string[] { "something@test.com" },
                    Subject = "Test message",
                    PlainTextContent = "plain old message"
                }, c => c.RequestId = NewId.NextGuid());

                Assert.True(emailConsumer.Consumed.Select<SendEmail>().Any());
            }
            finally
            {
                await harness.Stop();
            }

        }

        [Fact]
        public async Task EmailConsumerAsync_EmptyMessage_ShouldReturnTaskComplete()
        {
            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null));

            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();
            var consumer = CreateConsumer(service);
            var harness = new InMemoryTestHarness();

            var emailConsumer = harness.Consumer(() => consumer);

            await harness.Start();

            try
            {
                await harness.InputQueueSendEndpoint.Send<SendEmail>(new {});

                Assert.True(emailConsumer.Consumed.Select<SendEmail>().Any());
            }
            finally
            {
                await harness.Stop();
            }

        }

    }
}
