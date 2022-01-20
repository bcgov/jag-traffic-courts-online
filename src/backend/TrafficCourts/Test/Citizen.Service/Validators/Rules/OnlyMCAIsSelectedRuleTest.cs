using System;
using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class OnlyMCAIsSelectedRuleTest
{
    [Fact]
    public void TestFieldsMissing()
    {
        // Given
        OcrViolationTicket violationTicket = new OcrViolationTicket();
        OnlyMCAIsSelectedRule rule = new OnlyMCAIsSelectedRule(violationTicket);

        // When
        rule.Run();

        // Then. There should be 8 errors, all of the form "Unknown ... selection."
        Assert.Equal(8, rule.ValidationErrors.Count);
        foreach (string errorMsg in rule.ValidationErrors)
        {
            Assert.Matches(@"^Unknown \w+ selection.*", errorMsg);
        }
    }

    [Theory]
    [InlineData("selected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", null)]
    [InlineData("unselected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", @"^MVA must be selected.*")]
    [InlineData("selected", "selected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", @"^MVA must be the only selected.*")]
    [InlineData("selected", "unselected", "selected", "unselected", "unselected", "unselected", "unselected", "unselected", @"^MVA must be the only selected.*")]
    [InlineData("selected", "unselected", "unselected", "selected", "unselected", "unselected", "unselected", "unselected", @"^MVA must be the only selected.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", "selected", "unselected", "unselected", "unselected", @"^MVA must be the only selected.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", "unselected", "selected", "unselected", "unselected", @"^MVA must be the only selected.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", "unselected", "unselected", "selected", "unselected", @"^MVA must be the only selected.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", "selected", @"^MVA must be the only selected.*")]
    [InlineData(null, "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", @"^Unknown \w+ selection.*")]
    [InlineData("selected", null, "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", @"^Unknown \w+ selection.*")]
    [InlineData("selected", "unselected", null, "unselected", "unselected", "unselected", "unselected", "unselected", @"^Unknown \w+ selection.*")]
    [InlineData("selected", "unselected", "unselected", null, "unselected", "unselected", "unselected", "unselected", @"^Unknown \w+ selection.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", null, "unselected", "unselected", "unselected", @"^Unknown \w+ selection.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", "unselected", null, "unselected", "unselected", @"^Unknown \w+ selection.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", "unselected", "unselected", null, "unselected", @"^Unknown \w+ selection.*")]
    [InlineData("selected", "unselected", "unselected", "unselected", "unselected", "unselected", "unselected", null, @"^Unknown \w+ selection.*")]
    public void TestFieldsBlank(string? mva, string? mca, string? cta, string? wla, string? faa, string? lca, string? tcr, string? other, string? expectedPattern)
    {
        // Given
        OcrViolationTicket violationTicket = new OcrViolationTicket();
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsMVA, new Field(mva));
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsMCA, new Field(mca));
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsCTA, new Field(cta));
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsWLA, new Field(wla));
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsFAA, new Field(faa));
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsLCA, new Field(lca));
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsTCR, new Field(tcr));
        violationTicket.Fields.Add(OcrViolationTicket.OffenseIsOther, new Field(other));
        OnlyMCAIsSelectedRule rule = new OnlyMCAIsSelectedRule(violationTicket);

        // When
        rule.Run();

        // Then.
        if (expectedPattern is not null)
        {
            Assert.Single(rule.ValidationErrors);
            foreach (string errorMsg in rule.ValidationErrors)
            {
                Assert.True(Regex.IsMatch(errorMsg, expectedPattern), String.Format("Expected '{0}', Actual '{1}'", expectedPattern, errorMsg));
            }
        }
        else
        {
            Assert.Empty(rule.ValidationErrors);
        }
    }
}
