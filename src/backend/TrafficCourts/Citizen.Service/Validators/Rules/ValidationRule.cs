namespace TrafficCourts.Citizen.Service.Validators.Rules;

public abstract class ValidationRule
{

    public static readonly string FieldIsBlankError = "Field is blank";

    public List<string> ValidationErrors { get; set; } = new List<string>();

    public bool IsValid() {
        return ValidationErrors.Count == 0;
    }

    public abstract void Run();

}
