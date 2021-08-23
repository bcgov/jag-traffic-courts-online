using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gov.CitizenApi.Features.Tickets.DBModel
{
    public class Ticket
    {
        [Key][Required]
        public int Id { get; set; }
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
        public string LastName { get; set; }
        public string GivenNames { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string Birthdate { get; set; }
        public string Gender { get; set; }
        public string CourtHearingLocation { get; set; }
        public string DetachmentLocation { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string DriverLicenseProvince { get; set; }
        public string Photo { get; set; }

        public ICollection<Offence> Offences { get; set; }
    }

 
}
