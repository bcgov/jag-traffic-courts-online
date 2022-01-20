using System.Text.RegularExpressions;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class FieldMatchesRegexRule : ValidationRule
{
    private readonly Field? _field;
    private readonly string _pattern;
    private readonly string _reason;

    public FieldMatchesRegexRule(Field? field, string pattern, string reason)
    {
        this._field = field;
        this._pattern = pattern;
        this._reason = reason;
    }

    public override void Run()
    {
        if (_field is null)
        {
            ValidationErrors.Add(String.Format(ValidationMessages.FieldIsBlankError, "Field"));
        }
        else if (_field.Value is null)
        {
            ValidationErrors.Add(String.Format(ValidationMessages.FieldIsBlankError, _field.Name));
        }
        else if (!Regex.IsMatch(_field.Value, _pattern))
        {
            ValidationErrors.Add(_reason);
        }
    }
}
