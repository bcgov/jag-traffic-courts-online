using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Interfaces;

namespace TrafficCourts.OracleDataApi.Test;

public class DependencyInjectionTest
{
    private readonly ServiceCollection _services = new();
    private readonly IConfiguration _configuration = GetConfiguration();

    [Fact]
    public void can_resolve_IOracleDataApiService_after_calling_AddOracleDataApi()
    {
        OracleDataApiExtensions.AddOracleDataApi(_services, _configuration);

        var serviceProvider = _services.BuildServiceProvider();

        serviceProvider.GetRequiredService<IOracleDataApiService>();
    }

    [Fact]
    public void can_resolve_IOracleDataApiService_with_null_HttpClientBuilder_action()
    {
        OracleDataApiExtensions.AddOracleDataApi(_services, _configuration, null);

        var serviceProvider = _services.BuildServiceProvider();

        serviceProvider.GetRequiredService<IOracleDataApiService>();
    }

    [Fact]
    public void can_resolve_IOracleDataApiService_with_custom_HttpClient_config()
    {
        bool builderCalled = false;

        OracleDataApiExtensions.AddOracleDataApi(_services, _configuration, builder =>
        {
            builderCalled = true; // test to ensure this action got called
        });

        var serviceProvider = _services.BuildServiceProvider();

        serviceProvider.GetRequiredService<IOracleDataApiService>();

        Assert.True(builderCalled);
    }

    private static IConfiguration GetConfiguration()
    {
        Dictionary<string, string?> settings = new()
        {
            { $"{OracleDataApiConfiguration.Section}:{nameof(OracleDataApiConfiguration.BaseUrl)}", "http://localhost/" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return configuration;
    }
}
