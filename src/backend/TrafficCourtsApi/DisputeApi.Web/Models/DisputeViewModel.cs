using System.ComponentModel.DataAnnotations;
using DisputeApi.Web.Features.Disputes;
using TrafficCourts.Common.Contract;

namespace DisputeApi.Web.Models
{
    public class DisputeViewModel 
    {
        [Key] [Required] public int Id { get; set; }
        public string ViolationTicketNumber { get; set; }
        public int OffenceNumber { get; set; }
        public string EmailAddress { get; set; }
        public TrafficCourts.Common.Contract.OffenceAgreementStatus OffenceAgreementStatus { get; set; }
        public bool RequestReduction { get; set; }
        public bool RequestMoreTime { get; set; }
        public string ReductionReason { get; set; }
        public string MoreTimeReason { get; set; }
        public bool LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public bool WitnessPresent { get; set; }
        public string InterpreterLanguage { get; set; }
        public bool InformationCertified { get; set; }
        public TrafficCourts.Common.Contract.DisputeStatus Status { get; set; }

        public Dispute ToDispute ()
        {
            return new Dispute
            {
                Id = this.Id,
                ViolationTicketNumber=this.ViolationTicketNumber,
                OffenceNumber = this.OffenceNumber,
                EmailAddress = this.EmailAddress,
                OffenceAgreementStatus= this.OffenceAgreementStatus,
                RequestReduction=this.RequestReduction,
                RequestMoreTime = this.RequestMoreTime,
                InformationCertified = this.InformationCertified,
                InterpreterLanguage = this.InterpreterLanguage,
                InterpreterRequired = this.InterpreterRequired,
                LawyerPresent = this.LawyerPresent,
                MoreTimeReason = this.MoreTimeReason,
                ReductionReason = this.ReductionReason,
                Status = this.Status,
                WitnessPresent= this.WitnessPresent,                
            };
        }
    }

 
}
