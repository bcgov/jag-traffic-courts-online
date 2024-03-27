using TrafficCourts.Messaging.Models;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class DisputeSubmittedEmailTemplate : EmailTemplate<Dispute>, IDisputeSubmittedEmailTemplate
{
    public override EmailMessage Create(Dispute data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.EmailAddress,
            Subject = $"Ticket Dispute for {data.TicketNumber} submitted.",
            TextContent = $@"Thank you for using the Ticket information in British Columbia online dispute process. Ticket {data.TicketNumber} has been submitted and will be reviewed. You will receive further communication to the email or mailing address provided. Do not reply to this email. If you have any questions, please email Courts.TCO@gov.bc.ca."
        };

        return email;
    }
}
