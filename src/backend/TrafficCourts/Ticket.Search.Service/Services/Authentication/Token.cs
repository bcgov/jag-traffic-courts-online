using System.Text.Json.Serialization;

namespace TrafficCourts.Ticket.Search.Service.Services.Authentication
{
    public class Token
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// The number of seconds after the token was issued that it expires.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }
}
