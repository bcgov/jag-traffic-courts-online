using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Workflow.Service.Models;
using TrafficCourts.Workflow.Service.Services;
using TrafficCourts.Workflow.Service.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using Moq;
using Xunit;

namespace TrafficCourts.Test.Workflow.Service
{
    public class EmailSenderClientTests
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<ILogger<EmailSenderService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ISmtpClientFactory> _mockSmtpClientFactory;
        private readonly Mock<ISmtpClient> _mockSmtpClient;

        public EmailSenderClientTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _mockLogger = new Mock<ILogger<EmailSenderService>>();
            _mockMapper = new Mock<IMapper>();
            _mockSmtpClientFactory = new Mock<ISmtpClientFactory>();

            _mockLogger = _mockRepository.Create<ILogger<EmailSenderService>>();
            _mockSmtpClientFactory = _mockRepository.Create<ISmtpClientFactory>();
            _mockSmtpClient = _mockRepository.Create<ISmtpClient>();
        }

        private EmailSenderService CreateService()
        {
            var configValues = new EmailConfiguration
            {
                Sender = "test@test.com",
                AllowList = new string[]{ "test@test.com" }
            };

            IOptions<EmailConfiguration> options = Options.Create<EmailConfiguration>(configValues);

            return new EmailSenderService(
                _mockLogger.Object,
                options,
                _mockSmtpClientFactory.Object);
        }

        [Fact(Skip = "Integration Test")]
        public async Task SendEmailAsync_WithCorrectParams_ShouldReturnTaskComplete()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                From = "test@test.com",
                To = new string[] { "test@test.com" },
                Subject = "Test message",
                PlainTextContent = "plain old message"
            };
            /*
             *             Headers[HeaderId.From] = string.Empty;
            Date = DateTimeOffset.Now;
            Subject = string.Empty;
            MessageId = MimeUtils.GenerateMessageId();

             * */


            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null));
            _mockSmtpClient.Setup(client => client.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()));

            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage);

            await result;

            // Assert
            Assert.True(result.IsCompletedSuccessfully);
            _mockRepository.VerifyAll();
        }

    }
}
