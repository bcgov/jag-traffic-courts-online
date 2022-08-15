using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Authentication;

public class OAuthOptions : IValidatable
{
    public const string Section = "OAuth";

    /// <summary>
    /// The OAuth endpoint that issues the access tokens.
    /// </summary>
    [Required]
    public Uri? TokenUri { get; set; }

    [Required]
    public string? ClientId { get; set; }

    [Required]
    public string? ClientSecret { get; set; }

    public void Validate()
    {
        if (TokenUri is null) throw new SettingsValidationException(Section, nameof(TokenUri), "is required");
        if (ClientId is null) throw new SettingsValidationException(Section, nameof(ClientId), "is required");
        if (ClientSecret is null) throw new SettingsValidationException(Section, nameof(ClientSecret), "is required");
    }
}
