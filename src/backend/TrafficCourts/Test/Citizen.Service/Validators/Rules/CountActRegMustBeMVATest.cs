using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class CountActRegMustBeMVATest
{

    [Theory]
    [InlineData("MVA", true)]
    [InlineData("CTA", false)]
    [InlineData("", false)]
    public async Task TestACTREGsFields1(string countAct, bool expectValid)
    {
        // Given
        Field field = new(countAct);
        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.Count1ActRegs, field);
        CountActRegMustBeMVA rule = new(field, 1);

        // When
        await rule.RunAsync();

        // Then.
        if (expectValid)
        {
            Assert.Empty(rule.Field.ValidationErrors);
        }
        else
        {
            Assert.NotEmpty(rule.Field.ValidationErrors);
        }

    }

    [Theory]
    [InlineData("MVA", "45(a)", true)]
    [InlineData("", "MVA 45(a)", true)]
    [InlineData(null, "MVA 45(a)", true)]
    public async Task TestACTREGsFields2(string? actReg, string? section, bool expectValid)
    {
        // Given
        Field actRegField = new(actReg);
        Field sectionField = new(section);

        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.Count1ActRegs, actRegField);
        violationTicket.Fields.Add(OcrViolationTicket.Count1Section, sectionField);
        CountActRegMustBeMVA rule = new(actRegField, 1);

        // When
        FormRecognizerValidator.Sanitize(violationTicket);
        await rule.RunAsync();

        // Then.
        if (expectValid)
        {
            Assert.Empty(rule.Field.ValidationErrors);
        }
        else
        {
            Assert.NotEmpty(rule.Field.ValidationErrors);
        }

    }
}
