using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class OnlyMVAIsSelectedRuleTest
{

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
    public async Task TestFieldsBlank(string? mva, string? mca, string? cta, string? wla, string? faa, string? lca, string? tcr, string? other, string? expectedPattern)
    {
        // Given
        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVA, new Field(mva));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMCA, new Field(mca));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsCTA, new Field(cta));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsWLA, new Field(wla));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsFAA, new Field(faa));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsLCA, new Field(lca));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsTCR, new Field(tcr));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsOther, new Field(other));
        OnlyMVAIsSelectedRule rule = new(new Field(mva), violationTicket);

        // When
        await rule.RunAsync();

        // Then.
        if (expectedPattern is not null)
        {
            Assert.Single(rule.Field.ValidationErrors);
            foreach (string errorMsg in rule.Field.ValidationErrors)
            {
                Assert.True(Regex.IsMatch(errorMsg, expectedPattern), string.Format("Expected '{0}', Actual '{1}'", expectedPattern, errorMsg));
            }
        }
        else
        {
            Assert.Empty(rule.Field.ValidationErrors);
        }
    }
}
