using Moq;
using System.Collections.Generic;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class CountSectionRuleTest
{

    [Theory]
    [InlineData("24(1)", false)]
    [InlineData("23(1)", true)]
    [InlineData("24(1) ", false)]
    [InlineData(" 24(1)", false)]
    [InlineData("24 (1)", false)]
    [InlineData("224(1)", true)]
    public void TestLookup(string sectionValue, bool expectError)
    {
        // Given
        var lookupService = new Mock<ILookupService>();
        List<Statute> statutes = new();
        statutes.Add(new Statute((decimal) 18886, "MVA", "24(1)", "drive without licence"));
        lookupService.Setup(_ => _.GetStatutes()).Returns(statutes);

        Field field = new();
        field.TagName = Count1Section;
        field.Value = sectionValue;
        CountSectionRule rule = new(field, lookupService.Object);

        // When
        rule.Run();

        // Then
        if (expectError)
        {
            Assert.Single(rule.Field.ValidationErrors);
            foreach (string errorMsg in rule.Field.ValidationErrors)
            {
                Assert.Equal(string.Format(ValidationMessages.CountSectionInvalid, sectionValue), errorMsg);
            }
        }
        else {
            Assert.Empty(rule.Field.ValidationErrors);
        }
    }

}
