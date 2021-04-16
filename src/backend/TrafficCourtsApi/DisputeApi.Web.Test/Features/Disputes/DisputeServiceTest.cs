using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Infrastructure;
using DisputeApi.Web.Models;
using DisputeApi.Web.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.Disputes
{
    [ExcludeFromCodeCoverage]
    public class DisputeServiceTest
    {
        private IDisputeService _service;
        private readonly Mock<ILogger<DisputeService>> _loggerMock = LoggerServiceMock.LoggerMock<DisputeService>();


        private ViolationContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            optionsBuilder.UseInMemoryDatabase("DisputeApi");

            return new ViolationContext(optionsBuilder.Options);
        }

        [SetUp]
        public void SetUp()
        {
            _service = new DisputeService(_loggerMock.Object, CreateContext());
        }


        [Test]
        public void throw_ArgumentNullException_if_passed_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DisputeService(null, CreateContext()));
            Assert.Throws<ArgumentNullException>(() => new DisputeService(_loggerMock.Object, null));
        }

        [Test]
        public async Task get_disputes()
        {
            var result = await _service.GetAllAsync();
            Assert.IsInstanceOf<IEnumerable<DisputeViewModel>>(result);
            //_loggerMock.VerifyLog(LogLevel.Debug, "Returning list of mock disputes", Times.Once());
        }

        [Theory]
        [AutoData]
        public async Task create_and_get_dispute(DisputeViewModel expected)
        {
            var result = await _service.CreateAsync(expected);
            Assert.IsInstanceOf<DisputeViewModel>(result);

            //_loggerMock.VerifyLog(LogLevel.Debug, "Creating mock dispute", Times.Once());

            result = await _service.GetAsync(expected.Id);
            Assert.IsInstanceOf<DisputeViewModel>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, expected.Id);
            //_loggerMock.VerifyLog(LogLevel.Information, "Returning a specific mock dispute", Times.Once());
        }
    }
}
