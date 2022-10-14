using Microsoft.Extensions.Configuration;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Configuration;

public class HashidsOptions : IValidatable
{
    public const string Section = "Hashids";

    /// <summary>
    /// Salt
    /// </summary>
    public string? Salt { get; set; } = "";

    public void Validate()
    {
        if (string.IsNullOrEmpty(Salt)) throw new SettingsValidationException(Section, nameof(Salt), "is required");
        if (Salt.Length < 32) throw new SettingsValidationException(Section, nameof(Salt), "should be at least 32 characters long");
    }

    public static HashidsOptions Get(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new HashidsOptions();
        configuration.GetSection(Section).Bind(options);
        return options;
    }
}
