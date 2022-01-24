using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Citizen.Service.Models.Search
{
    /// <summary>
    /// Represents a violation ticket that is returned from search requests
    /// </summary>
    public class TicketSearchResult
    {
        public TicketSearchResult()
        {
            Offences = new List<TicketOffence>();
        }

        /// <summary>
        /// The violation ticket number. This will match the ticket number searched for.
        /// </summary>
        public string ViolationTicketNumber { get; set; }
        /// <summary>
        /// The date the violation ticket was issued.
        /// </summary>
        public DateTime ViolationDate { get; set; }
        /// <summary>
        /// The time of day the violation ticket was issued. This will match the time searched for.
        /// </summary>
        public string ViolationTime { get; set; }
        /// <summary>
        /// The list of offenses on this violation ticket.
        /// </summary>
        public List<TicketOffence> Offences { get; set; }
    }

    public class TicketOffence
    {
        /// <summary>
        /// The offence or count number.
        /// </summary>
        public int OffenceNumber { get; set; }

        /// <summary>
        /// The ticketed amount for this offence.
        /// </summary>
        public decimal TicketedAmount { get; set; }
        /// <summary>
        /// The current amount due.
        /// </summary>
        public decimal AmountDue { get; set; }
        /// <summary>
        /// </summary>
        public string InvoiceType { get; set; }
        /// <summary>
        /// The description of the offence.
        /// </summary>
        public string OffenceDescription { get; set; }

        /// <summary>
        /// The description of vehicle.
        /// </summary>
        public string? VehicleDescription { get; set; }
    }

    #region Obsolete



    #endregion

}
