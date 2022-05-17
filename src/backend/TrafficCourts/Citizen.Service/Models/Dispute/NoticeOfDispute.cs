using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Citizen.Service.Validators;

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
        [Required]
        public string TicketNumber { get; set; } = null!;

        /// <summary>
        /// The provincial court hearing location named on the violation ticket.
        /// </summary>
        [JsonPropertyName("provincial_court_hearing_location")]
        public string? ProvincialCourtHearingLocation { get; set; }

        /// <summary>
        /// The date and time the violation ticket was issue. Time must only be hours and minutes.
        /// </summary>
        [JsonPropertyName("issued_date")]
        [Required]
        public DateTime? IssuedDate { get; set; }

        /// <summary>
        /// The surname or corporate name.
        /// </summary>
        [JsonPropertyName("surname")]
        [Required]
        public string Surname { get; set; } = null!;

        /// <summary>
        /// The given names or corporate name continued.
        /// </summary>
        [JsonPropertyName("given_names")]
        [Required]
        public string GivenNames { get; set; } = null!;

        /// <summary>
        /// The disputant's birthdate.
        /// </summary>
        [JsonPropertyName("birthdate")]
        [SwaggerSchema(Format = "date")]
        [Required]
        public DateTime Birthdate { get; set; }

        /// <summary>
        /// The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
        /// </summary>
        [JsonPropertyName("drivers_licence_number")]
        [MaxLength(20)]
        [Required]
        public string DriversLicenceNumber { get; set; } = null!;

        /// <summary>
        /// The province or state the drivers licence was issued by.
        /// </summary>
        [JsonPropertyName("drivers_licence_province")]
        [MaxLength(30)]
        [Required]
        public string DriversLicenceProvince { get; set; } = null!;

        /// <summary>
        /// The mailing address of the disputant.
        /// </summary>
        [JsonPropertyName("address")]
        [Required]
        public string Address { get; set; } = null!;

        /// <summary>
        /// The mailing address city of the disputant.
        /// </summary>
        [JsonPropertyName("city")]
        [Required]
        public string City { get; set; } = null!;

        /// <summary>
        /// The mailing address province of the disputant.
        /// </summary>
        [JsonPropertyName("province")]
        [MaxLength(30)]
        [Required]
        public string Province { get; set; } = null!;

        /// <summary>
        /// The mailing address postal code or zip code of the disputant.
        /// </summary>
        [JsonPropertyName("postal_code")]
        [MaxLength(6)]
        [Required]
        public string PostalCode { get; set; } = null!;

        /// <summary>
        /// The disputant's home phone number.
        /// </summary>
        [JsonPropertyName("home_phone_number")]
        [Phone]
        [Required]
        public string HomePhoneNumber { get; set; } = null!;

        /// <summary>
        /// The disputant's work phone number.
        /// </summary>
        [JsonPropertyName("work_phone_number")]
        [Phone]
        public string? WorkPhoneNumber { get; set; }

        /// <summary>
        /// The disputant's email address.
        /// </summary>
        [JsonPropertyName("email_address")]
        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; } = null!;

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
        [RequiredIf("RepresentedByLawyer", true)]
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
        [Range(0, 99)]
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
        [JsonPropertyName("disputant_detected_ocr_issues")]
        public bool DisputantDetectedOcrIssues { get; set; }

        /// <summary>
        /// The description of the issue with OCR ticket if the citizen has detected any.
        /// </summary>
        [JsonPropertyName("disputant_ocr_issues_description")]
        [RequiredIf("DisputantDetectedOcrIssues", true)]
        public string? DisputantOcrIssuesDescription { get; set; }

        /// <summary>
        /// The unique identifier for the Violation Ticket (OCR or looked up) for this dispute.
        /// </summary>
        [JsonPropertyName("ticket_id")]
        [Required]
        public string TicketId { get; set; } = null!;
    }

}
