using System.Text.RegularExpressions;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

public class FieldMatchesRegexRule : ValidationRule
{
    private readonly string _pattern;
    private readonly string _reason;

    public FieldMatchesRegexRule(Field field, string pattern, string reason) : base(field)
    {
        this._pattern = pattern;
        this._reason = reason;
    }

    public override void Run()
    {
        if (Field.Value is null)
        {
            Field.ValidationErrors.Add(String.Format(ValidationMessages.FieldIsBlankError, Field.JsonName));
        }
        else if (!Regex.IsMatch(Field.Value, _pattern))
        {
            Field.ValidationErrors.Add(_reason);
        }
    }
}
