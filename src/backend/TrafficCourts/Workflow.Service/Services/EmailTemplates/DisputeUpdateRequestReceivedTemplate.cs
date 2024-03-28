using TrafficCourts.Messaging.Models;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class DisputeUpdateRequestReceivedTemplate : EmailTemplate<Dispute>, IDisputeUpdateRequestReceivedTemplate
{
    public override EmailMessage Create(Dispute data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.EmailAddress,
            Subject = $"Update Dispute for {data.TicketNumber} submitted.",
            TextContent = $@"
Thank you for updating your dispute for ticket {data.TicketNumber}. You will receive further communication to the email or mailing address provided. Do not reply to this email. If you have any questions, please email Courts.TCO@gov.bc.ca."
        };

        return email;
    }
}
