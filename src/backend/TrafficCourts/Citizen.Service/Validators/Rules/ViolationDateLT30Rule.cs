using System;
using TrafficCourts.Citizen.Service.Models.Tickets;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// In order for a Violation Ticket to be considered submissible, the Violation Date field must represent a valid date and be less than 30 days from today.
/// </summary>
public class ViolationDateLT30Rule : ValidationRule
{

    public ViolationDateLT30Rule(Field field) : base(field)
    {
    }

    public override void Run()
    {
        DateTime? violationDate = Field.GetDate();
        if (violationDate is null)
        {
            AddValidationError(String.Format(ValidationMessages.ViolationDateInvalid, Field.Value));
        }
        else
        {
            // Format the Field Value as recognized by the validator
            Field.Value = violationDate.Value.ToString("yyyy-MM-dd");

            DateTime dateTime = DateTime.Now;
            // remove time portion (which may affect the below calculations)
            DateTime now = new(dateTime.Year, dateTime.Month, dateTime.Day);
            if (violationDate > now)
            {
                // Violation Date is in the future ... we must have mis-read it.  Consider this invalid.
                AddValidationError(String.Format(ValidationMessages.ViolationDateFutureInvalid, Field.Value));
            }
            else if (violationDate < (now.AddDays(-30)))
            {
                AddValidationError(String.Format(ValidationMessages.ViolationDateGT30Days, violationDate.Value.ToString("yyyy-MM-dd")));
            }
        }
    }
}
