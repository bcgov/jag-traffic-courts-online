using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common;

namespace DisputeApi.Web.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class TicketDispute
    {
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
        public bool InformationCertified { get; set; }
        public Disputant Disputant { get; set; }
        public Additional Additional { get; set; }
        public List<Offence> Offences { get; set; }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Additional
    {
        public bool LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool WitnessPresent { get; set; }
    }

    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class Disputant
    {
        public string LastName { get; set; }
        public string GivenNames { get; set; }
        public string MailingAddress { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public string DriverLicense { get; set; }
        public string DriverLicenseProvince { get; set; }
        public string HomePhoneNumber { get; set; }
        public string WorkPhoneNumber { get; set; }

    }
}
