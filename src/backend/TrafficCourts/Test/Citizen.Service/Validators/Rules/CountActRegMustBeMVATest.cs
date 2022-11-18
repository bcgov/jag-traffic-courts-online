using System.Threading.Tasks;
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
    public async Task TestACTREGsFields(string countAct, bool expectValid)
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
}
