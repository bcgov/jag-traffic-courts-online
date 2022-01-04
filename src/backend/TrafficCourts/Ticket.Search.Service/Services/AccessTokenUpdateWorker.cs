using System.Net.Sockets;
using TrafficCourts.Ticket.Search.Service.Services.Authentication;

namespace TrafficCourts.Ticket.Search.Service.Services
{
    public class AccessTokenUpdateWorker : BackgroundService
    {        
        private const int _minSeconds = 60 * 10; // refresh the token 10-15 minutes before the previous token expires
        private const int _maxSeconds = 60 * 15;

        private readonly IAuthenticationClient _authenticationClient;
        private readonly ITokenCache _cache;
        private readonly ILogger<AccessTokenUpdateWorker> _logger;
        private readonly Random _random = new();


        public AccessTokenUpdateWorker(
            IAuthenticationClient authenticationClient,
            ITokenCache cache,
            ILogger<AccessTokenUpdateWorker> logger)
        {
            _authenticationClient = authenticationClient ?? throw new ArgumentNullException(nameof(authenticationClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // The HostOptions.BackgroundServiceExceptionBehavior is configured to StopHost.
            // A BackgroundService has thrown an unhandled exception, and the IHost instance is stopping.
            // To avoid this behavior, configure this to Ignore; however the BackgroundService will not be restarted.

            int retry = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWorkAsync(stoppingToken);
                    retry = 0;
                }
                catch (AuthenticationException exception) when (exception.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                {
                    var rethrow = await OnError(exception);
                    if (rethrow)
                    {
                        throw;
                    }
                }
                catch (HttpRequestException exception)
                {
                    var rethrow = await OnError(exception);
                    if (rethrow)
                    {
                        throw;
                    }
                }
            }

            async Task<bool> OnError(Exception exception)
            {
                _logger.LogInformation(exception, "Failed to execute update access token");

                retry++;
                ////if (lastSuccess == DateTimeOffset.MinValue)
                ////{
                ////    // never have been successful
                ////    if (retry > 30)
                ////    {
                ////        _logger.LogInformation(exception, "Too many retries");
                ////        return true;
                ////    }
                ////}

                await Task.Delay(TimeSpan.FromMilliseconds(500 + _random.Next(50, 100)), stoppingToken);
                return false;
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Updating access token");
            var nextRefresh = await UpdateTokenAsync(stoppingToken);

            var delay = nextRefresh - DateTimeOffset.UtcNow;
            if (TimeSpan.Zero < delay)
            {
                using (_logger.BeginScope(new Dictionary<string, object> { { "Delay", delay } }))
                {
                    _logger.LogInformation("Waiting for next access token refresh");
                }

                await Task.Delay(delay, stoppingToken);
            }

        }

        private async Task<DateTimeOffset> UpdateTokenAsync(CancellationToken cancellationToken)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            try
            {
                var token = await _authenticationClient.GetTokenAsync(cancellationToken);
                _cache.SaveToken(token);
                return now.AddSeconds(token.ExpiresIn - _random.Next(_minSeconds, _maxSeconds)); // generally 3600 seconds or 1 hour
            }
            catch (AuthenticationException ex) when (ex.StatusCode != System.Net.HttpStatusCode.Unauthorized)
            {
                // config error?
                // service down?
                _logger.LogWarning(ex, "Failed to get new access token");
                return now.AddSeconds(1); // try again in 1 second
            }
        }
    }
}
