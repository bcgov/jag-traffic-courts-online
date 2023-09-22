using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Core.Http;

public class OidcConfidentialClient : IOidcConfidentialClient
{
    private readonly OidcConfidentialClientConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<OidcConfidentialClient> _logger;

    public OidcConfidentialClient(OidcConfidentialClientConfiguration configuration, IMemoryCache memoryCache, ILogger<OidcConfidentialClient> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _memoryCache = memoryCache;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Token?> RequestAccessTokenAsync(CancellationToken cancellationToken)
    {
        string key = GetCacheKey();

        if (_memoryCache.TryGetValue<Token>(key, out var token))
        {
            return token;
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;
        token = await GetTokenAsync(cancellationToken);

        if (token is not null)
        {
            _memoryCache.Set(key, token, now.AddSeconds(token.ExpiresIn - 10));
        }

        return token;
    }

    private async Task<Token?> GetTokenAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Requesting access token");

        var data = new Dictionary<string, string>
        {
            {"client_id", _configuration.ClientId},
            {"client_secret", _configuration.ClientSecret},
            {"scope", "openid"},
            {"grant_type", "client_credentials"}
        };

        using var content = new FormUrlEncodedContent(data);
        var request = new HttpRequestMessage(HttpMethod.Post, _configuration.TokenEndpoint) { Content = content };
        request.Headers.Add("Accept", "application/json");

        _logger.LogDebug("Sending OIDC access token request to server");
        using HttpClient httpClient = new HttpClient(); // tODO: use HttpClientFactory
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            Token? token = await response.Content.ReadFromJsonAsync(SerializerContext.Default.Token, cancellationToken);
            if (token is null)
            {
                _logger.LogInformation("Could not deserialize OIDC token, returning null");
                // should probably throw?
                return null;
            }

            return token;
        }
        else
        {
            // should probably throw?
            _logger.LogInformation("Access token request failed with {StatusCode}, returning null", response.StatusCode);
            return null;
        }

    }

    private string GetCacheKey()
    {
        string[] values = new string[]
        {
            _configuration.TokenEndpoint?.ToString() ?? string.Empty,
            _configuration.ClientId ?? string.Empty,
            _configuration.ClientSecret ?? string.Empty
        };

        string value = string.Join("|", values);

        byte[] hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(value));
        string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

        return hashString;
    }
}
