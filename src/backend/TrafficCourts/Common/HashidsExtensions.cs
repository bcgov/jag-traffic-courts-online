using HashidsNet;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common
{
    public static class HashidsExtensions
    {

        /// <summary>
        /// Encodes the provided Guid.
        /// </summary>
        /// <param name="hashids">The provider to encode</param>
        /// <param name="value">The value to encode.</param>
        /// <returns>The encoded Guid.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hashids"/> is null</exception>
        public static string EncodeGuid(this IHashids hashids, Guid value)
        {
            ArgumentNullException.ThrowIfNull(hashids);

            // "n" format specifier - 32 digits without hyphens, braces or parentheses
            // example: 00000000000000000000000000000000
            return hashids.EncodeHex(value.ToString("n"));
        }

        /// <summary>
        /// Tries to decode a string into a value. 
        /// </summary>
        /// <param name="hashids"></param>
        /// <param name="value">The string to decode.</param>
        /// <param name="result"></param>
        /// <returns><c>true</c> if <paramref name="value"/> was successfully decoded; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hashids"/> is null</exception>
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
    }
}
