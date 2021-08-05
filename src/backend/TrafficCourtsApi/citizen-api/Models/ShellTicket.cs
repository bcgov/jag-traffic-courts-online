using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common;

namespace Gov.CitizenApi.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class ShellTicket
    {
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
        public List<Offence> Offences { get; set; }

    }
}
