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
            Subject = "TBD (ie. Dispute update request(s) received)",
            TextContent = $@"
TBD (ie. Your request to update dispute information was received)

If you have questions about the status of your Violation Ticket, contact the {ViolationTicketCentreContact}."
        };

        return email;
    }
}
