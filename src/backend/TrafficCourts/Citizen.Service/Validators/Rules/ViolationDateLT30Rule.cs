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
        Field? field = _violationTicket.GetField(OcrViolationTicket.ViolationDate);
        DateTime? violationDate = field?.GetDate();
        if (violationDate is null)
        {
            ValidationErrors.Add(String.Format(ValidationMessages.ViolationDateInvalid, field?.Value));
        }
        else {
            DateTime dateTime = DateTime.Now;
            // remove time portion (which may affect the below calculations)
            DateTime now = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            if (violationDate > now) {
                // Violation Date is in the future ... we must have mis-read it.  Consider this invalid.
            ValidationErrors.Add(String.Format(ValidationMessages.ViolationDateInvalid, field?.Value));
            }
            else if (violationDate < (now.AddDays(-30))) {
                ValidationErrors.Add(String.Format(ValidationMessages.ViolationDateGT30Days, violationDate.Value.ToString("yyyy-MM-dd")));
            }
        }
    }
}
