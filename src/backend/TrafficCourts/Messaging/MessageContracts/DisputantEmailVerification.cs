using MassTransit;
using TrafficCourts.Messaging.Models;

namespace TrafficCourts.Messaging.MessageContracts;

public class CheckEmailVerificationTokenRequest
{
    /// <summary>
    /// The notice of dispute id.
    /// </summary>
    public Guid NoticeOfDisputeGuid { get; set; }

    /// <summary>
    /// The token encoded in the email used to validate the 
    /// email was received.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;
}

public class CheckEmailVerificationTokenResponse
{
    public CheckEmailVerificationTokenStatus Status { get; set; }
    public DateTimeOffset CheckedAt { get; set; }
}

/// <summary>
/// Common base class for email verification messages.
/// </summary>
[ExcludeFromTopology]
public abstract class EmailVerificationMessage
{
    /// <summary>
    /// The notice of dispute id.
    /// </summary>
    public Guid NoticeOfDisputeGuid { get; set; }

    /// <summary>
    /// The ticket number associated with the dispute.
    /// </summary>
    public string TicketNumber { get; set; } = String.Empty;

    /// <summary>
    /// The email address that is the target of verification.
    /// </summary>
    public string EmailAddress { get; set; } = String.Empty;

    /// <summary>
    /// Is the message published for verification of a new email address as part of an update request.
    /// </summary>
    public bool IsUpdateEmailVerification { get; set; }
}

public class EmailVerificationSuccessful : EmailVerificationMessage
{
    public long DisputeId { get; set; }
    public DateTimeOffset VerifiedAt { get; set; }
}

public class UpdateEmailVerificationSuccessful : EmailVerificationSuccessful
{
}

/// <summary>
/// Indicates a new dispute has been submitted (created).
/// </summary>
public class NoticeOfDisputeSubmitted
{
    public long DisputeId { get; set; }
    public Guid NoticeOfDisputeGuid { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public bool RequiresEmailVerification { get; set; } = true;
}

/// <summary>
/// Indicates one or more disputant update request has been submitted (created).
/// </summary>
public class DisputantUpdateRequestSubmitted
{
    public long DisputeId { get; set; }
    public Guid NoticeOfDisputeGuid { get; set; }
    public string EmailAddress { get; set; } = string.Empty;
    public bool RequiresEmailVerification { get; set; } = true;
}


/// <summary>
/// This event is raised when we want to start or 
/// restart email verification on a given dispute.
/// </summary>
public class RequestEmailVerification : EmailVerificationMessage
{
    /// <summary>
    /// The optional dispute id if we have it already.
    /// it will be set when re-requesting for email 
    /// validation.
    /// </summary>
    public long? DisputeId { get; set; }
}

public class ResendEmailVerificationEmail : EmailVerificationMessage
{
}

/// <summary>
/// This event is raised when an email message should be created
/// and sent to the target email address for email validation.
/// </summary>
public class SendEmailVerificationEmail : EmailVerificationMessage
{
    /// <summary>
    /// The token encoded in the email used to validate the 
    /// email was received.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;
}


/// <summary>
/// This even is raised when the email could not be sent for 
/// validating email.
/// </summary>
public class SendEmailVerificationFailed : EmailVerificationMessage
{
    /// <summary>
    /// The reason the email could not be set.
    /// </summary>
    public string Reason { get; set; } = String.Empty;
}
