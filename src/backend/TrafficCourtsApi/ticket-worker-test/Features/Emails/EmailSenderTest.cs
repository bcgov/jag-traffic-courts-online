using AutoFixture;
using DotLiquid;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Liquid;
using Fluid;
using Gov.TicketWorker.Features.Emails;
using Gov.TicketWorker.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;
using Xunit;

namespace Gov.TicketWorker.Test.Features.Emails
{
    [ExcludeFromCodeCoverage(Justification = Justifications.UnitTestClass)]
    public class EmailSenderTest
    {
        private readonly Mock<ILogger<EmailSender>> _loggerMock;
        private Fixture _fixture;
        private EmailSender _sut;
        private Mock<IFluentEmail> _emailMock;
        private Mock<IEmailFilter> _emailFilterMock;

        public EmailSenderTest()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<EmailSender>>();
            _emailMock = new Mock<IFluentEmail>();
            _emailFilterMock = new Mock<IEmailFilter>();
            _sut = new EmailSender(_emailMock.Object, _loggerMock.Object, _emailFilterMock.Object);
        }

        [Fact]
        public void Throw_ArgumentNullException_if_FluentEmailObjectpassed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(null, _loggerMock.Object, _emailFilterMock.Object));
        }

        [Fact]
        public void Throw_ArgumentNullException_if_LoggerObjectpassed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(_emailMock.Object, null, _emailFilterMock.Object));
        }

        [Fact]
        public void Throw_ArgumentNullException_if_EmailFilterObjectpassed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(_emailMock.Object, _loggerMock.Object, null));
        }

        [Fact]
        public void Throw_ArgumentException_if_ToAddresspassed_null()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _sut.SendUsingTemplateAsync(null, "This is the subject line", It.IsAny<TicketDisputeContract>()));
        }

        [Fact]
        public void Throw_ArgumentException_if_ToAddresspassed_Empty()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _sut.SendUsingTemplateAsync("", "This is the subject line", It.IsAny<TicketDisputeContract>()));
        }

        [Fact]
        public void Throw_ArgumentException_if_DisputeEmailModelpassed_null()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SendUsingTemplateAsync("testEmail@test.com", "This is the subject line", null));
        }



        [Fact]
        public async Task Test_SendUsingTemplateSuccessful_EmailTemplateIsValid()
        {
            _emailMock.Setup(x => x.To(It.Is<string>(y => y != ""))).Returns(_emailMock.Object);
            _emailMock.Setup(x => x.Subject(It.IsAny<string>())).Returns(_emailMock.Object);
            _emailMock.Setup(x => x.UsingTemplateFromEmbedded(It.IsAny<string>(), It.IsAny<DisputeEmail>(),
                It.IsAny<Assembly>(), true)).Returns(_emailMock.Object);
            _emailMock.Setup(x => x.SendAsync(null)).ReturnsAsync(new SendResponse());

            var fixture = new Fixture();
            var disputeContractModel = fixture.Create<TicketDisputeContract>();
            var emailModel = new DisputeEmail(disputeContractModel);

             var assembly = Assembly.GetExecutingAssembly();
            string[] files = assembly.GetManifestResourceNames();
            Stream templateStream = assembly.GetManifestResourceStream("ticket_worker_test.Features.Emails.Resources.submissiontemplate.liquid");
            StreamReader reader = new StreamReader(templateStream);
            string templateString = reader.ReadToEnd();
            string expected = "";

            var options = new LiquidRendererOptions
            {
                FileProvider = null,
                ConfigureTemplateContext = null,
            };
            Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));

            Stream logoStream = assembly.GetManifestResourceStream("ticket_worker_test.Features.Emails.Resources.bc-gov-logo.png");
            byte[] bytes;
            MemoryStream memoryStream = new MemoryStream();
            logoStream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();

            string base64Data = Convert.ToBase64String(bytes);
            string dataScheme = $"data:image/png;base64,{base64Data}==";

            emailModel.LogoImage = dataScheme;

            Template template = Template.Parse(templateString); // Parses and compiles the template
            expected = template.Render(Hash.FromAnonymousObject(emailModel));

           
            await _sut.SendUsingTemplateAsync("testEmail@test.com", "This is the subject line", disputeContractModel);

            _emailMock.Verify(foo => foo.To("testEmail@test.com"), Times.Once());
            _emailMock.Verify(foo => foo.Subject("This is the subject line"), Times.Once());
            _emailMock.Verify(foo => foo.UsingTemplateFromEmbedded(It.IsAny<string>(), 
                It.Is<DisputeEmail>(model => model.ConfirmationNumber == emailModel.ConfirmationNumber && model.LogoImage == emailModel.LogoImage),
                It.IsAny<Assembly>(), true), Times.Once());

            var result = new Email("Sender@send.com")
                              .To("testEmail@test.com")
                              .Subject("This is the subject line")
                              .UsingTemplateFromEmbedded("ticket_worker_test.Features.Emails.Resources.submissiontemplate.liquid", emailModel, this.GetType().GetTypeInfo().Assembly);
            Assert.Equal(expected, result.Data.Body);

        }

        [Fact]
        public async Task Test_SendUsingTemplateSuccessful()
        {
            var fixture = new Fixture();
            var disputeContractModel = fixture.Create<TicketDisputeContract>();
            _emailMock.Setup(
                m => m.To(It.IsAny<string>())
                      .Subject(It.IsAny<string>())
                      .UsingTemplateFromEmbedded(It.IsAny<string>(), It.IsAny<DisputeEmail>(), It.IsAny<Assembly>(), It.IsAny<bool>())
                      .SendAsync(null)
            ).Returns(Task.FromResult<SendResponse>(new SendResponse()));
            _emailFilterMock.Setup(x => x.IsAllowed(It.IsAny<string>())).Returns(true);

            await _sut.SendUsingTemplateAsync("to", "subject", disputeContractModel);

            _emailMock.Verify(foo => foo.To("to").Subject("subject").UsingTemplateFromEmbedded(It.IsAny<string>(), It.IsAny<DisputeEmail>(), It.IsAny<Assembly>(), It.IsAny<bool>()).SendAsync(null), Times.Once());
        }

        [Fact]
        public async Task Sending_email_only_adds_a_single_recipient()
        {
            var email = new Email("from@example.com");

            var sut = new EmailSender(email, _loggerMock.Object, _emailFilterMock.Object);
            var subject = _fixture.Create<string>();
            var model = _fixture.Create<TicketDisputeContract>();

            // act
            await sut.SendUsingTemplateAsync("to@example.com", subject, model);

            // assert
            Assert.Single(email.Data.ToAddresses);
            Assert.True(email.Data.IsHtml);

        }

        [Fact]
        public async Task Test_SendAsyncNotCalled_WhenEmailNotAllowed()
        {
            var fixture = new Fixture();
            var disputeContractModel = fixture.Create<TicketDisputeContract>();
            _emailMock.Setup(
                m => m.To(It.IsAny<string>())
                      .Subject(It.IsAny<string>())
                      .UsingTemplateFromEmbedded(It.IsAny<string>(), It.IsAny<DisputeEmail>(), It.IsAny<Assembly>(), It.IsAny<bool>())
                      .SendAsync(null)
            ).Returns(Task.FromResult<SendResponse>(new SendResponse()));
            _emailFilterMock.Setup(x => x.IsAllowed(It.IsAny<string>())).Returns(false);

            await _sut.SendUsingTemplateAsync("to", "subject", disputeContractModel);

            _emailMock.Verify(foo => foo.To("to").Subject("subject").UsingTemplateFromEmbedded(It.IsAny<string>(), It.IsAny<DisputeEmail>(), It.IsAny<Assembly>(), It.IsAny<bool>()).SendAsync(null), Times.Never());
        }
    }
}
