using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Models.OAuth
{
    public class UserInfo
    {
        /// <summary>
        /// The email address.
        /// </summary>
        [JsonPropertyName("email")]
        [MaxLength(100)]
        public string? EmailAddress { get; set; } = null!;

        /// <summary>
        /// The display name 
        /// </summary>
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; } = null!;

        /// <summary>
        /// The given name 
        /// </summary>
        [JsonPropertyName("given_name")]
        public string? GivenName { get; set; } = null!;

        /// <summary>
        /// The given names
        /// </summary>
        [JsonPropertyName("given_names")]
        public string? GivenNames { get; set; } = null!;

        /// <summary>
        /// The surname or corporate name.
        /// </summary>
        [JsonPropertyName("family_name")]
        public string? Surename { get; set; } = null!;

        /// <summary>
        /// The birthdate.
        /// </summary>
        [JsonPropertyName("birthdate")]
        [SwaggerSchema(Format = "date")]
        public DateTime? Birthdate { get; set; }
    }
}