using System.ComponentModel.DataAnnotations;

namespace DisputeApi.Web.Features.TicketService.Models
{
    public class Ticket
    {
        [Key]
        [Required]
        public int TicketNumber { get; set; }
        public string Name { get; set; }
        public string DateOfIssue { get; set; }
        public string TimeOfIssue { get; set; }
        public string DriversLicence { get; set; }
    }
}
