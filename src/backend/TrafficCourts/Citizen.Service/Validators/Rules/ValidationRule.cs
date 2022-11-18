using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public abstract class ValidationRule
{

    public ValidationRule(Field field)
    {
        Field = field;
    }

    public Field Field { get; }

    public bool IsValid()
    {
        return Field.ValidationErrors.Count == 0;
    }

    public void AddValidationError(string message)
    {
        Field.ValidationErrors.Add(message);
        Field.FieldConfidence = 0;
    }

    public abstract Task RunAsync();

}
