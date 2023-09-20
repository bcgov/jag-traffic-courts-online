using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TrafficCourts.Core.Http;

namespace TrafficCourts.Core.Test.Http;

public class OidcConfidentialClientTest
{
    [Fact]
    public async Task X()
    {
        var configuration = new OidcConfidentialClientConfiguration {
            ClientId = "",
            ClientSecret = "",
            TokenEndpoint = new Uri("https://dev.loginproxy.gov.bc.ca/auth/realms/comsvcauth/protocol/openid-connect/token")
        };

        var options = Options.Create(new MemoryCacheOptions());
        var cache = new MemoryCache(options);

        var logger = NullLogger<OidcConfidentialClient>.Instance;


        var sut = new OidcConfidentialClient(configuration, cache, logger);

        var token = await sut.RequestAccessTokenAsync(CancellationToken.None);
    }
}
