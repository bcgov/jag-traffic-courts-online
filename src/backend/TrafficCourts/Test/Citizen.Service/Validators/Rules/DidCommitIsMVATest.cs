using System.Threading.Tasks;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class DidCommitIsMVATest
{

    [Theory]
    [ClassData(typeof(TestData))]
    public async Task TestCheckboxIsValid(string name, bool expectError)
    {
        // Given
        OcrViolationTicket violationTicket = new();
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVA, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsMVAR, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsCCLA, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsCTA, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsLCLA, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsTCSR, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsWLA, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsFVPA, new Field("unselected"));
        violationTicket.Fields.Add(OcrViolationTicket.OffenceIsOther, new Field("unselected"));
        violationTicket.Fields[name].Value = "selected";
        DidCommitIsMVA rule = new(new Field(), violationTicket);

        // When
        await rule.RunAsync();

        // Then.
        if (expectError)
        {
            foreach (string errorMsg in rule.Field.ValidationErrors)
            {
                if (!errorMsg.StartsWith(ValidationMessages.MVAMustBeSelectedError) 
                 && !errorMsg.StartsWith(ValidationMessages.OnlyMVAMustBeSelectedError)) {
                    Assert.Fail("Expected one of 2 validation messages");
                }
            }
        }
        else
        {
            Assert.Empty(rule.Field.ValidationErrors);
        }
    }

    public class TestData : TheoryData<string, bool>
    {
        public TestData()
        {
            Add(OcrViolationTicket.OffenceIsMVA, false);
            Add(OcrViolationTicket.OffenceIsMVAR, false);
            Add(OcrViolationTicket.OffenceIsCCLA, true);
            Add(OcrViolationTicket.OffenceIsCTA, true);
            Add(OcrViolationTicket.OffenceIsLCLA, true);
            Add(OcrViolationTicket.OffenceIsTCSR, true);
            Add(OcrViolationTicket.OffenceIsWLA, true);
            Add(OcrViolationTicket.OffenceIsFVPA, true);
            Add(OcrViolationTicket.OffenceIsOther, true);
        }
    }
}
