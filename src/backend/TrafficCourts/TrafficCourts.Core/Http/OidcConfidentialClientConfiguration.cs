namespace TrafficCourts.Core.Http;

public class OidcConfidentialClientConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public Uri TokenEndpoint { get; set; }
}
