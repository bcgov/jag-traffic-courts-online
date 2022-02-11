using TrafficCourts.Citizen.Service.Models.Tickets;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class LowConfidenceGlobalRule
{
    private static readonly float MinViableConfidence = 0.8f;

    public static void Run(OcrViolationTicket violationTicket)
    {
        // If any 3 out of the below fields have a low confidence, reject the entire ticket
        int numOfLowConfFields = 0;
        numOfLowConfFields += (violationTicket.Fields[Surname].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[GivenName].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[DriverLicenceNumber].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[DriverLicenceProvince].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1Description].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1ActRegs].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1Section].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count1TicketAmount].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count2Description].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count2ActRegs].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count2Section].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count2TicketAmount].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count3Description].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count3ActRegs].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count3Section].FieldConfidence < MinViableConfidence) ? 1 : 0;
        numOfLowConfFields += (violationTicket.Fields[Count3TicketAmount].FieldConfidence < MinViableConfidence) ? 1 : 0;
        if (numOfLowConfFields >= 3) {
            violationTicket.GlobalValidationErrors.Add(ValidationMessages.LowConfidenceError);
        }
    }
}
