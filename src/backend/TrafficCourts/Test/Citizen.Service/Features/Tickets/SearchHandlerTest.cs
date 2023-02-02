using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Features.Tickets;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services.Tickets.Search;
using Xunit;
using Xunit.Abstractions;

namespace TrafficCourts.Test.Citizen.Service.Features.Tickets
{
    public class SearchHandlerTest
    {
        private readonly Mock<ITicketSearchService> _serviceMock = new Mock<ITicketSearchService>();
        private readonly Mock<ILogger<Search.Handler>> _loggerMock = new Mock<ILogger<Search.Handler>>();
        private readonly Mock<IRedisCacheService> _redisCacheServiceMock = new Mock<IRedisCacheService>();
        private readonly ITestOutputHelper _output;

        public SearchHandlerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void constructor_throws_ArgumentNullException_when_passed_null()
        {
            Assert.Throws<ArgumentNullException>("service", () => new Search.Handler(null!, _loggerMock.Object, _redisCacheServiceMock.Object));
            Assert.Throws<ArgumentNullException>("logger", () => new Search.Handler(_serviceMock.Object, null!, _redisCacheServiceMock.Object));
            Assert.Throws<ArgumentNullException>("redisCacheService", () => new Search.Handler(_serviceMock.Object, _loggerMock.Object, null!));
        }


        [Fact]
        public async Task search_service_returns_null_empty_response_is_returned()
        {
            var expected = Search.Response.Empty;
            ViolationTicket? violationTicket = null;

            _serviceMock
                .Setup(_ => _.SearchAsync(It.IsAny<string>(), It.IsAny<TimeOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(violationTicket);

            var request = new Search.Request("AA00000000", "00:00");

            var sut = new Search.Handler(_serviceMock.Object, _loggerMock.Object, _redisCacheServiceMock.Object);

            var actual = await sut.Handle(request, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task search_service_returns_ViolationTicket_response_contains_same_value()
        {
            ViolationTicket? violationTicket = new();

            _serviceMock
                .Setup(_ => _.SearchAsync(It.IsAny<string>(), It.IsAny<TimeOnly>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(violationTicket);

            var request = new Search.Request("AA00000000", "00:00");

            var sut = new Search.Handler(_serviceMock.Object, _loggerMock.Object, _redisCacheServiceMock.Object);

            var actual = await sut.Handle(request, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.Same(violationTicket, actual.Result.Value);
        }


        [Fact]
        public async Task search_throws_exception()
        {
            Exception exception = new();

            _serviceMock
                .Setup(_ => _.SearchAsync(It.IsAny<string>(), It.IsAny<TimeOnly>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var request = new Search.Request("AA00000000", "00:00");

            var sut = new Search.Handler(_serviceMock.Object, _loggerMock.Object, _redisCacheServiceMock.Object);

            var actual = await sut.Handle(request, CancellationToken.None);

            Assert.NotNull(actual);
            Assert.Same(exception, actual.Result.Value);
        }
    }
}
