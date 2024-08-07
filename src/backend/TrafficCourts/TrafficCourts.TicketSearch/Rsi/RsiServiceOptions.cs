using TrafficCourts.Configuration.Validation;

namespace TrafficCourts.TicketSearch.Rsi;

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
        if (ResourceUrl is null) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(ResourceUrl), "is required");
        if (AuthenticationUrl is null) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(AuthenticationUrl), "is required");
        if (string.IsNullOrWhiteSpace(ClientId)) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(ClientId), "is required");
        if (string.IsNullOrWhiteSpace(ClientSecret)) throw new SettingsValidationException(nameof(RsiServiceOptions), nameof(ClientSecret), "is required");
    }
}
