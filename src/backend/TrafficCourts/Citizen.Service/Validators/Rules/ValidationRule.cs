using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

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
    }

    public abstract void Run();

}
