using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

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
    [InlineData(null, "1234567", true, true)]
    [InlineData("BC", null, true, true)]
    public async Task TestDriversLicenceIsValid(string province, string licenceNumber, bool expectProvValid, bool expectLicValid)
    {
        // Given
        Field provinceField = new(province);
        Field licenceField = new(licenceNumber);
        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.DriverLicenceProvince, provinceField);
        DriversLicenceValidRule rule = new(licenceField, violationTicket);

        // When
        await rule.RunAsync();

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
        else if (province is not null)
        {
            Assert.Single(provinceField.ValidationErrors); 
            Assert.Equal(ValidationMessages.DriversLicenceProvinceError, provinceField.ValidationErrors[0]); 
        } 
    }
}
