using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Authentication;

public class KeycloakOptions : IValidatable
{
    public const string Section = "KeycloakAdminApi";

    /// <summary>
    /// The base endpoint of the Keycloak Admin API.
    /// </summary>
    [Required]
    public Uri? BaseUri { get; set; }

    [Required]
    public string? Realm { get; set; }

    public void Validate()
    {
        if (BaseUri is null) throw new SettingsValidationException(Section, nameof(BaseUri), "is required");
        if (Realm is null) throw new SettingsValidationException(Section, nameof(Realm), "is required");
    }
}
