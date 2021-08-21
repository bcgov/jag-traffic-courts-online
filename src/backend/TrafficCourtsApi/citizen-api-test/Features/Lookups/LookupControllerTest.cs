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
using AutoFixture;

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
        public async Task GetLookups_ReturnsNotNull_ActualResponseEqualsExpected_with_OK()
#pragma warning restore IDE1006 // Naming Styles
        {
            Fixture fixture = new Fixture();

            var result = new ApiResultResponse<LookupsAll>(fixture.Create<LookupsAll>());

            _serviceMock.Setup(x => x.GetAllLookUpsAsync())
                .Returns(Task.FromResult(result.Result));

            var sut = new LookupController(_loggerMock.Object, _serviceMock.Object);

            var actual = await sut.Get();
            Assert.NotNull(actual);
            Assert.IsAssignableFrom<OkObjectResult>(actual);
            var actualObjectResult = (ObjectResult)actual;
            Assert.Equal(200, (actualObjectResult.StatusCode));
            Assert.IsType<ApiResultResponse<LookupsAll>>(actualObjectResult.Value);
            Assert.Equal<LookupsAll>(result.Result, ((ApiResultResponse<LookupsAll>)actualObjectResult.Value).Result);
        }
    }
}
