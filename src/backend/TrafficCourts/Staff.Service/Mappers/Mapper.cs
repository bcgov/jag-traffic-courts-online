using System.Text;
using TrafficCourts.Domain.Models;
using TrafficCourts.Messaging.MessageContracts;

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

        target.Surname = dispute.ViolationTicket?.DisputantSurname ?? dispute.DisputantSurname;
        var (givenName1, givenName2, givenName3) = SplitGivenNames(dispute.ViolationTicket?.DisputantGivenNames);
        if (givenName1 is null)
        {
            target.GivenName1 = dispute.DisputantGivenName1;
            target.GivenName2 = dispute.DisputantGivenName2;
            target.GivenName3 = dispute.DisputantGivenName3;
        } else {
            target.GivenName1 = givenName1;
            target.GivenName2 = givenName2;
            target.GivenName3 = givenName3;
        }

        target.TicketIssuanceDate = dispute.IssuedTs?.DateTime;
        target.TicketFileNumber = dispute.TicketNumber;

        // TCVP-2793 - issuing organziation should always be police (POL) and issuing location is the detachment location
        target.IssuingOrganization = "POL";
        target.IssuingLocation = dispute.ViolationTicket?.DetachmentLocation ?? String.Empty;

        // If DL Province is out of province, do not send drivers licence
        if (dispute.DriversLicenceNumber is not null && dispute.DriversLicenceIssuedProvinceSeqNo == 1 && dispute.DriversLicenceIssuedCountryId == 1)
            target.DriversLicence = dispute.DriversLicenceNumber;
        else target.DriversLicence = "";

        target.DisputeCounts = Map(dispute.DisputeCounts);
        target.ViolationTicketCounts = Map(dispute.ViolationTicket?.ViolationTicketCounts);

        // TODO: Lookup address province seq no and country id and set addressprovince to abbreviation code
        if (dispute.AddressProvinceSeqNo is not null) target.Province = dispute.AddressProvince;
        // only need two character code (province may be more than two chars if not USA or Canada)
        else if (dispute.AddressProvince is not null && dispute.AddressProvince.Length > 2) target.Province = dispute.AddressProvince.Substring(0, 2);
        else dispute.AddressProvince = null;

        target.StreetAddress = FormatStreetAddress(dispute);
        target.City = dispute.AddressCity;
        target.PostalCode = dispute.PostalCode;
        target.Email = dispute.EmailAddress;

        return target;
    }

    private static IList<Messaging.MessageContracts.ViolationTicketCount> Map(IEnumerable<Domain.Models.ViolationTicketCount>? counts)
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

    private static IList<DisputedCount> Map(IEnumerable<Domain.Models.DisputeCount>? counts)
    {
        if (counts is null)
        {
            return [];
        }

        var filteredCounts = counts
            // TCVP-2786 Filter out skipped counts before sending to ARC
            .Where(_ => _.RequestCourtAppearance != DisputeCountRequestCourtAppearance.N || _.RequestReduction != DisputeCountRequestReduction.N || _.RequestTimeToPay != DisputeCountRequestTimeToPay.N)
            .OrderBy(_ => _.CountNo)
            .ToList();

        var result = filteredCounts
            .Select(_ => new DisputedCount
            {
                Count = _.CountNo,
                DisputeType = GetArcDisputeType(_)  // either F (fine) or A (allegation)
            })
            .ToList();

        return result;
    }

    /// <summary>
    /// TCVP-2786 Determines and returns the Dispute Type based on the following criteria:
    /// If a user selects Written Reasons, each disputed count is dispute type = F (Fine)
    /// If a user selects Court Hearing, use the following:
    /// If the user selects that they agree to the offence, that count is dispute type = F (Fine)
    /// If the users select they do not agree to the offence, that count is dispute type = A (Allegation)
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private static string GetArcDisputeType(Domain.Models.DisputeCount count)
    {
        if (count is not null)
        {
            if (count.RequestCourtAppearance == DisputeCountRequestCourtAppearance.N) return "F"; // fine
            if (count.RequestCourtAppearance == DisputeCountRequestCourtAppearance.Y && count.PleaCode == DisputeCountPleaCode.G) return "F"; // fine
        }

        return "A"; // allegation
    }

    private static string FormatStreetAddress(Dispute dispute)
    {
        // short circuit, most of the time there is probably one address line
        if (string.IsNullOrWhiteSpace(dispute.AddressLine2) && string.IsNullOrWhiteSpace(dispute.AddressLine3))
        {
            return dispute.AddressLine1;
        }

        StringBuilder buffer = new();

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

    /// <summary>
    /// Splits a full name into three given names.
    /// </summary>
    /// <param name="fullName">The full name to be split.</param>
    /// <returns>A tuple containing three given names. If the full name is null or empty, all given names will be null.</returns>
    private static (string? GivenName1, string? GivenName2, string? GivenName3) SplitGivenNames(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (null, null, null);
        }

        string[] parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string? givenName1 = parts.Length > 0 ? parts[0] : null;
        string? givenName2 = parts.Length > 1 ? parts[1] : null;
        string? givenName3 = parts.Length > 2 ? parts[2] : null;

        return (givenName1, givenName2, givenName3);
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
        return ToFileHistoryWithNoticeOfDisputeId(noticeOfDisputeId, auditLogEntryType, actionByApplicationUser, null!);
    }

    public static SaveFileHistoryRecord ToFileHistoryWithNoticeOfDisputeId(string noticeOfDisputeId, FileHistoryAuditLogEntryType auditLogEntryType, string actionByApplicationUser, string comment)
    {
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.NoticeOfDisputeId = noticeOfDisputeId;
        fileHistoryRecord.AuditLogEntryType = auditLogEntryType;
        fileHistoryRecord.ActionByApplicationUser = actionByApplicationUser;
        // TCVP-2457 - Only allow Staff File Remark type audit log entry to be saved with comments.
        fileHistoryRecord.Comment = auditLogEntryType == FileHistoryAuditLogEntryType.FRMK ? comment : null;
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
        DisputeCancelled disputeCancelled = new()
        {
            Id = dispute.DisputeId,
            Email = dispute.EmailAddress,
            TicketNumber = dispute.TicketNumber,
            NoticeOfDisputeGuid = new Guid(dispute.NoticeOfDisputeGuid),
            Reason = dispute.RejectedReason,
        };        
        return disputeCancelled;
    }
}
