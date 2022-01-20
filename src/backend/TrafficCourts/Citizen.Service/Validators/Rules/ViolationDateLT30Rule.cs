using System;
using TrafficCourts.Citizen.Service.Models.Tickets;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// In order for a Violation Ticket to be considered submissible, the Violation Date field must represent a valid date and be less than 30 days from today.
/// </summary>
public class ViolationDateLT30Rule : ValidationRule
{
    private readonly OcrViolationTicket _violationTicket;

    public ViolationDateLT30Rule(OcrViolationTicket violationTicket)
    {
        this._violationTicket = violationTicket;
    }

    public override void Run()
    {
        DateTime? violationDate = _violationTicket.GetField(OcrViolationTicket.ViolationDate)?.GetDate();
        if (violationDate is null)
        {
            ValidationErrors.Add(ValidationMessages.ViolationDateInvalid);
        }
        else {
            DateTime dateTime = DateTime.Now;
            // remove time portion (which may affect the below calculations)
            DateTime now = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            if (violationDate > now) {
                // Violation Date is in the future ... we must have mis-read it.  Consider this invalid.
                ValidationErrors.Add(ValidationMessages.ViolationDateInvalid);
            }
            else if (violationDate < (now.AddDays(-30))) {
                ValidationErrors.Add(ValidationMessages.ViolationDateGT30Days);
            }
        }
    }
}
