using System.Text.Json.Serialization;

namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public abstract class ArcFileRecord
    {
        protected ArcFileRecord(string transactionType)
        {
            TransactionType = transactionType;
        }

        public int LockoutFlag { get; } = 0;

        public DateTime TransactionDateTime { get; set; }
        /// <summary>
        /// Mapped to seprate output field, so we need a separate property for date.
        /// </summary>
        [JsonIgnore]
        public DateTime TransactionDate => TransactionDateTime;

        /// <summary>
        /// Mapped to seprate output field, so we need a separate property for time.
        /// </summary>
        [JsonIgnore]
        public DateTime TransactionTime => TransactionDateTime;

        public int TransactionLocation { get; } = 20254;

        public string TransactionType { get; private set; }

        public string TransactionNumber { get; } = "00000";

        public DateTime EffectiveDate { get; set; }
        public string Owner { get; } = "00001";
        public string FileNumber { get; set; } = String.Empty;
        public string CountNumber { get; set; } = "000"; // 000 in an output would represent an error
        public string ReceivableType { get; } = "M";
        public string MvbClientNumber { get; set; } = String.Empty;
        public string UpdateFlag { get; } = String.Empty;
        public string Filler { get; } = String.Empty;
        public string ARCF0630RecordFlag { get; } = String.Empty;
    }

    public class AdnotatedTicket : ArcFileRecord
    {
        public AdnotatedTicket() : base("EV")
        {
        }

        public string Name { get; set; } = String.Empty;
        public string Section { get; set; } = String.Empty;
        public string Subsection { get; set; } = String.Empty;
        public string Paragraph { get; set; } = String.Empty;
        public string Act { get; set; } = String.Empty;
        public decimal OriginalAmount { get; set; }
        public string Organization { get; set; } = String.Empty;
        public string OrganizationLocation { get; set; } = String.Empty;
        public DateTime ServiceDate { get; set; }

    }

    public class DisputedTicket : ArcFileRecord
    {
        public DisputedTicket() : base("ED")
        {
        }


        public DateTime ServiceDate { get; set; }
        public string Name { get; set; } = String.Empty;
        public string DisputeType { get; set; } = String.Empty;
        public string StreetAddress { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public string Province { get; set; } = String.Empty;
        public string PostalCode { get; set; } = String.Empty;
    }
}
