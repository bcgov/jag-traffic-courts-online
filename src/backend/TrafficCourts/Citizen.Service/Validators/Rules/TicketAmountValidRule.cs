using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>Validates a Ticket Amount field - should be a parsable currency field, ie. '$110.00' or simply '120'.</summary>
public class TicketAmountValidRule : ValidationRule
{

    /// <summary>Validates a Ticket Amount field - should be a parsable currency field, ie. '$120.00' or simply '120'.</summary>
    public TicketAmountValidRule(OcrViolationTicket.Field field) : base(field)
    {
    }

    public override void Run()
    {
        if (Field.Value is not null)
        {
            float? currency = Field.GetCurrency();
            if (currency is null)
            {
                AddValidationError(String.Format(ValidationMessages.CurrencyInvalid, Field.Value));
            }
            else
            {
                // Format the Field Value as recognized by the validator
                Field.Value =  String.Format("{0:C}", currency);
            }
        }
    }
}
