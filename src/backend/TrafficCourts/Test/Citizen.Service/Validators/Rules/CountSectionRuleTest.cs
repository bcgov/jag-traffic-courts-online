using Moq;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class CountSectionRuleTest
{
    [Theory]
    [InlineData("MVA 100(1) ai", true)]
    [InlineData("23(1)", false)]
    [InlineData("MVA 100(1) ai ", true)]
    [InlineData(" MVA 100(1) ai", true)]
    [InlineData("224(1)", false)]
    [InlineData("", true)] 
    [InlineData("$", true)] // treat as blank (must have pulled this character from the adjacent Ticket Amount field).
    public async Task TestLookup(string sectionValue, bool expectValid)
    {
        // Given
        var lookupService = new Mock<IStatuteLookupService>();
        Statute expected = new ("19588", "MVA", "100", "1", "a", "i", "MVA 100(1) ai", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        lookupService.Setup(_ => _.GetBySectionAsync("MVA 100(1) ai")).Returns(Task.FromResult((Statute?)expected));

        Field field = new();
        field.TagName = Count1Section;
        field.Value = sectionValue;
        CountSectionRule rule = new(field, lookupService.Object);

        // When
        await rule.RunAsync();

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
