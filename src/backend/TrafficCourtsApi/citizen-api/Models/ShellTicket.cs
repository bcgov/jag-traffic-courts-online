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
        public string LastName { get; set; }
        public string GivenNames { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string Birthdate { get; set; } //2012-09-18
        public string Gender { get; set; } //M,F,O
        public string CourtHearingLocation { get; set; }//code
        public string DetachmentLocation { get; set; } //code
        public decimal? Count1Charge { get; set; } // charge code: 19023
        public decimal? Count1FineAmount { get; set; } //140.90
        public decimal? Count2Charge { get; set; } // charge code: 19023
        public decimal? Count2FineAmount { get; set; } //140.90
        public decimal? Count3Charge { get; set; }// charge code: 19023
        public decimal? Count3FineAmount { get; set; } //140.90
        public string Photo { get; set; } //base64 encoded image data.
        public string Address { get; set; } 
        public string City { get; set; } 
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string DriverLicenseProvince { get; set; }
    }
}
