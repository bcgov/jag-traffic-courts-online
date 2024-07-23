using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TrafficCourts.Common.Authorization;

public static class AccessTokenPropagationExtensions
{
    /// <summary>
    /// Propagates the Authorization header from the current incoming request to the HttpClient request.
    /// </summary>
    public static IHttpClientBuilder AddAuthorizationHeaderPropagation(this IHttpClientBuilder builder) =>
        builder.AddHttpMessageHandler(
            (services) =>
            {
                var contextAccessor = services.GetRequiredService<IHttpContextAccessor>();
                var options = services.GetRequiredService<IOptions<KeycloakAuthorizationOptions>>();
                var logger = services.GetRequiredService<ILogger<AccessTokenPropagationHandler>>();

                return new AccessTokenPropagationHandler(contextAccessor, options, logger);
            }
        );
}
