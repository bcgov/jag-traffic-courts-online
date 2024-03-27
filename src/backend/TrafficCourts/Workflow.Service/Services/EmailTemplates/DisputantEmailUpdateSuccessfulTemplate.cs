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
            Subject = "Email verification successful",
            TextContent = $@"
TBD (ie. Your email address has been updated successfully)

If you have questions about the status of your Violation Ticket, contact the {ViolationTicketCentreContact}."
        };

        return email;
    }
}
