using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;
using TrafficCourts.Citizen.Service.Models.Tickets;
using TrafficCourts.Common.Converters;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class NoticeOfDispute
    {
        public string? TicketNumber { get; set; }
        public string? ProvincialCourtHearingLocation { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? Surname { get; set; }
        public string? GivenNames { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? PostalCode { get; set; }
        public string? HomePhoneNumber { get; set; }
        public string? WorkPhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public IList<DisputedCount> DisputedCounts { get; set; } = new List<DisputedCount>();
        public bool RepresentedByLawyer { get; set; }
        public LegalRepresentation? LegalRepresentation { get; set; }
        public string? InterpreterLanguage { get; set; }
        public int NumberOfWitness { get; set; }
        public string? FineReductionReason { get; set; }
        public string? TimeToPayReason { get; set; }
        public bool CitizenDetectedOcrIssues { get; set; }
        public ViolationTicket ViolationTicket { get; set; } = new();
        public string? OcrKey { get; set; }
    }

    public class DisputedCount
    {
        public Plea Plea { get; set; }
        public int Count { get; set; }
        public bool RequestTimeToPay { get; set; }
        public bool RequestReduction { get; set; }
        public bool AppearInCourt { get; set; }
    }

    public class LegalRepresentation
    {
        public string LawFirmName { get; set; } = String.Empty;
        public string LawyerName { get; set; } = String.Empty;
        public string LawyerSurname { get; set; } = String.Empty;
        public string LawyerEmail { get; set; } = String.Empty;
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
