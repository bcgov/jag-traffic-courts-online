using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gov.TicketSearch.Services.RsiServices.Models
{
    public class RawTicketSearchResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }

}
