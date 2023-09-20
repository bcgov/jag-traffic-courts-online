using System.Text.Json.Serialization;

namespace TrafficCourts.Core.Http.Models;

public class Token
{
    /// <summary>
    /// The access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    /// When the access token expires.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// When the refresh token expires.
    /// </summary>
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    /// <summary>
    /// The refresh token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    /// The access token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    /// <summary>
    /// The session state ID.
    /// </summary>
    [JsonPropertyName("session_state")]
    public Guid SessionState { get; set; }

    /// <summary>
    /// The scope of the token.
    /// </summary>
    public string Scope { get; set; }

}
