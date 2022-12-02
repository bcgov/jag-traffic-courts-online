using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace TrafficCourts.Common.OpenAPIs.OracleDataAPI
{
    /// <summary>
    /// Sets x-username and x-fullName of the current user on the request.
    /// </summary>
    public class UserIdentityProviderHandler : DelegatingHandler
    {
        public const string UsernameClaimType = "idir_username";
        public const string FullNameClaimType = ClaimTypes.Name;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserIdentityProviderHandler> _logger;

        public UserIdentityProviderHandler(IHttpContextAccessor httpContextAccessor, ILogger<UserIdentityProviderHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
            {
                _logger.LogError("Cannot set x-username header, no HttpContext is available, the request not executing part of a HTTP web api request");
                return await base.SendAsync(request, cancellationToken);
            }

            if (AddUsernameHeader(request, httpContext.User))
            {
                AddFullNameHeader(request, httpContext.User);
            }
            else
            {
                using var scope = _logger.BeginScope(new Dictionary<string, object>
                {
                    ["IsAuthenticated"] = httpContext.User.Identity?.IsAuthenticated ?? false,
                    ["AuthenticationType"] = httpContext.User.Identity?.AuthenticationType ?? String.Empty
                });

                _logger.LogError("Could not find preferred_username claim on current user");
            }

            AddPartIdHeader(request, httpContext);

            return await base.SendAsync(request, cancellationToken);
        }

        private bool AddUsernameHeader(HttpRequestMessage request, ClaimsPrincipal user)
        {
            var username = user.Claims.FirstOrDefault(_ => _.Type == UsernameClaimType)?.Value;

            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            // we expect the username to be of the form: someone@domain
            int index = username.IndexOf("@");
            if (index > 0)
            {
                username = username[..index];
            }

            request.Headers.Add("x-username", username);
            return true;
        }

        private bool AddFullNameHeader(HttpRequestMessage request, ClaimsPrincipal user)
        {
            var fullName = user.Claims.FirstOrDefault(_ => _.Type == FullNameClaimType)?.Value;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                return false;
            }

            request.Headers.Add("x-fullName", fullName);
            return true;
        }

        private void AddPartIdHeader(HttpRequestMessage request, HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("partid", out var partIdHeader);
            string partId = partIdHeader.ToString();
            
            if (!string.IsNullOrWhiteSpace(partId))
            {
                request.Headers.Add("x-partId", partId);
            }
        }
    }
}
