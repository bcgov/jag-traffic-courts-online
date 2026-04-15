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

/// <summary>
/// This event is published when a disputant attempts to verify their email address using a valid token.
/// </summary>
public class EmailVerificationSuccessful : EmailVerificationMessage
{
    public DateTimeOffset VerifiedAt { get; set; }
}

/// <summary>
/// This command is issued when we want to start or 
/// restart email verification on a given dispute.
/// </summary>
public class RequestEmailVerification : EmailVerificationMessage;

/// <summary>
/// This command is issued to re-send a verification email.
/// </summary>
public class ResendEmailVerificationEmail : EmailVerificationMessage;

/// <summary>
/// This command is issued to create and send a validation email.
/// </summary>
public class SendEmailVerificationEmail : EmailVerificationMessage
{
    /// <summary>
    /// The token used for validation.
    /// </summary>
    public Guid Token { get; set; } = Guid.Empty;
}
