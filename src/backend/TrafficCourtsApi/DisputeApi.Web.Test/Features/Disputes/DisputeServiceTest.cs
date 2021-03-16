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

        [SetUp]
        public void SetUp()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            optionsBuilder.UseInMemoryDatabase("DisputeApi");
            _service = new DisputeService(_loggerMock.Object, new ViolationContext(optionsBuilder.Options));
        }

        [Test]
        public async Task get_disputes()
        {
            var result = await _service.GetAllAsync();
            Assert.IsInstanceOf<IEnumerable<Dispute>>(result);
            //_loggerMock.VerifyLog(LogLevel.Debug, "Returning list of mock disputes", Times.Once());
        }

        [Theory]
        [AutoData]
        public async Task create_and_get_dispute(Dispute expected)
        {
            var result = await _service.CreateAsync(expected);
            Assert.IsInstanceOf<Dispute>(result);

            //_loggerMock.VerifyLog(LogLevel.Debug, "Creating mock dispute", Times.Once());

            result = await _service.GetAsync(expected.Id);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Id, expected.Id);
            //_loggerMock.VerifyLog(LogLevel.Information, "Returning a specific mock dispute", Times.Once());
        }
    }
}
