using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Features.EmailVerificationToken;

public interface IDisputeEmailVerificationTokenEncoder
{
    string Encode(DisputeEmailVerificationToken token);
    bool TryDecode(string hash, [NotNullWhen(true)] out DisputeEmailVerificationToken? token);
}
