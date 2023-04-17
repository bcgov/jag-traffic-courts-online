using System.Text;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Staff.Service.Mappers;

public class Mapper
{
    public static DisputeApproved ToDisputeApproved(Dispute dispute)
    {
        ArgumentNullException.ThrowIfNull(dispute);

        DisputeApproved target = new();

        if (dispute.NoticeOfDisputeGuid is not null)
        {
            target.NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid);
        }

        target.Surname = dispute.DisputantSurname;
        target.GivenName1 = dispute.DisputantGivenName1;
        target.GivenName2 = dispute.DisputantGivenName2;
        target.GivenName3 = dispute.DisputantGivenName3;
        target.TicketIssuanceDate = dispute.IssuedTs?.DateTime;
        target.TicketFileNumber = dispute.TicketNumber;
        target.IssuingOrganization = dispute.ViolationTicket.DetachmentLocation;
        target.IssuingLocation = dispute.ViolationTicket.CourtLocation;

        // If DL Province is out of province, do not send drivers licence
        if (dispute.DriversLicenceIssuedProvinceSeqNo == 1 && dispute.DriversLicenceIssuedCountryId == 1)
            target.DriversLicence = dispute.DriversLicenceNumber;
        else target.DriversLicence = "";

        target.DisputeCounts = Map(dispute.DisputeCounts);
        target.ViolationTicketCounts = Map(dispute.ViolationTicket.ViolationTicketCounts);

        // TODO: Lookup address province seq no and country id and set addressprovince to abbreviation code
        if (dispute.AddressProvinceSeqNo!= null) target.Province = dispute.AddressProvince;
        else if (dispute.AddressProvince is not null && dispute.AddressProvince.Length > 2) target.Province = dispute.AddressProvince.Substring(0, 2);
        else dispute.AddressProvince= null;

        target.StreetAddress = FormatStreetAddress(dispute);
        target.City = dispute.AddressCity;        
        target.City = dispute.AddressCity;
        // only need two character code (province may be more than two chars if not USA or Canada)
        target.Province = dispute.AddressProvince is not null && dispute.AddressProvince.Length > 2
            ? dispute.AddressProvince[..2]
            : dispute.AddressProvince;

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
        // short circuit, most of the time there is probably one address line
        if (string.IsNullOrWhiteSpace(dispute.AddressLine2) && string.IsNullOrWhiteSpace(dispute.AddressLine3))
        {
            return dispute.AddressLine1;
        }

        StringBuilder buffer = new StringBuilder();

        void Append(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (buffer.Length > 0)
                {
                    buffer.Append(", ");
                }

                buffer.Append(value);
            }
        }

        Append(dispute.AddressLine1);
        Append(dispute.AddressLine2);
        Append(dispute.AddressLine3);

        return buffer.ToString();
    }

    public static DisputeRejected ToDisputeRejected(Dispute dispute)
    {
        DisputeRejected disputeRejected = new()
        {
            Reason = dispute.RejectedReason,
            Email = dispute.EmailAddress,
            TicketNumber = dispute.TicketNumber,
            NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
        };
        return disputeRejected;
    }

    public static SaveFileHistoryRecord ToFileHistoryWithNoticeOfDisputeId(string noticeOfDisputeId, FileHistoryAuditLogEntryType auditLogEntryType, string actionByApplicationUser)
    {
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.NoticeOfDisputeId = noticeOfDisputeId;
        fileHistoryRecord.AuditLogEntryType = auditLogEntryType;
        fileHistoryRecord.ActionByApplicationUser = actionByApplicationUser;
        return fileHistoryRecord;
    }

    public static SaveFileHistoryRecord ToFileHistoryWithTicketNumber(string ticketNumber, FileHistoryAuditLogEntryType auditLogEntryType, string actionByApplicationUser)
    {
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.TicketNumber = ticketNumber;
        fileHistoryRecord.AuditLogEntryType = auditLogEntryType;
        fileHistoryRecord.ActionByApplicationUser = actionByApplicationUser;
        return fileHistoryRecord;
    }

    public static DisputeCancelled ToDisputeCancelled(Dispute dispute)
    {
        DisputeCancelled disputeCancelled = new();
        disputeCancelled.Id = dispute.DisputeId;
        disputeCancelled.Email = dispute.EmailAddress;
        disputeCancelled.TicketNumber = dispute.TicketNumber;
        disputeCancelled.NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid);
        return disputeCancelled;
    }
}
