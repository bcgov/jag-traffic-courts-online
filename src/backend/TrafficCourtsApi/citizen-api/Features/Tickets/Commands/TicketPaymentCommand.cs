using Gov.CitizenApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class TicketPaymentCommand : IRequest<TicketPaymentResponse>
    {
        [FromQuery(Name = "ticketNumber")]
        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{6,}$",
    ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
        public string TicketNumber { get; set; }

        [FromQuery(Name = "time")]
        [Required]
        [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$",
            ErrorMessage = "time must be properly formatted 24 hour clock")]
        public string Time { get; set; }

        [FromQuery(Name = "counts")]
        [Required]
        [RegularExpression("^[1-3]+(,[1-3]+)*$", ErrorMessage = "counts must be properly formatted, user , as seperatoer")]
        public string Counts { get; set; }

        [FromQuery(Name = "amount")]
        [Required]
        [RegularExpression(@"^\d*\.?\d*$", ErrorMessage = "amount needs to be a valid decimal")]
        public string Amount { get; set; }
    }
}
