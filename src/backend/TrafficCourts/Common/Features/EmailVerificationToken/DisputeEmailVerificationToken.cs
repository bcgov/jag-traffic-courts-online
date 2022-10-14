namespace TrafficCourts.Common.Features.EmailVerificationToken;

/// <summary>
/// Represents a dispute and associated validation token.
/// </summary>
public class DisputeEmailVerificationToken
{
    public Guid NoticeOfDisputeId { get; set; }
    public Guid Token { get; set; }
}
