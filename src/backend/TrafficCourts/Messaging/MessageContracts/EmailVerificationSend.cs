using Microsoft.Extensions.Primitives;
using System.Security.Permissions;

namespace TrafficCourts.Messaging.MessageContracts;

/// <summary>
/// Interface message contract for sending verifying emails. An email would go out to an end user with a verification link so they can confirm the address is correct.
/// </summary>
public class EmailVerificationSend
{
    public EmailVerificationSend(Guid noticeOfDisputeGuid, string ticketNumber, string emailAddress)
    {
        NoticeOfDisputeGuid = noticeOfDisputeGuid;
        TicketNumber = ticketNumber;
        EmailAddress = emailAddress;
    }

    public Guid NoticeOfDisputeGuid { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; }
}