using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// In order for a Violation Ticket to be considered submissible, the Date of Service field must represent a valid date and be less than 30 days from today.
/// </summary>
public class DateOfServiceLT30Rule : ValidationRule
{

    public DateOfServiceLT30Rule(Field field) : base(field)
    {
    }

    public override Task RunAsync()
    {
        DateTime? dateOfService = Field.GetDate();
        if (dateOfService is null)
        {
            AddValidationError(string.Format(ValidationMessages.RequiredFieldError, Field.TagName));
        }
        else
        {
            // Format the Field Value as recognized by the validator
            Field.Value = dateOfService.Value.ToString("yyyy-MM-dd");

            DateTime dateTime = DateTime.Now;
            // remove time portion (which may affect the below calculations)
            DateTime now = new(dateTime.Year, dateTime.Month, dateTime.Day);
            if (dateOfService > now)
            {
                // TCVP-1676 Ticket is now permitted to be future dated
                //AddValidationError(String.Format(ValidationMessages.DateFutureInvalid, Field.TagName, Field.Value));
            }
            else if (dateOfService < (now.AddDays(-30)))
            {
                AddValidationError(string.Format(ValidationMessages.DateOfServiceGT30Days, dateOfService.Value.ToString("yyyy-MM-dd")));
            }
        }

        return Task.CompletedTask;
    }
}
