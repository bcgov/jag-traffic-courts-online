using System.Text.Json.Serialization;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class SubmitNoticeOfDispute : IMessage
    {
        /// <summary>
        /// The status of the dispute. Defaults to <see cref="DisputeStatus.New"/>.
        /// </summary>
        public DisputeStatus Status { get; set; } = DisputeStatus.New;
        
        /// <summary>
        /// The violation ticket number.
        /// </summary>
        public string? TicketNumber { get; set; }

        /// <summary>
        /// The provincial court hearing location named on the violation ticket.
        /// </summary>
        public string? ProvincialCourtHearingLocation { get; set; }

        /// <summary>
        /// The date and time the violation ticket was issue. Time must only be hours and minutes.
        /// </summary>
        public DateTime? IssuedDate { get; set; }

        /// <summary>
        /// The date the disputant submitted the dispute.
        /// </summary>
        public DateTime? SubmittedDate { get; set; }

        /// <summary>
        /// The surname or corporate name.
        /// </summary>
        public string? Surname { get; set; }

        /// <summary>
        /// The given names or corporate name continued.
        /// </summary>
        public string? GivenNames { get; set; }

        /// <summary>
        /// The disputant's birthdate.
        /// </summary>
        public DateTime? Birthdate { get; set; }

        /// <summary>
        /// The drivers licence number. Note not all jurisdictions will use numeric drivers licence numbers.
        /// </summary>
        public string? DriversLicenceNumber { get; set; }

        /// <summary>
        /// The province or state the drivers licence was issued by.
        /// </summary>
        public string? DriversLicenceProvince { get; set; }

        /// <summary>
        /// The mailing address of the disputant.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// The mailing address city of the disputant.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// The mailing address province of the disputant.
        /// </summary>
        public string? Province { get; set; }

        /// <summary>
        /// The mailing address postal code or zip code of the disputant.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// The disputant's home phone number.
        /// </summary>
        public string? HomePhoneNumber { get; set; }

        /// <summary>
        /// The disputant's work phone number.
        /// </summary>
        public string? WorkPhoneNumber { get; set; }

        /// <summary>
        /// The disputant's email address.
        /// </summary>
        public string? EmailAddress { get; set; }

        /// <summary>
        /// The count dispute details.
        /// </summary>
        public List<DisputedCount> DisputedCounts { get; set; } = new List<DisputedCount>();

        /// <summary>
        /// The disputant intends to be represented by a lawyer at the hearing.
        /// </summary>
        public bool RepresentedByLawyer { get; set; }

        /// <summary>
        /// The details of the lawyer who represents the disputant at the hearing.
        /// </summary>
        public LegalRepresentation? LegalRepresentation { get; set; }

        /// <summary>
        /// The disputant requires spoken language interpreter. The language name is indicated in this field.
        /// </summary>
        public string? InterpreterLanguage { get; set; }

        /// <summary>
        /// The number of witnesses that the disputant intends to call.
        /// </summary>
        public int NumberOfWitness { get; set; }

        /// <summary>
        /// The reason that disputant declares for requesting a fine reduction.
        /// </summary>
        public string? FineReductionReason { get; set; }

        /// <summary>
        /// The reason that disputant declares for requesting more time to pay the amount on the violation ticket.
        /// </summary>
        public string? TimeToPayReason { get; set; }

        /// <summary>
        /// Identifier for whether the citizen has detected any issues with the OCR ticket result or not.
        /// </summary>
        public bool DisputantDetectedOcrIssues { get; set; }

        /// <summary>
        /// The description of the issue with OCR ticket if the citizen has detected any.
        /// </summary>
        public string? DisputantOcrIssuesDescription { get; set; }

        /// <summary>
        /// Violation Ticket details
        /// </summary>
        public ViolationTicket? ViolationTicket { get; set; }

        /// <summary>
        /// JSON serialized OCR data.
        /// </summary>
        public string? OcrViolationTicket { get; set; }
    }
}
