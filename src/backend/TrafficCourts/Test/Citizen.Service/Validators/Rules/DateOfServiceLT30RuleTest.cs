using Moq;
using System;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class DateOfServiceLT30RuleTest
{

    [Theory]
    [InlineData("2021 11 02", true, 2021, 11, 2)]
    [InlineData("2021 11 30", true, 2021, 11, 30)]
    [InlineData("2021 1 2", true, 2021, 1, 2)]
    [InlineData("2021-11-02", true, 2021, 11, 2)]
    [InlineData("2021.11,2", true, 2021, 11, 2)]
    [InlineData("21.11,2", true, 2021, 11, 2)]
    [InlineData("20210102", true, 2021, 1, 2)]
    [InlineData("202101 02", true, 2021, 1, 2)]
    [InlineData("2021 0102", true, 2021, 1, 2)]
    [InlineData("2021 11 31", false, 0, 0, 0)] // November has 30 days
    [InlineData("2021 13 1", false, 0, 0, 0)] // Only 12 months
    [InlineData("1 13 1", false, 0, 0, 0)] // Invalid year
    [InlineData("201 13 1", false, 0, 0, 0)] // Invalid year
    public void TestDateParse(string dateStr, bool expectedValid, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Given
        Field dateField = new();
        dateField.Value = dateStr;

        // When
        DateTime? date = dateField.GetDate();

        // Then
        if (expectedValid)
        {
            Assert.NotNull(date);
            Assert.Equal(expectedYear, date?.Year);
            Assert.Equal(expectedMonth, date?.Month);
            Assert.Equal(expectedDay, date?.Day);
        }
        else
        {
            Assert.Null(date);
        }
    }

    [Theory]
    [InlineData("2021 11 02", "2021 11 03", "2021-11-02", "2021-11-03")]
    [InlineData("2021 02", "2021 11 03", "2021-11-03", "2021-11-03")] // Violation Date is not a valid date => null
    [InlineData("2021 11 02", "2021 03", "2021-11-02", "2021-11-02")] // Date of Service is not a valid date => null
    public void TestSanitizeDates(string violationDateStr, string dateOfServiceStr, string expectedVDStr, string expectedDoStr) {
        // ViolationDate and DateOfService are usually the same day. If either is misread/null, use the other date value.

        // Given
        OcrViolationTicket violationTicket = new();
        violationTicket.TicketVersion = ViolationTicketVersion.VT2;
        violationTicket.Fields.Add(OcrViolationTicket.ViolationDate, new Field(violationDateStr));
        violationTicket.Fields.Add(OcrViolationTicket.DateOfService, new Field(dateOfServiceStr));
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        FormRecognizerValidator formRecognizerValidator = new FormRecognizerValidator(_statuteLookupService.Object);

        // When
        formRecognizerValidator.Sanitize(violationTicket);

        // Then
        Assert.Equal(violationTicket.Fields[OcrViolationTicket.ViolationDate].GetDate()?.ToString("yyyy-MM-dd"), expectedVDStr);
        Assert.Equal(violationTicket.Fields[OcrViolationTicket.DateOfService].GetDate()?.ToString("yyyy-MM-dd"), expectedDoStr);
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public async Task TestDateLT30Rule(string dateStr, bool isValid)
    {
        // Given
        OcrViolationTicket violationTicket = new();
        Field dateOfService = new();
        dateOfService.Value = dateStr;
        violationTicket.Fields.Add(OcrViolationTicket.DateOfService, dateOfService);
        DateOfServiceLT30Rule rule = new(dateOfService);

        // When
        await rule.RunAsync();

        // Then
        Assert.Equal(isValid, rule.IsValid());
    }

    public class TestData : TheoryData<string, bool>
    {
        public TestData()
        {
            DateTime dateTime = DateTime.Now;
            // remove time portion (which may affect the below calculations)
            DateTime now = new(dateTime.Year, dateTime.Month, dateTime.Day);
            for (int i = -32; i < 1; i++)
            {
                // Valid date values are from 30 days ago to today. 
                // Future DateOfService dates are not permitted (TCVP-1676)
                string dateStr = now.AddDays(i).ToString("yyyy MM dd");
                bool isValid = i >= -30 && i <= 0;
                Add(dateStr, isValid);
            }
        }
    }

}
