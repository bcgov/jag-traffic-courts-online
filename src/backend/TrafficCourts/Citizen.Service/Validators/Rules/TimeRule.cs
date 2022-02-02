using System;
using TrafficCourts.Citizen.Service.Models.Tickets;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// Validates a Field that represents a time.
/// </summary>
public class TimeRule : ValidationRule
{

    public TimeRule(Field field) : base(field)
    {
    }

    public override void Run()
    {
        if (Field.Value is null)
        {
            AddValidationError(String.Format(ValidationMessages.FieldIsBlankError, Field.TagName));
        }
        else
        {
            TimeSpan? time = Field.GetTime();
            if (time is null)
            {
                AddValidationError(String.Format(ValidationMessages.TimeInvalid, Field.Value));
            }
        }
    }
}
