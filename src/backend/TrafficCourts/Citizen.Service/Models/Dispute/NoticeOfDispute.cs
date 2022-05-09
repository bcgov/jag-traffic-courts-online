using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    /// <summary>
    /// Represents a violation ticket notice of dispute.
    /// </summary>
    public class NoticeOfDispute
    {
        /// <summary>
        /// The violation ticket number.
        /// </summary>
        [JsonPropertyName("ticket_number")]
        [MaxLength(12)]
        public string? TicketNumber { get; set; }

        /// <summary>
        /// The provincial court hearing location named on the violation ticket.
        /// </summary>
        [JsonPropertyName("provincial_court_hearing_location")]
        public string? ProvincialCourtHearingLocation { get; set; }

        /// <summary>
        /// The date and time the violation ticket was issue. Time must only be hours and minutes.
        /// </summary>
        [JsonPropertyName("issued_date")]
        public DateTime? IssuedDate { get; set; }

        /// <summary>
        /// The surname or corporate name.
        /// </summary>
        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        /// <summary>
        /// The given names or corporate name continued.
        /// </summary>
        [JsonPropertyName("given_names")]
        public string? GivenNames { get; set; }

        /// <summary>
        /// The mailing address of the disputant.
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        /// <summary>
        /// The mailing address city of the disputant.
        /// </summary>
        [JsonPropertyName("city")]
        public string? City { get; set; }

        /// <summary>
        /// The mailing address province of the disputant.
        /// </summary>
        [JsonPropertyName("province")]
        public string? Province { get; set; }

        /// <summary>
        /// The mailing address postal code or zip code of the disputant.
        /// </summary>
        [JsonPropertyName("postal_code")]
        [MaxLength(6)]
        public string? PostalCode { get; set; }

        /// <summary>
        /// The disputant's home phone number.
        /// </summary>
        [JsonPropertyName("home_phone_number")]
        public string? HomePhoneNumber { get; set; }

        /// <summary>
        /// The disputant's work phone number.
        /// </summary>
        [JsonPropertyName("work_phone_number")]
        public string? WorkPhoneNumber { get; set; }

        /// <summary>
        /// The disputant's email address.
        /// </summary>
        [JsonPropertyName("email_address")]
        public string? EmailAddress { get; set; }

        /// <summary>
        /// The count dispute details.
        /// </summary>
        [JsonPropertyName("disputed_counts")]
        public List<DisputedCount> DisputedCounts { get; set; } = new List<DisputedCount>();

        /// <summary>
        /// The disputant intends to be represented by a lawyer at the hearing.
        /// </summary>
        [JsonPropertyName("represented_by_lawyer")]
        public bool RepresentedByLawyer { get; set; }

        /// <summary>
        /// The details of the lawyer who represents the disputant at the hearing.
        /// </summary>
        [JsonPropertyName("legal_representation")]
        public LegalRepresentation? LegalRepresentation { get; set; }

        /// <summary>
        /// The disputant requires spoken language interpreter. The language name is indicated in this field.
        /// </summary>
        [JsonPropertyName("interpreter_language")]
        public string? InterpreterLanguage { get; set; }

        /// <summary>
        /// The number of witnesses that the disputant intends to call.
        /// </summary>
        [JsonPropertyName("number_of_witness")]
        public int NumberOfWitness { get; set; }

        /// <summary>
        /// The reason that disputant declares for requesting a fine reduction.
        /// </summary>
        [JsonPropertyName("fine_reduction_reason")]
        public string? FineReductionReason { get; set; }

        /// <summary>
        /// The reason that disputant declares for requesting more time to pay the amount on the violation ticket.
        /// </summary>
        [JsonPropertyName("time_to_pay_reason")]
        public string? TimeToPayReason { get; set; }

        /// <summary>
        /// Identifier for whether the citizen has detected any issues with the OCR ticket result or not.
        /// </summary>
        [JsonPropertyName("citizen_detected_ocr_issues")]
        public bool CitizenDetectedOcrIssues { get; set; }

        /// <summary>
        /// The description of the issue with OCR ticket if the citizen has detected any.
        /// </summary>
        [JsonPropertyName("citizen_ocr_issues_description")]
        public string? CitizenOcrIssuesDescription { get; set; }

        /// <summary>
        /// The unique identifier for the Violation Ticket (OCR or looked up) for this dispute.
        /// </summary>
        [JsonPropertyName("ticket_id")]
        public string? TicketId { get; set; }
    }

    public class DisputedCount
    {
        /// <summary>
        /// Represents the dispuant plea on count.
        /// </summary>
        [JsonPropertyName("plea")]
        public Plea Plea { get; set; }

        /// <summary>
        /// The count number. Must be unique within an individual dispute.
        /// </summary>
        [JsonPropertyName("count")]
        [Range(1, 3)]
        public int Count { get; set; }

        /// <summary>
        /// The disputant is requesting time to pay the ticketed amount.
        /// </summary>
        [JsonPropertyName("request_time_to_pay")]
        public bool RequestTimeToPay { get; set; }

        /// <summary>
        /// The disputant is requesting a reduction of the ticketed amount.
        /// </summary>
        [JsonPropertyName("request_reduction")]
        public bool RequestReduction { get; set; }

        /// <summary>
        /// Does the want to appear in court?
        /// </summary>
        [JsonPropertyName("appear_in_court")]
        public bool AppearInCourt { get; set; }
    }

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
    }

    /// <summary>
    /// An enumeration of Plea Type on a DisputedCount record.
    /// </summary>
    public enum Plea
    {
        /// <summary>
        /// If the dispuant is pleads guilty, plea will always be Guilty. The dispuant has choice to attend court or not.
        /// </summary>
        Guilty,

        /// <summary>
        /// If the dispuant is pleads not guilty, the dispuant will have to attend court.
        /// </summary>
        NotGuilty
    }

}
