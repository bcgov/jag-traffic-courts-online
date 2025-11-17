using Microsoft.Extensions.Time.Testing;

namespace TrafficCourts.Core.Test
{
    public class ClockExtensionsTests
    {
        [Fact]
        public void GetCurrentPacificTime_ShouldReturnPacificTime_DuringDaylightSavings()
        {
            // Arrange
            var fakeUtcNow = new DateTimeOffset(2023, 7, 1, 12, 0, 0, TimeSpan.Zero); // Summer date
            var fakeTimeProvider = new FakeTimeProvider(fakeUtcNow);

            // Act
            var pacificTime = fakeTimeProvider.GetCurrentPacificTime();

            // Assert
            Assert.Equal(new DateTimeOffset(2023, 7, 1, 5, 0, 0, TimeSpan.FromHours(-7)), pacificTime); // DST offset
        }

        [Fact]
        public void GetCurrentPacificTime_ShouldReturnPacificTime_OutsideDaylightSavings()
        {
            // Arrange
            var fakeUtcNow = new DateTimeOffset(2023, 12, 1, 12, 0, 0, TimeSpan.Zero); // Winter date
            var fakeTimeProvider = new FakeTimeProvider(fakeUtcNow);

            // Act
            var pacificTime = fakeTimeProvider.GetCurrentPacificTime();

            // Assert
            Assert.Equal(new DateTimeOffset(2023, 12, 1, 4, 0, 0, TimeSpan.FromHours(-8)), pacificTime); // Standard offset
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
    }
}
