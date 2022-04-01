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
    [InlineData("24(1)", true)]
    [InlineData("23(1)", false)]
    [InlineData("24(1) ", true)]
    [InlineData(" 24(1)", true)]
    [InlineData("24 (1)", true)]
    [InlineData("224(1)", false)]
    public void TestLookup(string sectionValue, bool expectValid)
    {
        // Given
        var lookupService = new Mock<ILookupService>();
        List<Statute> statutes = new();
        statutes.Add(new Statute((decimal) 18886, "MVA", "24(1)", "drive without licence"));
        lookupService.Setup(_ => _.GetStatutes("24(1)")).Returns(statutes);

        Field field = new();
        field.TagName = Count1Section;
        field.Value = sectionValue;
        CountSectionRule rule = new(field, lookupService.Object);

        // When
        rule.Run();

        // Then
        if (expectValid)
        {
            Assert.Empty(rule.Field.ValidationErrors);
        }
        else
        {
            Assert.Single(rule.Field.ValidationErrors);
            foreach (string errorMsg in rule.Field.ValidationErrors)
            {
                Assert.Equal(string.Format(ValidationMessages.CountSectionInvalid, sectionValue), errorMsg);
            }
        }
    }

}
