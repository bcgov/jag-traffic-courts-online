using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class CountActRegMustBeMVATest
{

    [Theory]
    [InlineData("MVA", true)]
    [InlineData("CTA", false)]
    [InlineData("", false)]
    public void TestACTREGsFields(string countAct, bool expectValid)
    {
        // Given
        Field field = new(countAct);
        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.Count1ActRegs, field);
        CountActRegMustBeMVA rule = new(field, 1);

        // When
        rule.Run();

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
