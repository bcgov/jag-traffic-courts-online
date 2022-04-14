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
        private readonly Mock<ILogger<EmailSenderService>> _mockLogger;
        private readonly Mock<ISmtpClientFactory> _mockSmtpClientFactory;
        private readonly Mock<ISmtpClient> _mockSmtpClient;

        public EmailSenderClientTests()
        {

            _mockLogger = new Mock<ILogger<EmailSenderService>>();
            _mockSmtpClientFactory = new Mock<ISmtpClientFactory>();
            _mockSmtpClient = new Mock<ISmtpClient>();
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

        [Fact]
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
        }

        [Fact]
        public async Task SendEmailAsync_WithMissingParams_ShouldReturnFail()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                From = "test@test.com",
                To = new string[] { "fail@test.com" },
                Subject = "Test message",
                PlainTextContent = "plain old message"
            };

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.Is<MimeMessage>(mailMessage =>
                    mailMessage.From.ToString() == emailMessage.From &&
                    mailMessage.To.Count() == 0 &&
                    mailMessage.Subject == emailMessage.Subject &&
                    mailMessage.GetTextBody(TextFormat.Plain) == emailMessage.PlainTextContent
                ), It.IsAny<CancellationToken>(), null))
                .Throws(new InvalidOperationException());

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.Is<MimeMessage>(mailMessage =>
                    mailMessage.From.ToString() == emailMessage.From &&
                    mailMessage.To.Count() == 1 &&
                    mailMessage.Subject == emailMessage.Subject &&
                    mailMessage.GetTextBody(TextFormat.Plain) == emailMessage.PlainTextContent
                ), It.IsAny<CancellationToken>(), null));


            _mockSmtpClient.Setup(client => client.DisconnectAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()));

            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage);

            try
            {
                await result;
            } catch(Exception ex)
            {
                // Assert
                Assert.True(ex.Message == "Possible missing sender or recipient info");
            }

            Assert.False(result.IsCompletedSuccessfully);
        }

    }
}
