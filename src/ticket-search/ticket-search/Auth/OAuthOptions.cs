using System;
using System.ComponentModel.DataAnnotations;

namespace Gov.TicketSearch.Auth
{
    public class OAuthOptions
    {
        [Required]
        public string OAuthUrl { get; set; }

        [Required]
        public string ResourceUrl { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string Secret { get; set; }
    }
}
