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
        if (!String.IsNullOrEmpty(value))
        {
            value = Field.Value = Regex.Replace(value, @"\s+", "");
            IEnumerable<Statute> statutes = _lookupService.GetStatutes(value.ToLower().Trim());
            if (statutes is null || !statutes.Any()) {
                AddValidationError(String.Format(ValidationMessages.CountSectionInvalid, Field.Value));
            }
        }
    }
}
