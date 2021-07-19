using DisputeApi.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DisputeApi.Web.Features.Tickets.Queries
{
    public class TicketSearchQuery : IRequest<TicketDispute>
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
    }
}
