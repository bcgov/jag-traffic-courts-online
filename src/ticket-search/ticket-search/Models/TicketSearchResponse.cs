using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gov.TicketSearch.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class TicketSearchResponse
    {
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
        public List<Offence> Offences { get; set; }
    }
}
