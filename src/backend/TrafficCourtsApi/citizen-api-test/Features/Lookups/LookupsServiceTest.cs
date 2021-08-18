using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Gov.CitizenApi.Features.Lookups;
using Gov.CitizenApi.Infrastructure;
using Gov.CitizenApi.Test.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Gov.CitizenApi.Test.Features.Lookups
{
    [ExcludeFromCodeCoverage]
    public class LookupsServiceTest
    {
        private readonly Mock<ILogger<LookupsService>> _loggerMock = LoggerServiceMock.LoggerMock<LookupsService>();

        private static ViolationContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ViolationContext>();
            // use a random name because the in-memory database is shared anywhere the same name is used.
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            var context = new ViolationContext(optionsBuilder.Options);

            return context;
        }

        private LookupsService CreateSubjectUnderTest()
        {
            return new LookupsService(_loggerMock.Object);
        }

        public LookupsServiceTest()
        {
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void throw_ArgumentNullException_if_passed_null()
#pragma warning restore IDE1006 // Naming Styles
        {
            Assert.Throws<ArgumentNullException>(() => new LookupsService(null));
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public async Task get_lookupsAsync()
#pragma warning restore IDE1006 // Naming Styles
        {
            LookupsService service = CreateSubjectUnderTest();

            var result =await service.GetAllLookUps();
            Assert.IsAssignableFrom<LookupsAll>(result);
        }

       
    }
}
