namespace TrafficCourts.Common.Authentication;

public class OAuthOptions
{
    /// <summary>
    /// The OAuth endpoint that issues the access tokens.
    /// </summary>
    public Uri AuthorizationUri { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
