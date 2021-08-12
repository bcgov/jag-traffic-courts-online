using System.Collections.Generic;

namespace TrafficCourts.Common.Contract
{
    public class DisputeContract
    {
        public string ViolationTicketNumber { get; set; }
        public bool InformationCertified { get; set; }
        public string DisputantLastName { get; set; }
        public string DisputantFirstName { get; set; }
        public string DisputantMailingAddress { get; set; }
        public string DisputantMailingCity { get; set; }
        public string DisputantMailingProvince { get; set; }
        public string DisputantMailingPostalCode { get; set; }
        public string DisputantBirthDate { get; set; }
        public string DisputantEmailAddress { get; set; }
        public string DriverLicense { get; set; }
        public string DriverLicenseProvince { get; set; }
        public string HomePhoneNumber { get; set; }
        public string WorkPhoneNumber { get; set; }
        public bool LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public bool WitnessPresent { get; set; }
        public string InterpreterLanguage { get; set; }
        public ICollection<OffenceDisputeDetailContract> OffenceDisputeDetails { get; set; }
    }

    public enum DisputeStatus
    {
        New,
        Submitted,
        InProgress,//ticket already verified
        Complete,
        Rejected,
    }

}
