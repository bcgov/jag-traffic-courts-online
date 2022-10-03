using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.OpenAPIs.OracleDataAPI;

internal class OracleDataApiConfiguration : IValidatable
{
    public const string Section = "OracleDataApi";

    [Required]
    public string BaseUrl { get; set; } = String.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl)) throw new SettingsValidationException(Section, nameof(BaseUrl), "is required");
        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _)) throw new SettingsValidationException(Section, nameof(BaseUrl), "is not a valid uri");
    }
}
