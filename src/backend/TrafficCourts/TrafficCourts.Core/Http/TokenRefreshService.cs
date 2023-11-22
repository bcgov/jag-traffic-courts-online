using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TrafficCourts.Core.Http;
using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Http;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Hosted services are registered as singletons, so make each one required
/// is a subclass.
/// </remarks>
public abstract class TokenRefreshService : IHostedService, IDisposable
{ 
    private readonly IMemoryCache _memoryCache;
    private readonly OidcConfidentialClientConfiguration _configuration;
    private readonly IDictionary<string,string> _content;
    private readonly string _cacheKey;
    private readonly ILogger<TokenRefreshService> _logger;
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();

    private Timer? _timer = null;

    protected TokenRefreshService(
        IMemoryCache memoryCache,
        OidcConfidentialClientConfiguration configuration,
        ILogger<TokenRefreshService> logger)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
        _logger = logger;

        _content = GetRequestContent(configuration);
        _cacheKey = configuration.GetCacheKey();
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // schedule the refresh to occur right away, but do not run periodically
        // the timer will be changed to run at a period close to when the access
        // token expires
        _timer = new Timer(RefreshAccessToken, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private static IDictionary<string, string> GetRequestContent(OidcConfidentialClientConfiguration configuration)
    {
        var data = new Dictionary<string, string>
        {
            {"client_id", configuration.ClientId},
            {"client_secret", configuration.ClientSecret},
            {"scope", "openid"},
            {"grant_type", "client_credentials"}
        };

        // add more properties for other types like resource etc?

        return data;
    }

    private void RefreshAccessToken(object? state)
    {
        // have we have been requested to stop?
        if (_cts.IsCancellationRequested)
        {
            return;
        }

        // TODO: add retry logic
        var token = GetAccessTokenAsync(_cts.Token).Result;
        if (token is null)
        {
            // couldn't get access? try again right way?
            return;
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;
        _memoryCache.Set(_cacheKey, token, now.AddSeconds(token.ExpiresIn)); // should be refreshed by this

        ScheduleRefresh(token);
    }

    private async Task<Token?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage()
        { 
            Content = new FormUrlEncodedContent(_content),
            Method = HttpMethod.Post,
        };
        request.Headers.Add("Accept", "application/json");

        _logger.LogDebug("Sending OIDC access token request to server");
        using HttpClient httpClient = new HttpClient { BaseAddress = _configuration.TokenEndpoint };

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

    private void ScheduleRefresh(Token token)
    {
        double seconds = token.ExpiresIn * 2.0 / 3.0; // refresh again when there is only 1/3 of the life left
        ScheduleRefresh(TimeSpan.FromSeconds(seconds));
    }

    private void ScheduleRefresh(TimeSpan dueTime)
    {
        _timer?.Change(dueTime, Timeout.InfiniteTimeSpan);
    }


}
