namespace TrafficCourts.Citizen.Service.Validators.Rules;

public abstract class ValidationRule
{

    public List<string> ValidationErrors { get; set; } = new List<string>();

    public bool IsValid() {
        return ValidationErrors.Count == 0;
    }

    public abstract void Run();

}
