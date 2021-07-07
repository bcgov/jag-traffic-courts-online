using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Gov.TicketSearch.Services.RsiServices.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class RawTicketSearchResponse
    {
        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }
    }

}
