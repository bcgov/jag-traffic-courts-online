using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Common;
using TrafficCourts.Citizen.Service.Services.Tickets.Search.Mock;
using TrafficCourts.Common;
using Xunit;

namespace TrafficCourts.Test.Ticket.Search.Service.Features.Search.Mock
{
    public class EmbeddedMockDataProviderTests
    {
        [Fact]
        public void can_get_readable_stream()
        {
            Mock<ILogger<EmbeddedMockDataProvider>> mockLogger = new();

            EmbeddedMockDataProvider sut = new(mockLogger.Object);

            using var actual = sut.GetDataStream();
            Assert.NotNull(actual);
            Assert.True(actual!.CanRead);
        }

        [Fact]
        public void constructor_logger_parameter_is_required()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new EmbeddedMockDataProvider(null!));
            Assert.Equal("logger", actual.ParamName);
        }
    }

    public class MockTicketSearchServiceTests
    {
        [Fact]
        public async Task can_get_data_from_embedded_data_provider()
        {
            IMockDataProvider mockDataProvider = GetEmbeddedMockDataProvider();
            Mock<ILogger<MockTicketSearchService>> mockLogger = new();
            IClock clock = GetClock();

            MockTicketSearchService sut = new MockTicketSearchService(mockDataProvider, mockLogger.Object, clock);
            IEnumerable<Invoice>? actual = await sut.SearchAsync("EA02000460", new TimeOnly(9, 54), CancellationToken.None);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async Task will_adjust_dates_based_on_current_date()
        {
            IMockDataProvider mockDataProvider = GetEmbeddedMockDataProvider();
            Mock<ILogger<MockTicketSearchService>> mockLogger = new();
            IClock clock = GetClock(6);

            MockTicketSearchService sut = new MockTicketSearchService(mockDataProvider, mockLogger.Object, clock);
            IEnumerable<Invoice>? actual = await sut.SearchAsync("EA02000460", new TimeOnly(9, 54), CancellationToken.None);

            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Single(actual.Select(_ => _.ViolationDateTime).Distinct());
            Assert.Single(actual.Select(_ => _.ViolationDate).Distinct());

            // the day before the current date (June 1)
            Assert.Equal("2022-05-31T09:54", actual.Select(_ => _.ViolationDateTime).Distinct().Single());

            // now adjust clock to July
            clock = GetClock(7);

            sut = new MockTicketSearchService(mockDataProvider, mockLogger.Object, clock);
            actual = await sut.SearchAsync("EA02000460", new TimeOnly(9, 54), CancellationToken.None);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Single(actual.Select(_ => _.ViolationDateTime).Distinct());
            Assert.Single(actual.Select(_ => _.ViolationDate).Distinct());

            // the day before the current date (July 1)
            Assert.Equal("2022-06-30T09:54", actual.Select(_ => _.ViolationDateTime).Distinct().Single());
        }

        private IMockDataProvider GetEmbeddedMockDataProvider()
        {
            Mock<ILogger<EmbeddedMockDataProvider>> mockLogger = new();
            EmbeddedMockDataProvider provider = new(mockLogger.Object);
            return provider;
        }

        private IClock GetClock(int month = 6)
        {
            FakeClock clock = new FakeClock(Instant.FromDateTimeUtc(new DateTime(2022, month, 1, 7, 0, 0, DateTimeKind.Utc)));

            // validate pre-condiction
            var june1 = clock.GetCurrentPacificTime();
            Assert.Equal(2022, june1.Year);
            Assert.Equal(month, june1.Month);
            Assert.Equal(1, june1.Day);
            Assert.Equal(0, june1.Hour);
            Assert.Equal(0, june1.Minute);
            Assert.Equal(0, june1.Second);
            Assert.Equal(TimeSpan.FromHours(-7), june1.Offset);

            return clock;
        }
    }
}
