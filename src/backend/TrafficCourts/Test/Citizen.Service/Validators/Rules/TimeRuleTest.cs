using System;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class TimeRuleTest
{

    [Theory]
    [InlineData("11:59", true, 11, 59)]
    [InlineData("11 59", true, 11, 59)]
    [InlineData("11.59", true, 11, 59)]
    [InlineData("1:59", true, 1, 59)]
    [InlineData("11:05", true, 11, 5)]
    [InlineData("13:05", true, 13, 5)]
    [InlineData(null, false, 0, 0)]
    [InlineData("", false, 0, 0)]
    [InlineData("text", false, 0, 0)]
    public void TestTimeParse(string timeStr, bool expectedValid, int expectedHour, int expectedMinute)
    {
        // Given
        Field timeField = new();
        timeField.Value = timeStr;

        // When
        TimeSpan? time = timeField.GetTime();

        // Then
        if (expectedValid)
        {
            Assert.NotNull(time);
            Assert.Equal(expectedHour, time?.Hours);
            Assert.Equal(expectedMinute, time?.Minutes);
        }
        else
        {
            Assert.Null(time);
        }
    }

}
