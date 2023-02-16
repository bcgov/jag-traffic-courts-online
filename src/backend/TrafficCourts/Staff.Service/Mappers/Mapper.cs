using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
namespace TrafficCourts.Staff.Service.Mappers;

public class Mapper
{
    public static DisputeApproved ToDisputeApproved(Dispute dispute)
    {
        ArgumentNullException.ThrowIfNull(dispute);

        DisputeApproved target = new();
        target.Surname = dispute.DisputantSurname;
        target.GivenName1 = dispute.DisputantGivenName1;
        target.GivenName2 = dispute.DisputantGivenName2;
        target.GivenName3 = dispute.DisputantGivenName3;
        target.TicketIssuanceDate = dispute.IssuedTs?.DateTime;
        target.TicketFileNumber = dispute.TicketNumber;
        target.IssuingOrganization = dispute.ViolationTicket.DetachmentLocation;
        target.IssuingLocation = dispute.ViolationTicket.CourtLocation;
        target.DriversLicence = dispute.DriversLicenceNumber;

        target.DisputeCounts = Map(dispute.DisputeCounts);
        target.ViolationTicketCounts = Map(dispute.ViolationTicket.ViolationTicketCounts);

        target.StreetAddress = FormatStreetAddress(dispute);
        target.City = dispute.AddressCity;
        // only need two character code (province may be more than two chars if not USA or Canada)
        if (dispute.AddressProvince is not null && dispute.AddressProvince.Length > 2) target.Province = dispute.AddressProvince.Substring(0, 2);
        else target.Province = dispute.AddressProvince;
        target.PostalCode = dispute.PostalCode;
        target.Email = dispute.EmailAddress;

        return target;
    }

    private static IList<Messaging.MessageContracts.ViolationTicketCount> Map(IEnumerable<Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount>? counts)
    {
        if (counts is null)
        {
            return Array.Empty<Messaging.MessageContracts.ViolationTicketCount>();
        }

        var result = counts
            .Select(_ => new Messaging.MessageContracts.ViolationTicketCount
            {
                Count = _.CountNo,
                Subparagraph = _.Subparagraph,
                Section = _.Section,
                Subsection = _.Subsection,
                Paragraph = _.Paragraph,
                Act = _.ActOrRegulationNameCode,
                Amount = _.TicketedAmount
            }).ToList();

        return result;
    }

    private static IList<DisputedCount> Map(IEnumerable<Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount>? counts)
    {
        if (counts is null)
        {
            return Array.Empty<DisputedCount>();
        }

        var result = counts
            .Select(_ => new DisputedCount
            {
                Count = _.CountNo,
                DisputeType = _.PleaCode.ToString()
            })
            .ToList();

        return result;
    }

    private static string FormatStreetAddress(Dispute dispute)
    {
        // TODO: clean this up
        return dispute.AddressLine1 + ((dispute.AddressLine2 is null) ? "" : ", " + dispute.AddressLine2) + ((dispute.AddressLine3 is null) ? "" : ", " + dispute.AddressLine3);
    }

    public static DisputeRejected ToDisputeRejected(Dispute dispute)
    {
        DisputeRejected disputeRejected = new()
        {
            Reason = dispute.RejectedReason
        };
        return disputeRejected;
    }

    public static SaveFileHistoryRecord ToFileHistory(long disputeId, FileHistoryAuditLogEntryType auditLogEntryType)
    {
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.DisputeId = disputeId;
        fileHistoryRecord.AuditLogEntryType = auditLogEntryType;
        return fileHistoryRecord;
    }

    public static SaveFileHistoryRecord ToFileHistoryWithNoticeOfDisputeId(string noticeOfDisputeId, FileHistoryAuditLogEntryType auditLogEntryType)
    {
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.NoticeOfDisputeId = noticeOfDisputeId;
        fileHistoryRecord.AuditLogEntryType = auditLogEntryType;
        return fileHistoryRecord;
    }

    public static SaveFileHistoryRecord ToFileHistoryWithTicketNumber(string ticketNumber, FileHistoryAuditLogEntryType auditLogEntryType)
    {
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.TicketNumber = ticketNumber;
        fileHistoryRecord.AuditLogEntryType = auditLogEntryType;
        return fileHistoryRecord;
    }


    public static EmailVerificationSend ToEmailVerification(Guid guid)
    {
        EmailVerificationSend emailVerificationSend = new(guid);
        return emailVerificationSend;
    }

    public static DisputeCancelled ToDisputeCancelled(Dispute dispute)
    {
        DisputeCancelled disputeCancelled = new();
        disputeCancelled.Id = dispute.DisputeId;
        disputeCancelled.Email = dispute.EmailAddress;
        return disputeCancelled;
    }
}
