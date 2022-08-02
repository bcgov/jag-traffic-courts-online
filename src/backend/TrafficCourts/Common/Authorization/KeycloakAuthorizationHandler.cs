using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace TrafficCourts.Common.Authorization;

/// <summary>
/// Authorization handler that handles Keycloak requirements.
/// </summary>
public class KeycloakAuthorizationHandler : AuthorizationHandler<KeycloakAuthorizationRequirement>
{
    private readonly IOptions<KeycloakAuthorizationOptions> _options;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public KeycloakAuthorizationHandler(IOptions<KeycloakAuthorizationOptions> options, IHttpContextAccessor httpContextAccessor)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, KeycloakAuthorizationRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var options = _options.Value;
        var auth = await httpContext.AuthenticateAsync(options.RequiredScheme);
        if (!auth.Succeeded)
        {
            context.Fail();
            return;
        }

        var data = new Dictionary<string, string>
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket" },
            { "response_mode", "decision" },
            { "audience", options.Audience },
            { "permission", requirement.PolicyName }
        };

        var client = new HttpClient(options.BackchannelHandler);
        var token = await httpContext.GetTokenAsync(options.RequiredScheme, "access_token");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsync(options.TokenEndpoint, new FormUrlEncodedContent(data));
        if (response.IsSuccessStatusCode)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}
