using TrafficCourts.Messaging.Models;
using TrafficCourts.Messaging.MessageContracts;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public class ProcessingDisputeEmailTemplate : EmailTemplate<DisputeApproved>, IProcessingDisputeEmailTemplate
{
    public override EmailMessage Create(DisputeApproved data)
    {
        EmailMessage email = new()
        {
            From = Sender,
            To = data.Email,
            Subject = $"Ticket Dispute for {data.TicketFileNumber} is being processed.",
            TextContent = $@"
Your dispute for ticket {data.TicketFileNumber} is now being processed. You will receive more information once processing is complete, to the contact address or email address that was provided in your dispute. Please note:  If you have filed a dispute requesting a Hearing, this may take several months before a Hearing date is scheduled.  If you need to update your dispute, please use the “Manage / Update Dispute” link on the Ticket information in British Columbia website.

You may update your dispute online up to 5 business days prior to a set Hearing Date.

If you have questions about the status of your Violation Ticket, contact the {ViolationTicketCentreContact}.

{TechnicalHelpText}"
        };

        return email;
    }
}
