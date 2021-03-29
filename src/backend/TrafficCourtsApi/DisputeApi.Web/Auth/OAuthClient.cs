using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Auth
{
    public interface IOAuthClient
    {
        Task<Token> GetRefreshToken(CancellationToken cancellationToken);
    }
    public class OAuthClient : IOAuthClient
    {
        private readonly HttpClient _httpClient;
        public OAuthClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<Token> GetRefreshToken(CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
