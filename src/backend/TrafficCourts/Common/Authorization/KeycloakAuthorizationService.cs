using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TrafficCourts.Common.Authorization;

/// <summary>
/// Provides keycloak authorization services.
/// </summary>
/// <remarks>
/// <see cref="https://www.keycloak.org/docs/latest/authorization_services/#_service_overview"/>
/// </remarks>
public partial class KeycloakAuthorizationService : IKeycloakAuthorizationService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAuthorizationOptions _options;
    private readonly ILogger<KeycloakAuthorizationService> _logger;

    public KeycloakAuthorizationService(
        HttpClient httpClient,
        IOptions<KeycloakAuthorizationOptions> options,
        ILogger<KeycloakAuthorizationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    /// <remarks>
    /// <see cref="https://www.keycloak.org/docs/latest/authorization_services/#_service_obtaining_permissions"/>
    /// </remarks>
    public async Task<IList<Resource>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        HttpRequestMessage request = CreateGetPermissionsRequest();

        HttpResponseMessage responseMessage = await _httpClient.SendAsync(request, cancellationToken);

        if (responseMessage.StatusCode == HttpStatusCode.OK)
        {
            var response = await ReadResponseMessageAsync(responseMessage, cancellationToken);
            return response;
        }
        else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
        {
            await HandleUnauthorizedAsync(responseMessage, cancellationToken);
            return Array.Empty<Resource>();
        }
        else
        {
            await HandleOtherErrorAsync(responseMessage, cancellationToken);
            return Array.Empty<Resource>();
        }
    }

    private HttpRequestMessage CreateGetPermissionsRequest()
    {
        Dictionary<string, string> data = new()
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:uma-ticket" },
            { "response_mode", "permissions" },
            { "audience", _options.Audience }
        };

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(data)
        };

        return request;
    }

    private async Task<IList<Resource>> ReadResponseMessageAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        List<PermissionResponse>? resources = await responseMessage.Content.ReadFromJsonAsync(ResponseJsonContext.Default.ListPermissionResponse, cancellationToken);
        if (resources is null)
        {
            return Array.Empty<Resource>();
        }

        List<Resource> response = resources
            .Select(_ => new Resource
            {
                Id = _.ResourceId,
                Name = _.ResourceName,
                Scopes = _.Scopes
            })
            .ToList();

        return response;
    }

    private async Task<ErrorResponse?> ReadErrorResponseAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        ErrorResponse? error = await responseMessage.Content.ReadFromJsonAsync(ResponseJsonContext.Default.ErrorResponse, cancellationToken);
        return error;
    }

    private async Task HandleUnauthorizedAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        ErrorResponse? error = await ReadErrorResponseAsync(responseMessage, cancellationToken);
        if (error is not null)
        {
            LogUnauthorized(error);
        }
        else
        {
            LogUnauthorized();
        }
    }

    private async Task HandleOtherErrorAsync(HttpResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        ErrorResponse? error = await ReadErrorResponseAsync(responseMessage, cancellationToken);
        if (error is not null)
        {
            LogErrorGettingPermissions(error, responseMessage.StatusCode);
        }
        else
        {
            LogErrorGettingPermissions();
        }
    }

    [LoggerMessage(101, LogLevel.Information, "User is unauthorized")]
    private partial void LogUnauthorized();

    [LoggerMessage(102, LogLevel.Information, "User is unauthorized")]
    private partial void LogUnauthorized(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)] 
        ErrorResponse error);

    [LoggerMessage(103, LogLevel.Information, "Error getting user permissions")]
    private partial void LogErrorGettingPermissions();

    [LoggerMessage(104, LogLevel.Information, "Error getting user permissions")]
    private partial void LogErrorGettingPermissions(
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTags), OmitReferenceName = true)]
        ErrorResponse error,
        [TagProvider(typeof(TagProvider), nameof(TagProvider.RecordTag), OmitReferenceName = true)]
        HttpStatusCode httpStatusCode);
}
internal record PermissionResponse(
    [property: JsonPropertyName("rsid")] string ResourceId,
    [property: JsonPropertyName("rsname")] string ResourceName,
    [property: JsonPropertyName("scopes")] string[] Scopes);

internal record ErrorResponse(
    [property: JsonPropertyName("error")] string Error,
    [property: JsonPropertyName("error_description")] string Description);

[JsonSerializable(typeof(List<PermissionResponse>))]
[JsonSerializable(typeof(ErrorResponse))]
internal partial class ResponseJsonContext : JsonSerializerContext
{
}

internal static partial class TagProvider
{

    public static void RecordTags(ITagCollector collector, ErrorResponse errorResponse)
    {
        collector.Add("ErrorCode", errorResponse.Error);
        collector.Add("ErrorDescription", errorResponse.Description);
    }
}

public record Resource
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string[] Scopes { get; set; } = [];
}
