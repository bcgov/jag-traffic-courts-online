using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrafficCourts.Common.Authorization;

/// <summary>
/// Authorization handler that handles Keycloak requirements.
/// </summary>
public class KeycloakAuthorizationHandler : AuthorizationHandler<KeycloakAuthorizationRequirement>
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAuthorizationOptions _options;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<KeycloakAuthorizationHandler> _logger;

    public KeycloakAuthorizationHandler(
        HttpClient httpClient,
        IOptions<KeycloakAuthorizationOptions> options,
        IHttpContextAccessor httpContextAccessor,
        ILogger<KeycloakAuthorizationHandler> logger)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options.Value;
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Makes a decision if authorization is allowed based on a specific requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The requirement to evaluate.</param>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, KeycloakAuthorizationRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            NotAuthorized("No active HttpContext");
            context.Fail();
            return;
        }

        // record the parameters used to authorized this request
        using var scope = _logger.BeginScope(new Dictionary<string, string> { 
            { "Audience", _options.Audience },
            { "Permission", requirement.PolicyName },
            { "User", httpContext.User.Identity?.Name ?? string.Empty }
        });

        var auth = await httpContext.AuthenticateAsync(_options.RequiredScheme);
        if (!auth.Succeeded)
        {
            NotAuthorized("Request not authenticated");
            context.Fail();
            return;
        }

        var token = await httpContext.GetTokenAsync(_options.RequiredScheme, "access_token");
        if (token is null)
        {
            NotAuthorized("No access token");
            context.Fail();
            return;
        }

        // check with keycloak if the user is authorized
        bool authorized = await AuthorizeAsync(token, requirement.PolicyName);
        if (!authorized)
        {
            // the AuthorizeAsync should have logged the reason 
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private async Task<bool> AuthorizeAsync(string token, string policyName)
    {
        Debug.Assert(!string.IsNullOrEmpty(token));
        Debug.Assert(!string.IsNullOrEmpty(policyName));

        bool authorized = false;

        Dictionary<string, string> data = new()
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket" },
            { "response_mode", "decision" },
            { "audience", _options.Audience },
            { "permission", policyName }
        };

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(data)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            authorized = await ProcessDecisionAsync(response);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            // If the authorization request does not map to any permission, a 403 HTTP status code is returned instead.
            NotAuthorized("Authorization request does not map to any permission");
        }
        else
        {
            // not 200 or 403
            await LogErrorAsync(response);
        }

        return authorized;
    }

    private async Task<bool> ProcessDecisionAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        var decisionResponse = await JsonSerializer.DeserializeAsync(stream, DecisionResponseJsonContext.Default.DecisionResponse);
        if (decisionResponse is not null)
        {
            if (decisionResponse.Result)
            {
                _logger.LogDebug("User is authorized");
            }
            else
            {
                NotAuthorized("Keycloak returned not authorized");
            }

            return decisionResponse.Result;
        }

        NotAuthorized("Decision response could not be deserialized");
        return false;
    }

    /// <summary>
    /// Logs an authorization error message with the status code, error code and description of the error.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    private async Task LogErrorAsync(HttpResponseMessage response)
    {
        var properties = new Dictionary<string, string>
        {
            { "StatusCode", response.StatusCode.ToString() }
        };

        // {"error":"invalid_request","error_description":"Client does not support permissions"}
        var stream = await response.Content.ReadAsStreamAsync();
        var error = await JsonSerializer.DeserializeAsync(stream, DecisionResponseJsonContext.Default.DecisionResponseError);

        if (error is not null)
        {
            properties.Add("ErrorCode", error.Code);
            properties.Add("ErrorDescription", error.Description);
        }

        using var scope = _logger.BeginScope(properties);
        _logger.LogError("Could not authorize user due to an error");
    }

    private void NotAuthorized(string reason)
    {
        var scope = _logger.BeginScope(new Dictionary<string, string> { { "Reason", reason } });
        _logger.LogInformation("Not Authorized");
    }
}

/// <summary>
/// The result of a decision
/// </summary>
/// <see cref="https://github.com/keycloak/keycloak-documentation/blob/main/authorization_services/topics/service-authorization-obtaining-permission.adoc"/>
internal class DecisionResponse
{
    [JsonPropertyName("result")]
    public bool Result { get; set; }
}

internal class DecisionResponseError
{
    [JsonPropertyName("error")]
    public string Code { get; set; }

    [JsonPropertyName("error_description")]
    public string Description { get; set; }
}

[ExcludeFromCodeCoverage]
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DecisionResponse))]
[JsonSerializable(typeof(DecisionResponseError))]
internal partial class DecisionResponseJsonContext : JsonSerializerContext
{
}
