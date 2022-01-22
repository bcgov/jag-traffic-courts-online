using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Citizen.Service.Models.Search;

#pragma warning disable CS8618 // these types are deprecated, ignore nullable warnings

namespace TrafficCourts.Citizen.Service.Models
{
    public static class DeprecationExtensions
    {
        /// <summary>
        /// Creates deprecated object for backward compatibility.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Deprecated.TicketDispute CreateDeprecated(this TicketSearchResult result)
        {
            Deprecated.TicketDispute dispute = new();

            dispute.ViolationTicketNumber = result.ViolationTicketNumber;
            dispute.ViolationDate = result.ViolationDate.ToString("yyyy-MM-dd");
            dispute.ViolationTime = result.ViolationTime;
            dispute.Offences = new List<Deprecated.Offence>();

            foreach (var offence in result.Offences)
            {
                dispute.Offences.Add(new Deprecated.Offence
                {
                    AmountDue = offence.AmountDue,
                    DiscountAmount = offence.DiscountAmount,
                    DiscountDueDate = offence.DiscountDueDate.ToString("yyyy-MM-dd"),
                    OffenceDescription = offence.OffenceDescription,
                    VehicleDescription = offence.VehicleDescription!,
                    OffenceNumber = offence.OffenceNumber,
                }); ;
            }

            return dispute;
        }
    }

    namespace Deprecated
    {
        public class TicketDispute
        {
            public string ViolationTicketNumber { get; set; }
            public string ViolationTime { get; set; }
            public string ViolationDate { get; set; }
            public Disputant Disputant { get; set; }
            public Additional Additional { get; set; }
            public List<Offence> Offences { get; set; }
            public string DiscountDueDate { get; set; }//null or has valid or invalid value
            public decimal DiscountAmount { get; set; }//25 always
        }

        [ExcludeFromCodeCoverage(Justification = "Poco")]
        public class Additional
        {
            public bool LawyerPresent { get; set; }
            public bool InterpreterRequired { get; set; }
            public string InterpreterLanguage { get; set; }
            public bool WitnessPresent { get; set; }
            public int? NumberOfWitnesses { get; set; }
            public bool RequestReduction { get; set; }
            public bool RequestMoreTime { get; set; }
            public string ReductionReason { get; set; }
            public string MoreTimeReason { get; set; }
        }

        [ExcludeFromCodeCoverage(Justification = "Poco")]
        public class Disputant
        {
            public string LastName { get; set; }
            public string GivenNames { get; set; }
            public string MailingAddress { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string PostalCode { get; set; }
            public string Birthdate { get; set; }
            public string EmailAddress { get; set; }
            public string DriverLicenseNumber { get; set; }
            public string DriverLicenseProvince { get; set; }
            public string PhoneNumber { get; set; }

        }

        [ExcludeFromCodeCoverage(Justification = "Poco")]
        public class Offence
        {
            public int OffenceNumber { get; set; }
            public decimal TicketedAmount { get; set; }//total
            public decimal AmountDue { get; set; } //shell ticket: the same as total. But for rsi ticket : not change, just from RSI data. actual meaning: total-paid-discount
            public string ViolationDateTime { get; set; }
            public string OffenceDescription { get; set; }
            public string VehicleDescription { get; set; }
            public decimal DiscountAmount { get; set; }//discount, always 25
            public string DiscountDueDate { get; set; }
            public string InvoiceType { get; set; }
            public string OffenceAgreementStatus { get; set; }
            public bool RequestReduction { get; set; }
            public bool RequestMoreTime { get; set; }
            public bool? ReductionAppearInCourt { get; set; }
            public string ReductionReason { get; set; }
            public string MoreTimeReason { get; set; }
            public DisputeStatus Status { get; set; }
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

}
