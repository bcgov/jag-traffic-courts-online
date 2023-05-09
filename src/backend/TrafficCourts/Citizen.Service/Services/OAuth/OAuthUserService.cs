using System.Text.Json;
using System.Text;
using TrafficCourts.Citizen.Service.Models.OAuth;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Distributed;

namespace TrafficCourts.Citizen.Service.Services;

/// <summary>
/// Implentation of the IOAuthUserService that is used for retrieving user information from OAuth's services.
/// </summary>
public class OAuthUserService : IOAuthUserService
{
    private readonly Uri _userInfoEndpoint;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OAuthUserService> _logger;
    private readonly IDistributedCache _cache;

    private static readonly DistributedCacheEntryOptions CacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) };


    public OAuthUserService(Configuration.OAuthOptions options, HttpClient httpClient, ILogger<OAuthUserService> logger, IDistributedCache cache)
    {
        ArgumentNullException.ThrowIfNull(options);
        _userInfoEndpoint = options.UserInfoEndpoint ?? throw new ArgumentException($"{nameof(options.UserInfoEndpoint)} is required");
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<UserInfo?> GetUserInfoAsync(string token, CancellationToken cancellationToken)
    {
        // create the cache key
        var cacheKey = CreateKey(token);

        // check to see if we have the user info already
        UserInfo? userInfo = await GetCacheUserInfoAsync(token, cancellationToken);

        if (userInfo is not null)
        {
            return userInfo;
        }

        // dont have the user info, get it from the user info endpoint
        try
        {
            string? jwt = await GetUserInfoFromUserInfoEndpointAsync(token, cancellationToken);
            if (jwt is not null)
            {
                userInfo = await DeserializeAndCacheAsync(cacheKey, jwt, cancellationToken);
            }

            // if the jwt was null, we would be returning null
            return userInfo;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error getting user info, user info is not available");
            return null;
        }
    }

    /// <summary>
    /// Makes the request to the user info endpoint
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<string?> GetUserInfoFromUserInfoEndpointAsync(string token, CancellationToken cancellationToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = await _httpClient.GetAsync(_userInfoEndpoint, cancellationToken);
            string jwt = await response.Content.ReadAsStringAsync(cancellationToken);
            return jwt;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _logger.LogError(exception, "User access token not valid at user info endpoint, user info is not available");
            return null;
        }
    }

    private async Task<UserInfo?> DeserializeAndCacheAsync(string cacheKey, string jwt, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);
        ArgumentNullException.ThrowIfNull(jwt);

        // extract the payload (jwt format:  header payload signature)
        var parts = jwt.Split(".");
        if (parts.Length < 2)
        {
            // invalid response
            return null;
        }

        var encodedPayload = parts[1];
        try
        {
            var payload = DecodeUrlBase64(encodedPayload);
            var userInfo = JsonSerializer.Deserialize<UserInfo>(payload);

            // cache the payload after deserialize to ensure invalid data is not cached
            await _cache.SetAsync(cacheKey, payload, CacheEntryOptions, cancellationToken);

            return userInfo;
        }
        catch (FormatException exception)
        {
            // base 64 is invalid
            _logger.LogError(exception, "Error decoding JWT payload as base64 url string, user info is not available");
            return null;
        }
        catch (JsonException exception)
        {
            // user info deserialization problem
            _logger.LogError(exception, "Error deserializing JWT payload as UserInfo, user info is not available");
            return null;
        }
    }

    private async Task<UserInfo?> GetCacheUserInfoAsync(string cacheKey, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cacheKey);

        try
        {
            UserInfo? user = await _cache.GetJsonAsync<UserInfo>(cacheKey, cancellationToken);
            return user;
        }
        catch (JsonException exception)
        {
            // could happen if someone changes the object serialized
            _logger.LogError(exception, "JSON Deserialization error getting user info");
            return null;
        }
    }

    /// <summary>
    /// Create a hash of the input for creation of cache key
    /// </summary>
    private static string CreateKey(string input) => Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(input ?? string.Empty)));

    private static byte[] DecodeUrlBase64(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
        return Convert.FromBase64String(s);
    }
}
