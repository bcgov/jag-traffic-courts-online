using System;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class CheckboxIsValidRuleTest
{

    [Theory]
    [ClassData(typeof(TestData))]
    public void TestCheckboxIsValid(string name, string? value, bool expectError)
    {
        // Given
        Field field = new();
        field.TagName = name;
        field.Value = value;
        CheckboxIsValidRule rule = new(field);

        // When
        rule.Run();

        // Then.
        if (expectError)
        {
            Assert.Single(rule.Field.ValidationErrors);
            foreach (string errorMsg in rule.Field.ValidationErrors)
            {
                Assert.Equal(string.Format(ValidationMessages.CheckboxInvalid, name, value), errorMsg);
            }
        }
        else
        {
            Assert.Empty(rule.Field.ValidationErrors);
        }
    }

    public class TestData : TheoryData<string, string?, bool>
    {
        public TestData()
        {
            Add(OcrViolationTicket.OffenseIsMVA, "selected", false);
            Add(OcrViolationTicket.OffenseIsMVA, "unselected", false);
            Add(OcrViolationTicket.OffenseIsMVA, "randomText", true);
            Add(OcrViolationTicket.OffenseIsMVA, null, true);
        }
    }
}
