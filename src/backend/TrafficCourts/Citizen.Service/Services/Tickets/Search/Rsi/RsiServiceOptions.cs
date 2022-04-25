using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi;

public class RsiServiceOptions : IValidatable
{
    /// <summary>
    /// The url of the server that will authenicate the credentials.
    /// </summary>
    public Uri AuthenticationUrl { get; set; } = null!;

    /// <summary>
    /// The url of the resource to access.
    /// </summary>
    public Uri ResourceUrl { get; set; } = null!;

    /// <summary>
    /// The client id.
    /// </summary>
    public string ClientId { get; set; } = null!;

    /// <summary>
    /// The client secret.
    /// </summary>
    public string ClientSecret { get; set; } = null!;

    public void Validate()
    {
        if (ResourceUrl is null) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(RsiServiceOptions.ResourceUrl), "is required");
        if (AuthenticationUrl is null) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(RsiServiceOptions.AuthenticationUrl), "is required");
        if (string.IsNullOrWhiteSpace(ClientId)) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(RsiServiceOptions.ClientId), "is required");
        if (string.IsNullOrWhiteSpace(ClientSecret)) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(RsiServiceOptions.ClientSecret), "is required");
    }
}
