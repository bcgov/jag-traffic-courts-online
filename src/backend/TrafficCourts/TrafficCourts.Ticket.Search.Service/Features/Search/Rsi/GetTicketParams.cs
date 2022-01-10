using Refit;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Ticket.Search.Service.Features.Search.Rsi
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class GetTicketParams
    {
        [AliasAs("in")]
        public string TicketNumber { get; set; } = null!;

        [AliasAs("prn")]
        public string Prn { get; set; } = null!;

        [AliasAs("time")]
        public string IssuedTime { get; set; } = null!;
    }

}
