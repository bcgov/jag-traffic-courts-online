using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Citizen.Service.Configuration;

public class HashidsOptions : IValidatable
{
    public const string Section = "Hashids";

    /// <summary>
    /// Salt
    /// </summary>
    [Required]
    public string? Salt { get; set; } = "";

    public void Validate()
    {
        if (string.IsNullOrEmpty(Salt)) throw new SettingsValidationException(Section, nameof(Salt), "is required");
    }

    public static HashidsOptions Get(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new HashidsOptions();
        configuration.GetSection(Section).Bind(options);
        return options;
    }
}
