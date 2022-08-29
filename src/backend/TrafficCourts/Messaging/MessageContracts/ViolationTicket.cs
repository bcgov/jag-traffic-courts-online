using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrafficCourts.Common.Converters;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class ViolationTicket
    {
        public string? TicketNumber { get; set; }
        public string? DisputantSurname { get; set; }
        public string? DisputantGivenNames { get; set; }
        public string? DisputantDriversLicenceNumber { get; set; }
        public string? DriversLicenceProvince { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly DisputantBirthdate { get; set; }
        public string? Address { get; set; }
        public string? AddressCity { get; set; }
        public string? AddressProvince { get; set; }
        public string? AddressPostalCode { get; set; }
        public string? DetachmentLocation { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? CourtLocation { get; set; }
        public IList<TicketCount> ViolationTicketCounts { get; set; } = new List<TicketCount>();
    }

    public class TicketCount
    {
        public short CountNo { get; set; }
        public string? Description { get; set; }
        public string? Section { get; set; }
        public string? Subsection { get; set; }
        public string? Paragraph { get; set; }
        public string? Subparagraph { get; set; }
        public string? ActOrRegulationNameCode { get; set; }
        public float? TicketedAmount { get; set; }
        public ViolationTicketCountIsAct? IsAct { get; set; }
        public ViolationTicketCountIsRegulation? IsRegulation { get; set; }
        public DisputeCount? DisputeCount { get; set; } = null!;
    }
}
