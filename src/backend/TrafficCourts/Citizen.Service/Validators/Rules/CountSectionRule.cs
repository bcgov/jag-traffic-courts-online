using System.Text.RegularExpressions;
using System;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Citizen.Service.Services;
using static TrafficCourts.Citizen.Service.Models.Tickets.OcrViolationTicket;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// Validates a Count Section field - content should match that that of the a Statute pulled from the LookupService.
/// </summary>
public class CountSectionRule : ValidationRule
{

    private readonly ILookupService _lookupService;

    public CountSectionRule(Field field, Services.ILookupService lookupService) : base(field)
    {
        ArgumentNullException.ThrowIfNull(lookupService);
        _lookupService = lookupService;
    }

    public override void Run()
    {
        string? value = Field.Value;
        if (value is not null)
        {
            value = Field.Value = Regex.Replace(value, @"\s+", "");
            IEnumerable<Statute> statutes = _lookupService.GetStatutes();
            Statute? match = statutes
                .Where(s => s.Section is not null && s.Section.ToLower().Trim().Equals(value.ToLower().Trim()))
                .Select(a => a)
                .FirstOrDefault();
            if (match is null) {
                AddValidationError(String.Format(ValidationMessages.CountSectionInvalid, Field.Value));
            }
        }
    }
}
