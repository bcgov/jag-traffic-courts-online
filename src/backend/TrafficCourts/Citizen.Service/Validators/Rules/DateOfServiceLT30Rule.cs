using TrafficCourts.Domain.Models;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// In order for a Violation Ticket to be considered submissible, the Date of Service field must represent a valid date and be less than 30 days from today.
/// </summary>
public class DateOfServiceLT30Rule : ValidationRule
{
    private readonly TimeProvider _timeProvider;

    public DateOfServiceLT30Rule(Field field, TimeProvider timeProvider) : base(field)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public override Task RunAsync(CancellationToken cancellationToken)
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

            DateTime dateTime = _timeProvider.GetLocalNow().DateTime;
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
