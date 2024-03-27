using TrafficCourts.Messaging.Models;
using TrafficCourts.Domain.Models;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class DisputeUpdateRequestAcceptedTemplate : EmailTemplate<Dispute>, IDisputeUpdateRequestAcceptedTemplate
{
    public override EmailMessage Create(Dispute data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.EmailAddress,
            Subject = $"TBD (ie. Disputant contact information changes approved)",
            TextContent = $@"
TBD (ie. Your requested contact changes have been approved.)

If you have questions about the status of your Violation Ticket, contact the {ViolationTicketCentreContact}."
        };

        return email;
    }
}
