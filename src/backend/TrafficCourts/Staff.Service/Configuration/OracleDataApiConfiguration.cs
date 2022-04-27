using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Staff.Service.Configuration;

public class OracleDataApiConfiguration : IValidatable
{
    public const string Section = "OracleDataApi";

    [Required]
    public string? BaseUrl { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(BaseUrl)) throw new SettingsValidationException(Section, nameof(BaseUrl), "is required");
    }
}
