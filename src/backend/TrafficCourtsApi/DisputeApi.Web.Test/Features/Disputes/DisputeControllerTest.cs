using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Disputes
{
    [ExcludeFromCodeCoverage]
    public class DisputeControllerTest
    {
        private readonly Mock<ILogger<DisputeController>> _loggerMock = LoggerServiceMock.LoggerMock<DisputeController>();
        private DisputeController _controller;
        private Mock<IDisputeService> _disputeServiceMock;

        [SetUp]
        public void SetUp()
        {
            _disputeServiceMock = new Mock<IDisputeService>();
            
            _controller = new DisputeController(_loggerMock.Object, _disputeServiceMock.Object);
        }

        [Theory]
        [AutoData]
        public async Task get_disputes(Dispute dispute)
        {
            IEnumerable<Dispute> expected = new List<Dispute> { dispute };

            _disputeServiceMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(expected));

            var result = (OkObjectResult)await _controller.GetDisputes();
            Assert.IsInstanceOf<IEnumerable<Dispute>>(result.Value);
            Assert.IsNotNull(result);
            Assert.AreEqual(((IEnumerable<Dispute>)result.Value).Count(), 1);
            _disputeServiceMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task get_dispute(Dispute expected)
        {
            _disputeServiceMock
                .Setup(x => x.GetAsync(1))
                .Returns(Task.FromResult(expected));

            var result = (OkObjectResult)await _controller.GetDispute(1);
            Assert.IsInstanceOf<Dispute>(result.Value);
            Assert.IsNotNull(result);
            _disputeServiceMock.Verify(x => x.GetAsync(1), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task create_dispute(Dispute dispute)
        {
            _disputeServiceMock
                .Setup(x => x.CreateAsync(dispute))
                .Returns(Task.FromResult(dispute));

            var result = (OkObjectResult)await _controller.CreateDispute(dispute);
            Assert.IsInstanceOf<Dispute>(result.Value);
            Assert.IsNotNull(result.Value);
            _disputeServiceMock.Verify(x => x.CreateAsync(dispute), Times.Once);
        }
    }
}
