using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class TicketDispute
    {
        public string TicketNumber { get; set; } = String.Empty;
        public string CourtLocation { get; set; } = String.Empty;
        public DateTime ViolationDate { get; set; }
        public string DisputantSurname { get; set; } = String.Empty;
        public string GivenNames { get; set; } = String.Empty;
        public string StreetAddress { get; set; } = String.Empty;
        public string Province { get; set; } = String.Empty;
        public string PostalCode { get; set; } = String.Empty;
        public string HomePhone { get; set; } = String.Empty;
        public string EmailAddress { get; set; } = String.Empty;
        public string DriversLicence { get; set; } = String.Empty;
        public string DriversLicenceProvince { get; set; } = String.Empty;
        public string WorkPhone { get; set; } = String.Empty;
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        [SwaggerSchema(Format = "date")]
        public DateOnly DateOfBirth { get; set; }
        public string EnforcementOrganization { get; set; } = String.Empty;
        [JsonConverter(typeof(DateOnlyJsonConverter))]
        [SwaggerSchema(Format = "date")]       
        public DateOnly ServiceDate { get; set; }
        public List<TicketCount> TicketCounts { get; set; } = new();
        public bool LawyerRepresentation { get; set; }
        public string InterpreterLanguage { get; set; } = String.Empty;
        public bool WitnessIntent { get; set; }
        public string? OcrKey { get; set; }
    }
}
