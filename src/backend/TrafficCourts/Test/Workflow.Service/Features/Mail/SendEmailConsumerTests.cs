using MailKit.Net.Smtp;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Workflow.Service.Features.Mail;
using TrafficCourts.Workflow.Service.Services;
using Xunit;

namespace TrafficCourts.Test.Workflow.Service.Features.Mail
{
    public class SendEmailConsumerTests
    {
        private readonly Mock<ILogger<EmailSenderService>> _mockSenderLogger;
        private readonly Mock<ISmtpClient> _mockSmtpClient;
        private readonly Mock<ISmtpClientFactory> _mockSmtpClientFactory;
        private readonly Mock<IOracleDataApiService> _mockOracleDataApiService;
        private readonly Mock<IMapper> _mockMapper;

        public SendEmailConsumerTests()
        {
            _mockSenderLogger = new Mock<ILogger<EmailSenderService>>();
            _mockSmtpClientFactory = new Mock<ISmtpClientFactory>();
            _mockSmtpClient = new Mock<ISmtpClient>();
            _mockOracleDataApiService = new Mock<IOracleDataApiService>();
            _mockMapper = new Mock<IMapper>();
        }

        private EmailSenderService CreateService()
        {
            var configValues = new EmailConfiguration
            {
                Sender = "default@test.com",
                AllowList = "@test.com"
            };

            IOptions<EmailConfiguration> options = Options.Create<EmailConfiguration>(configValues);

            return new EmailSenderService(
                _mockSenderLogger.Object,
                options.Value,
                _mockSmtpClientFactory.Object,
                _mockOracleDataApiService.Object,
                _mockMapper.Object);
        }

        private SendEmailConsumer CreateConsumer(EmailSenderService senderService)
        {
            return new SendEmailConsumer(senderService);
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
                    FromEmailAddress = "mail@test.com",
                    ToEmailAddress = "something@test.com",
                    Subject = "Test message",
                    PlainTextContent = "plain old message",
                    TicketNumber = "TestTicket01",
                    SuccessfullySent = EmailHistorySuccessfullySent.N
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

        // Test retrieve mail template found
        // Test mail template not found.

        [Fact]
        public async Task EmailConsumerAsync_MailTemplate_ShouldReturnTaskComplete()
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
                SendEmail sendEmail = new();
                // Send email message to the submitter's entered email
                var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == "SubmitDisputeTemplate");
                if (template is not null)
                {
                    sendEmail = new SendEmail()
                    {
                        FromEmailAddress = template.Sender,
                        ToEmailAddress = "mail@test.com",
                        Subject = template.SubjectTemplate,
                        PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", "TestTicket01"),
                        TicketNumber = "TestTicket01",
                        SuccessfullySent = EmailHistorySuccessfullySent.N
                    };
                    Assert.NotNull(sendEmail);
                }

                Assert.NotNull(template);


                await harness.InputQueueSendEndpoint.Send<SendEmail>(sendEmail);

                Assert.True(emailConsumer.Consumed.Select<SendEmail>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }

        [Fact]
        public async Task EmailConsumerAsync_MissingMailTemplate()
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
                SendEmail sendEmail = new();
                // Send email message to the submitter's entered email
                var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == "UnknownTemplate");
                if (template is not null)
                {
                    sendEmail.FromEmailAddress = template.Sender;
                    sendEmail.ToEmailAddress = "mail@test.com";
                    sendEmail.Subject = template.SubjectTemplate;
                    sendEmail.PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", "TestTicket01");
                    sendEmail.TicketNumber = "TestTicket01";
                    sendEmail.SuccessfullySent = EmailHistorySuccessfullySent.N;

                    await harness.InputQueueSendEndpoint.Send<SendEmail>(sendEmail);

                }

                Assert.Null(template);

                Assert.False(emailConsumer.Consumed.Select<SendEmail>().Any());
            }
            finally
            {
                await harness.Stop();
            }
        }
    }
}
