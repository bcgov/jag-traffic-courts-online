﻿using System.ComponentModel.DataAnnotations;

namespace DisputeApi.Web.Features.TicketService.Models
{
    public class Ticket
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ViolationTicketNumber { get; set; }
        public string CourtLocation { get; set; }
        public string ViolationDate { get; set; }
        public string SurName { get; set; }
        public string GivenNames { get; set; }
        public string Mailing { get; set; }
        public string Postal { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Licence { get; set; }
        public string ProvLicense { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Birthdate { get; set; }
    }
}
