namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public abstract class ArcFileRecord
    {
        public int LockoutFlag { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionTime { get; set; }
        public int TransactionLocation { get; set; }
        public string TransactionType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Owner { get; set; }
        public string FileNumber { get; set; }
        public string CountNumber { get; set; }
        public string ReceivableType { get; set; }
        public int TransactionNumber { get; set; }
        public string MvbClientNumber { get; set; }
        public string UpdateFlag { get; set; }
        public string Filler { get; set; }
        public string ARCF0630RecordFlag { get; set; }
    }

    public class AdnotatedTicket : ArcFileRecord
    {
        public string Name { get; set; }
        public int Section { get; set; }
        public int Subsection { get; set; }
        public string Paragraph { get; set; }
        public string Act { get; set; }
        public double OriginalAmount { get; set; }
        public string Organization { get; set; }
        public string OrganizationLocation { get; set; }
        public DateTime ServiceDate { get; set; }

    }

    public class DisputedTicket : ArcFileRecord
    {
        public DateTime ServiceDate { get; set; }
        public string Name { get; set; }
        public string DisputeType { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
    }
}
