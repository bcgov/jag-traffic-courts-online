using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using static TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class LowConfidenceGlobalRule
{
    private static readonly float _minViableConfidence = 0.01f;

    public static void Run(OcrViolationTicket violationTicket)
    {
        // If any {minViableThreshold} out of the below fields have a low confidence, reject the entire ticket,
        // where {minViableThreshold} is 4, 6, or 8 for 1 Count, 2 Counts, 3 Counts populated respectively.
        int numOfLowConfFields = 0;
        int minViableThreshold = 4;
        numOfLowConfFields += (violationTicket.Fields[Surname].FieldConfidence < _minViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[GivenName].FieldConfidence < _minViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[DriverLicenceNumber].FieldConfidence < _minViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[DriverLicenceProvince].FieldConfidence < _minViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1Description].FieldConfidence < _minViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1ActRegs].FieldConfidence < _minViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1Section].FieldConfidence < _minViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1TicketAmount].FieldConfidence < _minViableConfidence) ? 1 : 0;
        if (violationTicket.IsCount2Populated())
        {
            numOfLowConfFields += (violationTicket.Fields[Count2Description].FieldConfidence < _minViableConfidence) ? 1 : 0;
            numOfLowConfFields += (violationTicket.Fields[Count2ActRegs].FieldConfidence < _minViableConfidence) ? 1 : 0;
            numOfLowConfFields += (violationTicket.Fields[Count2Section].FieldConfidence < _minViableConfidence) ? 1 : 0;
            numOfLowConfFields += (violationTicket.Fields[Count2TicketAmount].FieldConfidence < _minViableConfidence) ? 1 : 0;
            minViableThreshold += 2;
        }
        if (violationTicket.IsCount3Populated())
        {
            numOfLowConfFields += (violationTicket.Fields[Count3Description].FieldConfidence < _minViableConfidence) ? 1 : 0;
            numOfLowConfFields += (violationTicket.Fields[Count3ActRegs].FieldConfidence < _minViableConfidence) ? 1 : 0;
            numOfLowConfFields += (violationTicket.Fields[Count3Section].FieldConfidence < _minViableConfidence) ? 1 : 0;
            numOfLowConfFields += (violationTicket.Fields[Count3TicketAmount].FieldConfidence < _minViableConfidence) ? 1 : 0;
            minViableThreshold += 2;
        }
        if (numOfLowConfFields >= minViableThreshold)
        {
            violationTicket.GlobalValidationErrors.Add(ValidationMessages.LowConfidenceError);
        }
    }
}
