using TrafficCourts.Messaging.Models;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class RejectedDisputeEmailTemplate : EmailTemplate<DisputeRejected>, IRejectedDisputeEmailTemplate
{
    public override EmailMessage Create(DisputeRejected data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.Email,
            Subject = $"Ticket Dispute for {data.TicketNumber} rejected.",
            TextContent = $@"
Your dispute has been rejected and will not be processed.

If you have questions about the status of your Violation Ticket, contact the {ViolationTicketCentreContact}."
        };

        return email;
    }
}
