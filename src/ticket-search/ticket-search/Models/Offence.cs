using System.Diagnostics.CodeAnalysis;

namespace Gov.TicketSearch.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Offence
    {
        public int OffenceNumber { get; set; }
        /// <summary>
        /// Total ticketed amount.
        /// </summary>
        public decimal TicketedAmount { get; set; }

        /// <summary>
        /// The total discounted amount due.
        /// </summary>
        public decimal AmountDue { get; set; }
        public string ViolationDateTime { get; set; }
        public string OffenceDescription { get; set; }
        public string VehicleDescription { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountDueDate { get; set; }
        public string InvoiceType { get; set; }
    }
}
