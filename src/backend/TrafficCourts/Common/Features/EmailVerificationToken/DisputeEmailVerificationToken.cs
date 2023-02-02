using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Features.EmailVerificationToken;

/// <summary>
/// Represents a dispute and associated validation token.
/// </summary>
[ExcludeFromCodeCoverage]
public class DisputeEmailVerificationToken
{
    public Guid NoticeOfDisputeGuid { get; set; }
    public Guid Token { get; set; }
}
