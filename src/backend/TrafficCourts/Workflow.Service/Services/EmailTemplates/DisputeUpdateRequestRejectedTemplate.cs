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
            Subject = $"TBD (ie. Disputant contact information changes rejected)",
            TextContent = $@"
TBD (ie. Your requested contact changes have been rejected.)

If you have questions about the status of your Violation Ticket, contact the {ViolationTicketCentreContact}."
        };

        return email;
    }
}
