using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gov.CitizenApi.Features.Tickets.DBModel
{
    public class Offence
    {
        [Key]
        public int Id { get; set; }
        public int OffenceNumber { get; set; }
        public decimal OffenceCode { get; set; }
        public decimal? TicketedAmount { get; set; }//total
        public decimal AmountDue { get; set; } //total-discount-paid
        public string ViolationDateTime { get; set; }
        public string OffenceDescription { get; set; }
        public string VehicleDescription { get; set; }
        public decimal DiscountAmount { get; set; }//discount
        public string DiscountDueDate { get; set; }
        public string InvoiceType { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
