using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common;

namespace DisputeApi.Web.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class OffenceDispute
    {
        public string ViolationTicketNumber { get; set; }
        public int OffenceNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
        public bool InformationCertified { get; set; }
        public Additional Additional { get; set; }
        public OffenceDisputeDetail OffenceDisputeDetail { get; set; }

    }


 

 
}
