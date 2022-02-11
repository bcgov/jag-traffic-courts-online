using System;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class TicketAmountValidRuleTest
{

    [Theory]
    [InlineData("$120.00", 120.0f)]
    [InlineData("$120.53", 120.53f)]
    [InlineData("120.53", 120.53f)]
    [InlineData("120.53.123", null)]
    [InlineData("", null)]
    [InlineData("abc", null)]
    public void TestCurrencyIsValid(string value, float? expectedCurrency)
    {
        // Given
        Field field = new();
        field.TagName = "Field Label";
        field.Value = value;
        TicketAmountValidRule rule = new(field);

        // When
        float? actualCurrency = field.GetCurrency();
        rule.Run();

        // Then.
        if (expectedCurrency is null)
        {
            Assert.Null(actualCurrency);
            Assert.False(rule.IsValid());
            Assert.Single(rule.Field.ValidationErrors);
            if (value is null)
            {
                foreach (string errorMsg in rule.Field.ValidationErrors)
                {
                    Assert.Equal(String.Format(ValidationMessages.FieldIsBlankError, field.TagName), errorMsg);
                }
            }
            else
            {
                foreach (string errorMsg in rule.Field.ValidationErrors)
                {
                    Assert.Equal(String.Format(ValidationMessages.CurrencyInvalid, value), errorMsg);
                }
            }
        }
        else
        {
            Assert.NotNull(actualCurrency);
            Assert.Equal(expectedCurrency, actualCurrency);
            Assert.True(rule.IsValid());
            Assert.Empty(rule.Field.ValidationErrors);
        }
    }
}
