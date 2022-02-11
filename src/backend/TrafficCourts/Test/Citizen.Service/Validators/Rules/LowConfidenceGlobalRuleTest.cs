using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Validators;
using TrafficCourts.Citizen.Service.Validators.Rules;
using Xunit;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Test.Citizen.Service.Validators.Rules;

public class LowConfidenceGlobalRuleTest
{

    [Theory]
    [InlineData(0,  new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(1,  new float[] {0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(2,  new float[] {0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, true)]
    [InlineData(3,  new float[] {0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, true)]
    [InlineData(4,  new float[] {0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(5,  new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(6,  new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(7,  new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(8,  new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(9,  new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(10, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(11, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(12, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(13, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(14, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(15, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(16, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(17, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(18, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(19, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(20, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, true)]
    [InlineData(21, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, true)]
    [InlineData(22, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(23, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(24, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(25, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(26, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, true)]
    [InlineData(27, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f}, true)]
    [InlineData(28, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(29, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f,0.80f}, false)]
    [InlineData(30, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f,0.80f}, false)]
    [InlineData(31, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f,0.80f}, false)]
    [InlineData(32, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f,0.80f}, false)]
    [InlineData(33, new float[] {0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.80f,0.79f,0.79f,0.79f}, false)]
    public void TestLowConfidence(int testNum, float[] fieldConfidences, bool expectError)
    {
        // TCVP-932 if 3 of a set of specific fields have a low (<80%) confidence, expect to see a global validation error
        // Given
        OcrViolationTicket violationTicket = new();
        AddField(violationTicket, ViolationTicketTitle, fieldConfidences[0]);
        AddField(violationTicket, ViolationTicketNumber, fieldConfidences[1]);
        AddField(violationTicket, Surname, fieldConfidences[2]);
        AddField(violationTicket, GivenName, fieldConfidences[3]);
        AddField(violationTicket, DriverLicenceProvince, fieldConfidences[4]);
        AddField(violationTicket, DriverLicenceNumber, fieldConfidences[5]);
        AddField(violationTicket, ViolationDate, fieldConfidences[6]);
        AddField(violationTicket, ViolationTime, fieldConfidences[7]);
        AddField(violationTicket, OffenseIsMVA, fieldConfidences[8]);
        AddField(violationTicket, OffenseIsMCA, fieldConfidences[9]);
        AddField(violationTicket, OffenseIsCTA, fieldConfidences[10]);
        AddField(violationTicket, OffenseIsWLA, fieldConfidences[11]);
        AddField(violationTicket, OffenseIsFAA, fieldConfidences[12]);
        AddField(violationTicket, OffenseIsLCA, fieldConfidences[13]);
        AddField(violationTicket, OffenseIsTCR, fieldConfidences[14]);
        AddField(violationTicket, OffenseIsOther, fieldConfidences[15]);
        AddField(violationTicket, Count1Description, fieldConfidences[16]);
        AddField(violationTicket, Count1ActRegs, fieldConfidences[17]);
        AddField(violationTicket, Count1IsACT, fieldConfidences[18]);
        AddField(violationTicket, Count1IsREGS, fieldConfidences[19]);
        AddField(violationTicket, Count1Section, fieldConfidences[20]);
        AddField(violationTicket, Count1TicketAmount, fieldConfidences[21]);
        AddField(violationTicket, Count2Description, fieldConfidences[22]);
        AddField(violationTicket, Count2ActRegs, fieldConfidences[23]);
        AddField(violationTicket, Count2IsACT, fieldConfidences[24]);
        AddField(violationTicket, Count2IsREGS, fieldConfidences[25]);
        AddField(violationTicket, Count2Section, fieldConfidences[26]);
        AddField(violationTicket, Count2TicketAmount, fieldConfidences[27]);
        AddField(violationTicket, Count3Description, fieldConfidences[28]);
        AddField(violationTicket, Count3ActRegs, fieldConfidences[29]);
        AddField(violationTicket, Count3IsACT, fieldConfidences[30]);
        AddField(violationTicket, Count3IsREGS, fieldConfidences[31]);
        AddField(violationTicket, Count3Section, fieldConfidences[32]);
        AddField(violationTicket, Count3TicketAmount, fieldConfidences[33]);
        AddField(violationTicket, HearingLocation, fieldConfidences[34]);
        AddField(violationTicket, DetachmentLocation, fieldConfidences[35]);

        // When
        LowConfidenceGlobalRule.Run(violationTicket);

        // Then
        if (expectError)
        {
            Assert.True(violationTicket.GlobalValidationErrors.Count == 1, "Test number: " + testNum);
            Assert.Equal(ValidationMessages.LowConfidenceError, violationTicket.GlobalValidationErrors[0]);
        }
        else
        {
            Assert.Empty(violationTicket.GlobalValidationErrors);
        }
    }

    private static void AddField(OcrViolationTicket violationTicket, string fieldName, float fieldConfidence)
    {
        Field field = new();
        field.FieldConfidence = fieldConfidence;
        violationTicket.Fields.Add(fieldName, field);
    }
}
