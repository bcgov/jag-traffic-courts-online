using TrafficCourts.Messaging.Models;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class CancelledDisputeEmailTemplate : EmailTemplate<DisputeCancelled>, ICancelledDisputeEmailTemplate
{
    public override EmailMessage Create(DisputeCancelled data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.Email,
            Subject = $"Ticket Dispute for {data.TicketNumber} cancelled.",
            TextContent = $@"
Your ticket has been canceled and will no longer be disputed.

If you need more help, contact the {ViolationTicketCentreContact}."
        };

        return email;
    }
}
