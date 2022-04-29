﻿using TrafficCourts.Common.Features.Mail.Model;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;

namespace TrafficCourts.Staff.Service.Mappers;

public class Mapper
{
    public static DisputeApproved ToDisputeApproved(Dispute dispute)
    {
        DisputeApproved target = new();
        target.CitizenName = dispute.GivenNames + " " + dispute.DisputantSurname;
        target.TicketIssuanceDate = dispute.ViolationDate.DateTime;
        target.TicketFileNumber = dispute.TicketNumber;
        target.IssuingOrganization = dispute.EnforcementOrganization;
        target.IssuingLocation = dispute.CourtLocation;
        target.DriversLicence = dispute.DriversLicense;
        target.TicketDetails = new List<TicketDetails>(); // FIXME: Dispute model missing details
        target.StreetAddress = dispute.StreetAddress;
        target.City = ""; // FIXME: no City field in Dispute model
        target.Province = dispute.Province;
        target.PostalCode = dispute.PostalCode;
        target.Email = dispute.EmailAddress;
        target.DisputeDetails = Array.Empty<Dictionary<string, DisputeDetails>>(); // TODO: populate from model

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
            sendEmail.Subject = template.SubjectTemplate;
            sendEmail.PlainTextContent = template.PlainContentTemplate?.Replace("<ticketid>", dispute.TicketNumber);
            
        }
        return sendEmail;

    }
}
