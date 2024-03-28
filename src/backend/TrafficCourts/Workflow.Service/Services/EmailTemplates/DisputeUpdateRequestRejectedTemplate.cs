using TrafficCourts.Messaging.Models;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class DisputeUpdateRequestRejectedTemplate : EmailTemplate<Dispute>, IDisputeUpdateRequestRejectedTemplate
{
    public override EmailMessage Create(Dispute data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.EmailAddress,
            Subject = $"Update Dispute for {data.TicketNumber} rejected.",
            TextContent = $@"This is to advise you that your online dispute update for ticket {data.TicketNumber} has been rejected. Do not reply to this email. 

If you have any questions, please email Courts.TCO@gov.bc.ca."
        };

        return email;
    }
}
