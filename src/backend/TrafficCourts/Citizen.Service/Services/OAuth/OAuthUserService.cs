using StackExchange.Redis;
using System.Text.Json;
using System.Text;
using System.Buffers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Models.OAuth;
using System.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using FlatFiles.TypeMapping;
using System.Security.Cryptography;

namespace TrafficCourts.Citizen.Service.Services;

/// <summary>
/// Implentation of the IOAuthUserService that is used for retrieving user information from OAuth's services.
/// </summary>
public class OAuthUserService : IOAuthUserService
{
    private readonly Uri _userInfoEndpoint;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OAuthUserService> _logger;
    private readonly IRedisCacheService _redisCacheService;

    public OAuthUserService(Configuration.OAuthOptions options, HttpClient httpClient, ILogger<OAuthUserService> logger, IRedisCacheService redisCacheService)
    {
        ArgumentNullException.ThrowIfNull(options);
        _userInfoEndpoint = options.UserInfoEndpoint ?? throw new ArgumentException($"{nameof(options.UserInfoEndpoint)} is required");
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
    }

    public async Task<TrafficCourts.Citizen.Service.Models.OAuth.UserInfo?> GetUserInfoAsync<UserInfo>(string token, CancellationToken cancellationToken)
    {
        try
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", token);
            HttpResponseMessage response = await _httpClient.GetAsync(_userInfoEndpoint);
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var user = Encoding.UTF8.GetString(DecodeUrlBase64(responseBody.Split(".")[1]));

            // store in Redis Cache for an hour
            await CacheUserInfo(token, JsonSerializer.Deserialize<TrafficCourts.Citizen.Service.Models.OAuth.UserInfo>(user));

            return JsonSerializer.Deserialize<TrafficCourts.Citizen.Service.Models.OAuth.UserInfo>(user);
        }
        catch (Exception ex)
        {
            // attempt to retrieve from Redis cache using hashed access token
            TrafficCourts.Citizen.Service.Models.OAuth.UserInfo? user = await GetCachedUserInfo(token);
            return user;
        }
    }

    private async Task CacheUserInfo(string token, UserInfo? user) {
        try
        {
            var hashedToken = CreateMD5(token);

            await _redisCacheService.SetRecordAsync(hashedToken, user, new TimeSpan(2,0,0));
            return;
        } catch (Exception ex) {
            _logger.LogError(ex, "Could not cache user info.");
            throw;
        }
    }

    private async Task<UserInfo?> GetCachedUserInfo(string token)
    {
        try
        {
            var hashedToken = CreateMD5(token);
            TrafficCourts.Citizen.Service.Models.OAuth.UserInfo? user = await _redisCacheService.GetRecordAsync<TrafficCourts.Citizen.Service.Models.OAuth.UserInfo?>(hashedToken);
            if (user == null) { throw new FileNotFoundException("User not found"); }
            return user;
        } catch (Exception ex) {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// https://stackoverflow.com/questions/11454004/calculate-a-md5-hash-from-a-string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string CreateMD5(string input)
    {
        // Use input string to calculate MD5 hash
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return Convert.ToHexString(hashBytes); // .NET 5 +
        }
    }

    private static byte[] DecodeUrlBase64(string s)
    {
        s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
        return Convert.FromBase64String(s);
    }
}
