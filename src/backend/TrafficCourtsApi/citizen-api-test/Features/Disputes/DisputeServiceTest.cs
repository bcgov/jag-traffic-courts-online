using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Gov.CitizenApi.Features.Disputes;
using Gov.CitizenApi.Features.Disputes.DBModel;
using Gov.CitizenApi.Infrastructure;
using Gov.CitizenApi.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Disputes
{
    [ExcludeFromCodeCoverage]
    public class DisputeServiceTest
    {
        private readonly Mock<ILogger<DisputeService>> _loggerMock = LoggerServiceMock.LoggerMock<DisputeService>();

        private static ViolationContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            // use a random name because the in-memory database is shared anywhere the same name is used.
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            var context = new ViolationContext(optionsBuilder.Options);

            return context;
        }

        private DisputeService CreateSubjectUnderTest(ViolationContext context)
        {
            return new DisputeService(_loggerMock.Object, context);
        }

        private DisputeService CreateSubjectUnderTest()
        {
            ViolationContext context = CreateContext();
            return new DisputeService(_loggerMock.Object, context);
        }

        public DisputeServiceTest()
        {
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void throw_ArgumentNullException_if_passed_null()
#pragma warning restore IDE1006 // Naming Styles
        {
            Assert.Throws<ArgumentNullException>(() => new DisputeService(null, CreateContext()));
            Assert.Throws<ArgumentNullException>(() => new DisputeService(_loggerMock.Object, null));
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public async Task get_disputes()
#pragma warning restore IDE1006 // Naming Styles
        {
            DisputeService service = CreateSubjectUnderTest();

            var result = await service.GetAllAsync();
            Assert.IsAssignableFrom<IEnumerable<Dispute>>(result);
            _loggerMock.VerifyLog(LogLevel.Debug, "Getting all disputes", Times.Once());
        }

        [Theory]
        [AllowCirculationAutoData]
#pragma warning disable IDE1006 // Naming Styles
        public async Task create_new_and_get_dispute(Dispute toCreate)
#pragma warning restore IDE1006 // Naming Styles
        {
            DisputeService service = CreateSubjectUnderTest();

            var result = await service.CreateAsync(toCreate);
            Assert.IsAssignableFrom<Dispute>(result);
            Assert.NotEqual(0, result.Id);

            result = await service.GetAsync(result.Id);
            Assert.IsAssignableFrom<Dispute>(result);
            Assert.NotNull(result);
        }

        [Theory]
        [AllowCirculationAutoData]
#pragma warning disable IDE1006 // Naming Styles
        public async Task create_existed_dispute_get_id0(Dispute toCreate)
#pragma warning restore IDE1006 // Naming Styles
        {
            DisputeService service = CreateSubjectUnderTest();

            var result = await service.CreateAsync(toCreate);
            Assert.IsType<Dispute>(result);
            Assert.NotEqual(0, result.Id);
            _loggerMock.VerifyLog(LogLevel.Debug, "Creating dispute", Times.Once());

            result = await service.CreateAsync(toCreate);
            Assert.IsAssignableFrom<Dispute>(result);
            Assert.Equal(0, result.Id);
        }

        [Theory]
        [AllowCirculationAutoData]
        public async Task FindDispute_get_dispute(Dispute toCreate, string findTicketNumber)
        {
            DisputeService service = CreateSubjectUnderTest();

            toCreate.ViolationTicketNumber = findTicketNumber;
            //toCreate.OffenceNumber = findOffenceNumber;
            var result = await service.CreateAsync(toCreate);
            Assert.IsAssignableFrom<Dispute>(result);

            result = await service.FindTicketDisputeAsync(findTicketNumber);
            Assert.IsAssignableFrom<Dispute>(result);
            Assert.Equal(findTicketNumber, result.ViolationTicketNumber);
            //Assert.Equal(findOffenceNumber, result.OffenceNumber);
        }

        [Theory]
        [AllowCirculationAutoData]
#pragma warning disable IDE1006 // Naming Styles
        public async Task update_dispute_get_return_updatedRecords(Dispute toUpdate)
#pragma warning restore IDE1006 // Naming Styles
        {
            ViolationContext context = CreateContext();
            DisputeService service = CreateSubjectUnderTest(context);

            context.Disputes.Add(toUpdate);
            await context.SaveChangesAsync();
            toUpdate.DisputantFirstName = "updatedFirstName";

            var result = await service.UpdateAsync(toUpdate);
            Assert.IsAssignableFrom<Dispute>(result);
            Assert.Equal(toUpdate.Id, result.Id);
            Assert.Equal("updatedFirstName", result.DisputantFirstName);
            _loggerMock.VerifyLog(LogLevel.Debug, "Update dispute", Times.Once());
        }
    }
}
