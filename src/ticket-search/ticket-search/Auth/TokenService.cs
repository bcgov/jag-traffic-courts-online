using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Auth
{
    /// <summary>
    /// Provides and save oauth token, and manage token expired.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Get the oauth token and save it in memory. When existing token expired, will get
        /// new oauth token.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Token> GetTokenAsync(CancellationToken cancellationToken);
    }

    public class TokenService : ITokenService
    {
        /// <summary>
        /// The key used to cache the fetched access token.
        /// </summary>
        public const string TokenCacheKey = "oauth-token";

        /// <summary>
        /// The number of seconds before the access token expires to expire the cached token from the cache.
        /// </summary>
        private const int OauthTokenExpireBuffer = 60;

        private readonly IOAuthClient _oAuthClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOAuthClient oAuthClient, IMemoryCache cache, ILogger<TokenService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oAuthClient = oAuthClient ?? throw new ArgumentNullException(nameof(oAuthClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<Token> GetTokenAsync(CancellationToken cancellationToken)
        {
            var token = _cache.Get<Token>(TokenCacheKey);
            if (token == null)
            {
                _logger.LogDebug("Cached token not found, requesting a new token");
                return await RefreshTokenAsync(cancellationToken);
            }
            return token;
        }

        private async Task<Token> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            Token token = await _oAuthClient.GetRefreshToken(cancellationToken);

            int buffer = token.ExpiresIn < OauthTokenExpireBuffer ? 0 : OauthTokenExpireBuffer;

            var expires = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - buffer);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.BeginScope(new Dictionary<string, object>
                {
                    ["CacheKey"] = TokenCacheKey,
                    ["TokenExpires"] = expires
                });

                _logger.LogInformation("Refrshed access token and will be cached");
            }

            _cache.Set(TokenCacheKey, token, expires);
            return token;
        }
    }
}
