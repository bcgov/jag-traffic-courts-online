using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class LegalRepresentation
    {
        /// <summary>
        /// Name of the law firm that will represent the disputant at the hearing.
        /// </summary>
        [JsonPropertyName("law_firm_name")]
        public string LawFirmName { get; set; } = String.Empty;

        /// <summary>
        /// Full name of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        [JsonPropertyName("lawyer_full_name")]
        public string LawyerFullName { get; set; } = String.Empty;

        /// <summary>
        /// Email address of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        [JsonPropertyName("lawyer_email")]
        public string LawyerEmail { get; set; } = String.Empty;

        /// <summary>
        /// Address of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        [JsonPropertyName("lawyer_address")]
        public string LawyerAddress { get; set; } = String.Empty;

        /// <summary>
        /// Address of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        [JsonPropertyName("lawyer_phone_number")]
        public string LawyerPhoneNumber { get; set; } = String.Empty;
    }

}
