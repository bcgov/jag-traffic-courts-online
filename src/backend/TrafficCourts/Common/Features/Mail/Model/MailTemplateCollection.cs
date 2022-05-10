using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Common.Features.Mail.Model
{
    public class MailTemplateCollection
    {
        public static readonly List<MailTemplate> DefaultMailTemplateCollection = new List<MailTemplate>
        {
        new MailTemplate()
            {
                TemplateName =  "SubmitDisputeTemplate",
                Sender = "DoNotReply@tickets.gov.bc.ca",
                SubjectTemplate = "Ticket Dispute for <ticketid> submitted.",
                PlainContentTemplate = "The ticket <ticketid> has been submitted, and will be reviewed.\n\nIf you need more help, contact the Violation Ticket Centre toll free 1-877-661-8026, open weekdays 9am to 4pm."
            },
        new MailTemplate()
            {
                TemplateName = "CancelledDisputeTemplate",
                Sender = "DoNotReply@tickets.gov.bc.ca",
                SubjectTemplate = "Ticket Dispute for <ticketid> cancelled.",
                PlainContentTemplate = "Your ticket has been canceled and will no longer be disputed.\n\nIf you need more help, contact the Violation Ticket Centre toll free 1-877-661-8026, open weekdays 9am to 4pm."
            },
        new MailTemplate()
            {
                TemplateName = "RejectedDisputeTemplate",
                Sender = "DoNotReply@tickets.gov.bc.ca",
                SubjectTemplate = "Ticket Dispute for <ticketid> rejected.",
                PlainContentTemplate = "Your dispute has been rejected and will not be processed.\n\nIf you need more help, contact the Violation Ticket Centre toll free 1-877-661-8026, open weekdays 9am to 4pm."
            },
        new MailTemplate()
            {
                TemplateName = "ProcessingDisputeTemplate",
                Sender = "DoNotReply@tickets.gov.bc.ca",
                SubjectTemplate = "Ticket Dispute for <ticketid> is being processed.",
                PlainContentTemplate = "Your dispute is now being processed. You will receive more information once processing is complete.\n\nIf you need more help, contact the Violation Ticket Centre toll free 1-877-661-8026, open weekdays 9am to 4pm."
            }
        };

        public List<MailTemplate> MailTemplates { get; set; } = new List<MailTemplate>();
    }

    public class MailTemplate
    {
        public string TemplateName { get; set; } = String.Empty;
        public string? Sender { get; set; }
        public string SubjectTemplate { get; set; } = String.Empty;
        public string? PlainContentTemplate { get; set; }
        public string? HtmlContentTemplate { get; set; }
    }
}
