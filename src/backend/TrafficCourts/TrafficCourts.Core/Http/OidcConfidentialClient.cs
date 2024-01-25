using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using TrafficCourts.Core.Http.Models;
using TrafficCourts.Http;

namespace TrafficCourts.Core.Http;

public partial class OidcConfidentialClient : IOidcConfidentialClient
{
    private readonly OidcConfidentialClientConfiguration _configuration;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeProvider _clock;
    private readonly ILogger<OidcConfidentialClient> _logger;

    public OidcConfidentialClient(OidcConfidentialClientConfiguration configuration, IMemoryCache memoryCache, TimeProvider clock, ILogger<OidcConfidentialClient> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Token?> RequestAccessTokenAsync(CancellationToken cancellationToken)
    {
        string key = GetCacheKey();

        if (_memoryCache.TryGetValue<Token>(key, out var token))
        {
            if (token is not null)
            {
                LogGotTokenFromCache(key, token);
                return token;
            }
        }

        // token was not available in the cache, we need to request a new one

        DateTimeOffset now = _clock.GetUtcNow();
        token = await GetTokenAsync(cancellationToken);

        if (token is not null)
        {
            var expiresAt = now.AddSeconds(token.ExpiresIn - 10);
            _memoryCache.Set(key, token, expiresAt);
            LogGotNewToken(key, token, expiresAt);
        }
        else
        {
            LogNoTokenAvailable();
        }

        return token;
    }

    private async Task<Token?> GetTokenAsync(CancellationToken cancellationToken)
    {
        LogRequestingNewToken();

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

        using HttpClient httpClient = new HttpClient(); // tODO: use HttpClientFactory
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            Token? token = await response.Content.ReadFromJsonAsync(SerializerContext.Default.Token, cancellationToken);
            if (token is null)
            {
                LogTokenDeserializationError();
                // should probably throw?
                return null;
            }

            return token;
        }
        else
        {
            LogTokenRequestNotSuccessStatusCode(response.StatusCode);
            // should probably throw?
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

    [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "Got token from cache")]
    public partial void LogGotTokenFromCache(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordCacheKeyTag), OmitReferenceName = true)]
        string cacheKey,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        Token token);

    [LoggerMessage(EventId = 1, Level = LogLevel.Trace, Message = "Got new token")]
    public partial void LogGotNewToken(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordCacheKeyTag), OmitReferenceName = true)]
        string cacheKey,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        Token token,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        DateTimeOffset expiresAt);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "No token is available")]
    public partial void LogNoTokenAvailable();

    [LoggerMessage(EventId = 3, Level = LogLevel.Debug, Message = "Requesting new access token")]
    public partial void LogRequestingNewToken();

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Could not deserialize OIDC token, returning null")]
    public partial void LogTokenDeserializationError();

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Access token request failed, returning null")]
    public partial void LogTokenRequestNotSuccessStatusCode(
        HttpStatusCode statusCode);

}
