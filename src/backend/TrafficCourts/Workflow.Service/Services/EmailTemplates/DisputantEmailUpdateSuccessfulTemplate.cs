using TrafficCourts.Messaging.Models;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class DisputantEmailUpdateSuccessfulTemplate : EmailTemplate<Dispute>, IDisputantEmailUpdateSuccessfulTemplate
{
    public override EmailMessage Create(Dispute data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.EmailAddress,
            Subject = $"Update Dispute for {data.TicketNumber} processed",
            TextContent = $@"This is to advise you that your request to update ticket {data.TicketNumber} has now been processed. Further communication may be sent to the email or mailing address provided. Do not reply to this email. If you have any questions please refer to the website at tickets.gov.bc.ca or email Courts.TCO@gov.bc.ca."
        };

        return email;
    }
}
