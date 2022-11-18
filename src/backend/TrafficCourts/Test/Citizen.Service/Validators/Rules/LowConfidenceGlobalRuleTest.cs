using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using Xunit;
using static TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class LowConfidenceGlobalRuleTest
{

    [Theory]
    [ClassData(typeof(TestData))]
    public void TestLowConfidence(int testNum, float?[] fieldConfidences, bool expectValid)
    {
        // TCVP-932 if 4, 6, or 8 of a set of specific fields have a low (<80%) confidence, expect to see a global validation error
        // Given
        OcrViolationTicket violationTicket = new();
        AddField(violationTicket, ViolationTicketTitle, 0.80f);
        AddField(violationTicket, ViolationTicketNumber, 0.80f);
        AddField(violationTicket, ViolationDate, 0.80f);
        AddField(violationTicket, ViolationTime, 0.80f);
        AddField(violationTicket, OffenceIsMVA, 0.80f);
        AddField(violationTicket, OffenceIsMCA, 0.80f);
        AddField(violationTicket, OffenceIsCTA, 0.80f);
        AddField(violationTicket, OffenceIsWLA, 0.80f);
        AddField(violationTicket, OffenceIsFAA, 0.80f);
        AddField(violationTicket, OffenceIsLCA, 0.80f);
        AddField(violationTicket, OffenceIsTCR, 0.80f);
        AddField(violationTicket, OffenceIsOther, 0.80f);

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
            Add(1, new float?[] { 0.79f, 0.79f, 0.79f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, null, null, null, null, null, null, null, null }, true);
            Add(2, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, null, null, null, null, null, null, null, null }, true);

            // 1 count, >= 4 fields with low confidence
            Add(3, new float?[] { 0.79f, 0.79f, 0.79f, 0.79f, 0.80f, 0.80f, 0.80f, 0.80f, null, null, null, null, null, null, null, null }, false);
            Add(4, new float?[] { 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.80f, 0.80f, 0.80f, null, null, null, null, null, null, null, null }, false);
            Add(5, new float?[] { 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.80f, 0.80f, null, null, null, null, null, null, null, null }, false);
            Add(6, new float?[] { 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.80f, null, null, null, null, null, null, null, null }, false);
            Add(7, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, null, null, null, null, null, null, null, null }, false);

            // 2 counts, < 6 fields with low confidence
            Add(8, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, null, null, null, null, null, null, null }, true);
            Add(9, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, null, null, null, null, null, null }, true);
            Add(10, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, null, null, null, null, 0.79f, null, null, null }, true);
            Add(11, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, null, null, null, null, 0.79f, 0.79f, null, null }, true);
            // // 2 counts, >= 6 fields with low confidence
            Add(12, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, null, null, null, null, null }, false);
            Add(13, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, null, null, null, null }, false);
            Add(14, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, null, null, null, null, 0.79f, 0.79f, 0.79f, null }, false);
            Add(15, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, null, null, null, null, 0.79f, 0.79f, 0.79f, 0.79f }, false);

            // 3 counts, < 8 fields with low confidence
            Add(16, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, null, null, 0.79f, 0.79f, null, null }, true);
            // 3 counts, >= 8 fields with low confidence
            Add(17, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, 0.79f, null, 0.79f, 0.79f, null, null }, false);

            // 3 counts, all okay
            Add(18, new float?[] { 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f, 0.80f }, true);
        }
    }
}
