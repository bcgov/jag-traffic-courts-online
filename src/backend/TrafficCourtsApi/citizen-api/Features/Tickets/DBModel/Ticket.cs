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
        public string SurName { get; set; }
        public string GivenName { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string Birthdate { get; set; }
        public string Gender { get; set; }
        public string ProvincialHearingLocation { get; set; }
        public string OrganizationLocation { get; set; }

        public ICollection<Offence> Offences { get; set; }
    }

 
}
