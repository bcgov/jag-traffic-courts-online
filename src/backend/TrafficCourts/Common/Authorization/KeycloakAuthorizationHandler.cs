using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrafficCourts.Common.Authorization;

/// <summary>
/// Authorization handler that handles Keycloak requirements.
/// </summary>
public partial class KeycloakAuthorizationHandler : AuthorizationHandler<KeycloakAuthorizationRequirement>
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
            var noUserScope = new AuthorizationRequirementScope(_options.Audience, requirement.PolicyName, string.Empty);
            NotAuthorized(noUserScope, "No active HttpContext");
            context.Fail();
            return;
        }

        var scope = new AuthorizationRequirementScope(_options.Audience, requirement.PolicyName, httpContext.User.Identity?.Name ?? string.Empty);

        var authenticateResult = await httpContext.AuthenticateAsync(_options.RequiredScheme);
        if (!authenticateResult.Succeeded)
        {
            NotAuthorized(scope, "Request not authenticated", authenticateResult);
            context.Fail();
            return;
        }

        var token = await httpContext.GetTokenAsync(_options.RequiredScheme, "access_token");
        if (token is null)
        {
            NotAuthorized(scope, "No access token");
            context.Fail();
            return;
        }

        // check with keycloak if the user is authorized
        bool authorized = await AuthorizeAsync(token, scope);
        if (!authorized)
        {
            // the AuthorizeAsync should have logged the reason 
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private async Task<bool> AuthorizeAsync(string token, AuthorizationRequirementScope scope)
    {
        Debug.Assert(!string.IsNullOrEmpty(token));
        Debug.Assert(!string.IsNullOrEmpty(scope.Permission));

        bool authorized = false;

        Dictionary<string, string> data = new()
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket" },
            { "response_mode", "decision" },
            { "audience", _options.Audience },
            { "permission", scope.Permission }
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
            authorized = await ProcessDecisionAsync(response, scope);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            // If the authorization request does not map to any permission, a 403 HTTP status code is returned instead.
            NotAuthorized(scope, "Authorization request does not map to any permission");
        }
        else
        {
            // not 200 or 403
            await LogErrorAsync(response, scope);
        }

        return authorized;
    }

    private async Task<bool> ProcessDecisionAsync(HttpResponseMessage response, AuthorizationRequirementScope scope)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        var decisionResponse = await JsonSerializer.DeserializeAsync(stream, DecisionResponseJsonContext.Default.DecisionResponse);
        if (decisionResponse is not null)
        {
            if (decisionResponse.Result)
            {
                LogAuthorized(scope);
            }
            else
            {
                NotAuthorized(scope, "Keycloak returned not authorized");
            }

            return decisionResponse.Result;
        }

        NotAuthorized(scope, "Decision response could not be deserialized");
        return false;
    }

    /// <summary>
    /// Logs an authorization error message with the status code, error code and description of the error.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    private async Task LogErrorAsync(HttpResponseMessage response, AuthorizationRequirementScope scope)
    {
        // {"error":"invalid_request","error_description":"Client does not support permissions"}
        var error = await response.Content.ReadFromJsonAsync(DecisionResponseJsonContext.Default.DecisionResponseError);
        LogAuthorizationError(response.StatusCode, error);
    }

    private void NotAuthorized(AuthorizationRequirementScope scope, string reason)
    {
        LogNotAuthorized(scope, reason);
    }

    private void NotAuthorized(AuthorizationRequirementScope scope, string reason, AuthenticateResult result)
    {
        if (result.Failure is not null)
        {
            LogNotAuthorized(result.Failure, "Request not authenticated", result);
        }
        else
        {
            LogNotAuthorized("Request not authenticated", result);
        }
    }

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "User is authorized")]
    private partial void LogAuthorized(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        AuthorizationRequirementScope scope);

    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Not Authorized")]
    private partial void LogNotAuthorized(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        AuthorizationRequirementScope scope,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordReasonTag), OmitReferenceName = true)]
        string reason);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Not Authorized")]
    private partial void LogNotAuthorized(
        Exception exception,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordReasonTag), OmitReferenceName = true)]
        string reason,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        AuthenticateResult result);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Not Authorized")]
    private partial void LogNotAuthorized(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordReasonTag), OmitReferenceName = true)]
        string reason,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        AuthenticateResult result);

    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "Could not authorize user due to an error")]
    private partial void LogAuthorizationError(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTag), OmitReferenceName = true)]
        HttpStatusCode statusCode,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        DecisionResponseError? error);
}

internal record AuthorizationRequirementScope(string Audience, string Permission, string User);
    
internal static partial class TagProvider
{

    public static void RecordTag(ITagCollector collector, HttpStatusCode statusCode)
    {
        collector.Add("StatusCode", statusCode.ToString());
    }


    public static void RecordTags(ITagCollector collector, DecisionResponseError? error)
    {
        if (error is not null)
        {
            collector.Add("ErrorCode", error.Code);
            collector.Add("ErrorDescription", error.Description);
        }
    }

    public static void RecordTags(ITagCollector collector, AuthorizationRequirementScope scope)
    {
        collector.Add(nameof(scope.Audience), scope.Audience);
        collector.Add(nameof(scope.Permission), scope.Permission);
        collector.Add(nameof(scope.User), scope.User);
    }

    public static void RecordTags(ITagCollector collector, AuthenticateResult result)
    {
        if (result is null)
        {
            return;
        }

        if (result.Properties is not null)
        {
            if (result.Properties.IssuedUtc is not null) collector.Add("IssuedUtc", result.Properties.IssuedUtc);
            if (result.Properties.ExpiresUtc is not null) collector.Add("ExpiresUtc", result.Properties.ExpiresUtc);
            if (result.Properties.AllowRefresh is not null) collector.Add("AllowRefresh", result.Properties.AllowRefresh);
        }
    }

    public static void RecordReasonTag(ITagCollector collector, string reason)
    {
        if (!string.IsNullOrEmpty(reason))
        {
            collector.Add("Reason", reason);
        }
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
    public string Code { get; set; } = String.Empty;

    [JsonPropertyName("error_description")]
    public string Description { get; set; } = String.Empty;
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(DecisionResponse))]
[JsonSerializable(typeof(DecisionResponseError))]
internal partial class DecisionResponseJsonContext : JsonSerializerContext
{
}
