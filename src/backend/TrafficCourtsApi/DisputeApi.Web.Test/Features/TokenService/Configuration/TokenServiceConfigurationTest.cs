using System.Linq;
using DisputeApi.Web.Features.TokenService.Configuration;
using DisputeApi.Web.Features.TokenService.Service;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace DisputeApi.Web.Test.Features.TokenService.Configuration
{
    public class TokenServiceConfigurationExtensionTest
    {
        [Test]
        public void should_register_necessary_service()
        {
            var services = new ServiceCollection();
            services.AddTokenService();
            Assert.IsTrue(services.Any(x => x.ServiceType == typeof(ITokensService)));
        }
    }
}
