﻿namespace TrafficCourts.Arc.Dispute.Service.Models
{
    public abstract class ArcFileRecord
    {
        public int LockoutFlag { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime TransactionTime { get; set; }
        public int TransactionLocation { get; } = 20254;
        public DateTime EffectiveDate { get; set; }
        public string Owner { get; } = "00001";
        public string FileNumber { get; set; } = String.Empty;
        public string CountNumber { get; set; } = "001";
        public string ReceivableType { get; } = "M";
        public string MvbClientNumber { get; set; } = String.Empty;
        public string UpdateFlag { get; set; } = String.Empty;
        public string Filler { get; set; } = String.Empty;
        public string ARCF0630RecordFlag { get; set; } = String.Empty;
    }

    public class AdnotatedTicket : ArcFileRecord
    {
        public string TransactionType { get; } = "EV";
        public string TransactionNumber { get; } = "00000";
        public string Name { get; set; } = String.Empty;
        public string Section { get; set; } = String.Empty;
        public string Subsection { get; set; } = String.Empty;
        public string Paragraph { get; set; } = String.Empty;
        public string Subparagraph { get; set; } = String.Empty;
        public string Act { get; set; } = String.Empty;
        public double OriginalAmount { get; set; }
        public string Organization { get; set; } = String.Empty;
        public string OrganizationLocation { get; set; } = String.Empty;
        public DateTime ServiceDate { get; set; }

    }

    public class DisputedTicket : ArcFileRecord
    {
        public string TransactionType { get; } = "ED";
        public string TransactionNumber { get; } = "00000";
        public DateTime ServiceDate { get; set; }
        public string Name { get; set; } = String.Empty;
        public string DisputeType { get; set; } = String.Empty;
        public string StreetAddress { get; set; } = String.Empty;
        public string City { get; set; } = String.Empty;
        public string Province { get; set; } = String.Empty;
        public string PostalCode { get; set; } = String.Empty;
    }
}
