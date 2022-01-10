using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Ticket.Search.Service.Features.Search
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Offence
    {
        /// <summary>
        /// The offence number, should be 1, 2 or 3
        /// </summary>
        public int OffenceNumber { get; set; }

        /// <summary>
        /// The total ticketed amount
        /// </summary>
        public decimal TicketedAmount { get; set; }

        /// <summary>
        /// The total discounted amount due.
        /// </summary>
        public decimal AmountDue { get; set; }

        /// <summary>
        /// The offence happened time in format of yyyy-mm-ddThh:mm.
        /// </summary>
        public string? ViolationDateTime { get; set; }

        /// <summary>
        /// The offence description.
        /// </summary>
        public string? OffenceDescription { get; set; }

        /// <summary>
        /// The vehicle description.
        /// </summary>
        public string? VehicleDescription { get; set; }

        /// <summary>
        /// The disount amount.
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// The discount amount due date
        /// </summary>
        public string? DiscountDueDate { get; set; }

        /// <summary>
        /// The invoice type.
        /// </summary>
        public string? InvoiceType { get; set; }
    }
}
