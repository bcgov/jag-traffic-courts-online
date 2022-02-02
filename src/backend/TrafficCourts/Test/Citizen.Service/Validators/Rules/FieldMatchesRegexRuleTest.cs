using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class FieldMatchesRegexRuleTest
{
    [Theory]
    [InlineData("AAAA", @"^AAAA$", 0)]
    [InlineData("", @"^AAAA$", 1)]
    [InlineData("AAAA", @"^BBBB$", 1)]
    [InlineData("VIOLATION TICKET", @"^VIOLATION TICKET$", 0)]
    [InlineData("VIOLATION TICKET", @"^Violation Ticket$", 1)]
    [InlineData("VVIOLATION TICKET", @"^VIOLATION TICKET$", 1)]
    [InlineData(" VIOLATION TICKET", @"^VIOLATION TICKET$", 1)]
    [InlineData("VIOLATION TICKET ", @"^VIOLATION TICKET$", 1)]
    [InlineData("AA12345678", @"^A[A-Z]\d{8}$", 0)]
    [InlineData("AAA2345678", @"^A[A-Z]\d{8}$", 1)]
    [InlineData("AA123456789", @"^A[A-Z]\d{8}$", 1)]
    [InlineData("BA12345678", @"^A[A-Z]\d{8}$", 1)]
    [InlineData("B123456789", @"^A[A-Z]\d{8}$", 1)]
    public void TestRegexPattern(string value, string pattern, int expectedErrorCount)
    {
        // Given
        Field field = new();
        field.Value = value;
        FieldMatchesRegexRule rule = new(field, pattern, "ERROR: Pattern mismatch ...");

        // When
        rule.Run();

        // Then
        Assert.Equal(expectedErrorCount == 0, rule.IsValid());
        Assert.Equal(expectedErrorCount, rule.Field.ValidationErrors.Count);
        if (expectedErrorCount > 0 && value is not null)
        {
            Assert.StartsWith("ERROR", rule.Field.ValidationErrors[0]);
        }
    }

    [Fact]
    public void TestFieldValueNull()
    {
        // Given
        Field field = new();
        field.JsonName = "Field";
        field.Value = null;
        FieldMatchesRegexRule rule = new(field, "AAAA", "Field is blank");

        // When
        rule.Run();

        // Then
        Assert.False(rule.IsValid());
        Assert.Single(rule.Field.ValidationErrors);
        Assert.StartsWith("Field is blank", rule.Field.ValidationErrors[0]);
    }
}
