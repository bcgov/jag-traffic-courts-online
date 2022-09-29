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
    public string? Salt { get; set; } = "6e0589b9-a753-4182-aa6f-b350a9345bf5";

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
