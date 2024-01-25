using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
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
public abstract partial class TokenRefreshService<TImplementation> : IHostedService, IDisposable where TImplementation : TokenRefreshService<TImplementation>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _httpClientName;
    private readonly TimeProvider _timeProvider;
    private readonly ITokenCache _cache;
    private readonly OidcConfidentialClientConfiguration _configuration;
    private readonly IDictionary<string,string> _content;
    private readonly ILogger<TImplementation> _logger;
    private readonly CancellationTokenSource _cts = new();

    private ITimer? _timer = null;

    protected TokenRefreshService(
        IHttpClientFactory httpClientFactory,
        string httpClientName,
        TimeProvider timeProvider,
        ITokenCache cache,
        OidcConfidentialClientConfiguration configuration,
        ILogger<TImplementation> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClientName = httpClientName ?? throw new ArgumentNullException(nameof(httpClientName));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _content = GetRequestContent(configuration);
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
        _timer = _timeProvider.CreateTimer(RefreshAccessToken, new State(_timeProvider), TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        LogStarted();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _timer?.Change(TimeSpan.MaxValue, TimeSpan.Zero);
        LogStopped();
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
        ArgumentNullException.ThrowIfNull(state);

        var refreshState = (State)state;

        try
        {
            var task = RefreshAccessTokenAsync(refreshState);
            task.Wait();
        } 
        catch (AggregateException e)
        {
            LogRefreshAccessTokenFailed(e.InnerException ?? e);

            TimeSpan nextTry = ComputeNextTry(refreshState);

            ScheduleRefresh(nextTry);
        }
    }

    private TimeSpan ComputeNextTry(State state)
    {
        // if we already have a token that expires in the future,
        // dont try right away
        TimeSpan nextTry = TimeSpan.FromSeconds(15);

        if (state.Expires is not null)
        {
            nextTry = (state.Expires.Value - _timeProvider.GetUtcNow());
            if (nextTry <= TimeSpan.Zero)
            {
                // already expired try again in 15 seconds
                nextTry = TimeSpan.FromSeconds(15);
            }
            else if (nextTry > TimeSpan.FromMinutes(1))
            {
                // expires in more than a minute, try again in one minute
                nextTry = TimeSpan.FromMinutes(1);
            }
        }

        return nextTry;
    }

    private async Task RefreshAccessTokenAsync(State state)
    {
        // have we have been requested to stop?
        if (_cts.IsCancellationRequested)
        {
            LogCancellationRequested();
            return;
        }

        Token? token = await GetAccessTokenAsync(_cts.Token);
        if (token is null)
        {
            // couldn't get access? try again right way?
            return;
        }

        // flag in the job's state that access token updated
        DateTimeOffset expiresAt = state.TokenUpdated(token);

        LogGotToken(token, expiresAt);

        _cache.SaveToken(_configuration, token, expiresAt);
        
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

        LogRequestingAccessToken();

        using HttpClient httpClient = _httpClientFactory.CreateClient(_httpClientName);
        httpClient.BaseAddress = _configuration.TokenEndpoint;

        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            Token? token = await response.Content.ReadFromJsonAsync(SerializerContext.Default.Token, cancellationToken)
                .ConfigureAwait(false);

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
            // should probably throw?
            LogNotSuccessStatusCode(response.StatusCode);
            return null;
        }
    }

    /// <summary>
    /// If the computed next refresh interval is higher than this, we will cap the retry to this value
    /// </summary>
    private static TimeSpan TwoMinutes = TimeSpan.FromMinutes(2);

    private void ScheduleRefresh(Token token)
    {
        // assumption: token will not be returned with a "ExpiresIn" that is less than or equal to zero

        TimeSpan expires = TimeSpan.FromSeconds(token.ExpiresIn);
        // start refreshing the token two minutes before it expires
        TimeSpan nextFresh = expires - TwoMinutes;

        // if the original token expires in two or less minutes which would be odd,
        // then refresh when the token has 1/2 its life left
        if (nextFresh <= TimeSpan.Zero)
        {
            nextFresh = expires / 2.0;
        }
        
        ScheduleRefresh(nextFresh);
    }

    private void ScheduleRefresh(TimeSpan dueTime)
    {
        if (dueTime <= TimeSpan.Zero)
        {
            LogRefreshScheduledLessThanOrEqualToZero(dueTime);
            dueTime = TimeSpan.FromSeconds(1); // try again in one second, is this a good value?
        }

        LogRefreshScheduled(dueTime);
        _timer?.Change(dueTime, Timeout.InfiniteTimeSpan);
    }

    class State
    {
        private readonly TimeProvider _timeProvider;

        public State(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        /// <summary>
        /// The time the token expires, or null if we dont have a token yet.
        /// </summary>
        public DateTimeOffset? Expires { get; private set; }

        /// <summary>
        /// Flag that the token was updated
        /// </summary>
        /// <returns></returns>
        public DateTimeOffset TokenUpdated(Token token) 
        {
            var expires = _timeProvider.GetUtcNow().AddSeconds(token.ExpiresIn);
            Expires = expires;
            return expires;
        }
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "Obtained new token from OIDC server")]
    public partial void LogGotToken(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        Token token,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordExpiresAtTag), OmitReferenceName = true)]
        DateTimeOffset expiresAt
    );

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Service started")]
    public partial void LogStarted();

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Service stopped")]
    public partial void LogStopped();

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Cancellation has been requested, refresh access token aborted")]
    public partial void LogCancellationRequested();

    [LoggerMessage(EventId = 5, Level = LogLevel.Debug, Message = "Sending OIDC access token request to server")]
    public partial void LogRequestingAccessToken();

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning, Message = "Could not deserialize OIDC token, returning null")]
    public partial void LogTokenDeserializationError();

    [LoggerMessage(EventId = 7, Level = LogLevel.Warning, Message = "Access token request failed, returning null")]
    public partial void LogNotSuccessStatusCode(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordHttpStatusCodeTag), OmitReferenceName = true)]
        HttpStatusCode statusCode);

    [LoggerMessage(EventId = 8, Level = LogLevel.Debug, Message = "Schedule token refresh in {Duration}")]
    public partial void LogRefreshScheduled(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordDurationTag), OmitReferenceName = true)]
        TimeSpan duration);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "Error occurred while getting access token")]
    public partial void LogRefreshAccessTokenFailed(Exception exception);

    [LoggerMessage(EventId = 10, Level = LogLevel.Warning, Message = "Requested scheduled token refresh was less than or equal to zero")]
    public partial void LogRefreshScheduledLessThanOrEqualToZero(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordDurationTag), OmitReferenceName = true)]
        TimeSpan duration);
}
