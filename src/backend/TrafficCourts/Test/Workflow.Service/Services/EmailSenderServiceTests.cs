﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Workflow.Service.Services;
using MailKit.Net.Smtp;
using MimeKit;
using Moq;
using Xunit;
using TrafficCourts.Workflow.Service.Configuration;
using TrafficCourts.Common.Features.Mail;

namespace TrafficCourts.Test.Workflow.Service.Services
{
    public class EmailSenderServiceTests
    {
        private readonly Mock<ILogger<EmailSenderService>> _mockLogger;
        private readonly Mock<EmailConfiguration> _emailConfiguration;
        private readonly Mock<ISmtpClientFactory> _mockSmtpClientFactory;
        private readonly Mock<ISmtpClient> _mockSmtpClient;

        public EmailSenderServiceTests()
        {
            _mockLogger = new Mock<ILogger<EmailSenderService>>();
            _mockSmtpClientFactory = new Mock<ISmtpClientFactory>();
            _mockSmtpClient = new Mock<ISmtpClient>();
            _emailConfiguration = new Mock<EmailConfiguration>();
        }

        /// <summary>
        /// Creates EmailSenderService with allow list 
        /// </summary>
        /// <returns></returns>
        private EmailSenderService CreateService()
        {
            var configValues = new EmailConfiguration
            {
                Sender = "default@test.com",
                AllowList = "@test.com"
            };

            IOptions<EmailConfiguration> options = Options.Create<EmailConfiguration>(configValues);

            return new EmailSenderService(
                options.Value,
                _mockSmtpClientFactory.Object,
                _mockLogger.Object);
        }

        // Scenarios:
        // Happy path
        // stmp client factory failed (simulate cancel token timeout)
        // allowlist filter-out non-allowed emails
        // default sender when no from email supplied
        // verify invalid email supplied

        [Fact]
        public async Task SendEmailAsync_WithCorrectParams_ShouldReturnTaskComplete()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                From = "mail@test.com",
                To = "something@test.com",
                Subject = "Test message",
                TextContent = "plain old message"
            };

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), 
                    It.IsAny<CancellationToken>(),
                    null));

            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage, cancellationToken);

            await result;

            // Assert
            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task SendEmailAsync_SmtpCancelled_ShouldReturnTaskFail()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                From = "mail@test.com",
                To = "something@test.com",
                Subject = "Test message",
                TextContent = "plain old message"
            };

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null));

            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Throws(new OperationCanceledException());

            var service = CreateService();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage, cancellationToken);

            try
            {

                await result;
            }
            catch (Exception ex)
            {
                // Assert
                Assert.True(ex.Message == "The operation was canceled");
            }

            // Assert
            Assert.False(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task sending_email_to_only_not_allowed_list_will_not_fail()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                From = "mail@test.com",
                To = "fail@fail.com",
                Subject = "Test message",
                TextContent = "plain old message"
            };

            var service = CreateService();

            // test e-mail send.
            await service.SendEmailAsync(emailMessage, CancellationToken.None);
        }

        /* [Fact]
        public async Task SendEmailAsync_TestAllowList_ShouldReturnSuccess()
        {
            // Arrange
            var emailMessage = new SendEmail
            {
                From = "mail@test.com",
                To = "works@test.com, fake@fake.com",
                Subject = "Test message",
                TextContent = "plain old message",
                TicketNumber = "TestTicket01",
                SuccessfullySent = EmailHistorySuccessfullySent.N
            };

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.Is<MimeMessage>(mailMessage =>
                    mailMessage.From.ToString() == emailMessage.From &&
                    mailMessage.To.Count() != 1 &&
                    mailMessage.Subject == emailMessage.Subject &&
                    mailMessage.GetTextBody(TextFormat.Plain) == emailMessage.TextContent
                ), It.IsAny<CancellationToken>(), null))
                .Throws(new InvalidOperationException());

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.Is<MimeMessage>(mailMessage =>
                    mailMessage.From.ToString() == emailMessage.From &&
                    mailMessage.To.Count() == 1 &&
                    mailMessage.Subject == emailMessage.Subject &&
                    mailMessage.GetTextBody(TextFormat.Plain) == emailMessage.TextContent
                ), It.IsAny<CancellationToken>(), null));


            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage, cancellationToken);

            try
            {
                await result;
            }
            catch (Exception)
            {
            }

            Assert.True(result.IsCompletedSuccessfully);
        } */

        [Fact]
        public async Task SendEmailAsync_TestDefaultSender_ShouldReturnSuccess()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                To = "works@test.com",
                Subject = "Test message",
                TextContent = "plain old message"
            };


            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null));


            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage, cancellationToken);

            try
            {
                await result;
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message == "Host or message is null");
            }

            Assert.True(result.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task SendEmailAsync_WithBadToEmail_ShouldReturnFail()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                From = "mail@test.com",
                To = "just junk mail",
                Subject = "Test message",
                TextContent = "plain old message"
            };

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null));

            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage, cancellationToken);

            try
            {
                await result;
            }
            catch (Exception ex)
            {
                // Assert
                Assert.True(ex.Message == "Missing recipient info");
            }

            Assert.False(result.IsCompletedSuccessfully);
        }
/*
        [Fact]
        public async Task SendEmailAsync_ConnectTimeout_ShouldReturnFail()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                From = "mail@test.com",
                To = new string[] { "test@test.com" },
                Subject = "Test message",
                TextContent = "plain old message"
            };

            _mockSmtpClient.Setup(client => client.SendAsync(
                    It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), null));


            _mockSmtpClientFactory.Setup(generator => generator.CreateAsync(It.IsAny<CancellationToken>()))
                .Callback(() => Thread.Sleep(6000))
                .Returns(Task.FromResult(_mockSmtpClient.Object));

            var service = CreateService();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // test e-mail send.
            var result = service.SendEmailAsync(emailMessage, cancellationToken);

            try
            {
                await result;
            }
            catch (Exception ex)
            {
                // Assert
                Assert.True(ex.Message == "The operation was canceled");
            }

            Assert.False(result.IsCompletedSuccessfully);
        }*/

    }
}
