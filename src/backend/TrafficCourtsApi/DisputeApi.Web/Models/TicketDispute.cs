using DisputeApi.Web.Features.TicketLookup;
using System.Collections.Generic;

namespace DisputeApi.Web.Models
{
    public class TicketDispute
    {
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }

        public List<Offence> Offences { get; set; }

        /// <summary>
        /// Gets or sets the raw response returned from the RSI Pay BC API.
        /// Used only for troubleshooting during development. Will be removed
        /// once the API usage is understood.
        /// </summary>
        public RawTicketSearchResponse RawResponse { get; set; }
    }

    public class Offence
    {
        public int OffenceNumber { get; set; }
        public decimal TicketedAmount { get; set; }//total
        public decimal AmountDue { get; set; } //total-discount-paid
        public string ViolationDateTime { get; set; }
        public string OffenceDescription { get; set; }
        public string VehicleDescription { get; set; }
        public Dispute Dispute { get; set; }
        public decimal DiscountAmount { get; set; }//discount
        public string DiscountDueDate { get; set; }
        public string InvoiceType { get; set; }
    }
}
