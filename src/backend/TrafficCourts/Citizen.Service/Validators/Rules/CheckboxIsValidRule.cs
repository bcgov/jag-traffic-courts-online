using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class CheckboxIsValidRule : ValidationRule
{
    public CheckboxIsValidRule(Field field) : base(field)
    {
    }

    public override Task RunAsync()
    {
        if (Field.IsCheckboxSelected() is null) {
            AddValidationError(String.Format(ValidationMessages.CheckboxInvalid, Field.TagName, Field.Value));
        }

        return Task.CompletedTask;
    }
}
