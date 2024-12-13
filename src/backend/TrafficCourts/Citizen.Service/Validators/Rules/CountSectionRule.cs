using System.Text.RegularExpressions;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Citizen.Service.Validators.Rules;

/// <summary>
/// Validates a Count Section field - content should match that that of the a Statute pulled from the LookupService.
/// </summary>
public class CountSectionRule : ValidationRule
{
    private readonly IStatuteLookupService _lookupService;

    public CountSectionRule(Field field, IStatuteLookupService lookupService) : base(field)
    {
        ArgumentNullException.ThrowIfNull(lookupService);
        _lookupService = lookupService;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(Field.Value))
        {
            Field.Value = Field.Value.Trim(); // remove whitespace only from beginning and end
            Field.Value = Regex.Replace(Field.Value, @"^\$$", ""); // remove $ if it's the only character.
            if (!string.IsNullOrEmpty(Field.Value))
            {
                var statute = await _lookupService.GetBySectionAsync(Field.Value, cancellationToken);
                if (statute is null)
                {
                    AddValidationError(string.Format(ValidationMessages.CountSectionInvalid, Field.Value));
                }
            }
        }
    }
}
