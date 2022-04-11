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
        if (!String.IsNullOrEmpty(Field.Value))
        {
            Field.Value = Regex.Replace(Field.Value, @"\s+", ""); // remove whitespace
            Field.Value = Regex.Replace(Field.Value, @"^\$$", ""); // remove $ if it's the only character.
            if (!String.IsNullOrEmpty(Field.Value))
            {
                IEnumerable<Statute> statutes = _lookupService.GetStatutes(Field.Value);
                if (!statutes.Any())
                {
                    AddValidationError(String.Format(ValidationMessages.CountSectionInvalid, Field.Value));
                }
            }
        }
    }
}
