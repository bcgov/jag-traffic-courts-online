using HashidsNet;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Features.EmailVerificationToken;

public class DisputeEmailVerificationTokenEncoder : IDisputeEmailVerificationTokenEncoder
{
    private const string GuidFormat = "n";
    private static readonly int GuidLength = Guid.Empty.ToString(GuidFormat).Length;
    private readonly IHashids _hashids;

    public DisputeEmailVerificationTokenEncoder(IHashids hashids)
    {
        _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));
    }

    public string Encode(DisputeEmailVerificationToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        string hex = token.NoticeOfDisputeId.ToString(GuidFormat) + token.Token.ToString(GuidFormat);
        return _hashids.EncodeHex(hex);
    }

    public bool TryDecode(string hash, [NotNullWhen(true)] out DisputeEmailVerificationToken? token)
    {
        if (hash is null)
        {
            token = default;
            return false;
        }

        // DecodeHex returns empty string if hash is empty or not a valid hex string
        string hex = _hashids.DecodeHex(hash);
        if (hex.Length != GuidLength * 2)
        {
            // too short or malformed
            token = default;
            return false;
        }

        token = new DisputeEmailVerificationToken
        {
            NoticeOfDisputeId = new Guid(hex[..GuidLength]),
            Token = new Guid(hex.Substring(GuidLength, GuidLength))
        };

        return true;
    }
}
