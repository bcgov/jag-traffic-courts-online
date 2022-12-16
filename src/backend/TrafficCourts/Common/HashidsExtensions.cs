using HashidsNet;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common
{
    public static class HashidsExtensions
    {
        public static bool TryDecodeGuid(this IHashids hashids, [NotNullWhen(true)] string value, out Guid result)
        {
            ArgumentNullException.ThrowIfNull(hashids);

            if (!String.IsNullOrWhiteSpace(value))
            {
                var hex = hashids.DecodeHex(value);
                if (hex.Length == 32)
                {
                    // cant see why this would ever return false
                    // IHashids.DecodeHex should only return valid hex characters based on it's contract
                    // but we will play it safe in case
                    return Guid.TryParse(hex, out result);
                }
            }

            result = default;
            return false;
        }

        public static string EncodeGuid(this IHashids hashids, Guid value)
        {
            ArgumentNullException.ThrowIfNull(hashids);

            // "n" format specifier - 32 digits without hyphens, braces or parentheses
            // example: 00000000000000000000000000000000
            return hashids.EncodeHex(value.ToString("n"));
        }
    }
}
