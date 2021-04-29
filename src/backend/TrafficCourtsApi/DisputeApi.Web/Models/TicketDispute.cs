using DisputeApi.Web.Features.Disputes.DBModel;
using DisputeApi.Web.Features.TicketLookup;
using System.Collections.Generic;

namespace DisputeApi.Web.Models
{
    public class TicketDispute
    {
        public string ViolationTicketNumber { get; set; }
        public string ViolationTime { get; set; }
        public string ViolationDate { get; set; }
        public bool InformationCertified { get; set; }
        public Disputant Disputant { get; set; }
        public Additional Additional { get; set; }
        public List<Offence> Offences { get; set; }
        /// <summary>
        /// Gets or sets the raw response returned from the RSI Pay BC API.
        /// Used only for troubleshooting during development. Will be removed
        /// once the API usage is understood.
        /// </summary>
        public RawTicketSearchResponse RawResponse { get; set; }
    }

    public class Additional
    {
        public string LawyerPresent { get; set; }
        public bool InterpreterRequired { get; set; }
        public string InterpreterLanguage { get; set; }
        public string WitnessPresent { get; set; }
    }

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
