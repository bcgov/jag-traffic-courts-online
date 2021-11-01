using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Ticket.Search.Service.Configuration
{
    public class TicketSearchServiceConfiguration : TrafficCourtsConfiguration
    {
        public ServiceCredentials Credentials { get; set; }
    }

    public class TicketSearchServiceConfigurationProvider : TrafficCourtsConfigurationProvider
    {
        public override void Load()
        {
            // legacy configuration
            Add("OAUTH__OAUTHURL", "Credentials:AuthenticationUrl");
            Add("OAUTH__RESOURCEURL", "Credentials:ResourceUrl");
            Add("OAUTH__CLIENTID", "Credentials:ClientId");
            Add("OAUTH__SECRET", "Credentials:ClientSecret");

            // new configuration, change this variables?
            Add("OAUTH_OAUTHURL", "Credentials:AuthenticationUrl");
            Add("OAUTH_RESOURCEURL", "Credentials:ResourceUrl");
            Add("OAUTH_CLIENTID", "Credentials:ClientId");
            Add("OAUTH_SECRET", "Credentials:ClientSecret");
        }
    }

    public class ServiceCredentials
    {
        /// <summary>
        /// The url of the server that will authenicate the credentials.
        /// </summary>
        [Required]
        public string AuthenticationUrl { get; set; }

        /// <summary>
        /// The url of the resource to access.
        /// </summary>
        [Required]
        public string ResourceUrl { get; set; }

        /// <summary>
        /// The client id.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// The client secret.
        /// </summary>
        [Required]
        public string ClientSecret { get; set; }
    }
}
