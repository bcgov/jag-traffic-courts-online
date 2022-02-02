using TrafficCourts.Citizen.Service.Models.Tickets;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class CheckboxIsValidRule : ValidationRule
{
    public CheckboxIsValidRule(OcrViolationTicket.Field field) : base(field)
    {
    }

    public override void Run()
    {
        if (Field.IsCheckboxSelected() is null) {
            Field.ValidationErrors.Add(String.Format(ValidationMessages.CheckboxInvalid, Field.TagName, Field.Value));
        }
    }
}