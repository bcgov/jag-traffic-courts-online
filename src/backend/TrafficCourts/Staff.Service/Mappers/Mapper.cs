using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Mappers;

public class Mapper
{
    public static DisputeApproved ToDisputeApproved(Dispute dispute)
    {
        DisputeApproved target = new();
        target.CitizenName = dispute.GivenNames + " " + dispute.Surname;
        target.TicketIssuanceDate = dispute.IssuedDate.HasValue ? dispute.IssuedDate.Value.DateTime : (DateTime?)null;
        target.TicketFileNumber = dispute.TicketNumber;
        target.IssuingOrganization = dispute.ViolationTicket.OrganizationLocation;
        target.IssuingLocation = dispute.ProvincialCourtHearingLocation;
        target.DriversLicence = dispute.ViolationTicket.DriversLicenceNumber;
        List <Messaging.MessageContracts.ViolationTicketCount> violationTicketCounts = new();
        foreach (var violationTicketCount in dispute.ViolationTicket.ViolationTicketCounts)
        {
            Messaging.MessageContracts.ViolationTicketCount ticketCount = new()
            {
                Count = violationTicketCount.Count,
                Section = violationTicketCount.FullSection,
                Act = violationTicketCount.ActRegulation,
                Amount = violationTicketCount.TicketedAmount
            };

            violationTicketCounts.Add(ticketCount);
        }
        target.ViolationTicketCounts = violationTicketCounts;
        target.StreetAddress = dispute.Address;
        target.City = dispute.City;
        target.Province = dispute.Province;
        target.PostalCode = dispute.PostalCode;
        target.Email = dispute.EmailAddress;
        List<Messaging.MessageContracts.DisputeCount> disputeCounts = new();
        foreach (var dc in dispute.DisputedCounts)
        {
            Messaging.MessageContracts.DisputeCount disputeCount = new()
            {
                Count = dc.Count,
                DisputeType = nameof(dc.Plea)
            };

            disputeCounts.Add(disputeCount);
        }
        target.DisputeCounts = disputeCounts;

        return target;
    }

    public static DisputeRejected ToDisputeRejected(Dispute dispute)
    {
        DisputeRejected disputeRejected = new();
        disputeRejected.Reason = dispute.RejectedReason;
        return disputeRejected;
    }

    public static DisputeCancelled ToDisputeCancelled(Dispute dispute)
    {
        DisputeCancelled disputeCancelled = new();
        disputeCancelled.Id = dispute.Id;
        disputeCancelled.Email = dispute.EmailAddress;
        return disputeCancelled;
    }

    public static SendEmail ToCancelSendEmail(Dispute dispute)
    {
        return ToSendEmail(dispute, "CancelledDisputeTemplate");
    }

    public static SendEmail ToRejectSendEmail(Dispute dispute)
    {
        return ToSendEmail(dispute, "RejectedDisputeTemplate");
    }

    public static SendEmail ToProcessingSendEmail(Dispute dispute)
    {
        return ToSendEmail(dispute, "ProcessingDisputeTemplate");
    }

    private static SendEmail ToSendEmail(Dispute dispute, string messageTemplateName)
    {
        SendEmail sendEmail = new();
        // Send email message to the submitter's entered email
        var template = MailTemplateCollection.DefaultMailTemplateCollection.FirstOrDefault(t => t.TemplateName == messageTemplateName);
        if (template is not null)
        {

            sendEmail.From = template.Sender;
            sendEmail.To.Add(dispute.EmailAddress);
            sendEmail.Subject = template.SubjectTemplate.Replace("<ticketid>", dispute.TicketNumber);
            sendEmail.PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", dispute.TicketNumber);
            
        }
        return sendEmail;

    }
}
