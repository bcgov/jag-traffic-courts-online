using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Ticket.Search.Service.Configuration
{
    public class TicketSearchServiceConfiguration : TrafficCourtsConfiguration
    {
        public TicketSearchProperties? TicketSearch { get; set; }
    }

    public enum TicketSearchType
    {
        RoadSafety,
        Mock
    }

    public class TicketSearchServiceConfigurationProvider : TrafficCourtsConfigurationProvider
    {
        public override void Load()
        {
            // legacy configuration
            Add("OAUTH__OAUTHURL", "TicketSearch:AuthenticationUrl");
            Add("OAUTH__RESOURCEURL", "TicketSearch:ResourceUrl");
            Add("OAUTH__CLIENTID", "TicketSearch:ClientId");
            Add("OAUTH__SECRET", "TicketSearch:ClientSecret");

            // new configuration, change this variables?
            Add("OAUTH_OAUTHURL", "TicketSearch:AuthenticationUrl");
            Add("OAUTH_RESOURCEURL", "TicketSearch:ResourceUrl");
            Add("OAUTH_CLIENTID", "TicketSearch:ClientId");
            Add("OAUTH_SECRET", "TicketSearch:ClientSecret");
        }
    }

    public class TicketSearchProperties
    {
        public TicketSearchType SearchType { get; set; }

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

        public bool IsValid()
        {
            if (SearchType == TicketSearchType.Mock) return true;
            return AuthenticationUrl != null && ResourceUrl != null && !string.IsNullOrEmpty(ClientId) || !string.IsNullOrEmpty(ClientSecret);
        }
    }
}
