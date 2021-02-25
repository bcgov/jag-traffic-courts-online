
using DisputeApi.Web.Features.TokenService.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DisputeApi.Web.Features.TokenService.Configuration
{
    /// <summary>
    /// Extension to inject token configuration service in service collection
    /// </summary>
    public static class TokenServiceConfigurationExtension
    {
        public static void AddTokenService(this IServiceCollection collection)
        {
            collection.AddScoped<ITokensService, TokensService>();
        }
    }
}
