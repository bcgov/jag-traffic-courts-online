using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;
using static TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class LowConfidenceGlobalRuleTest
{
    const float lowConfValue = 0.01f;
    const float noConfValue = 0.00f;

    [Theory]
    [ClassData(typeof(TestData))]
    public void TestLowConfidence(int testNum, float?[] fieldConfidences, bool expectValid)
    {
        // TCVP-932 if 4, 6, or 8 of a set of specific fields have a low (<80%) confidence, expect to see a global validation error
        // Given
        OcrViolationTicket violationTicket = new();
        AddField(violationTicket, ViolationTicketTitle, lowConfValue);
        AddField(violationTicket, ViolationTicketNumber, lowConfValue);
        AddField(violationTicket, ViolationDate, lowConfValue);
        AddField(violationTicket, ViolationTime, lowConfValue);

        AddField(violationTicket, Surname, fieldConfidences[0]);
        AddField(violationTicket, GivenName, fieldConfidences[1]);
        AddField(violationTicket, DriverLicenceProvince, fieldConfidences[2]);
        AddField(violationTicket, DriverLicenceNumber, fieldConfidences[3]);
        AddField(violationTicket, Count1Description, fieldConfidences[4]);
        AddField(violationTicket, Count1ActRegs, fieldConfidences[5]);
        AddField(violationTicket, Count1Section, fieldConfidences[6]);
        AddField(violationTicket, Count1TicketAmount, fieldConfidences[7]);
        AddField(violationTicket, Count2Description, fieldConfidences[8]);
        AddField(violationTicket, Count2ActRegs, fieldConfidences[9]);
        AddField(violationTicket, Count2Section, fieldConfidences[10]);
        AddField(violationTicket, Count2TicketAmount, fieldConfidences[11]);
        AddField(violationTicket, Count3Description, fieldConfidences[12]);
        AddField(violationTicket, Count3ActRegs, fieldConfidences[13]);
        AddField(violationTicket, Count3Section, fieldConfidences[14]);
        AddField(violationTicket, Count3TicketAmount, fieldConfidences[15]);

        // When
        LowConfidenceGlobalRule.Run(violationTicket);

        // Then
        if (expectValid)
        {
            Assert.Empty(violationTicket.GlobalValidationErrors);
        }
        else
        {
            Assert.True(violationTicket.GlobalValidationErrors.Count == 1, "Test number: " + testNum);
            Assert.Equal(ValidationMessages.LowConfidenceError, violationTicket.GlobalValidationErrors[0]);
        }
    }

    private static void AddField(OcrViolationTicket violationTicket, string fieldName, float? fieldConfidence)
    {
        Field field = new();
        field.FieldConfidence = fieldConfidence;
        if (fieldConfidence is not null)
        {
            field.Value = "a";
        }
        violationTicket.Fields.Add(fieldName, field);
    }

    public class TestData : TheoryData<int, float?[], bool>
    {
        public TestData()
        {
            // 1 count, < 4 fields with low confidence
            Add(1, new float?[] { noConfValue, noConfValue, noConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, null, null, null, null, null, null, null, null }, true);
            Add(2, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, null, null, null, null }, true);

            // 1 count, >= 4 fields with low confidence
            Add(3, new float?[] { noConfValue, noConfValue, noConfValue, noConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, null, null, null, null, null, null, null, null }, false);
            Add(4, new float?[] { lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, lowConfValue, lowConfValue, lowConfValue, null, null, null, null, null, null, null, null }, false);
            Add(5, new float?[] { lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, lowConfValue, lowConfValue, null, null, null, null, null, null, null, null }, false);
            Add(6, new float?[] { lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, lowConfValue, null, null, null, null, null, null, null, null }, false);
            Add(7, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, null, null, null, null }, false);

            // 2 counts, < 6 fields with low confidence
            Add(8, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, null, null, null }, true);
            Add(9, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, null, null }, true);
            Add(10, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, noConfValue, null, null, null }, true);
            Add(11, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, noConfValue, noConfValue, null, null }, true);
            // // 2 counts, >= 6 fields with low confidence
            Add(12, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, null }, false);
            Add(13, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null }, false);
            Add(14, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, null, null, null, null, noConfValue, noConfValue, noConfValue, null }, false);
            Add(15, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, null, null, null, null, noConfValue, noConfValue, noConfValue, noConfValue }, false);

            // 3 counts, < 8 fields with low confidence
            Add(16, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, null, null, noConfValue, noConfValue, null, null }, true);
            // 3 counts, >= 8 fields with low confidence
            Add(17, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, noConfValue, null, noConfValue, noConfValue, null, null }, false);

            // 3 counts, all okay
            Add(18, new float?[] { lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue, lowConfValue }, true);
        }
    }
}
