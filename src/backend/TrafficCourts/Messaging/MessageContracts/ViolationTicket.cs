using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class ViolationTicket
    {
        public string? TicketNumber { get; set; }
        public string? Surname { get; set; }
        public string? GivenNames { get; set; }
        public string? DriversLicenceNumber { get; set; }
        public string? DriversLicenceProvince { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly Birthdate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? OrganizationLocation { get; set; }
        public IList<TicketCount> ViolationTicketCounts { get; set; } = new List<TicketCount>();
    }

    public class TicketCount
    {
        public short Count { get; set; }
        public string? Description { get; set; }
        public string? FullSection { get; set; }
        public string? ActRegulation { get; set; }
        public float? TicketedAmount { get; set; }
        public bool? IsAct { get; set; }
        public bool? IsRegulation { get; set; }
    }
}
