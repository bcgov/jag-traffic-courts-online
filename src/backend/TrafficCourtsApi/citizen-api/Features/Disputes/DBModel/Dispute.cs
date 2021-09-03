using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Contract;

namespace Gov.CitizenApi.Features.Disputes.DBModel
{
    public class Dispute
    {
        [Key][Required]
        public int Id { get; set; }
        public string ViolationTicketNumber { get; set; }
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
        public string PhoneNumber { get; set; } 
        public bool LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public bool WitnessPresent { get; set; }
        public int? NumberOfWitnesses { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool RequestReduction { get; set; }
        public bool RequestMoreTime { get; set; }
        public string ReductionReason { get; set; }
        public string MoreTimeReason { get; set; }
        public string ConfirmationNumber { get; set; }

        public ICollection<OffenceDisputeDetail> OffenceDisputeDetails { get; set; }
    }

 
}
