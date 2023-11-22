using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace TrafficCourts.Core.Http;

public class OidcConfidentialClientConfiguration
{
    private string _cacheKey;

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public Uri? TokenEndpoint { get; set; }

    /// <summary>
    /// Computes the cache key for this configuration
    /// </summary>
    /// <returns></returns>
    public string GetCacheKey()
    {
        // note, this does not handle if the properties are modified after computing the cache key
        _cacheKey ??= ComputeCacheKey();
        return _cacheKey;
    }

    // Qualify as uncommon-path
    [MethodImpl(MethodImplOptions.NoInlining)]
    private string ComputeCacheKey()
    {
        string[] values = new string[]
{
            TokenEndpoint?.ToString() ?? string.Empty,
            ClientId ?? string.Empty,
            ClientSecret ?? string.Empty
};

        string value = string.Join("|", values);

        byte[] hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(value));
        string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hashString;
    }
}
