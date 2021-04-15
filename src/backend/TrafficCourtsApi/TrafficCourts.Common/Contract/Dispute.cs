using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Common.Contract
{
    public interface IDispute
    {
        public int Id { get; }
        public string ViolationTicketNumber { get; }
        public int OffenceNumber { get; }
        public string EmailAddress { get; }
        public OffenceAgreementStatus OffenceAgreementStatus { get; }
        public bool RequestReduction { get; }
        public bool RequestMoreTime { get; }
        public string ReductionReason { get; }
        public string MoreTimeReason { get; }
        public bool LawyerPresent { get; }
        public bool InterpreterRequired { get; }
        public bool WitnessPresent { get; }
        public string InterpreterLanguage { get; }
        public bool InformationCertified { get; }
        public DisputeStatus Status { get; }
    }

    public enum DisputeStatus
    {
        New,
        Submitted,
        InProgress,//ticket already verified
        Complete,
        Rejected,
    }

    public enum OffenceAgreementStatus
    {
        AgreeOffenceNotInCourt,
        AgreeOffenceInCourt,
        NotAgreeOffence,
    }
}
