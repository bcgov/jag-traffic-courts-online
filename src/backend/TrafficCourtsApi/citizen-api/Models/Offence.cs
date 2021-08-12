using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common;
using TrafficCourts.Common.Contract;

namespace Gov.CitizenApi.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Offence
    {
        public int OffenceNumber { get; set; }
        public decimal TicketedAmount { get; set; }//total
        public decimal AmountDue { get; set; } //shell ticket: the same as total. But for rsi ticket : not change, just from RSI data. actual meaning: total-paid-discount
        public string ViolationDateTime { get; set; }
        public string OffenceDescription { get; set; }
        public string VehicleDescription { get; set; }
        public decimal DiscountAmount { get; set; }//discount, always 25
        public string DiscountDueDate { get; set; }
        public string InvoiceType { get; set; }
        public string OffenceAgreementStatus { get; set; }
        public bool RequestReduction { get; set; }
        public bool RequestMoreTime { get; set; }
        public bool? ReductionAppearInCourt { get; set; }
        public string ReductionReason { get; set; }
        public string MoreTimeReason { get; set; }
        public DisputeStatus Status { get; set; }
    }
}
