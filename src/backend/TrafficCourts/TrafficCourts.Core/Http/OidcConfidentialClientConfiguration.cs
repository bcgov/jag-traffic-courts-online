namespace TrafficCourts.Core.Http;

public class OidcConfidentialClientConfiguration
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public Uri TokenEndpoint { get; set; } = new Uri("http://localhost");
}
