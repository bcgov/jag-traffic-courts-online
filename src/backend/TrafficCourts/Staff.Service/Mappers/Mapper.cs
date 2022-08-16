﻿using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Staff.Service.Mappers;

public class Mapper
{
    public static DisputeApproved ToDisputeApproved(Dispute dispute)
    {
        DisputeApproved target = new();
        target.CitizenName = dispute.DisputantGivenName1 + " " + dispute.DisputantSurname;
        target.TicketIssuanceDate = dispute.IssuedDate?.DateTime;
        target.TicketFileNumber = dispute.ViolationTicket.TicketNumber;
        target.IssuingOrganization = dispute.ViolationTicket.DetachmentLocation;
        target.IssuingLocation = dispute.CourtLocation;
        target.DriversLicence = dispute.DriversLicenceNumber;
        List <Messaging.MessageContracts.ViolationTicketCount> violationTicketCounts = new();
        foreach (var violationTicketCount in dispute.ViolationTicket.ViolationTicketCounts)
        {
            Messaging.MessageContracts.ViolationTicketCount ticketCount = new()
            {
                Count = violationTicketCount.CountNo,
                FullSection = violationTicketCount.FullSection,
                Section = violationTicketCount.Section,
                Subsection = violationTicketCount.Subsection,
                Paragraph = violationTicketCount.Paragraph,
                Act = violationTicketCount.ActOrRegulationNameCode,
                Amount = violationTicketCount.TicketedAmount
            };

            if (violationTicketCount.DisputeCount != null)
            {
                Messaging.MessageContracts.DisputeCount disputeCount = new()
                {
                    CountNo = (short)violationTicketCount.DisputeCount.CountNo,
                    PleaCode = violationTicketCount.DisputeCount.PleaCode,
                    RequestCourtAppearance = violationTicketCount.DisputeCount.RequestCourtAppearance,
                    RequestReduction = violationTicketCount.DisputeCount.RequestReduction,
                    RequestTimeToPay = violationTicketCount.DisputeCount.RequestTimeToPay
                };
                ticketCount.DisputeCount = disputeCount;
            }

            violationTicketCounts.Add(ticketCount);
        }
        target.ViolationTicketCounts = violationTicketCounts;
        target.StreetAddress = dispute.Address;
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

    public static DisputeCancelled ToDisputeCancelled(Dispute dispute)
    {
        DisputeCancelled disputeCancelled = new();
        disputeCancelled.Id = dispute.DisputeId;
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
            sendEmail.Subject = template.SubjectTemplate.Replace("<ticketid>", dispute.ViolationTicket.TicketNumber);
            sendEmail.PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", dispute.ViolationTicket.TicketNumber);
            
        }
        return sendEmail;

    }
}
