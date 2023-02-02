using StackExchange.Redis;
using System.Text.Json;
using System.Text;
using System.Buffers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using TrafficCourts.Citizen.Service.Models.OAuth;
using System.Configuration;

namespace TrafficCourts.Citizen.Service.Services
{
    /// <summary>
    /// Implentation of the IOAuthUserService that is used for retrieving user information from OAuth's services.
    /// </summary>
    public class OAuthUserService : IOAuthUserService
    {
        private readonly Uri _userInfoEndpoint;
        private readonly HttpClient _httpClient;
        private readonly ILogger<OAuthUserService> _logger;

        public OAuthUserService(Configuration.OAuthOptions options, HttpClient httpClient, ILogger<OAuthUserService> logger)
        {
            ArgumentNullException.ThrowIfNull(options);
            _userInfoEndpoint = options.UserInfoEndpoint ?? throw new ArgumentException($"{nameof(options.UserInfoEndpoint)} is required");
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserInfo?> GetUserInfoAsync<UserInfo>(string token, CancellationToken cancellationToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", token);
                HttpResponseMessage response = await _httpClient.GetAsync(_userInfoEndpoint);
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var user = Encoding.UTF8.GetString(Convert.FromBase64String(responseBody.Split(".")[1]));
                return JsonSerializer.Deserialize<UserInfo>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not retrieve user information from OAuth's userInfo service");
                throw;
            }
        }
    }
}
