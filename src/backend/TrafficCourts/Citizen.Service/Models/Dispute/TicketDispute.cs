using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class TicketDispute
    {
        public string TicketNumber { get; set; }
        public string CourtLocation { get; set; }
        public DateTime ViolationDate { get; set; }
        public string DisputantSurname { get; set; }
        public string GivenNames { get; set; }
        public string StreetAddress { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string EmailAddress { get; set; }
        public string DriversLicence { get; set; }
        public string DriversLicenceProvince { get; set; }
        public string WorkPhone { get; set; }

        [JsonConverter(typeof(DateOnlyJsonConverter))]
        [SwaggerSchema(Format = "date")]
        public DateOnly DateOfBirth { get; set; }
        public string EnforcementOrganization { get; set; }
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        [SwaggerSchema(Format = "date")]
        public DateOnly ServiceDate { get; set; }
        public List<TicketCount> TicketCounts { get; set; }
        public bool LawyerRepresentation { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool WitnessIntent { get; set; }
        public string? OcrKey { get; set; }
    }
}
