using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Infrastructure;
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
        private ViolationContext _violationContext;

        private ViolationContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            optionsBuilder.UseInMemoryDatabase("DisputeApi");

            return new ViolationContext(optionsBuilder.Options);
        }

        [SetUp]
        public void SetUp()
        {
            _violationContext = CreateContext();
            _service = new DisputeService(_loggerMock.Object, _violationContext);
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
            Assert.IsInstanceOf<IEnumerable<Dispute>>(result);
            _loggerMock.VerifyLog(LogLevel.Debug, "Getting all disputes", Times.Once());
        }

        [Theory, AllowCirculationAutoData]
        public async Task create_new_and_get_dispute(Dispute toCreate)
        {
            var result = await _service.CreateAsync(toCreate);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.AreNotEqual(0, result.Id);

            result = await _service.GetAsync(result.Id);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.IsNotNull(result);
        }

        [Theory, AllowCirculationAutoData]
        public async Task create_existed_dispute_get_id0(Dispute toCreate)
        {
            var result = await _service.CreateAsync(toCreate);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.AreNotEqual(0, result.Id);
            _loggerMock.VerifyLog(LogLevel.Debug, "Creating dispute", Times.Once());

            result = await _service.CreateAsync(toCreate);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.AreEqual(0, result.Id);
        }

        [Theory]
        [AllowCirculationAutoData]
        public async Task FindDispute_get_dispute(Dispute toCreate, string findTicketNumber, int findOffenceNumber)
        {
            toCreate.ViolationTicketNumber = findTicketNumber;
            //toCreate.OffenceNumber = findOffenceNumber;
            var result = await _service.CreateAsync(toCreate);
            Assert.IsInstanceOf<Dispute>(result);

            result = await _service.FindTicketDisputeAsync(findTicketNumber);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.AreEqual(findTicketNumber, result.ViolationTicketNumber);
            //Assert.AreEqual(findOffenceNumber, result.OffenceNumber);
        }

        [Theory, AllowCirculationAutoData]
        public async Task update_dispute_get_return_updatedRecords(Dispute toUpdate)
        {
            _violationContext.Disputes.Add(toUpdate);
            await _violationContext.SaveChangesAsync();
            toUpdate.DisputantFirstName = "updatedFirstName";

            var result = await _service.UpdateAsync(toUpdate);
            Assert.IsInstanceOf<Dispute>(result);
            Assert.AreEqual(toUpdate.Id, result.Id);
            Assert.AreEqual("updatedFirstName", result.DisputantFirstName);
            _loggerMock.VerifyLog(LogLevel.Debug, "Update dispute", Times.Once());
        }
    }
}
