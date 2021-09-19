﻿using AutoFixture;
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

        public EmailSenderTest()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<EmailSender>>();
            _emailMock = new Mock<IFluentEmail>();
                    //_emailMock.Object.AddLiquidRenderer()
                    //.AddSmtpSender(new SmtpClient(sender)
                    //{
                    //    EnableSsl = false,
                    //    Port = int.Parse(port),
                    //    DeliveryMethod = SmtpDeliveryMethod.Network
                    //});
            _sut = new EmailSender(_emailMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Throw_ArgumentNullException_if_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new EmailSender(null, _loggerMock.Object));
            Assert.Throws<ArgumentNullException>(() => new EmailSender(_emailMock.Object, null));
            Assert.ThrowsAsync<ArgumentException>(() => _sut.SendUsingTemplate(null, "This is the subject line", It.IsAny<TicketDisputeContract>()));
            Assert.ThrowsAsync<ArgumentException>(() => _sut.SendUsingTemplate("", "This is the subject line", It.IsAny<TicketDisputeContract>()));
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SendUsingTemplate("testEmail@test.com", "This is the subject line", null));
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

            Email.DefaultRenderer = new LiquidRenderer(Options.Create(new LiquidRendererOptions { FileProvider = null,
            ConfigureTemplateContext = null }));

            string base64Data = Convert.ToBase64String(bytes);
            string dataScheme = $"data:image/png;base64,{base64Data}==";

            emailModel.LogoImage = dataScheme;

            Template template = Template.Parse(templateString); // Parses and compiles the template
            expected = template.Render(Hash.FromAnonymousObject(emailModel));

           
            _sut.SendUsingTemplate("testEmail@test.com", "This is the subject line", disputeContractModel);

            _emailMock.Verify(foo => foo.To("testEmail@test.com"), Times.Once());
            _emailMock.Verify(foo => foo.Subject("This is the subject line"), Times.Once());
            _emailMock.Verify(foo => foo.UsingTemplateFromEmbedded(It.IsAny<string>(), 
                It.Is<DisputeEmail>(model => model.ConfirmationNumber == emailModel.ConfirmationNumber && model.LogoImage == emailModel.LogoImage),
                It.IsAny<Assembly>(), true), Times.Once());

            var result = new Email("Sender@send.com")
                              .To("testEmail@test.com")
                              .Subject("This is the subject line")
                              //.UsingTemplate(templateString, emailModel);
                              .UsingTemplateFromEmbedded("ticket_worker_test.Features.Emails.Resources.submissiontemplate.liquid", emailModel, this.GetType().GetTypeInfo().Assembly);
            Assert.Equal(expected, result.Data.Body);

        }
    }
}
