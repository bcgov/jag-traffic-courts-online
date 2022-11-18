using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Staff.Service.Mappers;

public class Mapper
{
    public static DisputeApproved ToDisputeApproved(Dispute dispute)
    {
        DisputeApproved target = new();
        target.CitizenName = dispute.DisputantGivenName1 + " " + (dispute.DisputantGivenName2 is not null ? (dispute.DisputantGivenName2 + " ") : "") + (dispute.DisputantGivenName3 is not null ? (dispute.DisputantGivenName3 + " ") : "") + dispute.DisputantSurname;
        target.TicketIssuanceDate = dispute.IssuedTs?.DateTime;
        target.TicketFileNumber = dispute.TicketNumber;
        target.IssuingOrganization = dispute.ViolationTicket.DetachmentLocation;
        target.IssuingLocation = dispute.ViolationTicket.CourtLocation;
        target.DriversLicence = dispute.DriversLicenceNumber;
        List <Messaging.MessageContracts.ViolationTicketCount> violationTicketCounts = new();
        foreach (var violationTicketCount in dispute.ViolationTicket.ViolationTicketCounts)
        {
            Messaging.MessageContracts.ViolationTicketCount ticketCount = new()
            {
                Count = violationTicketCount.CountNo,
                Subparagraph = violationTicketCount.Subparagraph,
                Section = violationTicketCount.Section,
                Subsection = violationTicketCount.Subsection,
                Paragraph = violationTicketCount.Paragraph,
                Act = violationTicketCount.ActOrRegulationNameCode,
                Amount = violationTicketCount.TicketedAmount
            };

            violationTicketCounts.Add(ticketCount);
        }

        List<Messaging.MessageContracts.DisputedCount> disputeCounts = new();
        foreach (var dc in dispute.DisputeCounts)
        {
            Messaging.MessageContracts.DisputedCount disputeCount = new()
            {
                Count = dc.CountNo,
                DisputeType = nameof(dc.PleaCode)
            };

            disputeCounts.Add(disputeCount);
        }
        target.DisputeCounts = disputeCounts;
        target.ViolationTicketCounts = violationTicketCounts;
        target.StreetAddress = dispute.AddressLine1 + ((dispute.AddressLine2 is null) ? "" : ", " + dispute.AddressLine2) + ((dispute.AddressLine3 is null) ? "" : ", " + dispute.AddressLine3);
        target.City = dispute.AddressCity;
        target.Province = dispute.AddressProvince;
        target.PostalCode = dispute.PostalCode;
        target.Email = dispute.EmailAddress;

        return target;
    }

    public static DisputeRejected ToDisputeRejected(Dispute dispute)
    {
        DisputeRejected disputeRejected = new();
        disputeRejected.Reason = dispute.RejectedReason;
        return disputeRejected;
    }

    public static SaveFileHistoryRecord ToFileHistory(string ticketNumber, string description)
    {
        SaveFileHistoryRecord fileHistoryRecord = new();
        fileHistoryRecord.TicketNumber = ticketNumber;
        fileHistoryRecord.Description = description;
        return fileHistoryRecord;
    }

    public static EmailVerificationSend ToEmailVerification(Guid uuid)
    {
        EmailVerificationSend emailVerificationSend = new(uuid);
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
