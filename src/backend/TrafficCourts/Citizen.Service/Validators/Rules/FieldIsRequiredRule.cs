using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>Validates a field is not blank.</summary>
public class FieldIsRequiredRule : ValidationRule
{

    /// <summary>Validates a field is not blank.</summary>
    public FieldIsRequiredRule(Field field) : base(field)
    {
    }

    public override Task RunAsync()
    {
        if (Field.Value is null)
        {
            AddValidationError(String.Format(ValidationMessages.FieldIsBlankError, Field.TagName));
        }

        return Task.CompletedTask;
    }
}
