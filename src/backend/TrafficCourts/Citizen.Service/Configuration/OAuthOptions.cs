using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Citizen.Service.Configuration;

public class OAuthOptions : IValidatable
{
    public const string Section = "OAuth";

    /// <summary>
    /// User Information Endpoint
    /// </summary>
    [Required]
    public Uri? UserInfoEndpoint { get; set; }

    public void Validate()
    {
        if (UserInfoEndpoint is null) throw new SettingsValidationException(Section, nameof(UserInfoEndpoint), "is required");
    }

    public static OAuthOptions Get(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new OAuthOptions();
        configuration.GetSection(Section).Bind(options);
        return options;
    }
}
