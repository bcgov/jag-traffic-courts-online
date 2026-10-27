using Microsoft.Extensions.Time.Testing;

namespace TrafficCourts.Core.Test
{
    public class ClockExtensionsTests
    {
        [Theory]
        [InlineData(2025,  1, -8)]
        [InlineData(2025,  7, -7)] // DST
        [InlineData(2025, 12, -8)]
        [InlineData(2026,  1, -8)]
        [InlineData(2026,  7, -7)] // start of permanent DST
        [InlineData(2026, 12, -7)]
        [InlineData(2027,  1, -7)]
        [InlineData(2027,  7, -7)]
        [InlineData(2027, 12, -7)]
        public void GetCurrentPacificTime_ShouldReturnSpecifiedUtcOffsetAtDate(int year, int month, int utcOffset)
        {
            DateTimeOffset fakeUtcNow = new(year, month, 1, 12, 0, 0, TimeSpan.Zero);
            DateTimeOffset expected = fakeUtcNow.ToOffset(TimeSpan.FromHours(utcOffset));
            DateTimeOffset actual = new FakeTimeProvider(fakeUtcNow).GetCurrentPacificTime();

            Assert.Equal(expected, actual);
            Assert.Equal(expected.Offset, actual.Offset);
        }

        [Theory]
        [InlineData(DateTimeKind.Local)]
        [InlineData(DateTimeKind.Unspecified)]
        [InlineData(DateTimeKind.Utc)]
        public void UtcToLocalTime_ShouldConvertToSpecifiedTimeZone(DateTimeKind kind)
        {
            // Arrange
            var utc = new DateTime(2027, 10, 1, 12, 0, 0, kind);
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Vancouver");
            
            // Act
            var convertedDateTime = ClockExtensions.UtcToLocalTime(utc, timeZone);
            
            // Assert
            Assert.Equal(DateTimeKind.Unspecified, convertedDateTime.Kind);
            Assert.Equal(new DateTime(2027, 10, 1, 5, 0, 0), convertedDateTime);
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
    }
}
