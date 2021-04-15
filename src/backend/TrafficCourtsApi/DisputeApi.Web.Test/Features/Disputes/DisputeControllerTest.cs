﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Disputes
{
    [ExcludeFromCodeCoverage]
    public class DisputeControllerTest
    {
        private Mock<ILogger<DisputesController>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<IPublishEndpoint> _publishEndpointMock;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<DisputesController>();
            _disputeServiceMock = new Mock<IDisputeService>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
        }

        [Test]
        public void throw_ArgumentNullException_if_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DisputesController(null, _disputeServiceMock.Object, _publishEndpointMock.Object));
            Assert.Throws<ArgumentNullException>(() => new DisputesController(_loggerMock.Object, null, _publishEndpointMock.Object));
            Assert.Throws<ArgumentNullException>(() => new DisputesController(_loggerMock.Object, _disputeServiceMock.Object, null));
        }

        [Theory]
        [AutoData]
        public async Task get_disputes(Dispute dispute)
        {
            IEnumerable<Dispute> expected = new List<Dispute> { dispute };

            _disputeServiceMock
                .Setup(x => x.GetAllAsync())
                .Returns(Task.FromResult(expected));


            var sut = new DisputesController(_loggerMock.Object, _disputeServiceMock.Object, _publishEndpointMock.Object);

            var result = await sut.GetDisputes();
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);

            var objectResult = (ObjectResult) result;
            Assert.IsInstanceOf<IEnumerable<Dispute>>(objectResult.Value);

            var values = (IEnumerable<Dispute>) objectResult.Value;
            Assert.AreEqual(values.Count(), 1);

            _disputeServiceMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task get_dispute(Dispute expected)
        {
            _disputeServiceMock
                .Setup(x => x.GetAsync(expected.Id))
                .Returns(Task.FromResult(expected));

            var sut = new DisputesController(_loggerMock.Object, _disputeServiceMock.Object, _publishEndpointMock.Object);

            var result = await sut.GetDispute(expected.Id);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);

            Assert.IsInstanceOf<Dispute>(((OkObjectResult)result).Value);
            _disputeServiceMock.Verify(x => x.GetAsync(expected.Id), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task returns_not_found_if_dispute_service_returns_null(int disputeId)
        {
            _disputeServiceMock
                .Setup(x => x.GetAsync(It.IsAny<int>()))
                .Returns(Task.FromResult((Dispute)null));

            var sut = new DisputesController(_loggerMock.Object, _disputeServiceMock.Object, _publishEndpointMock.Object);
            
            var result = await sut.GetDispute(disputeId);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Theory]
        [AutoData]
        public async Task create_dispute(Dispute dispute)
        {
            _disputeServiceMock
                .Setup(x => x.CreateAsync(dispute))
                .Returns(Task.FromResult(dispute));

            var sut = new DisputesController(_loggerMock.Object, _disputeServiceMock.Object, _publishEndpointMock.Object);

            var result = await sut.CreateDispute(dispute);
            Assert.IsNotNull(result);
            _disputeServiceMock.Verify(x => x.CreateAsync(dispute), Times.Once);
        }

        [Theory]
        [AutoData]
        public async Task when_service_return_null_createDispute_return_badRequest(Dispute dispute)
        {
            _disputeServiceMock
                .Setup(x => x.CreateAsync(dispute))
                .Returns(Task.FromResult<Dispute>(null));

            var sut = new DisputesController(_loggerMock.Object, _disputeServiceMock.Object, _publishEndpointMock.Object);

            var result = (BadRequestObjectResult)await sut.CreateDispute(dispute);
            Assert.IsNotNull(result);
            _disputeServiceMock.Verify(x => x.CreateAsync(dispute), Times.Once);
            Assert.AreEqual(400, result.StatusCode);
        }
    }
}
