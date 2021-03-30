using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Auth
{
    public interface ITokenService    {
        Task<Token> GetTokenAsync(CancellationToken cancellationToken);
    }

    public class TokenService : ITokenService
    {
        private IOAuthClient _oAuthCient;
        private IMemoryCache _memoryCache;

        public TokenService(IOAuthClient oAuthClient, IMemoryCache memoryCache )
        {
            _oAuthCient = oAuthClient;
            _memoryCache = memoryCache;
        }

        public async Task<Token> GetTokenAsync(CancellationToken cancellationToken)
        {
            var token = _memoryCache.Get<Token>(Keys.OAUTH_TOKEN_KEY);
            if (token == null)
                return await RefreshTokenAsync(cancellationToken);
            return token;
        }

        private async Task<Token> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            Token token = await _oAuthCient.GetRefreshToken(cancellationToken);
            _memoryCache.Set(Keys.OAUTH_TOKEN_KEY, token, new TimeSpan(0, 0, token.ExpiresIn - Keys.OAUTH_TOKEN_EXPIRE_BUFFER));
            return token;
        }
    }
}
