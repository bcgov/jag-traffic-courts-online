namespace TrafficCourts.Messaging.Models;

public enum CheckEmailVerificationTokenStatus
{
    /// <summary>
    /// No active email validation in progress
    /// </summary>
    NotFound = 0,

    /// <summary>
    /// The supplied token is valid.
    /// </summary>
    Valid,

    /// <summary>
    /// The supplied token is not correct.
    /// </summary>
    Invalid
}
