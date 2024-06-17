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
This is to advise you that your online dispute for ticket {data.TicketNumber} has been cancelled.
The staff member who reviewed your dispute has provided the following message:
    {data.Reason}

Do not reply to this email. If you have any questions, please email Courts.TCO@gov.bc.ca."
        };

        return email;
    }
}
