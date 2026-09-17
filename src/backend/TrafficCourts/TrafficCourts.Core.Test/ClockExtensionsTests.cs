using Microsoft.Extensions.Time.Testing;

namespace TrafficCourts.Core.Test
{
    public class ClockExtensionsTests : IDisposable
    {
        // Hardcode America/Vancouver via the TZ environment variable for these tests so
        // GetCurrentConfiguredTime is exercised deterministically here, independent of
        // whatever TZ the host or CI environment happens to have configured.
        private const string TimeZoneId = "America/Vancouver";
        private readonly string? _originalTz;

        public ClockExtensionsTests()
        {
            _originalTz = Environment.GetEnvironmentVariable("TZ");
            Environment.SetEnvironmentVariable("TZ", TimeZoneId);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("TZ", _originalTz);
        }

        [Fact]
        public void GetCurrentConfiguredTime_ShouldReturnUtcMinus7_DuringDaylightSavings_Before2026Change()
        {
            // Arrange - July 2025, before BC abolishes Daylight Saving Time in November 2026
            var fakeUtcNow = new DateTimeOffset(2025, 7, 1, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeUtcNow);

            // Act
            var configuredTime = fakeTimeProvider.GetCurrentConfiguredTime();

            // Assert
            Assert.Equal(new DateTimeOffset(2025, 7, 1, 5, 0, 0, TimeSpan.FromHours(-7)), configuredTime); // DST offset
        }

        [Fact]
        public void GetCurrentConfiguredTime_ShouldReturnUtcMinus8_OutsideDaylightSavings_Before2026Change()
        {
            // Arrange - December 2025, before BC abolishes Daylight Saving Time in November 2026
            var fakeUtcNow = new DateTimeOffset(2025, 12, 1, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeUtcNow);

            // Act
            var configuredTime = fakeTimeProvider.GetCurrentConfiguredTime();

            // Assert
            Assert.Equal(new DateTimeOffset(2025, 12, 1, 4, 0, 0, TimeSpan.FromHours(-8)), configuredTime); // Standard offset
        }

        [Fact]
        public void GetCurrentConfiguredTime_ShouldStayAtUtcMinus7_InWinter_AfterDaylightSavingIsAbolished()
        {
            // Arrange - December 2026, after BC's permanent move to UTC-7 (no more fall-back to UTC-8)
            var fakeUtcNow = new DateTimeOffset(2026, 12, 1, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeUtcNow);

            // Act
            var configuredTime = fakeTimeProvider.GetCurrentConfiguredTime();

            // Assert
            Assert.Equal(new DateTimeOffset(2026, 12, 1, 5, 0, 0, TimeSpan.FromHours(-7)), configuredTime);
        }

        [Fact]
        public void GetCurrentConfiguredTime_ShouldStayAtUtcMinus7_InSummer_AfterDaylightSavingIsAbolished()
        {
            // Arrange - July 2027, confirming the offset stays UTC-7 year round once DST is abolished
            var fakeUtcNow = new DateTimeOffset(2027, 7, 1, 12, 0, 0, TimeSpan.Zero);
            var fakeTimeProvider = new FakeTimeProvider(fakeUtcNow);

            // Act
            var configuredTime = fakeTimeProvider.GetCurrentConfiguredTime();

            // Assert
            Assert.Equal(new DateTimeOffset(2027, 7, 1, 5, 0, 0, TimeSpan.FromHours(-7)), configuredTime);
        }

        [Theory]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        [InlineData(DateTimeKind.Utc)]
        public void UtcToLocalTime_ShouldConvertToSpecifiedTimeZone(DateTimeKind kind)
        {
            // Arrange
            var utc = new DateTime(2023, 10, 1, 12, 0, 0, kind);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            
            // Act
            var convertedDateTime = ClockExtensions.UtcToLocalTime(utc, timeZone);
            
            // Assert
            Assert.NotNull(convertedDateTime);
            Assert.Equal(DateTimeKind.Unspecified, convertedDateTime.Kind);
            Assert.Equal(new DateTime(2023, 10, 1, 5, 0, 0), convertedDateTime);
        }

        [Fact]
        public void UtcToLocalTime_ShouldReturnNull_WhenDateTimeIsNull()
        {
            // Arrange
            DateTime? dateTime = null;
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");

            // Act
            var result = ClockExtensions.UtcToLocalTime(dateTime, timeZone);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void UtcToLocalTime_ShouldThrowArgumentNullException_WhenTimeZoneIsNull()
        {
            // Arrange
            var dateTime = new DateTime(2023, 10, 1, 12, 0, 0, DateTimeKind.Utc);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ClockExtensions.UtcToLocalTime(dateTime, null!));
        }

        [Fact]
        public void UtcToLocalTime_ShouldHandleDaylightSavingTime()
        {
            // Arrange
            var dateTime = new DateTime(2023, 11, 5, 1, 0, 0, DateTimeKind.Utc); // During DST transition
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

            // Act
            var result = ClockExtensions.UtcToLocalTime(dateTime, timeZone);

            // Assert
            Assert.Equal(new DateTime(2023, 11, 4, 21, 0, 0), result); // Adjusted for DST
        }

        [Theory]
        [InlineData(2023, 3, 12, 9, 59, 0, 2023, 3, 12, 1, 59, 0, -8)]
        [InlineData(2023, 3, 12, 10, 0, 0, 2023, 3, 12, 3, 0, 0, -7)]
        [InlineData(2023, 11, 5, 8, 59, 0, 2023, 11, 5, 1, 59, 0, -7)]
        [InlineData(2023, 11, 5, 9, 0, 0, 2023, 11, 5, 1, 0, 0, -8)]
        public void UtcToLocalTime_ShouldUseTimezoneRulesAtTransitionBoundary(
            int utcYear,
            int utcMonth,
            int utcDay,
            int utcHour,
            int utcMinute,
            int utcSecond,
            int localYear,
            int localMonth,
            int localDay,
            int localHour,
            int localMinute,
            int localSecond,
            int expectedOffsetHours)
        {
            var utc = new DateTime(utcYear, utcMonth, utcDay, utcHour, utcMinute, utcSecond, DateTimeKind.Utc);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");

            var result = ClockExtensions.UtcToLocalTime(utc, timeZone);

            Assert.Equal(new DateTime(localYear, localMonth, localDay, localHour, localMinute, localSecond), result);
            Assert.Equal(expectedOffsetHours, timeZone.GetUtcOffset(utc).Hours);
        }
    }
}
