using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Domain.Models;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class VersionVT1DisallowedRuleTest
{

    [Theory]
    [InlineData(ViolationTicketVersion.VT1, true)]
    [InlineData(ViolationTicketVersion.VT2, false)]
    public async Task TestViolationTicketVersion(ViolationTicketVersion version, bool expectError)
    {
        // Given
        OcrViolationTicket violationTicket = new()
        {
            TicketVersion = version
        };
        VersionVT1DisallowedRule rule = new(new Field(), violationTicket);

        // When
        await rule.RunAsync();

        // Then
        Assert.True(violationTicket.GlobalValidationErrors.Count == (expectError ? 1 : 0));
    }
}
