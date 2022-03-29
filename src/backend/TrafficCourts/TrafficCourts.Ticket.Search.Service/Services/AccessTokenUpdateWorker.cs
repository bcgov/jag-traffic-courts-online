using System.Net.Sockets;
using TrafficCourts.Ticket.Search.Service.Authentication;
using TrafficCourts.Ticket.Search.Service.Logging;

namespace TrafficCourts.Ticket.Search.Service.Services
{
    public class AccessTokenUpdateWorker : BackgroundService
    {
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
                catch (HttpRequestException exception) when (exception.InnerException is SocketException socketException && socketException.ErrorCode == (int)SocketError.TimedOut)
                {
                    // A connection attempt failed because the connected party did not properly respond after a period of time,
                    // or established connection failed because connected host has failed to respond.

                    // todo: log specific message about not being able to connect, not on VPN or address wrong
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
                var delay = TimeSpan.FromMilliseconds(500 + _random.Next(50, 100));
                _logger.LogInformation(exception, "Failed to update access token on {Attempt}, will delay {DelayTime} before next attempt", ++retry, delay);
                await Task.Delay(delay, stoppingToken);
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

        /// <summary>
        /// Gets the new access token and returns the date when the next refresh attempt should 
        /// occur.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DateTimeOffset> UpdateTokenAsync(CancellationToken cancellationToken)
        {
            using var activity = Diagnostics.Source.StartActivity("Update Access Token");

            try
            {
                var token = await _authenticationClient.GetTokenAsync(cancellationToken);
                // next refresh is in 1/2 the token life time +/- 30 seconds.
                var nextRefresh = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn / 2 - _random.Next(-30, 30));
                _cache.SaveToken(token);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok);
                return nextRefresh;
            }
            catch (AuthenticationException ex) when (ex.StatusCode != System.Net.HttpStatusCode.Unauthorized)
            {
                // config error?
                // service down?
                _logger.LogWarning(ex, "Failed to get new access token");
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error);
                return DateTimeOffset.UtcNow.AddSeconds(_random.Next(5, 30)); // try again in 5-30 seconds
            }
        }
    }
}
