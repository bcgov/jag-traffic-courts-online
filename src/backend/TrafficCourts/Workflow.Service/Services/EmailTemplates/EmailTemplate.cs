using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

/// <summary>
/// Base class for email templates
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EmailTemplate<T> : IEmailTemplate<T>
{
    /// <summary>
    /// Provides sender email address.
    /// </summary>
    protected const string Sender = "DoNotReply.TCO@gov.bc.ca";

    /// <summary>
    /// Provides the common help text.
    /// </summary>
    //protected const string HelpText = "If you need more help, contact the Violation Ticket Centre toll free 1-877-661-8026, open weekdays 9am to 4pm.";

    /// <summary>
    /// Provides the common technical help text.
    /// </summary>
    protected const string TechnicalHelpText = "If you are having technical difficulties with the Ticket information in British Columbia website, please email Courts.TCO@gov.bc.ca";

    protected const string ViolationTicketCentreContact = "Violation Ticket Centre toll free 1-877-661-8026, open weekdays 9am to 4pm";

    public abstract EmailMessage Create(T data);
}


public record EmailFields(string EmailAddress, string TicketNumber);
