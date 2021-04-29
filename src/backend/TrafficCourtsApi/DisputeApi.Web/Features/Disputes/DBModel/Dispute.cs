using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Contract;

namespace DisputeApi.Web.Features.Disputes.DBModel
{
    public class Dispute
    {
        [Key][Required]
        public int Id { get; set; }
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
        public string LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public string WitnessPresent { get; set; }
        public string InterpreterLanguage { get; set; }
        public ICollection<OffenceDisputeDetail> OffenceDisputeDetails { get; set; }
    }

 
}
