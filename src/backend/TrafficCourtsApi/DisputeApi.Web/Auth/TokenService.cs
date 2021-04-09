using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Auth
{
    /// <summary>
    /// provide and save oauth token, and manage token expired.
    /// </summary>
    public interface ITokenService    {
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
        private readonly IOAuthClient _oAuthClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOAuthClient oAuthClient, IMemoryCache memoryCache, ILogger<TokenService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _oAuthClient = oAuthClient ?? throw new ArgumentNullException(nameof(oAuthClient));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<Token> GetTokenAsync(CancellationToken cancellationToken)
        {
            var token = _memoryCache.Get<Token>(Keys.OAUTH_TOKEN_KEY);
            if (token == null)
            {
                _logger.LogDebug("Cached token not found, requesting a new token.");
                return await RefreshTokenAsync(cancellationToken);
            }
            return token;
        }

        private async Task<Token> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            Token token = await _oAuthClient.GetRefreshToken(cancellationToken);
            int expiredSeconds = token.ExpiresIn - Keys.OAUTH_TOKEN_EXPIRE_BUFFER;
            _logger.LogInformation(
                "Got new token and save it to memory(key={key}, expired in {expired} seconds.)", 
                Keys.OAUTH_TOKEN_KEY, 
                expiredSeconds );
            _memoryCache.Set(Keys.OAUTH_TOKEN_KEY, token, TimeSpan.FromSeconds(expiredSeconds));
            return token;
        }
    }
}
