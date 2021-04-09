using DisputeApi.Web.Features.TicketLookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public decimal TicketAmount { get; set; }
        public decimal AmountDue { get; set; }
        public string DueDate { get; set; }
        public string Description { get; set; }
        public Dispute Dispute { get; set; }
    }
}
