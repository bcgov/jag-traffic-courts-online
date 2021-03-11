using DisputeApi.Web.Features.TcoDispute.Controller;
using DisputeApi.Web.Features.TcoDispute.Models;
using DisputeApi.Web.Features.TcoDispute.Service;
using DisputeApi.Web.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisputeApi.Web.Test.Features.TcoDispute.Controller
{
    public class DisputeControllerTest
    {
        private DisputeController _controller;
        private readonly Mock<ILogger<DisputeController>> _loggerMock = LoggerServiceMock.LoggerMock<DisputeController>();
        private Mock<IDisputeService> _disputeServiceMock = new Mock<IDisputeService>();

        [SetUp]
        public void SetUp()
        {
            _controller = new DisputeController(_loggerMock.Object, _disputeServiceMock.Object);
        }

        [Test]
        public async Task get_disputes()
        {
            var dispute = new Dispute
            {
                Id = 1,
                TicketId = 1,
                EmailAddress = "jones_234@email.com",
                LawyerPresent = true,
                InterpreterRequired = true,
                InterpreterLanguage = "Spanish",
                CallWitness = false,
                CertifyCorrect = true,
                StatusCode = "SUBM"
            };

            _disputeServiceMock.Setup(x => x.GetDisputes()).Returns(
              Task.FromResult(new List<Dispute> { dispute }.AsQueryable()));

            var result = (OkObjectResult)await _controller.GetDisputes();
            Assert.IsInstanceOf<IQueryable<Dispute>>(result.Value);
            Assert.IsNotNull(result);
            Assert.AreEqual(((IQueryable<Dispute>)result.Value).Count(), 1);
            _disputeServiceMock.Verify(x => x.GetDisputes(), Times.Once);
        }

        [Test]
        public async Task get_dispute()
        {
            var dispute = new Dispute
            {
                Id = 1,
                TicketId = 1,
                EmailAddress = "jones_234@email.com",
                LawyerPresent = true,
                InterpreterRequired = true,
                InterpreterLanguage = "Spanish",
                CallWitness = false,
                CertifyCorrect = false,
                StatusCode = "INP"
            };

            _disputeServiceMock.Setup(x => x.GetDispute(1)).Returns(Task.FromResult(
               dispute));

            var result = (OkObjectResult)await _controller.GetDispute(1);
            Assert.IsInstanceOf<Dispute>(result.Value);
            Assert.IsNotNull(result);
            _disputeServiceMock.Verify(x => x.GetDispute(1), Times.Once);
        }

        [Test]
        public async Task create_dispute()
        {
            var dispute = new Dispute
            {
                Id = 2,
                TicketId = 2,
                EmailAddress = "jones_234@email.com",
                LawyerPresent = false,
                InterpreterRequired = false,
                CallWitness = false,
                CertifyCorrect = false,
                StatusCode = "NEW"
            };

            _disputeServiceMock.Setup(x => x.CreateDispute(dispute)).Returns(Task.FromResult(
               dispute));

            var result = (OkObjectResult)await _controller.CreateDispute(dispute);
            Assert.IsInstanceOf<Dispute>(result.Value);
            Assert.IsNotNull(result.Value);
            _disputeServiceMock.Verify(x => x.CreateDispute(dispute), Times.Once);
        }
    }
}
