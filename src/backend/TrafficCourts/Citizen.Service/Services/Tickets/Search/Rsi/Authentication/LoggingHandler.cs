namespace TrafficCourts.Citizen.Service.Services.Tickets.Search.Rsi.Authentication;

public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            bool hasAuthorizationHeader = request.Headers.Authorization is not null;
            _logger.LogDebug("Authenticated: {HasAuthorization} {Method} {RequestUri}", hasAuthorizationHeader, request.Method, request.RequestUri);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}