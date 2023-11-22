using TrafficCourts.Core.Http;
using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Http;

/// <summary>
/// Represents a cache for security tokens.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TToken">The type of the token.</typeparam>
public interface ITokenCache
{
    /// <summary>
    /// Gets the security token from the cache.
    /// </summary>
    /// <param name="key">The key of the token.</param>
    /// <returns>The token or <c>null</c> if the token does not exist or is expired.</returns>
    Token? GetToken(OidcConfidentialClientConfiguration key);

    /// <summary>
    /// Saves the security token to the cache. The security token will be cached until it expires.
    /// </summary>
    /// <param name="key">The key of the token.</param>
    /// <param name="token">The token.</param>
    /// <param name="tokenExpiresAtUtc">The token expires at UTC.</param>
    /// <remarks>
    /// The implementation will subtract one minute from the supplied expiration date to prevent
    /// issues with time drift between machines.
    /// </remarks>
    void SaveToken(OidcConfidentialClientConfiguration key, Token token, DateTimeOffset tokenExpiresAtUtc);
}
