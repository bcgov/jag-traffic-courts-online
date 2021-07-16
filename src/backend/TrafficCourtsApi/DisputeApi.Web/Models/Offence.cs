namespace DisputeApi.Web.Models
{
    public class Offence
    {
        public int OffenceNumber { get; set; }
        public decimal TicketedAmount { get; set; }//total
        public decimal AmountDue { get; set; } //total-discount-paid
        public string ViolationDateTime { get; set; }
        public string OffenceDescription { get; set; }
        public string VehicleDescription { get; set; }
        public OffenceDisputeDetail OffenceDisputeDetail { get; set; }
        public decimal DiscountAmount { get; set; }//discount
        public string DiscountDueDate { get; set; }
        public string InvoiceType { get; set; }
    }
}
