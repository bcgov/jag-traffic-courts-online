using System.Threading.Tasks;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class CheckboxIsValidRuleTest
{

    [Theory]
    [ClassData(typeof(TestData))]
    public async Task TestCheckboxIsValid(string name, string? value, bool expectError)
    {
        // Given
        Field field = new();
        field.TagName = name;
        field.Value = value;
        CheckboxIsValidRule rule = new(field);

        // When
        await rule.RunAsync();

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
            Add(OcrViolationTicket.OffenceIsMVA, "selected", false);
            Add(OcrViolationTicket.OffenceIsMVA, "unselected", false);
            Add(OcrViolationTicket.OffenceIsMVA, "randomText", true);
            Add(OcrViolationTicket.OffenceIsMVA, null, true);
        }
    }
}
