using System;
using System.ComponentModel.DataAnnotations;

namespace Gov.CitizenApi.Features.Tickets.DBModel
{
    public class Payment
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string RequestedCounts { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime CompletedDateTime { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string TransactionId { get; set; }
        public string ConfirmationNumber { get; set; }
    }

    public enum PaymentStatus
    {
        InProgress,
        Cancelled,
        Success
    }
}
