using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TrafficCourts.Common.Authorization;

/// <summary>
/// Defines options for Keycloak authorization
/// </summary>
public class KeycloakAuthorizationOptions
{
    /// <summary>
    /// Gets or sets the required aithentication scheme that holds the token.
    /// </summary>
    /// <value>
    /// The required scheme.
    /// </value>
    public string RequiredScheme { get; set; } = JwtBearerDefaults.AuthenticationScheme;

    public string TokenName { get; set; } = "access_token";

    /// <summary>
    /// Gets or sets the token endpoint.
    /// </summary>
    /// <value>
    /// The token endpoint.
    /// </value>
    public string TokenEndpoint { get; set; } = String.Empty;

    /// <summary>
    /// Gets or sets the audience.
    /// </summary>
    /// <value>
    /// The audience.
    /// </value>
    public string Audience { get; set; } = String.Empty;
}
