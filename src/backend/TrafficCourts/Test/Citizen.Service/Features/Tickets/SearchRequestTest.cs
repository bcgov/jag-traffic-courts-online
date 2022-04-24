using AutoFixture;
using System;
using System.Collections.Generic;
using TrafficCourts.Citizen.Service.Features.Tickets;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Features.Tickets
{
    public class SearchRequestTest
    {
        [Fact]
        public void can_create_request_with_valid_ticket_number_and_time()
        {
            var random = new Random();
            for (char c1 = 'A'; c1 <= 'Z'; c1++)
            {
                for (char c2 = 'A'; c2 <= 'Z'; c2++)
                {
                    var i = random.Next(0, 100_000_000);
                    var ticketNumber = $"{c1}{c2}{i:d8}";

                    var hour = random.Next(0, 24);
                    var minute = random.Next(0, 60);

                    var actual = new Search.Request(ticketNumber, $"{hour:d2}:{minute:d2}");
                    Assert.Equal(ticketNumber, actual.TicketNumber);
                    Assert.Equal(hour, actual.Hour);
                    Assert.Equal(minute, actual.Minute);
                }
            }
        }

        [Theory]
        [InlineData("aa123456", "lowercase letters")]
        [InlineData("Aa123456", "mix case letters")]
        [InlineData("aA123456", "mix case letters")]
        [InlineData("AA1234567", "too few numbers")]
        [InlineData("AA12345a", "extra non-numeric values")]
        public void create_request_requires_ticket_number_that_starts_with_2_uppercase_letters_and_eight_or_more_numbers(string ticketNumber, string useCase)
        {
            Assert.Throws<ArgumentException>("ticketNumber", () => new Search.Request(ticketNumber, "00:00"));
        }



        [Theory]
        [MemberData(nameof(EachTimeOfDay))]
        public void create_request_with_valid_times(TimeOnly expected)
        {
            // lpad to length 2
            string time = expected.ToString(@"HH\:mm");
            var actual = new Search.Request("AA00000000", time);

            Assert.Equal(expected.Hour, actual.Hour);
            Assert.Equal(expected.Minute, actual.Minute);

            // no padding
            time = $"{expected.Hour}:{expected.Minute}";
            actual = new Search.Request("AA00000000", time);

            Assert.Equal(expected.Hour, actual.Hour);
            Assert.Equal(expected.Minute, actual.Minute);
        }

        [Theory]
        [MemberData(nameof(EachTimeOfDay))]
        public void create_request_with_hour_minute_and_second_throws_ArgumentException(TimeOnly expected)
        {
            string time = expected.ToString(@"HH\:mm:ss");
            Assert.Throws<ArgumentException>("time", () => new Search.Request("AA00000000", time));
        }


        [Theory]
        [MemberData(nameof(EachTimeOfDay))]
        public void create_request_with_missing_colon_throws_ArgumentException(TimeOnly time)
        {
            Assert.Throws<ArgumentException>("time", () => new Search.Request("AA00000000", $"{time.Hour:d2}{time.Minute:d2}"));
        }

        [Theory]
        [InlineData(-1, 0, "hour less than zero")]
        [InlineData(24, 0, "hour more than 23")]
        [InlineData(0, -1, "minute less than zero")]
        [InlineData(0, 60, "minute more than 59")]
        public void create_request_with_invalid_time_throws_ArgumentException(int hour, int minute, string useCase)
        {
            Assert.Throws<ArgumentException>("time", () => new Search.Request("AA00000000", $"{hour:d2}:{minute:d2}"));
        }


        public static IEnumerable<object[]> EachTimeOfDay
        {
            get
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    for (int minute = 0; minute < 60; minute++)
                    {
                        yield return new object[] { new TimeOnly(hour, minute) };
                    }
                }
            }
        }

        public static IEnumerable<object[]> SingleDigitTimesOfDay
        {
            get
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    for (int minute = 0; minute < 60; minute++)
                    {
                        if (hour <= 9 && minute <= 9)
                        {
                            yield return new object[] { new TimeOnly(hour, minute) };
                        }
                    }
                }
            }
        }
    }
}
