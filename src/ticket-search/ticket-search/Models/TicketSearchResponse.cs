using System.Collections.Generic;

namespace Gov.TicketSearch.Models
{
    public class TicketSearchResponse
    {
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
        public bool InformationCertified { get; set; }
        public List<Offence> Offences { get; set; }
    }
}
