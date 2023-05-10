using MediatR;
using TrafficCourts.Citizen.Service.Models.OAuth;
using TrafficCourts.Citizen.Service.Services;

namespace TrafficCourts.Citizen.Service.Features.CurrentUserInfo;

/// <summary>
/// Gets the current user info.
/// </summary>
public class GetCurrentUserInfoHandler : IRequestHandler<GetCurrentUserInfoRequest, GetCurrentUserInfoResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOAuthUserService _userService;
    private readonly ILogger<GetCurrentUserInfoHandler> _logger;

    public GetCurrentUserInfoHandler(IHttpContextAccessor httpContextAccessor, IOAuthUserService userService, ILogger<GetCurrentUserInfoHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<GetCurrentUserInfoResponse> Handle(GetCurrentUserInfoRequest request, CancellationToken cancellationToken)
    {
        string token = GetAccessToken();

        if (string.IsNullOrEmpty(token))
        {
            // not authenticated
            return GetCurrentUserInfoResponse.Empty;
        }

        UserInfo? userInfo = await _userService.GetUserInfoAsync(token, cancellationToken);

        if (userInfo is null) 
        {
            _logger.LogInformation("Could not get user info");
            return GetCurrentUserInfoResponse.Empty;
        }

        return new GetCurrentUserInfoResponse(userInfo);
    }

    private string GetAccessToken()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            // should never happen if called from api
            _logger.LogInformation("No current HttpContext");
            return string.Empty;
        }

        bool isAuthenticated = context.User?.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            _logger.LogInformation("User is not authenticated");
            return string.Empty;
        }

        var token = context.Request.Headers.Authorization.FirstOrDefault();
        if (token is null) 
        {
            _logger.LogInformation("No authorization header present");
            return string.Empty;
        }

        return token;
    }
}