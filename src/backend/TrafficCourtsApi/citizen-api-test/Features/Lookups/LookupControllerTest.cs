using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using AutoFixture.Xunit2;
using Gov.CitizenApi.Features.Lookups;
using Gov.CitizenApi.Features;
using Gov.CitizenApi.Features.Disputes;
using Gov.CitizenApi.Features.Disputes.Commands;
using Gov.CitizenApi.Features.Disputes.Queries;
using Gov.CitizenApi.Test.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace Gov.CitizenApi.Test.Features.Lookups
{
    [ExcludeFromCodeCoverage]
    public class LookupControllerTest
    {
        private readonly Mock<ILogger<LookupController>> _loggerMock;
        private readonly Mock<ILookupsService> _serviceMock;

        public LookupControllerTest()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<LookupController>();
            _serviceMock = new Mock<ILookupsService>();
        }

    [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void throw_ArgumentNullException_if_passed_null()
    #pragma warning restore IDE1006 // Naming Styles
        {
            Assert.Throws<ArgumentNullException>(() => new LookupController(null, _serviceMock.Object));
            Assert.Throws<ArgumentNullException>(() => new LookupController(_loggerMock.Object, null));
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public async Task get_lookups()
#pragma warning restore IDE1006 // Naming Styles
        {
            LookupsAll expected = new LookupsAll { };

            _serviceMock.Setup(x => x.GetAllLookUps())
                .Returns(Task.FromResult(expected));

            var sut = new LookupController(_loggerMock.Object, _serviceMock.Object);

            var result = await sut.Get();
            Assert.NotNull(result);
            Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal(200, ((result as OkObjectResult).StatusCode));
        }
    }
}
