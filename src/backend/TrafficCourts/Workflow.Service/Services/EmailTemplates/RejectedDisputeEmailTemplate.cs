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
This is to advise you that your online dispute for ticket {data.TicketNumber} has been rejected. Do not reply to this email.

If you have any questions, please email Courts.TCO@gov.bc.ca."
        };

        return email;
    }
}
