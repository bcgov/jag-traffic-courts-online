using NodaTime;
using NodaTime.Testing;
using System;
using TrafficCourts.Common;
using Xunit;

namespace TrafficCourts.Test.Common
{
    public class ClockExtensionsTest
    {
        [Fact]
        public void pacific_standard_time_is_minus_8_hours_in_winter_from_DateTimeOffset()
        {
            var expected = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));
            FakeClock clock = new FakeClock(Instant.FromDateTimeOffset(expected));
            DateTimeOffset actual = ClockExtensions.GetCurrentPacificTime(clock);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void pacific_standard_time_is_minus_8_hours_in_winter_from_DateTime()
        {
            var utc = new DateTime(2020, 1, 1, 8, 0, 0, DateTimeKind.Utc); // 8AM UTC
            var expected = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.FromHours(-8));
            FakeClock clock = new FakeClock(Instant.FromDateTimeUtc(utc));
            DateTimeOffset actual = ClockExtensions.GetCurrentPacificTime(clock);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void pacific_daylight_time_is_minus_7_hours_in_summer_from_DateTimeOffset()
        {
            var expected = new DateTimeOffset(2020, 7, 1, 0, 0, 0, TimeSpan.FromHours(-7));
            FakeClock clock = new FakeClock(Instant.FromDateTimeOffset(expected));
            DateTimeOffset actual = ClockExtensions.GetCurrentPacificTime(clock);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void pacific_daylight_time_is_minus_7_hours_in_summer_from_DateTime()
        {
            var utc = new DateTime(2020, 7, 1, 7, 0, 0, DateTimeKind.Utc); // 7AM UTC
            var expected = new DateTimeOffset(2020, 7, 1, 0, 0, 0, TimeSpan.FromHours(-7));
            FakeClock clock = new FakeClock(Instant.FromDateTimeUtc(utc));
            DateTimeOffset actual = ClockExtensions.GetCurrentPacificTime(clock);

            Assert.Equal(expected, actual);
        }
    }
}
