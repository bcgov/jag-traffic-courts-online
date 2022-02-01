using System;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class DriversLicenceValidRuleTest
{

    [Theory]
    [InlineData("BC", "1234567", true, true)]
    [InlineData("BC", "1234567 ", true, false)]
    [InlineData("BC", " 1234567", true, false)]
    [InlineData("BC ", "1234567 ", false, false)]
    [InlineData(" BC", " 1234567", false, false)]
    [InlineData("BC", "12345678", true, false)]
    [InlineData("BC", "123456", true, false)]
    [InlineData("AB", "1234567", false, true)]
    [InlineData(null, "1234567", false, true)]
    [InlineData("BC", null, true, false)]
    public void TestDriversLicenceIsValid(string province, string licenceNumber, bool expectProvValid, bool expectLicValid)
    {
        // Given
        Field provinceField = new(province);
        Field licenceField = new(licenceNumber);
        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.DriverLicenceProvince, provinceField);
        DriversLicenceValidRule rule = new(licenceField, violationTicket);

        // When
        rule.Run();

        // Then.
        if (expectProvValid)
        {
            Assert.Empty(provinceField.ValidationErrors);

            // Testing of the Driver's Licence is only done if the province is first valid
            if (expectLicValid)
            {
                Assert.Empty(rule.Field.ValidationErrors);
            }
            else
            {
                Assert.Single(rule.Field.ValidationErrors);
                Assert.Equal(ValidationMessages.DriversLicenceNumberError, licenceField.ValidationErrors[0]);
            }
        }
        else
        {
            Assert.Single(provinceField.ValidationErrors);
            Assert.Equal(ValidationMessages.DriversLicenceProvinceError, provinceField.ValidationErrors[0]);
        }
    }
}
