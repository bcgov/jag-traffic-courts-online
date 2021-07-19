using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Gov.TicketSearch.Models
{
    public class TicketSearchRequest
    {
        /// <summary>
        /// The traffic violation ticket number
        /// </summary>
        [FromQuery(Name = "ticketNumber")]
        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{6,}$",
            ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
        public string TicketNumber { get; set; }

        /// <summary>
        /// The time of traffic violation
        /// </summary>
        [FromQuery(Name = "time")]
        [Required]
        [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$",
            ErrorMessage = "time must be properly formatted 24 hour clock")]
        public string Time { get; set; }
    }
}
